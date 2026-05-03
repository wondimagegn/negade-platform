using Mapster;
using MapsterMapper;
using MediatR;
using Negade.Application.BusinessProfiles.Common;
using Microsoft.Extensions.DependencyInjection;
using Negade.Application.Products.Common;
using Negade.Application.Rfqs.Common;
using Negade.Application.TradeTrust.Common;
using Negade.Domain.Entities;

namespace Negade.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.NewConfig<BusinessProfile, BusinessProfileDto>();
        config.NewConfig<CreateBusinessProfileDto, BusinessProfile>();
        config.NewConfig<UpdateBusinessProfileDto, BusinessProfile>();
        config.NewConfig<Product, ProductDto>()
            .Map(destination => destination.SupplierName, source => source.Supplier == null ? null : source.Supplier.BusinessName);
        config.NewConfig<CreateProductDto, Product>();
        config.NewConfig<UpdateProductDto, Product>();
        config.NewConfig<Rfq, RfqDto>();
        config.NewConfig<CreateRfqDto, Rfq>();
        config.NewConfig<CreateQuoteDto, Quote>();
        config.NewConfig<Quote, QuoteDto>()
            .Map(destination => destination.SupplierName, source => source.Supplier.BusinessName);
        config.NewConfig<TradeRating, TradeRatingDto>();
        config.NewConfig<TradeHistory, TradeHistoryDto>();

        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        return services;
    }
}
