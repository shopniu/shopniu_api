// Domain/Exceptions/BusinessRuleException.cs
namespace Shopniu_api.Domain.Exceptions
{
    // Para reglas de dominio específicas, ej: precio no puede ser negativo
    public class BusinessRuleException : DomainException
    {
        public BusinessRuleException(string message) : base(message) { }
    }
}