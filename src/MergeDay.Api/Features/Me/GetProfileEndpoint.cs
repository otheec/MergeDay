using MergeDay.Api.Common;
using MergeDay.Api.Domain.Entities;
using MergeDay.Api.Endpoints;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MergeDay.Api.Features.Me;

public static class GetProfileEndpoint
{
    public record GetProfileResponse(
        string Name,
        string? TogglApiToken,
        string? FakturoidSlug,
        string? FakturoidClientId,
        string? FakturoidClientSecret,
        decimal? PricePerHours);

    [EndpointGroup("Me")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapStandardGet<IResult>("", Handler)
                .WithName("GetProfile")
                .WithSummary("Get your profile.")
                .RequireAuthorization(AppPolicy.UserOrAdmin);
        }
    }

    public static async Task<IResult> Handler(
        [FromServices] UserManager<ApplicationUser> userManager,
        [FromServices] IHttpContextAccessor httpContextAccessor)
    {
        var principal = httpContextAccessor.HttpContext?.User;
        if (principal is null)
            return Results.Unauthorized();

        var user = await userManager.GetUserAsync(principal);
        if (user is null)
            return Results.Unauthorized();

        return Results.Ok(new GetProfileResponse(
            user.Name,
            user.TogglApiToken,
            user.FakturoidSlug,
            user.FakturoidClientId,
            user.FakturoidClientSecret,
            user.PricePerHour
        ));
    }
}
