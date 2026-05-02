using System.ComponentModel.DataAnnotations;

namespace Negade.Application.BusinessProfiles.Common;

public class VerifyBusinessProfileDto
{
    [Required]
    [RegularExpression("^(Pending|Verified|Rejected)$")]
    public string VerificationStatus { get; set; } = "Verified";
}
