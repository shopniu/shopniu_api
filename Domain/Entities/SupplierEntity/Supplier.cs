using Shopniu_api.Domain.Entities.common;
using Shopniu_api.Domain.Entities.ProductEntity;
using Shopniu_api.Domain.Exceptions.Common;

namespace Shopniu_api.Domain.Entities.SupplierEntity;

public class Supplier : BaseEntity
{
    public string Name { get; set; }

    /// <summary>Región/ubicación del proveedor (ej. "Bogotá", "China").</summary>
    public string? Region { get; set; }

    /// <summary>Costo de envío por defecto en unidades de moneda (el backend
    /// es dueño de precios y costos; solo informativo para dropshipping).</summary>
    public decimal DefaultShipping { get; set; }

    /// <summary>Días de despacho estimados por defecto.</summary>
    public int DefaultLeadTimeDays { get; set; }

    public bool IsActive { get; set; } = true;

    public List<Product> Products { get; set; } = new List<Product>();

    public Supplier(
        string name,
        string? region,
        decimal defaultShipping,
        int defaultLeadTimeDays,
        bool isActive = true)
    {
        ValidateDetails(name, defaultShipping, defaultLeadTimeDays);
        Name = name;
        Region = region;
        DefaultShipping = defaultShipping;
        DefaultLeadTimeDays = defaultLeadTimeDays;
        IsActive = isActive;
    }

    public void Update(string name, string? region, decimal defaultShipping, int defaultLeadTimeDays, bool isActive)
    {
        ValidateDetails(name, defaultShipping, defaultLeadTimeDays);
        Name = name;
        Region = region;
        DefaultShipping = defaultShipping;
        DefaultLeadTimeDays = defaultLeadTimeDays;
        IsActive = isActive;
    }

    private static void ValidateDetails(string name, decimal defaultShipping, int defaultLeadTimeDays)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ValidationsException("Supplier name cannot be empty.");
        if (defaultShipping < 0)
            throw new ValidationsException("Default shipping cannot be negative.");
        if (defaultLeadTimeDays < 1)
            throw new ValidationsException("Default lead time must be at least 1 day.");
    }
}
