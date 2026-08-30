using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopniu_api.Domain.Entities.SupplierEntity;

namespace Shopniu_api.Infrastructure.Persistance.Configurations;

public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("Suppliers");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(s => s.Region)
            .HasMaxLength(100);
        builder.Property(s => s.DefaultShipping)
            .HasColumnType("decimal(18,2)");
        builder.Property(s => s.DefaultLeadTimeDays)
            .IsRequired();
        builder.Property(s => s.IsActive)
            .IsRequired();
    }
}
