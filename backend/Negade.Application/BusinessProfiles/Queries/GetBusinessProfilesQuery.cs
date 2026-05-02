using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Negade.Application.BusinessProfiles.Common;
using Negade.Application.Common.Interfaces;

namespace Negade.Application.BusinessProfiles.Queries;

public record GetBusinessProfilesQuery(string? Region, string? BusinessType, bool VerifiedOnly)
    : IRequest<IEnumerable<BusinessProfileDto>>;

public class GetBusinessProfilesQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
    : IRequestHandler<GetBusinessProfilesQuery, IEnumerable<BusinessProfileDto>>
{
    public async Task<IEnumerable<BusinessProfileDto>> Handle(
        GetBusinessProfilesQuery request,
        CancellationToken cancellationToken)
    {
        var query = dbContext.BusinessProfiles.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Region))
        {
            query = query.Where(profile => profile.Region.ToLower().Contains(request.Region.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(request.BusinessType))
        {
            query = query.Where(profile => profile.BusinessType.ToLower().Contains(request.BusinessType.ToLower()));
        }

        if (request.VerifiedOnly)
        {
            query = query.Where(profile => profile.VerificationStatus == "Verified");
        }

        var profiles = await query
            .OrderByDescending(profile => profile.VerificationStatus == "Verified")
            .ThenByDescending(profile => profile.RatingAverage)
            .ThenBy(profile => profile.BusinessName)
            .ToListAsync(cancellationToken);

        return mapper.Map<IEnumerable<BusinessProfileDto>>(profiles);
    }
}
