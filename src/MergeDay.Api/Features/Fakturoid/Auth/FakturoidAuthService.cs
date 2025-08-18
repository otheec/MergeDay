using System.Security.Claims;
using MergeDay.Api.Features.Fakturoid;
using MergeDay.Api.Features.Fakturoid.Auth;
using MergeDay.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

public class FakturoidAuthService
{
    private readonly IFakturoidAuthApi _authApi;
    private readonly MergeDayDbContext _cbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly FakturoidOptions _options;

    private readonly Dictionary<Guid, (string Token, DateTime ExpiresAt)> _tokenCache = new();

    public FakturoidAuthService(
        IFakturoidAuthApi authApi,
        MergeDayDbContext cbContext,
        IHttpContextAccessor httpContextAccessor,
        IOptions<FakturoidOptions> options)
    {
        _authApi = authApi;
        _cbContext = cbContext;
        _httpContextAccessor = httpContextAccessor;
        _options = options.Value;
    }

    public async Task<(string Token, string Slug, string UserAgent)> GetAccessAsync(CancellationToken cancellationToken = default)
    {
        var userIdStr = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
            throw new InvalidOperationException("No authenticated user.");

        var user = await _cbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
            ?? throw new InvalidOperationException("User not found.");

        if (string.IsNullOrEmpty(user.FakturoidClientId) || string.IsNullOrEmpty(user.FakturoidClientSecret) || string.IsNullOrEmpty(user.FakturoidSlug))
            throw new InvalidOperationException("User does not have Fakturoid credentials configured.");

        if (_tokenCache.TryGetValue(userId, out var entry) && DateTime.UtcNow < entry.ExpiresAt)
            return (entry.Token, user.FakturoidSlug!, user.Email!);

        var basicAuth = "Basic " +
            Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{user.FakturoidClientId}:{user.FakturoidClientSecret}"));

        Console.WriteLine($"POST /oauth/token grant_type=client_credentials, Auth={basicAuth.Substring(0, 15)}..., UA={user.Email}");
        var response = await _authApi.GetTokenAsync(
            new FakturoidTokenRequest(),
            basicAuth,
            _options.UserAgent
        );

        var token = response.AccessToken;
        var expires = DateTime.UtcNow.AddSeconds(response.ExpiresIn - 60);

        _tokenCache[userId] = (token, expires);

        return (token, user.FakturoidSlug!, _options.UserAgent);
    }
}
