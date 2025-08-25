using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace MergeDay.Api.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public required string Name { get; set; }
    public required string Lastname { get; set; }

    public string? TogglApiToken { get; set; }

    public string? FakturoidSlug { get; set; }
    public string? FakturoidClientId { get; set; }
    public string? FakturoidClientSecret { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? PricePerHour { get; set; }

    public List<string> IBANs { get; set; } = [];

    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
}
