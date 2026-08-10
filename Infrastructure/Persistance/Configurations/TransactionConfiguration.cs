
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopniu_api.Domain.Entities.TransactionEntity;

namespace Shopniu_api.Infrastructure.Persistance.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.UserId)
            .IsRequired();
        builder.Property(t => t.IdempotencyKey)
            .IsRequired()
            .HasMaxLength(100);
        builder.HasIndex(t => t.IdempotencyKey)
            .IsUnique();
        builder.Property(t => t.Status)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(t => t.CreatedAt)
            .IsRequired();
        builder.Property(t => t.UpdatedAt)
            .IsRequired();

        builder.HasMany(t => t.Orders)
            .WithOne(o => o.Transaction)
            .HasForeignKey(o => o.TransactionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}