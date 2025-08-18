using System.Net.Http.Headers;

namespace MergeDay.Api.Features.Fakturoid.Auth;

public class FakturoidAuthHandler(FakturoidAuthService auth) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var (token, slug, userAgent) = await auth.GetAccessAsync(cancellationToken);

        request.Headers.Authorization = new AuthenticationHeaderValue(Constants_Fakturoid.AuthHeaderBearer, token);
        request.Headers.Add(Constants_Fakturoid.UserAgentHeader, userAgent);

        if (request.RequestUri != null && request.RequestUri.OriginalString.Contains(Constants_Fakturoid.SlugPlaceholder))
        {
            var newUri = request.RequestUri.OriginalString.Replace(Constants_Fakturoid.SlugPlaceholder, slug);
            request.RequestUri = new Uri(newUri, UriKind.RelativeOrAbsolute);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}