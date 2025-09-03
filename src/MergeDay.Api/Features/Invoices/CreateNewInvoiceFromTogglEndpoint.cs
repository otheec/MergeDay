using MergeDay.Api.Common;
using MergeDay.Api.Endpoints;
using MergeDay.Api.Features.Fakturoid.Connector.Request;
using MergeDay.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MergeDay.Api.Features.Invoices;

public static class CreateNewInvoiceFromTogglEndpoint
{
    public record CreateNewInvoiceFromTogglRequest(DateTime From, DateTime to, decimal pricePerHour);
    public record CreateNewInvoiceFromTogglResponse(int Id, string Number, string HtmlUrl, string PdfUrl);

    [EndpointGroup("Invoices")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapStandardPost<CreateNewInvoiceFromTogglRequest, CreateNewInvoiceFromTogglResponse>("/invoices-from-toggl", Handler)
                .WithName("Create and submit new invoice from toggle entries")
                .WithSummary("Creates a new invoice in Fakturoid based on toggle entries.")
                .RequireAuthorization(AppPolicy.UserOrAdmin);
        }
    }

    public static async Task<IResult> Handler(
        [FromBody] CreateNewInvoiceFromTogglRequest req,
        [FromServices] FakturoidService fakturoidService,
        [FromServices] TogglService togglService,
        [FromServices] MergeDayDbContext dbContext,
        ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger(nameof(CreateNewInvoiceFromTogglEndpoint));

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

        if (projects.Count != projectIds.Count)
        {
            logger.LogWarning("Some projects from time entries were not found in the database.");
        }

        var dto = new FakturoidInvoiceCreateDto
        {
            SubjectId = 27826666,
            Currency = "CZK",
            PaymentMethod = "bank",
            Lines = groupedEntries.Select(g => new FakturoidInvoiceLineDto()
            {
                Name = projects.FirstOrDefault(p => p.TogglId == g.Key)?.Name ?? string.Empty,
                UnitPrice = req.pricePerHour,
                Quantity = (decimal)g.Sum(t => (t.Stop!.Value - t.Start).TotalHours)
            }).ToList(),
        };

        var createdInvoice = await fakturoidService.CreateInvoiceAsync(dto);

        return Results.Ok(new CreateNewInvoiceFromTogglResponse(createdInvoice.Id, createdInvoice.Number, createdInvoice.Html_Url, createdInvoice.Pdf_Url));
    }
}
