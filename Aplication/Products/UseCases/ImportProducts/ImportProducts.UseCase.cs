using FluentValidation;
using Shopniu_api.Aplication.Common.Ports.Identity;
using Shopniu_api.Aplication.Products.Common.DTOs;
using Shopniu_api.Aplication.Products.UseCases.ImportProducts;
using Shopniu_api.Domain.Entities.ProductEntity;
using Shopniu_api.Domain.Exceptions.Common;
using Shopniu_api.Domain.Repositories;

namespace Shopniu_api.Aplication.Products;

/// <summary>Importación batch de catálogo de proveedor (dropshipping). Aplica
/// el markup configurado en `DropShipping:MarkupPercent` para derivar el precio
/// de venta desde el costo del proveedor. Cada ítem se procesa de forma
/// independiente: un ítem inválido no aborta el resto del lote.</summary>
public class ImportProductsUseCase
{
    private readonly IProductRepository _productRepository;
    private readonly ISupplierRepository _supplierRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly decimal _markupPercent;

    public ImportProductsUseCase(
        IProductRepository productRepository,
        ISupplierRepository supplierRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IConfiguration configuration)
    {
        _productRepository = productRepository;
        _supplierRepository = supplierRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _markupPercent = configuration.GetValue<decimal>("DropShipping:MarkupPercent", 30);
    }

    public async Task<ImportProductsResult> ExecuteAsync(ImportProductsRequest dto)
    {
        var userId = _currentUser.UserId;
        if (userId == 0)
        {
            throw new UnauthorizedException(
                "No authenticated user was resolved for this request.");
        }

        var products = new List<ProductResponseDTO>();
        var errors = new List<string>();

        foreach (var item in dto.Items)
        {
            try
            {
                // Si el ítem referencia un proveedor registrado, se valida su
                // existencia y se toma el nombre del proveedor como snapshot.
                string? supplierName = item.SupplierName;
                if (item.SupplierId.HasValue)
                {
                    var supplier = await _supplierRepository.GetByIdAsync(item.SupplierId.Value)
                        ?? throw new NotFoundException("Supplier", item.SupplierId.Value);
                    supplierName = supplier.Name;
                }

                var price = Math.Round(item.CostPrice * (1 + _markupPercent / 100m), 2);
                var product = await _productRepository.CreateAsync(new Product(
                    name: item.Name,
                    price: price,
                    imageUrl: item.ImageUrl,
                    description: item.Description,
                    stock: item.Stock,
                    userId: userId,
                    sourcing: ProductSourcing.External,
                    costPrice: item.CostPrice,
                    supplierName: supplierName,
                    leadTimeDays: item.LeadTimeDays,
                    supplierId: item.SupplierId
                ));

                // El importador queda registrado como dueño del producto.
                await _productRepository.AddOwnerAsync(new ProductOwner
                {
                    Product = product,
                    UserId = userId
                });

                products.Add(ProductResponseDTO.FromEntity(product, includeInternal: true));
            }
            catch (Exception ex) when (ex is ValidationsException or ValidationException or NotFoundException)
            {
                errors.Add($"{item.Name}: {ex.Message}");
            }
        }

        await _unitOfWork.SaveChangesAsync();
        return new ImportProductsResult(products.Count, errors, products);
    }
}
