using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopniu_api.Domain.Entities.ProductEntity;

namespace Shopniu_api.Infrastructure.Persistance.Configurations;

public class ProductOwnerConfiguration : IEntityTypeConfiguration<ProductOwner>
{
    public void Configure(EntityTypeBuilder<ProductOwner> builder)
    {
        builder.ToTable("ProductOwners");
        builder.HasKey(po => new { po.ProductId, po.UserId });

        builder.HasOne(po => po.Product)
            .WithMany()
            .HasForeignKey(po => po.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(po => po.UserId);
    }
}