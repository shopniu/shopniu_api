

using Shopniu_api.Domain.Repositories;
using Shopniu_api.Domain.Entities.ProductEntity;
using Shopniu_api.Aplication.Products.Common.DTOs;

namespace Shopniu_api.Aplication.Products.UseCases.GetAllProducts;

public class GetAllProductsUseCase
{
    private readonly IProductRepository _productRepository;

    public GetAllProductsUseCase(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<ProductResponseDTO>> ExecuteAsync(bool includeMedia = false)
    {
        var products = await _productRepository.GetAllAsync(includeMedia);
        return ProductResponseDTO.FromEntities(products, includeMedia);
    }
}