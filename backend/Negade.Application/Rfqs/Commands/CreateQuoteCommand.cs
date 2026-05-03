using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Negade.Application.Common.Interfaces;
using Negade.Application.Rfqs.Common;
using Negade.Domain.Entities;

namespace Negade.Application.Rfqs.Commands;

public enum CreateQuoteFailureReason
{
    None,
    RfqNotFound,
    SupplierNotFound,
    SupplierNotOwned
}

public record CreateQuoteResult(QuoteDto? Quote, CreateQuoteFailureReason FailureReason);

public record CreateQuoteCommand(Guid RfqId, CreateQuoteDto Quote, Guid? SupplierUserId) : IRequest<CreateQuoteResult>;

public class CreateQuoteCommandHandler(IApplicationDbContext dbContext, IMapper mapper)
    : IRequestHandler<CreateQuoteCommand, CreateQuoteResult>
{
    public async Task<CreateQuoteResult> Handle(CreateQuoteCommand request, CancellationToken cancellationToken)
    {
        var rfq = await dbContext.Rfqs.FirstOrDefaultAsync(r => r.Id == request.RfqId, cancellationToken);
        if (rfq is null)
        {
            return new CreateQuoteResult(null, CreateQuoteFailureReason.RfqNotFound);
        }

        var supplier = await dbContext.BusinessProfiles.FirstOrDefaultAsync(
            s => s.Id == request.Quote.SupplierId,
            cancellationToken);

        if (supplier is null)
        {
            return new CreateQuoteResult(null, CreateQuoteFailureReason.SupplierNotFound);
        }

        if (request.SupplierUserId is not null && supplier.OwnerUserId != request.SupplierUserId)
        {
            return new CreateQuoteResult(null, CreateQuoteFailureReason.SupplierNotOwned);
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
        return new CreateQuoteResult(mapper.Map<QuoteDto>(quote), CreateQuoteFailureReason.None);
    }
}
