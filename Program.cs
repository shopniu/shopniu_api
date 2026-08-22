using Shopniu_api.Infrastructure.Configuration;
using Shopniu_api.Infrastructure.Configuration.Authentication;
using Shopniu_api.Infrastructure.Configuration.Pipeline;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApiServices()
    .AddAuthenticationServices(builder.Configuration)
    .AddInfrastructureServices(builder.Configuration)
    .AddPersistenceServices(builder.Configuration)
    .AddRepositoryServices()
    .AddPaymentServices(builder.Configuration)
    .AddIntegration(builder.Configuration)
    .AddStorageServices(builder.Configuration)
    .AddApplicationServices();

var app = builder.Build();
await app.UseApiPipeline();
app.Run();