using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Negade.Application.BusinessProfiles.Common;
using Negade.Application.Common.Interfaces;

namespace Negade.Application.BusinessProfiles.Commands;

public record UpdateBusinessProfileCommand(Guid BusinessProfileId, UpdateBusinessProfileDto BusinessProfile)
    : IRequest<BusinessProfileDto?>;

public class UpdateBusinessProfileCommandHandler(IApplicationDbContext dbContext, IMapper mapper)
    : IRequestHandler<UpdateBusinessProfileCommand, BusinessProfileDto?>
{
    public async Task<BusinessProfileDto?> Handle(
        UpdateBusinessProfileCommand request,
        CancellationToken cancellationToken)
    {
        var profile = await dbContext.BusinessProfiles.FirstOrDefaultAsync(
            candidate => candidate.Id == request.BusinessProfileId,
            cancellationToken);

        if (profile is null)
        {
            return null;
        }

        mapper.Map(request.BusinessProfile, profile);
        await dbContext.SaveChangesAsync(cancellationToken);
        return mapper.Map<BusinessProfileDto>(profile);
    }
}
