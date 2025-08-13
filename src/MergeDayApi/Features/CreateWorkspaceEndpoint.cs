using MergeDayApi.Domain.Entities;
using MergeDayApi.Endpoints;
using MergeDayApi.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace MergeDayApi.Features;

public static class CreateWorkspaceEndpoint
{
    public record CreateWorkspaceRequest(ICollection<string> UserIds);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapStandardPost<CreateWorkspaceRequest, IResult>("", Handler)
                .WithName("Create workspace")
                .WithSummary("Create a new workspace with the specified users.")
                .AllowAnonymous();
        }
    }

    public static async Task<IResult> Handler(
        [FromBody] CreateWorkspaceRequest req,
        [FromServices] MergeDayDbContext dbContext,
        ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger(nameof(CreateWorkspaceEndpoint));

        if (req.UserIds == null || req.UserIds.Count == 0)
            return Results.BadRequest("UserIds cannot be empty.");

        var workspace = new Workspace
        {
            UserIds = req.UserIds.ToList()
        };

        dbContext.Workspaces.Add(workspace);

        await dbContext.SaveChangesAsync();

        logger.LogInformation("Created workspace {Id} with users: {Users}", workspace.Id, string.Join(", ", req.UserIds));

        return Results.Created();
    }
}
