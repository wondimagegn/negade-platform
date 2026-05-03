using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Negade.Application.Common.Interfaces;
using Negade.Application.Rfqs.Common;

namespace Negade.Application.Rfqs.Commands;

public record UpdateRfqStatusCommand(Guid RfqId, UpdateRfqStatusDto Status) : IRequest<RfqDto?>;

public class UpdateRfqStatusCommandHandler(IApplicationDbContext dbContext, IMapper mapper)
    : IRequestHandler<UpdateRfqStatusCommand, RfqDto?>
{
    private static readonly HashSet<string> AllowedStatuses = new(StringComparer.OrdinalIgnoreCase)
    {
        "Open",
        "Quoted",
        "Awarded",
        "Closed",
        "Cancelled"
    };

    public async Task<RfqDto?> Handle(UpdateRfqStatusCommand request, CancellationToken cancellationToken)
    {
        var rfq = await dbContext.Rfqs.FirstOrDefaultAsync(item => item.Id == request.RfqId, cancellationToken);
        if (rfq is null)
        {
            return null;
        }

        var status = request.Status.Status.Trim();
        if (!AllowedStatuses.Contains(status))
        {
            throw new ArgumentException("Unsupported RFQ status.");
        }

        rfq.Status = status;
        await dbContext.SaveChangesAsync(cancellationToken);

        return mapper.Map<RfqDto>(rfq);
    }
}
