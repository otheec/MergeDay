namespace MergeDay.Api.Features.Idoklad.IdokladApi.Dto;

public class IdokladInvoiceResponseDto
{
    public IdokladInvoiceDataDto Data { get; set; }
}

public class IdokladInvoiceDataDto
{
    public int Id { get; set; }
    public string DocumentNumber { get; set; }
}