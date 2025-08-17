using MergeDay.Api.Features.Fakturoid.Connector.Request;
using MergeDay.Api.Features.Fakturoid.Connector.Response;
using Refit;

namespace MergeDay.Api.Features.Fakturoid.Connector;

public interface IFakturoidApi
{
    [Post("/accounts/{slug}/invoices.json")]
    Task<FakturoidInvoiceResponseDto> CreateInvoiceAsync(
            string slug,
            [Body] FakturoidInvoiceCreateDto invoice,
            [Header("User-Agent")] string userAgent,
            [Header("Accept")] string accept = "application/json");
}
