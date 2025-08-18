using Refit;

namespace MergeDay.Api.Features.Fakturoid.Auth;

public interface IFakturoidAuthApi
{
    [Post("/oauth/token")]
    Task<FakturoidTokenResponse> GetTokenAsync(
        [Body(BodySerializationMethod.Serialized)] FakturoidTokenRequest request,
        [Header("Authorization")] string authorization,
        [Header(Constants_Fakturoid.UserAgentHeader)] string userAgent,
        [Header("Accept")] string accept = "application/json"
    );
}


