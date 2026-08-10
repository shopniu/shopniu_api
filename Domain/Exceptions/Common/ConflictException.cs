// Domain/Exceptions/ConflictException.cs
namespace Shopniu_api.Domain.Exceptions
{
    // Para reglas de negocio violadas, ej: stock insuficiente, SKU duplicado
    public class ConflictException : DomainException
    {
        public ConflictException(string message) : base(message) { }
    }
}