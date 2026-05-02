namespace Negade.Domain.Entities;

public class AppUser
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "Trader";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<BusinessProfile> BusinessProfiles { get; set; } = [];
}
