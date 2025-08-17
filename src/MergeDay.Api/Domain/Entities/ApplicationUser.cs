using Microsoft.AspNetCore.Identity;

namespace MergeDay.Api.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string? TogglApiToken { get; set; }
}
