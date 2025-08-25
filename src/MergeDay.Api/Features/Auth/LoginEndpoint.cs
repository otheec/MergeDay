using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using MergeDay.Api.Domain.Entities;
using MergeDay.Api.Endpoints;
using MergeDay.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MergeDay.Api.Features.Auth;

public static class LoginEndpoint
{
    public record LoginRequest(string Email, string Password);

    public record LoginResponse(string token, DateTime expires, string refreshToken, DateTime refreshTokenExpires);

    [EndpointGroup("Auth")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapStandardPost<LoginRequest, string?>("login", Handler)
                .WithName("Login user")
                .WithSummary("Authenticate and return a JWT token with roles and a refresh token.")
                .AllowAnonymous();
        }
    }

    public static async Task<IResult> Handler(
        [FromBody] LoginRequest request,
        [FromServices] SignInManager<ApplicationUser> signInManager,
        [FromServices] UserManager<ApplicationUser> userManager,
        [FromServices] MergeDayDbContext db,
        IOptions<JwtOptions> jwtOptions)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return Results.Unauthorized();

        var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
            return Results.Unauthorized();

        var accessTokenExpiry = DateTime.UtcNow.AddHours(2);
        var tokenString = await GenerateAccessTokenAsync(userManager, user, jwtOptions.Value, accessTokenExpiry);

        var refreshTokenPlain = GenerateSecureToken();
        var refreshTokenHash = Sha256(refreshTokenPlain);
        var refreshExpiry = DateTime.UtcNow.AddDays(30);

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            TokenHash = refreshTokenHash,
            ExpiresAt = refreshExpiry,
            CreatedAt = DateTime.UtcNow,
            ApplicationUserId = user.Id
        };

        db.RefreshTokens.Add(refreshToken);
        await db.SaveChangesAsync();

        return Results.Ok(new LoginResponse(tokenString, accessTokenExpiry, refreshTokenPlain, refreshExpiry));
    }

    private static async Task<string> GenerateAccessTokenAsync(
        UserManager<ApplicationUser> userManager,
        ApplicationUser user,
        JwtOptions jwtOptions,
        DateTime expires)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Name, user.Name),
            new(JwtRegisteredClaimNames.FamilyName, user.Lastname)
        };

        var roles = await userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtOptions.Issuer,
            audience: jwtOptions.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateSecureToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }

    private static string Sha256(string value)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToBase64String(bytes);
    }
}
