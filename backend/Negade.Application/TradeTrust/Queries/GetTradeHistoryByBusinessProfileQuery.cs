using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Negade.Application.Common.Interfaces;
using Negade.Application.TradeTrust.Common;

namespace Negade.Application.TradeTrust.Queries;

public record GetTradeHistoryByBusinessProfileQuery(Guid BusinessProfileId) : IRequest<IEnumerable<TradeHistoryDto>>;

public class GetTradeHistoryByBusinessProfileQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
    : IRequestHandler<GetTradeHistoryByBusinessProfileQuery, IEnumerable<TradeHistoryDto>>
{
    public async Task<IEnumerable<TradeHistoryDto>> Handle(
        GetTradeHistoryByBusinessProfileQuery request,
        CancellationToken cancellationToken)
    {
        var history = await dbContext.TradeHistory
            .AsNoTracking()
            .Where(record => record.BusinessProfileId == request.BusinessProfileId)
            .OrderByDescending(record => record.TradeDate)
            .ToListAsync(cancellationToken);

        return mapper.Map<IEnumerable<TradeHistoryDto>>(history);
    }
}
