using MergeDay.Api.Domain.Entities;
using MergeDay.Api.Endpoints;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MergeDay.Api.Features.Auth;

public static class RegisterEndpoint
{
    public record RegisterRequest(string Email, string Password);

    [EndpointGroup("Auth")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapStandardPost<RegisterRequest, IResult>("register", Handler)
                .WithName("Register user")
                .WithSummary("Register a new user.")
                .AllowAnonymous();
        }
    }

    public static async Task<IResult> Handler(
        [FromBody] RegisterRequest request,
        [FromServices] UserManager<ApplicationUser> userManager)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email
        };

        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return Results.BadRequest(result.Errors.Select(e => e.Description));
        }

        return Results.Ok(new { Message = "User registered successfully" });
    }
}
