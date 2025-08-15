using MergeDay.Api.Features.Idoklad.IdokladApi.Dto;
using MergeDay.Api.Features.IDoklad.IdokladAuth;

namespace MergeDay.Api.Features.Idoklad.IdokladApi;

public class IdokladService
{
    private readonly IIdokladAuthApi _authApi;
    private readonly IIdokladApi _api;
    private readonly IConfiguration _config;
    private string? _token;
    private DateTime _tokenExpiry = DateTime.MinValue;

    public IdokladService(IIdokladAuthApi authApi, IIdokladApi invoicesApi, IConfiguration config)
    {
        _authApi = authApi;
        _api = invoicesApi;
        _config = config;
    }

    public async Task<string> GetAccessTokenAsync()
    {
        if (!string.IsNullOrEmpty(_token) && DateTime.UtcNow < _tokenExpiry)
            return _token;

        var form = new Dictionary<string, string>
        {
            ["grant_type"] = "client_credentials",
            ["application_id"] = _config["Idoklad:ApplicationId"]!,
            ["client_id"] = _config["Idoklad:ClientId"]!,
            ["client_secret"] = _config["Idoklad:ClientSecret"]!,
            ["scope"] = "idoklad_api offline_access"
        };

        var tokenResponse = await _authApi.GetToken(form);

        _token = tokenResponse.AccessToken;
        _tokenExpiry = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn - 60);

        return _token;
    }

    public async Task<IdokladInvoiceResponseDto> CreateInvoiceAsync(int partnerId, int currencyId, string description)
    {
        // Step 1: Get a default invoice from iDoklad
        var defaultInvoice = await _api.GetDefaultInvoiceAsync();

        // Step 2: Modify with your data
        defaultInvoice.PartnerId = partnerId;
        defaultInvoice.CurrencyId = currencyId;
        defaultInvoice.Description = description;
        defaultInvoice.DateOfIssue = DateTime.UtcNow.ToString("yyyy-MM-dd");
        defaultInvoice.DateOfMaturity = DateTime.UtcNow.AddDays(14).ToString("yyyy-MM-dd");
        defaultInvoice.IsIncomeTax = true;
        defaultInvoice.Items = new List<IdokladInvoiceItemDto>
        {
            new IdokladInvoiceItemDto
            {
                Name = "Consulting Services",
                Amount = 1,
                UnitPrice = 1000m,
                PriceType = 0, // WithVat
                VatRate = 21,
                VatRateType = 1 // Basic
            }
        };

        // Step 3: Create invoice
        return await _api.CreateInvoiceAsync(defaultInvoice);
    }
}
