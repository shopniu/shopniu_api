using Shopniu_api.Domain.Repositories;
using Shopniu_api.Domain.Exceptions.Common;
using Shopniu_api.Domain.Entities.SupplierEntity;
using Shopniu_api.Aplication.Suppliers.Common.DTOs;
using Shopniu_api.Aplication.Common.Ports.Identity;

namespace Shopniu_api.Aplication.Suppliers.UseCases.CreateSupplier;

public class CreateSupplierUseCase
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public CreateSupplierUseCase(ISupplierRepository supplierRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _supplierRepository = supplierRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<SupplierResponseDTO> ExecuteAsync(SupplierRequest dto)
    {
        var userId = _currentUser.UserId;
        if (userId == 0)
        {
            throw new UnauthorizedException(
                "No authenticated user was resolved for this request.");
        }

        var supplier = await _supplierRepository.CreateAsync(new Supplier(
            name: dto.Name,
            region: dto.Region,
            defaultShipping: dto.DefaultShipping,
            defaultLeadTimeDays: dto.DefaultLeadTimeDays,
            isActive: dto.IsActive
        ));

        await _unitOfWork.SaveChangesAsync();
        return SupplierResponseDTO.FromEntity(supplier);
    }
}
