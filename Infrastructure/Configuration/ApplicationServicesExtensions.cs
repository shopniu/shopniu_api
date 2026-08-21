
using FluentValidation;
using Shopniu_api.Aplication.Payments;
using Shopniu_api.Aplication.Payments.UseCases.GetPaymentMethods;
using Shopniu_api.Aplication.Payments.UseCases.ProcessPaymentWebhook;
using Shopniu_api.Aplication.Products;
using Shopniu_api.Aplication.Products.UseCases.GetAllProducts;
using Shopniu_api.Aplication.Products.UseCases.GetProductsByUser;
using Shopniu_api.Aplication.Products.UseCases.UpdateProduct;
using Shopniu_api.Aplication.Transactions;
using Shopniu_api.Aplication.Transactions.UseCases.CreateTransaction;
using Shopniu_api.Aplication.Transactions.UseCases.VerifyTransactionStatus;

namespace Shopniu_api.Infrastructure.Configuration;

public static class ApplicationServicesExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ProductHandler>();
        services.AddScoped<GetAllProductsUseCase>();
        services.AddScoped<CreateProductUseCase>();
        services.AddScoped<GetProductsByUserUseCase>();
        services.AddScoped<UpdateProductUseCase>();

        services.AddScoped<TransactionHandler>();
        services.AddScoped<CreateTransactionUseCase>();
        services.AddScoped<VerifyTransactionStatusUseCase>();

        services.AddScoped<PaymentsHandler>();
        services.AddScoped<ProcessPaymentWebhookUseCase>();
        services.AddScoped<GetPaymentMethodsUseCase>();

        services.AddValidatorsFromAssembly(typeof(ApplicationServicesExtensions).Assembly);

        return services;
    }
}
