using Microsoft.EntityFrameworkCore;
using Shopniu_api.Domain.Entities.OrderEntity;
using Shopniu_api.Domain.Entities.TransactionEntity;
using Shopniu_api.Domain.Entities.PaymentDetailsEntity;
using Shopniu_api.Domain.Entities.ProductEntity;
using Shopniu_api.Domain.Entities.DeliveryEntity;
using Shopniu_api.Domain.Entities.UserPaymentDataEntity;

namespace Shopniu_api.Infrastructure.Persistance;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<PaymentDetails> PaymentDetails => Set<PaymentDetails>();
    public DbSet<Delivery> Deliveries => Set<Delivery>();
    public DbSet<UserPaymentData> UserPaymentData => Set<UserPaymentData>();
    public DbSet<ProductOwner> ProductOwners => Set<ProductOwner>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}