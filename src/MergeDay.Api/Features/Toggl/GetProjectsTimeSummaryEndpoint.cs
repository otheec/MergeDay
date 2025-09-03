using MergeDay.Api.Common;
using MergeDay.Api.Endpoints;
using MergeDay.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MergeDay.Api.Features.Toggl;

public static class GetProjectsTimeSummaryEndpoint
{
    public record GetProjectsTimeSummaryResponse(
        string ProjectName,
        decimal TotalHours
    );

    [EndpointGroup("Toggl")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapStandardGet<IEnumerable<GetProjectsTimeSummaryResponse>>(
                    "/time-entries/summary/by-project", Handler)
                .WithName("Get time summary grouped by project")
                .WithSummary("Retrieve total tracked time grouped by project within a date range.")
                .RequireAuthorization(AppPolicy.UserOrAdmin);
        }
    }

    public static async Task<IResult> Handler(
        [FromQuery] DateTime start,
        [FromQuery] DateTime end,
        ILoggerFactory loggerFactory,
        TogglService togglService,
        [FromServices] MergeDayDbContext dbContext)
    {
        var logger = loggerFactory.CreateLogger(nameof(Endpoint));

        var timeEntries = await togglService.GetTimeEntriesAsync(start, end);

        if (timeEntries == null || timeEntries.Count == 0)
        {
            logger.LogInformation("No time entries found for the requested range.");
            return Results.NotFound("No time entries found.");
        }

        var groupedEntries = timeEntries
            .GroupBy(te => te.ProjectId)
            .ToList();
        var projectIds = groupedEntries
            .Select(g => g.Key)
            .Where(id => id != null)
            .ToList();
        var projects = await dbContext.TogglProjects
            .Where(p => projectIds.Contains(p.TogglId))
            .ToListAsync();

        if (projects.Count != projectIds.Count)
        {
            logger.LogWarning("Some projects from time entries were not found in the database.");
        }

        var grouped = groupedEntries.Select(g => new GetProjectsTimeSummaryResponse(
            ProjectName: projects.FirstOrDefault(p => p.TogglId == g.Key)?.Name ?? string.Empty,
            TotalHours: (decimal)g.Sum(t => (t.Stop!.Value - t.Start).TotalHours)
        )).ToList();

        return Results.Ok(grouped);
    }

}
