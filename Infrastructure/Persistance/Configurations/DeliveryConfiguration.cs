using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopniu_api.Domain.Entities.DeliveryEntity;

namespace Shopniu_api.Infrastructure.Persistance.Configurations;

public class DeliveryConfiguration : IEntityTypeConfiguration<Delivery>
{
    public void Configure(EntityTypeBuilder<Delivery> builder)
    {
        builder.ToTable("Deliveries");
        builder.HasKey(d => new { d.UserId, d.TransactionId });
        builder.Property(d => d.Address)
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(d => d.City)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(d => d.State)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(d => d.DepartmentCode)
            .IsRequired()
            .HasMaxLength(10);
        builder.Property(d => d.CityCode)
            .IsRequired()
            .HasMaxLength(10);
        builder.Property(d => d.Status)
            .IsRequired();
    }
}