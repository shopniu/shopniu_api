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
    private readonly ISupplierRepository _supplierRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public CreateProductUseCase(
        IProductRepository productRepository,
        ISupplierRepository supplierRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _productRepository = productRepository;
        _supplierRepository = supplierRepository;
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

        // Si se asocia un proveedor registrado, el nombre se toma de él como
        // snapshot en vez del texto libre.
        string? supplierName = dto.SupplierName;
        if (dto.SupplierId.HasValue)
        {
            var supplier = await _supplierRepository.GetByIdAsync(dto.SupplierId.Value)
                ?? throw new NotFoundException("Supplier", dto.SupplierId.Value);
            supplierName = supplier.Name;
        }

        var product = await _productRepository.CreateAsync(new Product(
            name: dto.Name,
            price: dto.Price,
            imageUrl: dto.ImageUrl,
            description: dto.Description,
            stock: dto.Stock,
            userId: userId,
            sourcing: dto.Sourcing ?? ProductSourcing.LocalStock,
            certifiedOriginal: dto.CertifiedOriginal ?? false,
            costPrice: dto.CostPrice,
            supplierName: supplierName,
            leadTimeDays: dto.LeadTimeDays,
            supplierId: dto.SupplierId
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