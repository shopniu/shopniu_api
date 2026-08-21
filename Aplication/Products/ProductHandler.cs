using Shopniu_api.Aplication.Products.UseCases.CreateProduct;
using Shopniu_api.Aplication.Products.UseCases.GetAllProducts;
using Shopniu_api.Aplication.Products.UseCases.GetProductsByUser;
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
    public ProductHandler(CreateProductUseCase createProductUseCase, GetAllProductsUseCase getAllProductsUseCase, GetProductsByUserUseCase getProductsByUserUseCase, UpdateProductUseCase updateProductUseCase)
    {
        _getAllProductsUseCase = getAllProductsUseCase;
        _createProductUseCase = createProductUseCase;
        _getProductsByUserUseCase = getProductsByUserUseCase;
        _updateProductUseCase = updateProductUseCase;
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

    public async Task<ApiResponse<ProductResponseDTO>> UpdateProductAsync(int id, UpdateProductRequest dto)
    {
        var result = await _updateProductUseCase.ExecuteAsync(id, dto);
        return ApiResponse<ProductResponseDTO>.Ok(result, "Product Updated Successfully");
    }
}