using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Negade.Application.Common.Interfaces;
using Negade.Application.TradeTrust.Common;
using Negade.Domain.Entities;

namespace Negade.Application.TradeTrust.Commands;

public record CreateTradeHistoryCommand(
    Guid BusinessProfileId,
    CreateTradeHistoryDto TradeHistory,
    Guid? OwnerUserId) : IRequest<TradeHistoryDto?>;

public class CreateTradeHistoryCommandHandler(IApplicationDbContext dbContext, IMapper mapper)
    : IRequestHandler<CreateTradeHistoryCommand, TradeHistoryDto?>
{
    public async Task<TradeHistoryDto?> Handle(
        CreateTradeHistoryCommand request,
        CancellationToken cancellationToken)
    {
        var profile = await dbContext.BusinessProfiles.FirstOrDefaultAsync(
            candidate => candidate.Id == request.BusinessProfileId,
            cancellationToken);

        if (profile is null)
        {
            return null;
        }

        if (request.OwnerUserId is not null && profile.OwnerUserId != request.OwnerUserId)
        {
            return null;
        }

        var tradeHistory = new TradeHistory
        {
            Id = Guid.NewGuid(),
            BusinessProfileId = request.BusinessProfileId,
            Description = request.TradeHistory.Description,
            CounterpartyName = request.TradeHistory.CounterpartyName,
            Amount = request.TradeHistory.Amount,
            TradeDate = request.TradeHistory.TradeDate,
            CreatedAt = DateTime.UtcNow
        };

        profile.TradeCount += 1;

        await dbContext.TradeHistory.AddAsync(tradeHistory, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return mapper.Map<TradeHistoryDto>(tradeHistory);
    }
}
