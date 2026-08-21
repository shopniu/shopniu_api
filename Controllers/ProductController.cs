using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Shopniu_api.Aplication.Products;
using Shopniu_api.Aplication.Products.UseCases.CreateProduct;
using Shopniu_api.Aplication.Products.UseCases.UpdateProduct;

namespace Shopniu_api.Routes;

[ApiController]
[Route("api/v1/products")]
public class ProductController : ControllerBase
{
    private readonly ProductHandler _productHandler;

    public ProductController(ProductHandler productService)
    {
        _productHandler = productService;
    }


    [HttpGet]
    public async Task<IActionResult> GetAllProducts()
    {
        return Ok(await _productHandler.GetAllProductsAsync());
    }

    // Productos que el usuario autenticado tiene (ProductOwners). Requiere el
    // mismo permiso que crear: es un flujo de back-office.
    [Authorize(Policy = "product.create")]
    [HttpGet("own")]
    public async Task<IActionResult> GetMyProducts()
    {
        return Ok(await _productHandler.GetMyProductsAsync());
    }

    [Authorize(Policy = "product.create")]
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest dto)
    {
        var response = await _productHandler.CreateProductAsync(dto);
        return Ok(response);
    }

    // Solo el dueño del producto (ProductOwners) puede editarlo; el use case
    // responde 403 si no lo es.
    [Authorize(Policy = "product.update")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductRequest dto)
    {
        var response = await _productHandler.UpdateProductAsync(id, dto);
        return Ok(response);
    }
}