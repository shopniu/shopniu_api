using Shopniu_api.Aplication.Products.UseCases.CreateProduct;
using Shopniu_api.Aplication.Products.UseCases.GetAllProducts;
using Shopniu_api.Aplication.Products.UseCases.GetProductsByUser;
using Shopniu_api.Aplication.Products.UseCases.ImportProducts;
using Shopniu_api.Aplication.Products.UseCases.UpdateProduct;
using Shopniu_shared.Common;
using Shopniu_api.Aplication.Products.Common.DTOs;

namespace Shopniu_api.Aplication.Products;

public class ProductHandler
{
    private readonly GetAllProductsUseCase _getAllProductsUseCase;
    private readonly CreateProductUseCase _createProductUseCase;
    private readonly GetProductsByUserUseCase _getProductsByUserUseCase;
    private readonly UpdateProductUseCase _updateProductUseCase;
    private readonly ImportProductsUseCase _importProductsUseCase;
    private readonly IConfiguration _configuration;
    public ProductHandler(
        CreateProductUseCase createProductUseCase,
        GetAllProductsUseCase getAllProductsUseCase,
        GetProductsByUserUseCase getProductsByUserUseCase,
        UpdateProductUseCase updateProductUseCase,
        ImportProductsUseCase importProductsUseCase,
        IConfiguration configuration)
    {
        _getAllProductsUseCase = getAllProductsUseCase;
        _createProductUseCase = createProductUseCase;
        _getProductsByUserUseCase = getProductsByUserUseCase;
        _updateProductUseCase = updateProductUseCase;
        _importProductsUseCase = importProductsUseCase;
        _configuration = configuration;
    }

    public async Task<ApiResponse<IEnumerable<ProductResponseDTO>>> GetAllProductsAsync(bool includeMedia = false)
    {
        var result = await _getAllProductsUseCase.ExecuteAsync(includeMedia);
        return ApiResponse<IEnumerable<ProductResponseDTO>>.Ok(result, "Products Retrieved Successfully");
    }

    public async Task<ApiResponse<IEnumerable<ProductResponseDTO>>> GetMyProductsAsync()
    {
        var result = await _getProductsByUserUseCase.ExecuteAsync();
        return ApiResponse<IEnumerable<ProductResponseDTO>>.Ok(result, "Products Retrieved Successfully");
    }

    public async Task<ApiResponse<ProductResponseDTO>> CreateProductAsync(CreateProductRequest dto)
    {
        var result = await _createProductUseCase.ExecuteAsync(dto);
        return ApiResponse<ProductResponseDTO>.Ok(result, "Product Created Successfully");
    }

    public async Task<ApiResponse<ProductResponseDTO>> UpdateProductAsync(int id, UpdateProductRequest dto)
    {
        var result = await _updateProductUseCase.ExecuteAsync(id, dto);
        return ApiResponse<ProductResponseDTO>.Ok(result, "Product Updated Successfully");
    }

    public async Task<ApiResponse<ImportProductsResult>> ImportProductsAsync(ImportProductsRequest dto)
    {
        var result = await _importProductsUseCase.ExecuteAsync(dto);
        return ApiResponse<ImportProductsResult>.Ok(
            result,
            $"{result.Created} products imported successfully");
    }

    public ApiResponse<ImportMetaResponse> GetImportMeta()
    {
        var markupPercent = _configuration.GetValue<decimal>("DropShipping:MarkupPercent", 30);
        return ApiResponse<ImportMetaResponse>.Ok(
            new ImportMetaResponse(markupPercent),
            "Import meta retrieved successfully");
    }
}