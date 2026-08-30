using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopniu_api.Domain.Entities.SupplierEntity;

namespace Shopniu_api.Infrastructure.Persistance.Configurations;

public class SupplierSyncLogConfiguration : IEntityTypeConfiguration<SupplierSyncLog>
{
    public void Configure(EntityTypeBuilder<SupplierSyncLog> builder)
    {
        builder.ToTable("SupplierSyncLogs");
        builder.HasKey(l => l.Id);
        builder.Property(l => l.RunAt)
            .IsRequired();
        builder.Property(l => l.Succeeded)
            .IsRequired();
        builder.Property(l => l.Created)
            .IsRequired();
        builder.Property(l => l.Updated)
            .IsRequired();
        builder.Property(l => l.ErrorCount)
            .IsRequired();
        builder.Property(l => l.Errors)
            .HasMaxLength(4000);

        builder.HasOne(l => l.Supplier)
            .WithMany()
            .HasForeignKey(l => l.SupplierId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
