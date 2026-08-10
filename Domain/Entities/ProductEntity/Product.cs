using System.ComponentModel.DataAnnotations;
using Shopniu_api.Domain.Exceptions.Common;
using Shopniu_api.Domain.Entities.common;
using Shopniu_api.Domain.Entities.OrderEntity;
using Shopniu_api.Domain.Entities.ProductEntity.Exceptions;

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

        public List<Order> Orders { get; set; } = new List<Order>();


        public Product(string name, decimal price, string imageUrl, string description, int stock)
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
            Name = name;
            Price = price;
            ImageUrl = imageUrl;
            Description = description;
            Stock = stock;
        }

        // Method to validate stock availability
        public void ValidateStock(int requestedQuantity)
        {
            if (requestedQuantity > Stock)
            {
                throw new InsufficientStockException(Id, requestedQuantity, Stock);
            }
        }
    }
}