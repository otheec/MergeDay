using System.ComponentModel.DataAnnotations.Schema;

namespace MergeDay.Api.Domain.Entities;

public class BillItems
{
    public int Id { get; set; }

    public int BillId { get; set; }
    public Bill Bill { get; set; } = null!;

    public Guid ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; } = null!;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }
    public bool IsPaid { get; set; }
}
