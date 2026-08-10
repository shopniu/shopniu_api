using FluentValidation;
using Shopniu_api.Domain.Repositories;
using Shopniu_api.Aplication.Products.UseCases.CreateProduct;
using Shopniu_api.Domain.Entities.ProductEntity;
using Shopniu_api.Aplication.Products.Common.DTOs;

namespace Shopniu_api.Aplication.Products;

public class CreateProductUseCase
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductUseCase(IProductRepository productRepository, IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProductResponseDTO> ExecuteAsync(CreateProductRequest dto)
    {
        var product = await _productRepository.CreateAsync(new Product(
            name: dto.Name,
            price: dto.Price,
            imageUrl: dto.ImageUrl,
            description: dto.Description,
            stock: dto.Stock
        ));
        await _unitOfWork.SaveChangesAsync();
        return ProductResponseDTO.FromEntity(product);
    }
}