using MergeDay.Api.Common;
using MergeDay.Api.Endpoints;
using MergeDay.Api.Features.Fakturoid.Connector.Request;
using MergeDay.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static MergeDay.Api.Features.Fakturoid.CreateFakturoidInvoiceEndpoint;

namespace MergeDay.Api.Features.Invoices;

public static class CreateNewInvoiceEndpoint
{
    public record CreateNewInvoiceRequest(DateTime From, DateTime to);

    [EndpointGroup("Invoices")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapStandardPost<CreateNewInvoiceRequest, IResult>("/invoices", Handler)
                .WithName("Create and submit new invoice")
                .WithSummary("Creates a new invoice in Fakturoid.")
                .RequireAuthorization();
        }
    }

    public static async Task<IResult> Handler(
        [FromBody] CreateNewInvoiceRequest req,
        [FromServices] FakturoidService fakturoidService,
        [FromServices] TogglService togglService,
        [FromServices] MergeDayDbContext dbContext)
    {
        // Add time entries validation (project not null, descrption not null,...
        // filter inly time entries that have already stoped
        var timeEntries = await togglService.GetTimeEntriesAsync(req.From, req.to);
        timeEntries = timeEntries
            .Where(t => t.ProjectId != null)
            .Where(t => t.Description != null)
            .Where(t => t.Stop.HasValue)
            .ToList();
        var groupedEntries = timeEntries
            .GroupBy(te => te.ProjectId)
            .ToList();
        var projectIds = groupedEntries
            .Select(g => g.Key)
            .Where(id => id != null)
            .ToList();
        var projects = await dbContext.TogglProjects
            .Where(p => projectIds.Contains(p.TogglId))
            .ToListAsync();

        var dto = new FakturoidInvoiceCreateDto
        {
            SubjectId = 27826666,
            Currency = "CZK",
            PaymentMethod = "bank",
            Lines = groupedEntries.Select(g => new FakturoidInvoiceLineDto()
            {
                Name = projects.First(p => p.TogglId == g.Key).Name,
                UnitPrice = 100, // TODO: Add real price from user profile
                Quantity = (decimal)g.Sum(t => (t.Stop!.Value - t.Start).TotalHours)
            }).ToList(),
        };

        var invoice = await fakturoidService.CreateInvoiceAsync(dto);

        return Results.Created(invoice.Html_Url,
            new CreateFakturoidInvoiceResponse(invoice.Id, invoice.Number, invoice.Pdf_Url));
    }
}
