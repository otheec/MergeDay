using MergeDay.Api.Features.Fakturoid;
using MergeDay.Api.Features.Fakturoid.Connector;
using MergeDay.Api.Features.Fakturoid.Connector.Request;
using MergeDay.Api.Features.Fakturoid.Connector.Response;
using Microsoft.Extensions.Options;

namespace MergeDay.Api.Common;

public class FakturoidService
{
    private readonly IFakturoidApi _api;
    private readonly FakturoidOptions _options;

    public FakturoidService(IFakturoidApi api, IOptions<FakturoidOptions> options)
    {
        _api = api;
        _options = options.Value;
    }

    public async Task<FakturoidInvoiceResponseDto> CreateInvoiceAsync(FakturoidInvoiceCreateDto dto)
    {
        var reponse = await _api.CreateInvoiceAsync(_options.Slug, dto, _options.UserAgent);
        return reponse;
    }
}
