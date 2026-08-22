using Shopniu_api.Domain.Entities.common;

namespace Shopniu_api.Domain.Entities.MediaEntity;

/// <summary>Recurso multimedia (imagen) subido al blob storage. Guarda las URLs
/// de las variantes (original, web, thumbnail) y su vínculo opcional a un
/// producto.</summary>
public class MediaAsset : BaseEntity
{
    /// <summary>Producto al que está asociado; null si aún no se vinculó.</summary>
    public int? ProductId { get; set; }

    /// <summary>Imagen principal del producto (alimenta Product.ImageUrl).</summary>
    public bool IsMain { get; set; }

    /// <summary>Path del blob original (sin el nombre del contenedor), ej.
    /// "2026/08/<guid>.jpg".</summary>
    public string BlobPath { get; set; } = string.Empty;

    public string OriginalUrl { get; set; } = string.Empty;
    public string WebUrl { get; set; } = string.Empty;
    public string ThumbUrl { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long Size { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    /// <summary>Usuario (identity) que subió el archivo.</summary>
    public int UploadedBy { get; set; }
}
