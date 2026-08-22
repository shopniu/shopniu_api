using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopniu_api.Domain.Entities.MediaEntity;
using Shopniu_api.Domain.Entities.ProductEntity;

namespace Shopniu_api.Infrastructure.Persistance.Configurations;

public class MediaAssetConfiguration : IEntityTypeConfiguration<MediaAsset>
{
    public void Configure(EntityTypeBuilder<MediaAsset> builder)
    {
        builder.ToTable("MediaAssets");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.BlobPath)
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(m => m.OriginalUrl)
            .IsRequired()
            .HasMaxLength(512);
        builder.Property(m => m.WebUrl)
            .IsRequired()
            .HasMaxLength(512);
        builder.Property(m => m.ThumbUrl)
            .IsRequired()
            .HasMaxLength(512);
        builder.Property(m => m.ContentType)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(m => m.IsMain)
            .IsRequired();
        builder.Property(m => m.Size)
            .IsRequired();
        builder.Property(m => m.Width)
            .IsRequired();
        builder.Property(m => m.Height)
            .IsRequired();
        builder.Property(m => m.UploadedBy)
            .IsRequired();

        builder.HasOne<Product>()
            .WithMany(p => p.Media)
            .HasForeignKey(m => m.ProductId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
