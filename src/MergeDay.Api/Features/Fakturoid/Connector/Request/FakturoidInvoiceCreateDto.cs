using System.Text.Json.Serialization;

namespace MergeDay.Api.Features.Fakturoid.Connector.Request;
public class FakturoidInvoiceCreateDto
{
    [JsonPropertyName("subject_id")]
    public required int SubjectId { get; set; }

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = "CZK";

    [JsonPropertyName("payment_method")]
    public string PaymentMethod { get; set; } = "bank";

    [JsonPropertyName("lines")]
    public List<FakturoidInvoiceLineDto> Lines { get; set; } = new();
}

public class FakturoidInvoiceLineDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    [JsonPropertyName("quantity")]
    public decimal Quantity { get; set; } = 1;

    [JsonPropertyName("unit_price")]
    public decimal UnitPrice { get; set; }
}
