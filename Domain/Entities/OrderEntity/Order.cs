
using Shopniu_api.Domain.Entities.common;
using Shopniu_api.Domain.Entities.TransactionEntity;
using Shopniu_api.Domain.Entities.ProductEntity;

namespace Shopniu_api.Domain.Entities.OrderEntity;

public class Order : BaseEntity
{
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public int TransactionId { get; set; }
    public int Quantity { get; set; } = 1;

    public Product Product { get; set; } = null!;
    public Transaction Transaction { get; set; } = null!;

    private Order() { }

    public Order(int userId, int productId, int quantity, Transaction transaction)
    {
        UserId = userId;
        ProductId = productId;
        Quantity = quantity;
        Transaction = transaction;
        TransactionId = transaction.Id;
    }

}