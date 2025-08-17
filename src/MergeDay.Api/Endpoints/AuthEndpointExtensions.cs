using MergeDay.Api.Common;
using Microsoft.AspNetCore.Authorization;

namespace MergeDay.Api.Endpoints;

public static class AuthEndpointExtensions
{
    public static RouteHandlerBuilder RequireAuthorization(this RouteHandlerBuilder builder, UserRole role)
    {
        return builder.RequireAuthorization(new AuthorizeAttribute { Roles = role.ToString() });
    }

    public static RouteHandlerBuilder RequireAuthorization(this RouteHandlerBuilder builder, IEnumerable<UserRole> roles)
    {
        var roleNames = string.Join(",", roles.Select(r => r.ToString()));
        return builder.RequireAuthorization(new AuthorizeAttribute { Roles = roleNames });
    }
}

