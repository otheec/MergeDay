using System.ComponentModel.DataAnnotations.Schema;

namespace MergeDay.Api.Domain.Entities;

public class Bill
{
    public Guid Id { get; set; }

    public Guid ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; } = null!;

    public required string Name { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public required decimal Total { get; set; }

    public required string IBAN { get; set; } = string.Empty;

    public string? Note { get; set; }

    public List<BillItem> Items { get; set; } = [];

    public DateTime CreatedAt { get; set; }
    public DateTime OrderDate { get; set; }

    public DateTime? PaidAt { get; set; }
    public bool IsPaid { get; set; }
}
