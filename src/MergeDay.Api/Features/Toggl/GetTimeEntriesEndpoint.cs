using MergeDay.Api.Common;
using MergeDay.Api.Endpoints;
using Microsoft.AspNetCore.Mvc;

namespace MergeDay.Api.Features.Toggl;

public class GetTimeEntriesEndpoint
{
    public record GetTimeEntriesResponse(long? ProjectId, long? TaskId, string? Description, DateTime Start, DateTime End);

    [EndpointGroup("Toggl")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapStandardGet<IEnumerable<GetTimeEntriesResponse>>("/time-entries", Handler)
                .WithName("Get time entries")
                .WithSummary("Retrieve time entries for a specific user in a workspace.")
                .RequireAuthorization(AppPolicy.UserOrAdmin);
        }
    }

    public static async Task<IResult> Handler(
        [FromQuery] DateTime start,
        [FromQuery] DateTime end,
        ILoggerFactory loggerFactory,
        TogglService togglService)
    {
        var logger = loggerFactory.CreateLogger(nameof(GetTimeEntriesEndpoint));

        var timeEntries = await togglService.GetTimeEntriesAsync(start, end);

        if (timeEntries == null || timeEntries.Count == 0)
        {
            logger.LogInformation("No time entries found in the last month.");
            return Results.NotFound("No time entries found.");
        }

        var mapped = timeEntries.Select(te => new GetTimeEntriesResponse(
            te.ProjectId,
            te.TaskId,
            te.Description,
            te.Start,
            te.Stop ?? te.Start
        ));

        return Results.Ok(mapped);
    }
}
