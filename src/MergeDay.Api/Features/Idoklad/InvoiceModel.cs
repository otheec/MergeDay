namespace MergeDay.Api.Features.IDoklad;

public class InvoiceModel
{
    public int CurrencyId { get; set; }
    public string DateOfIssue { get; set; }
    public string DateOfMaturity { get; set; }
    public DateTime DateOfTaxing { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsIncomeTax { get; set; }
    public int PartnerId { get; set; }
    public int PaymentOptionId { get; set; }
    public List<InvoiceItem> Items { get; set; } = new();
}

public class InvoiceItem
{
    public decimal Amount { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int PriceType { get; set; }
    public int VatRateType { get; set; }
}
