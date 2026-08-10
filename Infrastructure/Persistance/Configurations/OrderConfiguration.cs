using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopniu_api.Domain.Entities.OrderEntity;



namespace Shopniu_api.Infrastructure.Persistance.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");
        builder.HasKey(o => new { o.UserId, o.ProductId, o.TransactionId });

        builder.Property(o => o.Quantity)
            .IsRequired();
        builder.Property(o => o.CreatedAt)
            .IsRequired();
        builder.Property(o => o.UpdatedAt)
            .IsRequired();

        builder.HasOne(o => o.Transaction)
            .WithMany(t => t.Orders)
            .HasForeignKey(o => o.TransactionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(o => o.Product)
            .WithMany(p => p.Orders)
            .HasForeignKey(o => o.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}