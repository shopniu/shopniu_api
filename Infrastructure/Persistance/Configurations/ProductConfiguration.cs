

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopniu_api.Domain.Entities.ProductEntity;

namespace Shopniu_api.Infrastructure.Persistance.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(p => p.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2)");
        builder.Property(p => p.ImageUrl)
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(1000);
        builder.Property(p => p.Stock)
            .IsRequired();
        builder.Property(p => p.Sourcing)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(32)
            .HasDefaultValue(ProductSourcing.LocalStock);
        builder.Property(p => p.CertifiedOriginal)
            .IsRequired()
            .HasDefaultValue(false);
        builder.Property(p => p.CostPrice)
            .HasColumnType("decimal(18,2)");
        builder.Property(p => p.SupplierName)
            .HasMaxLength(200);
        builder.Property(p => p.LeadTimeDays);

        builder.HasOne(p => p.Supplier)
            .WithMany(s => s.Products)
            .HasForeignKey(p => p.SupplierId)
            .OnDelete(DeleteBehavior.SetNull);

    }
}