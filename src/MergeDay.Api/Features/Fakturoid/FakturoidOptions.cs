namespace MergeDay.Api.Features.Fakturoid;

public class FakturoidOptions
{
    public required string UserAgent { get; init; }
    public required string ClientId { get; init; }
    public required string ClientSecret { get; init; }
    public required string Slug { get; init; }
}
