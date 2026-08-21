namespace Shopniu_api.Domain.Exceptions.Common;

/// <summary>Autenticado pero sin autorización sobre el recurso (403).</summary>
public class ForbiddenException : DomainException
{
    public ForbiddenException(string message)
        : base(message)
    {

    }
}
