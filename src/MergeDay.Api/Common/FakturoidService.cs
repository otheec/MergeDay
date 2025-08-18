using MergeDay.Api.Features.Fakturoid.Connector;
using MergeDay.Api.Features.Fakturoid.Connector.Request;
using MergeDay.Api.Features.Fakturoid.Connector.Response;

namespace MergeDay.Api.Common;

public class FakturoidService(IFakturoidApi api)
{
    public async Task<FakturoidInvoiceResponseDto> CreateInvoiceAsync(FakturoidInvoiceCreateDto dto)
    {
        var reponse = await api.CreateInvoiceAsync(dto);
        return reponse;
    }
}
