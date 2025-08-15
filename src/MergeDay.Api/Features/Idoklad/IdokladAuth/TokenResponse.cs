using System.Text.Json.Serialization;

namespace MergeDay.Api.Features.Idoklad.IdokladAuth;

public record TokenResponse(
    [property: JsonPropertyName("access_token")] string AccessToken,
    [property: JsonPropertyName("expires_in")] int ExpiresIn,
    [property: JsonPropertyName("token_type")] string TokenType
);
