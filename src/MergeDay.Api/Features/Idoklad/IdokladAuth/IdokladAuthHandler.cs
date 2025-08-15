using System.Net.Http.Headers;
using MergeDay.Api.Features.Idoklad.IdokladApi;

namespace MergeDay.Api.Features.Idoklad.IdokladAuth;

public class IdokladAuthHandler : DelegatingHandler
{
    private readonly IdokladService _service;

    public IdokladAuthHandler(IdokladService service) => _service = service;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var tokenValue = await _service.GetAccessTokenAsync();
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenValue);
        return await base.SendAsync(request, cancellationToken);
    }
}
