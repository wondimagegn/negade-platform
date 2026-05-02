using MapsterMapper;
using MediatR;
using Negade.Application.BusinessProfiles.Common;
using Negade.Application.Common.Interfaces;
using Negade.Domain.Entities;

namespace Negade.Application.BusinessProfiles.Commands;

public record CreateBusinessProfileCommand(CreateBusinessProfileDto BusinessProfile) : IRequest<BusinessProfileDto>;

public class CreateBusinessProfileCommandHandler(IApplicationDbContext dbContext, IMapper mapper)
    : IRequestHandler<CreateBusinessProfileCommand, BusinessProfileDto>
{
    public async Task<BusinessProfileDto> Handle(CreateBusinessProfileCommand request, CancellationToken cancellationToken)
    {
        var businessProfile = mapper.Map<BusinessProfile>(request.BusinessProfile);
        businessProfile.Id = Guid.NewGuid();
        businessProfile.VerificationStatus = "Pending";
        businessProfile.CreatedAt = DateTime.UtcNow;

        await dbContext.BusinessProfiles.AddAsync(businessProfile, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return mapper.Map<BusinessProfileDto>(businessProfile);
    }
}
