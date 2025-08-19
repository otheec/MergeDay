using System.Globalization;
using MergeDay.Api.Endpoints;
using MergeDay.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MergeDay.Api.Features.SplitBill;

public static class GetBillItemQrCodeEndpoint
{
    public record GetBillItemQrCodeResponse(bool IsPaid, string QrCode);

    [EndpointGroup("Bills")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapStandardGet<string>("/bill-item/{billItemId}/qr-code", Handler)
                .WithName("GetBillItemQrCode")
                .WithSummary("Get QR code for a bill item")
                .WithDescription("Retrieves the QR code for a specific bill item.")
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .AllowAnonymous();
        }
    }

    public static async Task<IResult> Handler(
        [FromRoute] Guid billItemId,
        [FromServices] MergeDayDbContext dbContext)
    {
        var billItem = await dbContext.BillItems
            .Include(bi => bi.Bill)
            .FirstOrDefaultAsync(bi => bi.Id == billItemId);

        if (billItem == null)
        {
            return Results.NotFound();
        }

        var spayd = GenerateSpayd(billItem.Bill.IBAN, billItem.Price, "Za chalky");

        return Results.Ok(new GetBillItemQrCodeResponse(billItem.IsPaid, spayd));
    }

    public static string GenerateSpayd(string IBAN, decimal amount, string message)
    {
        return $"SPD*1.0*ACC:{IBAN}*AM:{amount.ToString("F2", CultureInfo.InvariantCulture)}*CC:CZK*MSG:{message}";
    }
}
