using MergeDay.Api.Common;
using MergeDay.Api.Domain.Entities;
using MergeDay.Api.Endpoints;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MergeDay.Api.Features.Me;

public static class EditProfileEndpoint
{
    public record EditProfileRequest(
        string? TogglApiToken,
        string? FakturoidSlug,
        string? FakturoidClientId,
        string? FakturoidClientSecret,
        decimal? PricePerHour);

    [EndpointGroup("Me")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapStandardPut<EditProfileRequest, IResult>("", Handler)
                .WithName("EditProfile")
                .WithSummary("Edit your profile.")
                .RequireAuthorization(AppPolicy.UserOrAdmin);
        }
    }
    public static async Task<IResult> Handler(
        [FromBody] EditProfileRequest request,
        [FromServices] UserManager<ApplicationUser> userManager,
        [FromServices] IHttpContextAccessor httpContextAccessor)
    {
        var principal = httpContextAccessor.HttpContext?.User;
        if (principal is null)
            return Results.Unauthorized();

        var user = await userManager.GetUserAsync(principal);
        if (user is null)
            return Results.Unauthorized();

        user.TogglApiToken = request.TogglApiToken;
        user.FakturoidSlug = request.FakturoidSlug;
        user.FakturoidClientId = request.FakturoidClientId;
        user.FakturoidClientSecret = request.FakturoidClientSecret;
        user.PricePerHours = request.PricePerHour;

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return Results.BadRequest(result.Errors);

        return Results.Ok();
    }
}
