using System.Net.Http.Headers;

public class FakturoidAuthHandler : DelegatingHandler
{
    private readonly FakturoidAuthService _auth;

    public FakturoidAuthHandler(FakturoidAuthService auth) => _auth = auth;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var (token, slug, userAgent) = await _auth.GetAccessAsync(cancellationToken);

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Add("User-Agent", userAgent);

        if (request.RequestUri != null && request.RequestUri.OriginalString.Contains("slug-placeholder"))
        {
            var newUri = request.RequestUri.OriginalString.Replace("slug-placeholder", slug);
            request.RequestUri = new Uri(newUri, UriKind.RelativeOrAbsolute);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
