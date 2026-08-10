using Shopniu_api.Aplication.Products.UseCases.CreateProduct;
using Shopniu_api.Aplication.Products.UseCases.GetAllProducts;
using Shopniu_shared.Common;
using Shopniu_api.Aplication.Products.Common.DTOs;

namespace Shopniu_api.Aplication.Products;

public class ProductHandler
{
    private readonly GetAllProductsUseCase _getAllProductsUseCase;
    private readonly CreateProductUseCase _createProductUseCase;
    public ProductHandler(CreateProductUseCase createProductUseCase, GetAllProductsUseCase getAllProductsUseCase)
    {
        _getAllProductsUseCase = getAllProductsUseCase;
        _createProductUseCase = createProductUseCase;
    }

    public async Task<ApiResponse<IEnumerable<ProductResponseDTO>>> GetAllProductsAsync()
    {
        var result = await _getAllProductsUseCase.ExecuteAsync();
        return ApiResponse<IEnumerable<ProductResponseDTO>>.Ok(result, "Products Retrieved Successfully");
    }

    public async Task<ApiResponse<ProductResponseDTO>> CreateProductAsync(CreateProductRequest dto)
    {
        var result = await _createProductUseCase.ExecuteAsync(dto);
        return ApiResponse<ProductResponseDTO>.Ok(result, "Product Created Successfully");
    }
}