using MergeDay.Api.Common;
using Microsoft.AspNetCore.Authorization;

namespace MergeDay.Api.Endpoints;

public static class AuthEndpointExtensions
{
    public static RouteHandlerBuilder RequireAuthorization(this RouteHandlerBuilder builder, AppPolicy policy)
        => builder.RequireAuthorization(policy.ToString());

    public static RouteGroupBuilder RequireAuthorization(this RouteGroupBuilder group, AppPolicy policy)
        => group.RequireAuthorization(policy.ToString());


    public static void AddPolicy(this AuthorizationOptions options, AppPolicy policy, Action<AuthorizationPolicyBuilder> configure)
    {
        options.AddPolicy(policy.ToString(), configure);
    }

    public static void AddPolicies(this AuthorizationOptions options)
    {
        options.AddPolicy(AppPolicy.UserOrAdmin, p =>
            p.RequireAuthenticatedUser()
            .RequireRole(nameof(UserRole.User), nameof(UserRole.Admin)));

        options.AddPolicy(AppPolicy.AdminOnly, p =>
            p.RequireRole(nameof(UserRole.Admin)));
    }
}

