using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MergeDay.Api.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(256)]
    public string TokenHash { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }

    [MaxLength(256)]
    public string? ReplacedByTokenHash { get; set; }

    // FK to user
    public Guid ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; } = null!;

    [NotMapped]
    public bool IsActive => DateTime.UtcNow < ExpiresAt;
}
