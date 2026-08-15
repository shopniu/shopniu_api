using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Shopniu_api.Aplication.Products;
using Shopniu_api.Aplication.Products.UseCases.CreateProduct;

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

    [Authorize(Policy = "product.create")]
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest dto)
    {
        var response = await _productHandler.CreateProductAsync(dto);
        return Ok(response);
    }
}