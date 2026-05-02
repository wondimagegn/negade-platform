using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Negade.Application.BusinessProfiles.Common;
using Negade.Application.Common.Interfaces;

namespace Negade.Application.BusinessProfiles.Commands;

public record VerifyBusinessProfileCommand(Guid BusinessProfileId, VerifyBusinessProfileDto Verification)
    : IRequest<BusinessProfileDto?>;

public class VerifyBusinessProfileCommandHandler(IApplicationDbContext dbContext, IMapper mapper)
    : IRequestHandler<VerifyBusinessProfileCommand, BusinessProfileDto?>
{
    public async Task<BusinessProfileDto?> Handle(
        VerifyBusinessProfileCommand request,
        CancellationToken cancellationToken)
    {
        var profile = await dbContext.BusinessProfiles.FirstOrDefaultAsync(
            candidate => candidate.Id == request.BusinessProfileId,
            cancellationToken);

        if (profile is null)
        {
            return null;
        }

        profile.VerificationStatus = request.Verification.VerificationStatus;
        await dbContext.SaveChangesAsync(cancellationToken);

        return mapper.Map<BusinessProfileDto>(profile);
    }
}
