using MapsterMapper;
using MediatR;
using Negade.Application.Common.Interfaces;
using Negade.Application.Rfqs.Common;
using Negade.Domain.Entities;

namespace Negade.Application.Rfqs.Commands;

public record CreateRfqCommand(CreateRfqDto Rfq) : IRequest<RfqDto>;

public class CreateRfqCommandHandler(IApplicationDbContext dbContext, IMapper mapper)
    : IRequestHandler<CreateRfqCommand, RfqDto>
{
    public async Task<RfqDto> Handle(CreateRfqCommand request, CancellationToken cancellationToken)
    {
        var rfq = mapper.Map<Rfq>(request.Rfq);
        rfq.Id = Guid.NewGuid();
        rfq.Status = "Open";
        rfq.CreatedAt = DateTime.UtcNow;

        await dbContext.Rfqs.AddAsync(rfq, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return mapper.Map<RfqDto>(rfq);
    }
}
