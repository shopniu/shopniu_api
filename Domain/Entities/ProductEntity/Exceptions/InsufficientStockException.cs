

using Shopniu_api.Domain.Exceptions;

namespace Shopniu_api.Domain.Entities.ProductEntity.Exceptions;

public class InsufficientStockException : DomainException
{
    public InsufficientStockException(int productId, int requestedQuantity, int availableQuantity)
        : base($"Insufficient stock for product with ID {productId}. Requested quantity: {requestedQuantity}, Available quantity: {availableQuantity}.")
    {
    }
}