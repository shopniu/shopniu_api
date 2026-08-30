using Shopniu_api.Aplication.Orders.Common.DTOs;
using Shopniu_api.Aplication.Orders.UseCases.ListOrders;
using Shopniu_api.Aplication.Orders.UseCases.UpdateFulfillmentStatus;
using Shopniu_api.Domain.Entities.DeliveryEntity;
using Shopniu_shared.Common;

namespace Shopniu_api.Aplication.Orders;

public class OrderHandler
{
    private readonly ListOrdersUseCase _listOrdersUseCase;
    private readonly UpdateFulfillmentStatusUseCase _updateFulfillmentStatusUseCase;

    public OrderHandler(
        ListOrdersUseCase listOrdersUseCase,
        UpdateFulfillmentStatusUseCase updateFulfillmentStatusUseCase)
    {
        _listOrdersUseCase = listOrdersUseCase;
        _updateFulfillmentStatusUseCase = updateFulfillmentStatusUseCase;
    }

    public async Task<ApiResponse<IEnumerable<OrderFulfillmentDTO>>> ListOrdersAsync(DeliveryStatus? status = null)
    {
        var result = await _listOrdersUseCase.ExecuteAsync(status);
        return ApiResponse<IEnumerable<OrderFulfillmentDTO>>.Ok(result, "Orders Retrieved Successfully");
    }

    public async Task<ApiResponse<OrderFulfillmentDTO>> UpdateStatusAsync(int transactionId, UpdateFulfillmentStatusRequest dto)
    {
        var result = await _updateFulfillmentStatusUseCase.ExecuteAsync(transactionId, dto);
        return ApiResponse<OrderFulfillmentDTO>.Ok(result, "Order status updated successfully");
    }
}
