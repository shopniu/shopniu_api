// Infrastructure/Adapters/Payment/Wompi/WompiStatusMapper.cs
using Shopniu_api.Domain.Entities.TransactionEntity;
using Shopniu_api.Domain.Exceptions;

namespace Shopniu_api.Infrastructure.ExternalServices.Payment.Wompi.Services;

public static class WompiStatusMapper
{
    public static TransactionStatus Map(string wompiStatus) => wompiStatus switch
    {
        "APPROVED" => TransactionStatus.COMPLETED,
        "DECLINED" => TransactionStatus.FAILED,
        "PENDING" => TransactionStatus.PENDING,
        "VOIDED" => TransactionStatus.CANCELED,
        "ERROR" => TransactionStatus.REFUNDED,
        _ => throw new BusinessRuleException($"Estado de transacción desconocido: '{wompiStatus}'")
    };
}