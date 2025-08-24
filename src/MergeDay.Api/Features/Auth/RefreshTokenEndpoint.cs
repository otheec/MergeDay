using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using MergeDay.Api.Domain.Entities;
using MergeDay.Api.Endpoints;
using MergeDay.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MergeDay.Api.Features.Auth;

public static class RefreshTokenEndpoint
{
    public record RefreshRequest(string RefreshToken);
    public record RefreshResponse(string token, DateTime expires, string refreshToken, DateTime refreshTokenExpires);

    [EndpointGroup("Auth")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapStandardPost<RefreshRequest, IResult>("refresh", Handler)
               .WithName("Refresh access token")
               .WithSummary("Use a refresh token to get a new access token and rotated refresh token.")
               .AllowAnonymous();
        }
    }

    public static async Task<IResult> Handler(
        [FromBody] RefreshRequest request,
        [FromServices] MergeDayDbContext db,
        [FromServices] UserManager<ApplicationUser> userManager,
        IOptions<JwtOptions> jwtOptions)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return Results.BadRequest("Refresh token is required.");

        var providedHash = Sha256(request.RefreshToken);

        var stored = await db.RefreshTokens
            .Include(rt => rt.ApplicationUser)
            .FirstOrDefaultAsync(rt => rt.TokenHash == providedHash);

        if (stored is null)
            return Results.Unauthorized();

        var newPlainToken = GenerateSecureToken();
        var newHash = Sha256(newPlainToken);
        var newExpiry = DateTime.UtcNow.AddDays(30);

        stored.ReplacedByTokenHash = newHash;

        var newToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            TokenHash = newHash,
            ExpiresAt = newExpiry,
            CreatedAt = DateTime.UtcNow,
            ApplicationUserId = stored.ApplicationUserId
        };

        db.RefreshTokens.Add(newToken);

        var accessTokenExpiry = DateTime.UtcNow.AddHours(2);
        var newAccessToken = await GenerateAccessTokenAsync(userManager, stored.ApplicationUser, jwtOptions.Value, accessTokenExpiry);

        await db.SaveChangesAsync();

        return Results.Ok(new RefreshResponse(newAccessToken, accessTokenExpiry, newPlainToken, newExpiry));
    }

    private static async Task<string> GenerateAccessTokenAsync(
        UserManager<ApplicationUser> userManager,
        ApplicationUser user,
        JwtOptions jwtOptions,
        DateTime expires)
    {
        var roles = await userManager.GetRolesAsync(user);
        var claims = new List<System.Security.Claims.Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id.ToString())
        };
        claims.AddRange(roles.Select(r => new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, r)));

        var key = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key));
        var creds = new Microsoft.IdentityModel.Tokens.SigningCredentials(key, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256);

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
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(value));
        return Convert.ToBase64String(bytes);
    }
}
