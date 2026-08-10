
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopniu_api.Domain.Entities.PaymentDetailsEntity;

namespace Shopniu_api.Infrastructure.Persistance.Configurations;

public class PaymentDetailsConfiguration : IEntityTypeConfiguration<PaymentDetails>
{
    public void Configure(EntityTypeBuilder<PaymentDetails> builder)
    {
        builder.ToTable("PaymentDetails");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.AmountInCents)
            .IsRequired()
            .HasPrecision(18, 2);
        builder.Property(p => p.DeliveryInCents)
            .IsRequired()
            .HasPrecision(18, 2);
        builder.Property(p => p.TaxInCents)
            .IsRequired()
            .HasPrecision(18, 2);
        builder.Property(p => p.TotalInCents)
            .IsRequired()
            .HasPrecision(18, 2);
        builder.Property(p => p.Currency)
            .IsRequired()
            .HasMaxLength(10);
        builder.Property(p => p.PaymentMethod)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(p => p.CreatedAt)
            .IsRequired();
        builder.Property(p => p.UpdatedAt)
            .IsRequired();
        builder.HasOne(p => p.Transaction)
            .WithOne(t => t.PaymentDetails)
            .HasForeignKey<PaymentDetails>(p => p.TransactionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}