using System.Net.Http.Headers;

namespace MergeDay.Api.Features.Fakturoid.Auth;

public class FakturoidAuthHandler : DelegatingHandler
{
    private readonly FakturoidAuthService _auth;

    public FakturoidAuthHandler(FakturoidAuthService auth) => _auth = auth;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _auth.GetAccessTokenAsync();
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await base.SendAsync(request, cancellationToken);
    }
}
