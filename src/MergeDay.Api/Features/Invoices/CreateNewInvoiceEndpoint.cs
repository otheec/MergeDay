using MergeDay.Api.Common;
using MergeDay.Api.Endpoints;
using MergeDay.Api.Features.Fakturoid.Connector.Request;
using Microsoft.AspNetCore.Mvc;
using static MergeDay.Api.Features.Invoices.CreateNewInvoiceFromTogglEndpoint;

namespace MergeDay.Api.Features.Invoices;

public static class CreateNewInvoiceEndpoint
{
    public record CreateNewInvoiceRequest(IEnumerable<CreateNewInvoiceRequestLine> lines);
    public record CreateNewInvoiceRequestLine(decimal Quantity, decimal UnitPrice, string Unit);

    public record CreateNewInvoiceResponse(int Id, string Number, string HtmlUrl, string PdfUrl);

    [EndpointGroup("Invoices")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapStandardPost<CreateNewInvoiceRequest, IResult>("/invoices", Handler)
                .WithName("Create and submit new invoice")
                .WithSummary("Creates a new invoice in Fakturoid.")
                .RequireAuthorization(AppPolicy.UserOrAdmin);
        }
    }

    public static async Task<IResult> Handler(
        [FromBody] CreateNewInvoiceRequest req,
        [FromServices] FakturoidService fakturoidService,
        ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger(nameof(CreateNewInvoiceEndpoint));

        var dto = new FakturoidInvoiceCreateDto
        {
            SubjectId = 27826666,
            Currency = "CZK",
            PaymentMethod = "bank",
            Lines = req.lines.Select(l => new FakturoidInvoiceLineDto()
            {
                Name = l.Unit,
                UnitPrice = l.UnitPrice,
                Quantity = l.Quantity
            }).ToList()
        };

        var createdInvoice = await fakturoidService.CreateInvoiceAsync(dto);
        if (createdInvoice == null)
        {
            logger.LogError("Failed to create invoice in Fakturoid.");
            return Results.StatusCode(500);
        }

        logger.LogInformation("Created invoice {InvoiceId} in Fakturoid.", createdInvoice.Id);
        return Results.Ok(new CreateNewInvoiceResponse(createdInvoice.Id, createdInvoice.Number, createdInvoice.Html_Url, createdInvoice.Pdf_Url));
    }
}
