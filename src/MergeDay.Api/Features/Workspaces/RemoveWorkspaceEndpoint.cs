using MergeDay.Api.Endpoints;
using MergeDay.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace MergeDay.Api.Features.Workspaces;

public static class RemoveWorkspaceEndpoint
{
    [EndpointGroup("Workspaces")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapStandardDelete("/{workspaceId}", Handler)
                .WithName("Remove workspace")
                .WithSummary("Remove a workspace by its ID.")
                .RequireAuthorization();
        }
    }

    public static async Task<IResult> Handler(
        [FromRoute] long workspaceId,
        [FromServices] MergeDayDbContext dbContext,
        ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger(nameof(RemoveWorkspaceEndpoint));

        var workspace = await dbContext.Workspaces.FindAsync(workspaceId);
        if (workspace == null)

        {
            logger.LogInformation("Workspace with ID {WorkspaceId} not found.", workspaceId);
            return Results.NotFound($"Workspace with ID {workspaceId} not found.");
        }

        dbContext.Workspaces.Remove(workspace);

        await dbContext.SaveChangesAsync();

        logger.LogInformation("Workspace with ID {WorkspaceId} removed successfully.", workspaceId);

        return Results.Ok();
    }
}
