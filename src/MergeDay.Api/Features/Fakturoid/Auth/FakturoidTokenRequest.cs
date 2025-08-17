using System.Text.Json.Serialization;

namespace MergeDay.Api.Features.Fakturoid.Auth;

public class FakturoidTokenRequest
{
    [JsonPropertyName("grant_type")]
    public string GrantType { get; } = "client_credentials";
}

