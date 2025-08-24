using MergeDay.Api.Common;
using MergeDay.Api.Domain.Entities;
using MergeDay.Api.Endpoints;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MergeDay.Api.Features.Auth;

public static class RegisterUserEndpoint
{
    public record RegisterUserRequest(string Email, string Password, string Name, string Lastname);
    public record RegisterUserResponse(string Email, string Role, string Name, string Lastname);

    [EndpointGroup("Auth")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapStandardPost<RegisterUserRequest, IResult>("register", Handler)
               .WithName("Register user")
               .WithSummary("Register a new user.")
               .AllowAnonymous();
        }
    }

    public static async Task<IResult> Handler(
        [FromBody] RegisterUserRequest request,
        [FromServices] UserManager<ApplicationUser> userManager,
        [FromServices] RoleManager<IdentityRole<Guid>> roleManager)
    {
        if (!await roleManager.RoleExistsAsync(UserRole.User.ToString()))
            await roleManager.CreateAsync(new IdentityRole<Guid>(UserRole.User.ToString()));

        if (!await roleManager.RoleExistsAsync(UserRole.Admin.ToString()))
            await roleManager.CreateAsync(new IdentityRole<Guid>(UserRole.Admin.ToString()));

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            Name = request.Name,
            Lastname = request.Lastname
        };

        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            return Results.BadRequest(result.Errors.Select(e => e.Description));

        var roleToAssign = UserRole.User.ToString();
        await userManager.AddToRoleAsync(user, roleToAssign);

        return Results.Ok(new RegisterUserResponse(user.Email!, roleToAssign, user.Name, user.Lastname));
    }
}
