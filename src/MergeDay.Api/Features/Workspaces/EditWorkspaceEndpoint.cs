using MergeDay.Api.Endpoints;
using MergeDay.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace MergeDay.Api.Features.Workspaces;

public static class EditWorkspaceEndpoint
{
    public record EditWorkspaceRequest(string Name, ICollection<string> UserIds);

    [EndpointGroup("Workspaces")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapStandardPut<EditWorkspaceRequest>("/{workspaceId}", Handler)
                .WithName("Edit workspace")
                .WithSummary("Edit an existing workspace by its ID.")
                .RequireAuthorization();
        }
    }

    public static async Task<IResult> Handler(
        [FromRoute] long workspaceId,
        [FromBody] EditWorkspaceRequest request,
        [FromServices] MergeDayDbContext dbContext,
        ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger(nameof(EditWorkspaceEndpoint));

        var workspace = await dbContext.Workspaces.FindAsync(workspaceId);

        if (workspace == null)
        {
            logger.LogInformation("Workspace with ID {WorkspaceId} not found.", workspaceId);
            return Results.NotFound($"Workspace with ID {workspaceId} not found.");
        }

        workspace.Name = request.Name;
        workspace.UserIds = request.UserIds;

        await dbContext.SaveChangesAsync();
        logger.LogInformation("Workspace with ID {WorkspaceId} updated successfully.", workspaceId);

        return Results.Ok();
    }
}
