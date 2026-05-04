using MediatR;
using Microsoft.EntityFrameworkCore;
using Negade.Application.Common.Interfaces;
using Negade.Application.Rfqs.Common;

namespace Negade.Application.Rfqs.Queries;

public record GetMyQuotesQuery(Guid SupplierUserId) : IRequest<IEnumerable<SupplierQuoteDto>>;

public class GetMyQuotesQueryHandler(IApplicationDbContext dbContext)
    : IRequestHandler<GetMyQuotesQuery, IEnumerable<SupplierQuoteDto>>
{
    public async Task<IEnumerable<SupplierQuoteDto>> Handle(GetMyQuotesQuery request, CancellationToken cancellationToken)
    {
        return await dbContext.Quotes
            .AsNoTracking()
            .Include(quote => quote.Supplier)
            .Include(quote => quote.Rfq)
            .Where(quote => quote.SupplierUserId == request.SupplierUserId)
            .OrderByDescending(quote => quote.CreatedAt)
            .Select(quote => new SupplierQuoteDto(
                quote.Id,
                quote.RfqId,
                quote.SupplierId,
                quote.SupplierUserId,
                quote.Supplier.BusinessName,
                quote.UnitPrice,
                quote.QuantityAvailable,
                quote.DeliveryTimeInDays,
                quote.Notes,
                quote.CreatedAt,
                quote.Rfq.ProductName,
                quote.Rfq.Category,
                quote.Rfq.Quantity,
                quote.Rfq.Unit,
                quote.Rfq.DeliveryRegion,
                quote.Rfq.DeliveryCity,
                quote.Rfq.Status,
                quote.Rfq.BuyerName,
                quote.Rfq.BuyerBusinessName))
            .ToListAsync(cancellationToken);
    }
}
