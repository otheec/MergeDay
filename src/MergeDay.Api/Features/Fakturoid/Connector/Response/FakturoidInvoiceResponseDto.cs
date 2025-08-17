namespace MergeDay.Api.Features.Fakturoid.Connector.Response;

public class FakturoidInvoiceResponseDto
{
    public int Id { get; set; }
    public string Number { get; set; }
    public string Status { get; set; }
    public string Html_Url { get; set; }
    public string Pdf_Url { get; set; }
}
