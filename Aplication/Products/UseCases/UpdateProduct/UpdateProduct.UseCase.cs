using Shopniu_api.Domain.Repositories;
using Shopniu_api.Domain.Exceptions.Common;
using Shopniu_api.Aplication.Products.UseCases.UpdateProduct;
using Shopniu_api.Aplication.Products.Common.DTOs;
using Shopniu_api.Aplication.Common.Ports.Identity;

namespace Shopniu_api.Aplication.Products;

public class UpdateProductUseCase
{
    private readonly IProductRepository _productRepository;
    private readonly ISupplierRepository _supplierRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public UpdateProductUseCase(
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

    public async Task<ProductResponseDTO> ExecuteAsync(int id, UpdateProductRequest dto)
    {
        // La política product.update ya garantiza un usuario autenticado; si
        // llega 0 es una misconfiguración de issuer/claims.
        var userId = _currentUser.UserId;
        if (userId == 0)
        {
            throw new UnauthorizedException(
                "No authenticated user was resolved for this request.");
        }

        var product = await _productRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("Product", id);

        // Solo el dueño (según ProductOwners) puede editar el producto.
        if (!await _productRepository.IsOwnedByAsync(id, userId))
        {
            throw new ForbiddenException(
                $"User {userId} is not allowed to edit product {id}.");
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

        product.Update(
            name: dto.Name,
            price: dto.Price,
            imageUrl: dto.ImageUrl,
            description: dto.Description,
            stock: dto.Stock,
            sourcing: dto.Sourcing,
            certifiedOriginal: dto.CertifiedOriginal,
            costPrice: dto.CostPrice,
            supplierName: supplierName,
            leadTimeDays: dto.LeadTimeDays,
            supplierId: dto.SupplierId
        );

        await _productRepository.UpdateAsync(product);
        await _unitOfWork.SaveChangesAsync();
        return ProductResponseDTO.FromEntity(product, includeInternal: true);
    }
}
