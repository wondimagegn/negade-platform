using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Negade.Application.Common.Interfaces;
using Negade.Application.Rfqs.Common;

namespace Negade.Application.Rfqs.Queries;

public record GetQuotesByRfqQuery(Guid RfqId) : IRequest<IEnumerable<QuoteDto>>;

public class GetQuotesByRfqQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
    : IRequestHandler<GetQuotesByRfqQuery, IEnumerable<QuoteDto>>
{
    public async Task<IEnumerable<QuoteDto>> Handle(GetQuotesByRfqQuery request, CancellationToken cancellationToken)
    {
        var quotes = await dbContext.Quotes
            .AsNoTracking()
            .Include(quote => quote.Supplier)
            .Where(quote => quote.RfqId == request.RfqId)
            .OrderBy(quote => quote.UnitPrice)
            .ToListAsync(cancellationToken);

        return mapper.Map<IEnumerable<QuoteDto>>(quotes);
    }
}
