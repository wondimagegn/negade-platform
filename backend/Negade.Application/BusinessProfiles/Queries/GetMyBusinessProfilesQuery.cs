using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Negade.Application.BusinessProfiles.Common;
using Negade.Application.Common.Interfaces;

namespace Negade.Application.BusinessProfiles.Queries;

public record GetMyBusinessProfilesQuery(Guid OwnerUserId) : IRequest<IEnumerable<BusinessProfileDto>>;

public class GetMyBusinessProfilesQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
    : IRequestHandler<GetMyBusinessProfilesQuery, IEnumerable<BusinessProfileDto>>
{
    public async Task<IEnumerable<BusinessProfileDto>> Handle(
        GetMyBusinessProfilesQuery request,
        CancellationToken cancellationToken)
    {
        var profiles = await dbContext.BusinessProfiles
            .AsNoTracking()
            .Where(profile => profile.OwnerUserId == request.OwnerUserId)
            .OrderByDescending(profile => profile.CreatedAt)
            .ToListAsync(cancellationToken);

        return mapper.Map<IEnumerable<BusinessProfileDto>>(profiles);
    }
}
