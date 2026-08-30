using Shopniu_api.Aplication.Common.Ports;
using Shopniu_api.Domain.Repositories;
using Shopniu_api.Infrastructure.Persistance;
using Shopniu_api.Infrastructure.Persistance.Repositories;
using Shopniu_api.Infrastructure.Services.Users;

namespace Shopniu_api.Infrastructure.Configuration;

public static class RepositoryServicesExtensions
{
    public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IProductRepository, ProductsRepository>();
        services.AddScoped<ISupplierRepository, SupplierRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IDeliveryRepository, DeliveryRepository>();
        services.AddScoped<IDashboardRepository, DashboardRepository>();
        services.AddScoped<IUserPaymentDataRepository, UserPaymentDataRepository>();
        services.AddScoped<IMediaRepository, MediaRepository>();

        return services;
    }
}
