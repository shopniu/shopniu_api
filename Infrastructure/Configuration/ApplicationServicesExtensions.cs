
using FluentValidation;
using Shopniu_api.Aplication.Dashboard;
using Shopniu_api.Aplication.Dashboard.UseCases.GetDashboardSummary;
using Shopniu_api.Aplication.Media;
using Shopniu_api.Aplication.Media.UseCases.ConfirmMediaUpload;
using Shopniu_api.Aplication.Media.UseCases.CreateMediaUpload;
using Shopniu_api.Aplication.Media.UseCases.DeleteMedia;
using Shopniu_api.Aplication.Media.UseCases.LinkMedia;
using Shopniu_api.Aplication.Media.UseCases.SetMainMedia;
using Shopniu_api.Aplication.Payments;
using Shopniu_api.Aplication.Payments.UseCases.GetPaymentMethods;
using Shopniu_api.Aplication.Payments.UseCases.ProcessPaymentWebhook;
using Shopniu_api.Aplication.Products;
using Shopniu_api.Aplication.Products.UseCases.GetAllProducts;
using Shopniu_api.Aplication.Products.UseCases.GetProductsByUser;
using Shopniu_api.Aplication.Products.UseCases.ImportProducts;
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
        services.AddScoped<ImportProductsUseCase>();

        services.AddScoped<DashboardHandler>();
        services.AddScoped<GetDashboardSummaryUseCase>();

        services.AddScoped<TransactionHandler>();
        services.AddScoped<CreateTransactionUseCase>();
        services.AddScoped<VerifyTransactionStatusUseCase>();

        services.AddScoped<PaymentsHandler>();
        services.AddScoped<ProcessPaymentWebhookUseCase>();
        services.AddScoped<GetPaymentMethodsUseCase>();

        services.AddScoped<MediaHandler>();
        services.AddScoped<CreateMediaUploadUseCase>();
        services.AddScoped<ConfirmMediaUploadUseCase>();
        services.AddScoped<SetMainMediaUseCase>();
        services.AddScoped<LinkMediaUseCase>();
        services.AddScoped<DeleteMediaUseCase>();

        services.AddValidatorsFromAssembly(typeof(ApplicationServicesExtensions).Assembly);

        return services;
    }
}
