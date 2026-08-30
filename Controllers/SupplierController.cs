using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Shopniu_api.Aplication.Suppliers;
using Shopniu_api.Aplication.Suppliers.Common.DTOs;

namespace Shopniu_api.Routes;

[ApiController]
[Route("api/v1/suppliers")]
public class SupplierController : ControllerBase
{
    private readonly SupplierHandler _supplierHandler;

    public SupplierController(SupplierHandler supplierHandler)
    {
        _supplierHandler = supplierHandler;
    }

    // Gestión de proveedores (dropshipping). Back-office: admin y seller
    // tienen product.create (mismo gate que /products/own).
    [Authorize(Policy = "product.create")]
    [HttpGet]
    public async Task<IActionResult> GetSuppliers()
    {
        return Ok(await _supplierHandler.ListSuppliersAsync());
    }

    [Authorize(Policy = "product.create")]
    [HttpPost]
    public async Task<IActionResult> CreateSupplier([FromBody] SupplierRequest dto)
    {
        return Ok(await _supplierHandler.CreateSupplierAsync(dto));
    }

    [Authorize(Policy = "product.create")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateSupplier(int id, [FromBody] SupplierRequest dto)
    {
        return Ok(await _supplierHandler.UpdateSupplierAsync(id, dto));
    }

    // Sincroniza el catálogo del proveedor on-demand (crea/actualiza productos
    // externos con el markup y el stock reportado por el proveedor).
    [Authorize(Policy = "product.create")]
    [HttpPost("{id:int}/sync")]
    public async Task<IActionResult> SyncSupplier(int id)
    {
        return Ok(await _supplierHandler.SyncSupplierAsync(id));
    }
}
