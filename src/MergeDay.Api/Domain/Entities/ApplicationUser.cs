using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace MergeDay.Api.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string? TogglApiToken { get; set; }

    public string? FakturoidSlug { get; set; }
    public string? FakturoidClientId { get; set; }
    public string? FakturoidClientSecret { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? PricePerHour { get; set; }
}
