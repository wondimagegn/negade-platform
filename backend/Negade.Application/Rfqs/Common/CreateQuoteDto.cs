using System.ComponentModel.DataAnnotations;

namespace Negade.Application.Rfqs.Common;

public class CreateQuoteDto
{
    [Required]
    public Guid SupplierId { get; set; }

    [Range(0.01, 9999999999.99)]
    public decimal UnitPrice { get; set; }

    [Range(0.01, 9999999999.99)]
    public decimal QuantityAvailable { get; set; }

    [Range(0, 365)]
    public int DeliveryTimeInDays { get; set; }

    [MaxLength(2000)]
    public string? Notes { get; set; }
}
