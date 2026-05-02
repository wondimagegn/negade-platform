using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Negade.Application.Common.Interfaces;
using Negade.Application.TradeTrust.Common;
using Negade.Domain.Entities;

namespace Negade.Application.TradeTrust.Commands;

public record CreateTradeRatingCommand(Guid SupplierId, CreateTradeRatingDto Rating, Guid? RaterUserId)
    : IRequest<TradeRatingDto?>;

public class CreateTradeRatingCommandHandler(IApplicationDbContext dbContext, IMapper mapper)
    : IRequestHandler<CreateTradeRatingCommand, TradeRatingDto?>
{
    public async Task<TradeRatingDto?> Handle(CreateTradeRatingCommand request, CancellationToken cancellationToken)
    {
        var supplier = await dbContext.BusinessProfiles.FirstOrDefaultAsync(
            profile => profile.Id == request.SupplierId,
            cancellationToken);

        if (supplier is null)
        {
            return null;
        }

        var rating = new TradeRating
        {
            Id = Guid.NewGuid(),
            SupplierId = request.SupplierId,
            RaterUserId = request.RaterUserId,
            Score = request.Rating.Score,
            Comment = request.Rating.Comment,
            CreatedAt = DateTime.UtcNow
        };

        supplier.RatingAverage =
            ((supplier.RatingAverage * supplier.RatingCount) + rating.Score) / (supplier.RatingCount + 1);
        supplier.RatingCount += 1;

        await dbContext.TradeRatings.AddAsync(rating, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return mapper.Map<TradeRatingDto>(rating);
    }
}
