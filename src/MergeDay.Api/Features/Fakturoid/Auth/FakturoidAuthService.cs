using MergeDay.Api.Features.Fakturoid;
using MergeDay.Api.Features.Fakturoid.Auth;
using Microsoft.Extensions.Options;

public class FakturoidAuthService
{
    private readonly IFakturoidAuthApi _authApi;
    private readonly FakturoidOptions _options;

    private string? _accessToken;
    private DateTime _expiresAt;

    public FakturoidAuthService(IFakturoidAuthApi authApi, IOptions<FakturoidOptions> options)
    {
        _authApi = authApi;
        _options = options.Value;
    }

    public async Task<string> GetAccessTokenAsync()
    {
        if (_accessToken != null && DateTime.UtcNow < _expiresAt)
            return _accessToken;

        var basicAuth = "Basic " +
            Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{_options.ClientId}:{_options.ClientSecret}"));

        var response = await _authApi.GetTokenAsync(
            new FakturoidTokenRequest(),
            basicAuth,
            _options.UserAgent
        );

        _accessToken = response.AccessToken;
        _expiresAt = DateTime.UtcNow.AddSeconds(response.ExpiresIn - 60);
        return _accessToken;
    }
}
