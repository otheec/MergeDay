namespace MergeDay.Api.Features.Idoklad.IdokladApi.Dto;

public class IdokladInvoiceDto
{
    public int PartnerId { get; set; }
    public int CurrencyId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string DateOfIssue { get; set; }
    public string DateOfMaturity { get; set; }
    public bool IsIncomeTax { get; set; }
    public List<IdokladInvoiceItemDto> Items { get; set; } = new();
}

public class IdokladInvoiceItemDto
{
    public decimal Amount { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int PriceType { get; set; }
    public decimal VatRate { get; set; }
    public int VatRateType { get; set; }
}

