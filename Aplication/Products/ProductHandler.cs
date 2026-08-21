using Shopniu_api.Aplication.Products.UseCases.CreateProduct;
using Shopniu_api.Aplication.Products.UseCases.GetAllProducts;
using Shopniu_api.Aplication.Products.UseCases.GetProductsByUser;
using Shopniu_shared.Common;
using Shopniu_api.Aplication.Products.Common.DTOs;

namespace Shopniu_api.Aplication.Products;

public class ProductHandler
{
    private readonly GetAllProductsUseCase _getAllProductsUseCase;
    private readonly CreateProductUseCase _createProductUseCase;
    private readonly GetProductsByUserUseCase _getProductsByUserUseCase;
    public ProductHandler(CreateProductUseCase createProductUseCase, GetAllProductsUseCase getAllProductsUseCase, GetProductsByUserUseCase getProductsByUserUseCase)
    {
        _getAllProductsUseCase = getAllProductsUseCase;
        _createProductUseCase = createProductUseCase;
        _getProductsByUserUseCase = getProductsByUserUseCase;
    }

    public async Task<ApiResponse<IEnumerable<ProductResponseDTO>>> GetAllProductsAsync()
    {
        var result = await _getAllProductsUseCase.ExecuteAsync();
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
}