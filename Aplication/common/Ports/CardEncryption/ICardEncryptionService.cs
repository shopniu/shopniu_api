namespace Shopniu_api.Aplication.Common.Ports.CardEncryption;

/// <summary>Cifrado del PAN en reposo. El único dato de tarjeta que el backend
/// persiste es el PAN, siempre cifrado; el navegador del dueño lo recupera
/// descifrado solo para re-tokenizarlo en la próxima compra.</summary>
public interface ICardEncryptionService
{
    string? Encrypt(string? plainText);
    string? Decrypt(string? cipherText);
}