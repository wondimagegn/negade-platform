using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Negade.Application.Common.Interfaces;
using Negade.Application.TradeTrust.Common;

namespace Negade.Application.TradeTrust.Queries;

public record GetTradeRatingsBySupplierQuery(Guid SupplierId) : IRequest<IEnumerable<TradeRatingDto>>;

public class GetTradeRatingsBySupplierQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
    : IRequestHandler<GetTradeRatingsBySupplierQuery, IEnumerable<TradeRatingDto>>
{
    public async Task<IEnumerable<TradeRatingDto>> Handle(
        GetTradeRatingsBySupplierQuery request,
        CancellationToken cancellationToken)
    {
        var ratings = await dbContext.TradeRatings
            .AsNoTracking()
            .Where(rating => rating.SupplierId == request.SupplierId)
            .OrderByDescending(rating => rating.CreatedAt)
            .ToListAsync(cancellationToken);

        return mapper.Map<IEnumerable<TradeRatingDto>>(ratings);
    }
}
