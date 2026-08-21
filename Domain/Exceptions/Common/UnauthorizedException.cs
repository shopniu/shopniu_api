namespace Shopniu_api.Domain.Exceptions.Common;

/// <summary>La petición no resolvió un usuario autenticado cuando el flujo lo
/// exige (ej. userId 0 detrás de una política de permisos: suele indicar una
/// configuración de issuer/claims incorrecta).</summary>
public class UnauthorizedException : DomainException
{
    public UnauthorizedException(string message)
        : base(message)
    {
    }
}