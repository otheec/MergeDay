using System.Security.Claims;

namespace MergeDay.Api.Common.Extensions;

public static class UserPrincipalExtensions
{
    public static string? GetUserName(this ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException();
    }

    public static string? GetUserId(this ClaimsPrincipal user)
    {
        return user.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value
            ?? throw new UnauthorizedAccessException();
    }
}
