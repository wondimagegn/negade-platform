using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Negade.Application.Common.Interfaces;
using Negade.Application.Rfqs.Common;
using Negade.Domain.Entities;

namespace Negade.Application.Rfqs.Commands;

public record CreateQuoteCommand(Guid RfqId, CreateQuoteDto Quote, Guid? SupplierUserId) : IRequest<QuoteDto?>;

public class CreateQuoteCommandHandler(IApplicationDbContext dbContext, IMapper mapper)
    : IRequestHandler<CreateQuoteCommand, QuoteDto?>
{
    public async Task<QuoteDto?> Handle(CreateQuoteCommand request, CancellationToken cancellationToken)
    {
        var rfq = await dbContext.Rfqs.FirstOrDefaultAsync(r => r.Id == request.RfqId, cancellationToken);
        var supplier = await dbContext.BusinessProfiles.FirstOrDefaultAsync(
            s => s.Id == request.Quote.SupplierId,
            cancellationToken);

        if (rfq is null || supplier is null)
        {
            return null;
        }

        if (request.SupplierUserId is not null && supplier.OwnerUserId != request.SupplierUserId)
        {
            return null;
        }

        var quote = mapper.Map<Quote>(request.Quote);
        quote.Id = Guid.NewGuid();
        quote.RfqId = request.RfqId;
        quote.SupplierUserId = request.SupplierUserId;
        quote.CreatedAt = DateTime.UtcNow;

        rfq.QuoteCount += 1;

        await dbContext.Quotes.AddAsync(quote, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        quote.Supplier = supplier;
        return mapper.Map<QuoteDto>(quote);
    }
}
