using MergeDay.Api.Features.Idoklad.IdokladAuth;
using Refit;

namespace MergeDay.Api.Features.IDoklad.IdokladAuth;

public interface IIdokladAuthApi
{
    [Post("/connect/token")]
    [Headers("Content-Type: application/x-www-form-urlencoded")]
    Task<TokenResponse> GetToken([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, string> form);
}
