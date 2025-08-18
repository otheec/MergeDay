using MergeDay.Api.Common;
using MergeDay.Api.Domain;
using MergeDay.Api.Endpoints;
using MergeDay.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MergeDay.Api.Features.Absences;

public static class GetWorkersAbsenceEndpoint
{
    public record GetWorkersAbsenceResponse(string UserId, ICollection<AbsenceDto> Absences);
    public record AbsenceDto(long Id, DateTime StartDate, DateTime EndDate, AbsenceKind Kind, AbsenceStatus Status);

    [EndpointGroup("Absences")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapStandardGet<GetWorkersAbsenceResponse>("/workers/{userId}", Handler)
                .WithName("Get worker's absence")
                .WithSummary("Retrieve the absence details for a specific user in a workspace.")
                .RequireAuthorization(AppPolicy.UserOrAdmin);
        }
    }

    public static async Task<IResult> Handler(
        [FromRoute] string userId,
        [FromServices] MergeDayDbContext dbContext,
        ILoggerFactory loggerFactory,
        [FromQuery] DateTime? StartDate = null,
        [FromQuery] DateTime? EndDate = null)
    {
        var logger = loggerFactory.CreateLogger(nameof(GetWorkersAbsenceEndpoint));

        var absences = await dbContext.Absences
            .Where(a => a.UserId == userId)
            .Where(a => !StartDate.HasValue || a.EndDate >= StartDate.Value)
            .Where(a => !EndDate.HasValue || a.StartDate <= EndDate.Value)
            .ToListAsync();

        if (absences == null || absences.Count == 0)
        {
            logger.LogInformation("No absences found for user {UserId}", userId);
            return Results.NotFound("No absences found for this user.");
        }

        logger.LogInformation("Retrieved {Count} approved absences for user {User}.",
            absences.Count, userId);

        var responseAbsences = absences.Select(a => new AbsenceDto(
            a.Id,
            a.StartDate,
            a.EndDate,
            a.Kind,
            a.Status
        )).ToList();

        var response = new GetWorkersAbsenceResponse(userId, responseAbsences);

        return Results.Ok(responseAbsences);
    }
}
