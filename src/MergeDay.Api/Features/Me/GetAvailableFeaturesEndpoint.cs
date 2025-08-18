using System.Security.Claims;
using MergeDay.Api.Common;
using MergeDay.Api.Endpoints;
using MergeDay.Api.Infrastructure.Persistence;

namespace MergeDay.Api.Features.Me;

public class GetAvailableFeaturesEndpoint
{
    public record GetFeaturesResponse(IEnumerable<Feature> Features);
    public enum Feature
    {
        Toggl,
        Fakturoid
    }

    [EndpointGroup("Me")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapStandardGet<GetFeaturesResponse>("/available-features", Handler)
               .WithName("Get available features for current user")
               .WithDescription("Retrieves the features available to the current user based on their settings.")
               .RequireAuthorization(AppPolicy.UserOrAdmin);
        }
    }

    public static async Task<IResult> Handler(
        MergeDayDbContext dbContext,
        IHttpContextAccessor accessor)
    {
        var userIdStr = accessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
            return Results.Unauthorized();

        var user = await dbContext.Users.FindAsync(userId);
        if (user is null)
            return Results.NotFound();

        var features = new List<Feature>();

        if (!string.IsNullOrEmpty(user.TogglApiToken))
            features.Add(Feature.Toggl);

        if (!string.IsNullOrEmpty(user.FakturoidClientId) &&
            !string.IsNullOrEmpty(user.FakturoidClientSecret) &&
            !string.IsNullOrEmpty(user.FakturoidSlug))
        {
            features.Add(Feature.Fakturoid);
        }

        return Results.Ok(new GetFeaturesResponse(features));
    }
}
