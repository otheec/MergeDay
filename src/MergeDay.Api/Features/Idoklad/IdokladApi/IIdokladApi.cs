using MergeDay.Api.Features.Idoklad.IdokladApi.Dto;
using Refit;

namespace MergeDay.Api.Features.Idoklad.IdokladApi;

public interface IIdokladApi
{
    // Get default invoice model
    [Get("/v3/IssuedInvoices/Default")]
    Task<IdokladInvoiceDto> GetDefaultInvoiceAsync([Query] int? templateId = null);

    // Create new invoice
    [Post("/v3/IssuedInvoices")]
    Task<IdokladInvoiceResponseDto> CreateInvoiceAsync([Body] IdokladInvoiceDto invoice);
}

