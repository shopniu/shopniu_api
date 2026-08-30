using FluentValidation;
using Shopniu_api.Domain.Repositories;
using Shopniu_api.Domain.Exceptions.Common;
using Shopniu_api.Aplication.Products.UseCases.CreateProduct;
using Shopniu_api.Domain.Entities.ProductEntity;
using Shopniu_api.Aplication.Products.Common.DTOs;
using Shopniu_api.Aplication.Common.Ports.Identity;

namespace Shopniu_api.Aplication.Products;

public class CreateProductUseCase
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public CreateProductUseCase(IProductRepository productRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<ProductResponseDTO> ExecuteAsync(CreateProductRequest dto)
    {
        // La política product.create ya garantiza un usuario autenticado; si
        // llega 0 es una misconfiguración de issuer/claims y se reporta como
        // 401 en vez de guardar productos huérfanos.
        var userId = _currentUser.UserId;
        if (userId == 0)
        {
            throw new UnauthorizedException(
                "No authenticated user was resolved for this request.");
        }

        var product = await _productRepository.CreateAsync(new Product(
            name: dto.Name,
            price: dto.Price,
            imageUrl: dto.ImageUrl,
            description: dto.Description,
            stock: dto.Stock,
            userId: userId
        ));

        // Flujo de propiedad: el creador queda registrado como dueño del
        // producto (futuras organizaciones podrán compartirlo).
        await _productRepository.AddOwnerAsync(new ProductOwner
        {
            Product = product,
            UserId = userId
        });

        await _unitOfWork.SaveChangesAsync();
        return ProductResponseDTO.FromEntity(product, includeInternal: true);
    }
}