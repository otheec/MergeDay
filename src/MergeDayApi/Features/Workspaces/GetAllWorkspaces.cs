using MergeDayApi.Endpoints;
using MergeDayApi.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MergeDayApi.Features.Workspaces;

public static class GetAllWorkspaces
{
    public record GetAllWorkspacesResponse(ICollection<WorkspaceDto> Workspaces);
    public record WorkspaceDto(long Id, string Name, ICollection<string> UserIds);

    [EndpointGroup("Workspaces")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapStandardGet<GetAllWorkspacesResponse[]>("", Handler)
                .WithName("Get all workspaces")
                .WithSummary("Retrieve all workspaces.")
                .AllowAnonymous();
        }
    }

    public static async Task<IResult> Handler(
        [FromServices] MergeDayDbContext dbContext,
        ILoggerFactory loggerFactory,
        CancellationToken ct)
    {
        var logger = loggerFactory.CreateLogger(nameof(GetAllWorkspaces));

        var response = await dbContext.Workspaces
            .Select(ws => new WorkspaceDto(ws.Id, ws.Name, ws.UserIds))
            .ToArrayAsync(ct);

        logger.LogInformation("Retrieved {Count} workspaces", response.Length);

        return Results.Ok(response);
    }
}
