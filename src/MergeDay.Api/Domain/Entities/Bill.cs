using System.ComponentModel.DataAnnotations.Schema;

namespace MergeDay.Api.Domain.Entities;

public class Bill
{
    public int Id { get; set; }

    public Guid ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    [Column(TypeName = "decimal(18,2)")]
    public decimal Total { get; set; }
    public string? Note { get; set; }

    public ICollection<BillItems> BillItems { get; set; } = [];

    public DateTime CreatedAt { get; set; }
    public DateTime OrderDate { get; set; }

    public DateTime? PaidAt { get; set; }
    public bool IsPaid { get; set; }
}
