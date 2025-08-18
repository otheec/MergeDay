namespace MergeDay.Api.Features.Fakturoid.Connector.Response;

public class FakturoidInvoiceResponseDto
{
    public int Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Html_Url { get; set; } = string.Empty;
    public string Pdf_Url { get; set; } = string.Empty;
}
