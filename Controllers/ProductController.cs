using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Shopniu_api.Aplication.Products;
using Shopniu_api.Aplication.Products.UseCases.CreateProduct;
using Shopniu_api.Aplication.Products.UseCases.ExtractProductFromUrl;
using Shopniu_api.Aplication.Products.UseCases.ImportProducts;
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
    public async Task<IActionResult> GetAllProducts([FromQuery] bool includeMedia = false)
    {
        return Ok(await _productHandler.GetAllProductsAsync(includeMedia));
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

    // Importación batch de catálogo de proveedor (dropshipping): cada ítem se
    // valida y crea de forma independiente. El precio se deriva del costo con
    // el markup configurado, nunca se recibe del cliente.
    [Authorize(Policy = "product.create")]
    [HttpPost("import")]
    public async Task<IActionResult> ImportProducts([FromBody] ImportProductsRequest dto)
    {
        var response = await _productHandler.ImportProductsAsync(dto);
        return Ok(response);
    }

    // Metadatos del flujo de importación (markup configurado) para que el
    // back-office previsualice el precio de venta antes de importar.
    [Authorize(Policy = "product.create")]
    [HttpGet("import/meta")]
    public IActionResult GetImportMeta()
    {
        return Ok(_productHandler.GetImportMeta());
    }

    // Extrae la info de un producto desde su URL (JSON-LD/OpenGraph) para
    // previsualizarla en el flujo de importación. No crea nada.
    [Authorize(Policy = "product.create")]
    [HttpPost("import/from-url")]
    public async Task<IActionResult> ExtractProductFromUrl([FromBody] ExtractProductFromUrlRequest dto)
    {
        return Ok(await _productHandler.ExtractProductFromUrlAsync(dto));
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