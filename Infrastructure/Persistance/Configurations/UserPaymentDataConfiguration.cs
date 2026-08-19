using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopniu_api.Domain.Entities.UserPaymentDataEntity;

namespace Shopniu_api.Infrastructure.Persistance.Configurations;

public class UserPaymentDataConfiguration : IEntityTypeConfiguration<UserPaymentData>
{
    public void Configure(EntityTypeBuilder<UserPaymentData> builder)
    {
        builder.ToTable("UserPaymentData");
        builder.HasKey(upd => upd.Id);

        builder.Property(upd => upd.CardHolderName)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(upd => upd.Address)
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(upd => upd.PhoneNumber)
            .HasMaxLength(20);
        builder.Property(upd => upd.LastFour)
            .IsRequired();
        builder.Property(upd => upd.PaymentMethod)
            .IsRequired();
        builder.HasIndex(upd => new { upd.UserId, upd.LastFour, upd.Address })
            .IsUnique();
    }
}