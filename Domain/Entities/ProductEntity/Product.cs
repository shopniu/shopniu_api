using System.ComponentModel.DataAnnotations;
using Shopniu_api.Domain.Exceptions.Common;
using Shopniu_api.Domain.Entities.common;
using Shopniu_api.Domain.Entities.MediaEntity;
using Shopniu_api.Domain.Entities.OrderEntity;
using Shopniu_api.Domain.Entities.ProductEntity.Exceptions;
using Shopniu_api.Domain.Entities.SupplierEntity;

namespace Shopniu_api.Domain.Entities.ProductEntity
{
    public class Product : BaseEntity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public string ImageUrl { get; set; }
        [Required]
        public int Stock { get; set; } = 0;
        public string Description { get; set; }

        /// <summary>Origen del inventario: stock local o despacho por
        /// proveedor externo (dropshipping).</summary>
        public ProductSourcing Sourcing { get; set; } = ProductSourcing.LocalStock;

        /// <summary>Indica que el producto es original certificado de la
        /// marca (autenticidad verificada).</summary>
        public bool CertifiedOriginal { get; set; } = false;

        /// <summary>Costo de compra al proveedor (dropshipping). Solo interno:
        /// nunca se expone al cliente ni al front.</summary>
        public decimal? CostPrice { get; set; }

        /// <summary>Nombre del proveedor que despacha el producto.</summary>
        public string? SupplierName { get; set; }

        /// <summary>Días estimados de despacho del proveedor.</summary>
        public int? LeadTimeDays { get; set; }

        /// <summary>Proveedor asociado (opcional). Null si el producto se
        /// despacha con stock local.</summary>
        public int? SupplierId { get; set; }

        public Supplier? Supplier { get; set; }

        /// <summary>Usuario (identity) que creó el producto. Null solo en
        /// filas legacy anteriores a registrar el creador.</summary>
        public int? UserId { get; set; }

        public List<Order> Orders { get; set; } = new List<Order>();
        public List<MediaAsset> Media { get; set; } = new List<MediaAsset>();


        public Product(
            string name,
            decimal price,
            string imageUrl,
            string description,
            int stock,
            int? userId = null,
            ProductSourcing sourcing = ProductSourcing.LocalStock,
            bool certifiedOriginal = false,
            decimal? costPrice = null,
            string? supplierName = null,
            int? leadTimeDays = null,
            int? supplierId = null)
        {
            ValidateDetails(name, price, imageUrl, stock);
            Name = name;
            Price = price;
            ImageUrl = imageUrl;
            Description = description;
            Stock = stock;
            UserId = userId;
            Sourcing = sourcing;
            CertifiedOriginal = certifiedOriginal;
            CostPrice = costPrice;
            SupplierName = supplierName;
            LeadTimeDays = leadTimeDays;
            SupplierId = supplierId;
        }

        /// <summary>Actualiza los datos editables del producto con las mismas
        /// validaciones de creación.</summary>
        public void Update(string name, decimal price, string imageUrl, string description, int stock)
        {
            ValidateDetails(name, price, imageUrl, stock);
            Name = name;
            Price = price;
            ImageUrl = imageUrl;
            Description = description;
            Stock = stock;
        }

        private static void ValidateDetails(string name, decimal price, string imageUrl, int stock)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ValidationsException("Product name cannot be empty.");
            if (price < 0)
                throw new ValidationsException("Product price cannot be negative.");
            if (string.IsNullOrWhiteSpace(imageUrl))
                throw new ValidationsException("Product image URL cannot be empty.");
            if (!Uri.TryCreate(imageUrl, UriKind.Absolute, out _))
                throw new ValidationsException("Product image URL is invalid.");
            if (stock < 0)
                throw new ValidationsException("Stock quantity cannot be negative.");
        }

        public void ValidateStock(int requestedQuantity)
        {
            if (requestedQuantity > Stock)
            {
                throw new InsufficientStockException(Id, requestedQuantity, Stock);
            }
        }

        /// <summary>Descuenta stock por una compra confirmada (pago aprobado).</summary>
        public void DecreaseStock(int quantity)
        {
            if (quantity <= 0)
                throw new ValidationsException("Stock quantity to decrease must be positive.");
            if (quantity > Stock)
                throw new InsufficientStockException(Id, quantity, Stock);

            Stock -= quantity;
        }
    }
}