using Microsoft.AspNetCore.Identity;

namespace Negade.Domain.Entities;

public class AppUser : IdentityUser<Guid>
{
    public string FullName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<BusinessProfile> BusinessProfiles { get; set; } = [];
}
