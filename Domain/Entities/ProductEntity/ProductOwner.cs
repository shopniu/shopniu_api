namespace Shopniu_api.Domain.Entities.ProductEntity;

/// <summary>Relación usuario-producto: quiénes "tienen" un producto (flujo de
/// propiedad/visibilidad). El creador exacto vive en Product.UserId; esta
/// tabla intermedia permite que en el futuro varios usuarios de una misma
/// organización compartan el mismo producto.</summary>
public class ProductOwner
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    /// <summary>Usuario del servicio de identidad (sin FK local).</summary>
    public int UserId { get; set; }
}