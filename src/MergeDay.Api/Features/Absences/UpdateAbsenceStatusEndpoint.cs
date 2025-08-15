using MergeDay.Api.Domain;
using MergeDay.Api.Endpoints;
using MergeDay.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace MergeDay.Api.Features.Absences;

public static class UpdateAbsenceStatusEndpoint
{
    public record UpdateAbsenceStatusRequest(UpdateStatus UpdateStatus, string? Note);

    public enum UpdateStatus
    {
        Approved,
        Rejected
    }

    [EndpointGroup("Absences")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapStandardPost<UpdateAbsenceStatusRequest, IResult>("/{id}/approve", Handler)
                .WithName("Approve absence")
                .WithSummary("Approves an existing absence request by id.")
                .AllowAnonymous();
        }
    }

    public static async Task<IResult> Handler(
        [FromRoute] long id,
        [FromBody] UpdateAbsenceStatusRequest req,
        [FromServices] MergeDayDbContext dbContext,
        ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger(nameof(UpdateAbsenceStatusEndpoint));

        var entity = await dbContext.Absences.FindAsync(id);
        if (entity is null)
            return Results.NotFound($"Absence {id} not found.");

        if (entity.Status == AbsenceStatus.Approved)
            return Results.BadRequest($"Absence {id} is already approved.");
        if (entity.Status == AbsenceStatus.Rejected)
            return Results.BadRequest($"Absence {id} was rejected and cannot be approved.");

        entity.Status = req.UpdateStatus == UpdateStatus.Approved
            ? AbsenceStatus.Approved
            : AbsenceStatus.Rejected;
        entity.AuditorNote = req.Note ?? string.Empty;

        await dbContext.SaveChangesAsync();

        logger.LogInformation("Absence {Id} for {User} status updated to {statusUpdate}", entity.Id, entity.UserId, req.UpdateStatus.ToString());
        return Results.Ok(entity);
    }
}
