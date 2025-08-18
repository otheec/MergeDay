using MergeDay.Api.Common;
using MergeDay.Api.Endpoints;
using MergeDay.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MergeDay.Api.Features.Workspaces;

public static class GetAllWorkspacesEndpoint
{
    public record GetAllWorkspacesResponse(ICollection<WorkspaceDto> Workspaces);
    public record WorkspaceDto(long Id, string Name, ICollection<string> UserIds);

    [EndpointGroup("Workspaces")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapStandardGet<ICollection<GetAllWorkspacesResponse>>("", Handler)
                .WithName("Get all workspaces")
                .WithSummary("Retrieve all workspaces.")
                .RequireAuthorization(AppPolicy.UserOrAdmin);
        }
    }

    public static async Task<IResult> Handler(
        [FromServices] MergeDayDbContext dbContext,
        ILoggerFactory loggerFactory,
        CancellationToken ct)
    {
        var logger = loggerFactory.CreateLogger(nameof(GetAllWorkspacesEndpoint));

        var response = await dbContext.Workspaces
            .Select(ws => new WorkspaceDto(ws.Id, ws.Name, ws.UserIds))
            .ToArrayAsync(ct);

        logger.LogInformation("Retrieved {Count} workspaces", response.Length);

        return Results.Ok(response);
    }
}
