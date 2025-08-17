using MergeDay.Api.Domain.Entities;
using MergeDay.Api.Endpoints;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MergeDay.Api.Features.User;

public static class RemoveUserEndpoint
{
    [EndpointGroup("Users")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapStandardDelete("users/{id}", Handler)
                .WithName("DeleteUser")
                .WithSummary("Delete a user (no self-verification).")
                .RequireAuthorization();
        }
    }

    public static async Task<IResult> Handler(
        [FromRoute] Guid id,
        UserManager<ApplicationUser> userManager)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user is null)
            return Results.NotFound(new { message = "User not found" });

        var result = await userManager.DeleteAsync(user);
        if (!result.Succeeded)
            return Results.BadRequest(result.Errors);

        return Results.Ok(new { message = "User deleted successfully" });
    }
}
