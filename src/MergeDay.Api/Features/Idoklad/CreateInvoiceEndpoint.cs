using MergeDay.Api.Endpoints;
using MergeDay.Api.Features.Idoklad.IdokladApi;
using Microsoft.AspNetCore.Mvc;

namespace MergeDay.Api.Features.IDoklad;

public class CreateInvoiceEndpoint
{
    public record CreateInvoiceRequest(int PartnerId, int CurrencyId, string Description);
    public record CreateInvoiceResponse(int InvoiceId, string DocumentNumber);

    [EndpointGroup("Idoklad")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/invoices", Handler)
               .WithName("Create new invoice")
               .WithSummary("Creates a new invoice in iDoklad.");
        }
    }

    public static async Task<IResult> Handler([FromBody] CreateInvoiceRequest request, [FromServices] IdokladService service)
    {
        var result = await service.CreateInvoiceAsync(request.PartnerId, request.CurrencyId, request.Description);

        if (result?.Data == null)
            return Results.BadRequest("Failed to create invoice.");

        return Results.Ok(new CreateInvoiceResponse(result.Data.Id, result.Data.DocumentNumber));
    }
}

