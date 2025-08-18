using MergeDay.Api.Common;
using MergeDay.Api.Endpoints;
using MergeDay.Api.Features.Fakturoid.Connector.Request;
using Microsoft.AspNetCore.Mvc;

namespace MergeDay.Api.Features.Fakturoid;

public static class CreateFakturoidInvoiceEndpoint
{
    public record CreateFakturoidInvoiceRequest(
        int SubjectId,
        decimal Amount,
        string Description
    );

    public record CreateFakturoidInvoiceResponse(int Id, string Number, string PdfUrl);

    [EndpointGroup("Fakturoid")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapStandardPost<CreateFakturoidInvoiceRequest, CreateFakturoidInvoiceResponse>("/invoices", Handler)
                .WithName("CreateFakturoidInvoice")
                .WithSummary("Creates a new invoice in Fakturoid.")
                .RequireAuthorization(AppPolicy.UserOrAdmin);
        }
    }

    public static async Task<IResult> Handler(
        [FromBody] CreateFakturoidInvoiceRequest request,
        [FromServices] FakturoidService service)
    {
        var dto = new FakturoidInvoiceCreateDto
        {
            SubjectId = 27826666,
            Currency = "CZK",
            PaymentMethod = "bank",
            Lines = new List<FakturoidInvoiceLineDto>
            {
                new()
                {
                    Name = "Popisek",
                    UnitPrice = 10,
                    Quantity = 1
                }
            }
        };

        var invoice = await service.CreateInvoiceAsync(dto);

        return Results.Created(invoice.Html_Url,
            new CreateFakturoidInvoiceResponse(invoice.Id, invoice.Number, invoice.Pdf_Url));
    }
}
