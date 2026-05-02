using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Negade.Application.Common.Interfaces;
using Negade.Application.Rfqs.Common;

namespace Negade.Application.Rfqs.Queries;

public record GetRfqsQuery(string? Category, string? Region, string? Status) : IRequest<IEnumerable<RfqDto>>;

public class GetRfqsQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
    : IRequestHandler<GetRfqsQuery, IEnumerable<RfqDto>>
{
    public async Task<IEnumerable<RfqDto>> Handle(GetRfqsQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Rfqs.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Category))
        {
            query = query.Where(rfq => rfq.Category.ToLower().Contains(request.Category.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(request.Region))
        {
            query = query.Where(rfq => rfq.DeliveryRegion.ToLower().Contains(request.Region.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            query = query.Where(rfq => rfq.Status == request.Status);
        }

        var rfqs = await query
            .OrderByDescending(rfq => rfq.CreatedAt)
            .ToListAsync(cancellationToken);

        return mapper.Map<IEnumerable<RfqDto>>(rfqs);
    }
}
