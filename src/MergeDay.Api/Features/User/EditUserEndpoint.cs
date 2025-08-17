using MergeDay.Api.Domain.Entities;
using MergeDay.Api.Endpoints;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MergeDay.Api.Features.User;

public static class EditUserEndpoint
{
    public record EditUserRequest(
        string? TogglApiToken,
        string? FakturoidSlug,
        string? FakturoidClientId,
        string? FakturoidClientSecret);

    [EndpointGroup("Users")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapStandardPut<EditUserRequest, IResult>("users/{id:guid}", Handler)
                .WithName("EditUser")
                .WithSummary("Edit a user's profile (no self-verification).");
        }
    }

    public static async Task<IResult> Handler(
        [FromRoute] Guid id,
        [FromBody] EditUserRequest request,
        [FromServices] UserManager<ApplicationUser> userManager)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user is null)
            return Results.NotFound(new { message = "User not found" });

        user.TogglApiToken = request.TogglApiToken;
        user.FakturoidSlug = request.FakturoidSlug;
        user.FakturoidClientId = request.FakturoidClientId;
        user.FakturoidClientSecret = request.FakturoidClientSecret;

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return Results.BadRequest(result.Errors);

        return Results.Ok();
    }
}

