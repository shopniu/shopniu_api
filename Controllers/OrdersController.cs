using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Shopniu_api.Aplication.Orders;
using Shopniu_api.Aplication.Orders.Common.DTOs;
using Shopniu_api.Domain.Entities.DeliveryEntity;

namespace Shopniu_api.Routes;

[ApiController]
[Route("api/v1/orders")]
public class OrdersController : ControllerBase
{
    private readonly OrderHandler _orderHandler;

    public OrdersController(OrderHandler orderHandler)
    {
        _orderHandler = orderHandler;
    }

    // Listado de pedidos pagados (fulfillment). Back-office: admin y seller
    // tienen product.create (mismo gate que /products/own y /suppliers).
    [Authorize(Policy = "product.create")]
    [HttpGet]
    public async Task<IActionResult> GetOrders([FromQuery] DeliveryStatus? status = null)
    {
        return Ok(await _orderHandler.ListOrdersAsync(status));
    }

    // Pedidos del comprador autenticado (solo los suyos). Cualquier sesión
    // sirve: la resolución del usuario viene del token, no de la request.
    [Authorize]
    [HttpGet("mine")]
    public async Task<IActionResult> GetMyOrders()
    {
        return Ok(await _orderHandler.ListMyOrdersAsync());
    }

    // Transición de estado del despacho (enviado/entregado) + tracking.
    [Authorize(Policy = "order.update")]
    [HttpPatch("{transactionId:int}/status")]
    public async Task<IActionResult> UpdateOrderStatus(int transactionId, [FromBody] UpdateFulfillmentStatusRequest dto)
    {
        return Ok(await _orderHandler.UpdateStatusAsync(transactionId, dto));
    }
}
