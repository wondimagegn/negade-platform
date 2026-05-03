namespace Negade.Domain.Entities;

public class City
{
    public Guid Id { get; set; }
    public Guid RegionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Region Region { get; set; } = null!;
}
