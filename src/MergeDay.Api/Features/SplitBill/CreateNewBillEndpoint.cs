using System.Security.Claims;
using MergeDay.Api.Common;
using MergeDay.Api.Domain.Entities;
using MergeDay.Api.Endpoints;
using MergeDay.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace MergeDay.Api.Features.SplitBill;

public static class CreateNewBillEndpoint
{
    public record CreateNewBillRequest(
        string Name,
        decimal Total,
        string IBAN,
        DateTime OrderDate,
        string? Note,
        List<CreateNewBillItemRequest> Items
    );

    public record CreateNewBillItemRequest(
        Guid ApplicationUserId,
        decimal Price
    );

    public record CreateNewBillResponse(
        Guid Id,
        string Name,
        decimal Total,
        DateTime OrderDate,
        string? Note,
        bool IsPaid,
        IEnumerable<BillItemResponse> Items
    );

    public record BillItemResponse(
        Guid Id,
        Guid ApplicationUserId,
        decimal Price,
        bool IsPaid
    );


    [EndpointGroup("Bills")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapStandardPost<CreateNewBillRequest, CreateNewBillResponse>("", Handler)
                .WithName("CreateNewBill")
                .WithSummary("Create a new bill")
                .WithDescription("Creates a new bill with the provided items.")
                .Produces<CreateNewBillResponse>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status500InternalServerError)
                .RequireAuthorization(AppPolicy.UserOrAdmin);
        }
    }

    public static async Task<IResult> Handler(
        [FromBody] CreateNewBillRequest req,
        [FromServices] MergeDayDbContext dbContext,
        [FromServices] IHttpContextAccessor httpContextAccessor)
    {
        var userIdStr = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
            return Results.Unauthorized();

        var bill = new Bill
        {
            ApplicationUserId = userId,
            Name = req.Name,
            Total = req.Total,
            IBAN = req.IBAN,
            Note = req.Note,
            OrderDate = req.OrderDate,
            CreatedAt = DateTime.UtcNow,
            IsPaid = false,
            BillItems = req.Items.Select(i => new BillItems
            {
                ApplicationUserId = i.ApplicationUserId,
                Price = i.Price,
                IsPaid = false
            }).ToList()
        };

        dbContext.Bills.Add(bill);
        await dbContext.SaveChangesAsync();

        var response = new CreateNewBillResponse(
            bill.Id,
            bill.Name,
            bill.Total,
            bill.OrderDate,
            bill.Note,
            bill.IsPaid,
            bill.BillItems.Select(i => new BillItemResponse(i.Id, i.ApplicationUserId, i.Price, i.IsPaid))
        );

        return Results.Ok(response);
    }
}
