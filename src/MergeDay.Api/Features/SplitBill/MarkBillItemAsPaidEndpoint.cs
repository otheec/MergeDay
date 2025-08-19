using System.Security.Claims;
using MergeDay.Api.Common;
using MergeDay.Api.Endpoints;
using MergeDay.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace MergeDay.Api.Features.SplitBill;

public static class MarkBillItemAsPaidEndpoint
{
    [EndpointGroup("Bills")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapStandardPost<EmptyBodyBehavior, IResult>("/bill-item/{billItemId}/mark-paid", Handler)
                .WithName("MarkBillItemAsPaid")
                .WithSummary("Mark a bill item as paid")
                .WithDescription("Marks a specific bill item as paid.")
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound)
                .RequireAuthorization(AppPolicy.UserOrAdmin);
        }
    }

    public static async Task<IResult> Handler(
        [FromRoute] Guid billItemId,
        [FromServices] MergeDayDbContext dbContext,
        [FromServices] HttpContext httpContext)
    {
        var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
        {
            return Results.Unauthorized();
        }

        var billItem = await dbContext.BillItems
            .Include(bi => bi.Bill)
                .ThenInclude(bi => bi.BillItems)
            .FirstOrDefaultAsync(bi => bi.Id == billItemId);

        if (billItem is null || billItem.ApplicationUserId != Guid.Parse(userId))
        {
            return Results.Unauthorized();
        }

        if (billItem == null)
        {
            return Results.NotFound();
        }

        if(billItem.Bill.BillItems.All(bi => bi.IsPaid))
        {
            billItem.Bill.IsPaid = true;
            billItem.Bill.PaidAt = DateTime.UtcNow;
        }

        billItem.IsPaid = true;
        billItem.PaidAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();
        return Results.Ok();
    }
}
