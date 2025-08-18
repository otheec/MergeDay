using MergeDay.Api.Features.Fakturoid.Connector.Request;
using MergeDay.Api.Features.Fakturoid.Connector.Response;
using Refit;

namespace MergeDay.Api.Features.Fakturoid.Connector;

public interface IFakturoidApi
{
    [Post("/accounts/slug-placeholder/invoices.json")]
    Task<FakturoidInvoiceResponseDto> CreateInvoiceAsync(
        [Body] FakturoidInvoiceCreateDto invoice,
        [Header("Accept")] string accept = "application/json");
}
