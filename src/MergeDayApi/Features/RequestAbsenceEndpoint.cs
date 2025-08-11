using MergeDayApi.Domain;
using MergeDayApi.Endpoints;
using MergeDayApi.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace MergeDayApi.Features;

public static class RequestAbsenceEndpoint
{
    public record RequestAbsenceRequest(DateTime StartDate, DateTime EndDate, AbsenceKind AbsenceKind);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapStandardPost<RequestAbsenceRequest, IResult>("/request", Handler)
                .WithName("Request absence")
                .WithSummary("Submit a request to be marked as absent for a date range.")
                .AllowAnonymous();
        }
    }

    public static async Task<IResult> Handler(
        [FromBody] RequestAbsenceRequest req,
        [FromServices] MergeDayDbContext dbContext,
        ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger(nameof(RequestAbsenceEndpoint));

        //Sample user ID retrieval for dev purposes
        var userId = Guid.NewGuid().ToString();

        if (req.EndDate.Date < req.StartDate.Date)
            return Results.BadRequest("EndDate must be on or after StartDate.");

        var absence = new Absence
        {
            UserId = userId,
            StartDate = req.StartDate.Date,
            EndDate = req.EndDate.Date,
            Kind = req.AbsenceKind,
            Status = AbsenceStatus.Submitted
        };

        dbContext.Absences.Add(absence);
        await dbContext.SaveChangesAsync();

        logger.LogInformation("{User} requested absence {Start:d}..{End:d}", userId, absence.StartDate, absence.EndDate);

        return Results.Created();
    }
}
