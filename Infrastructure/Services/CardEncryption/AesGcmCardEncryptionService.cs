using System.Security.Cryptography;
using System.Text;
using Shopniu_api.Aplication.Common.Ports.CardEncryption;

namespace Shopniu_api.Infrastructure.Services.CardEncryption;

/// <summary>Cifra el PAN en reposo con AES-256-GCM. La clave (32 bytes en
/// base64) vive en la variable de entorno CARD_ENCRYPTION_KEY: en desarrollo
/// en user-secrets, en producción la inyecta el contenedor (hoy env var
/// directa; Key Vault queda pendiente). Formato almacenado:
/// base64(IV + ciphertext + tag), con IV aleatorio por registro.</summary>
public class AesGcmCardEncryptionService : ICardEncryptionService
{
    private const int IvSize = 12;
    private const int TagSize = 16;

    private readonly byte[] _key;

    public AesGcmCardEncryptionService(IConfiguration configuration)
    {
        var rawKey = configuration["CARD_ENCRYPTION_KEY"]
            ?? throw new InvalidOperationException(
                "CARD_ENCRYPTION_KEY is not configured.");
        _key = Convert.FromBase64String(rawKey);
        if (_key.Length != 32)
        {
            throw new InvalidOperationException(
                "CARD_ENCRYPTION_KEY must be a 32-byte key in base64.");
        }
    }

    public string? Encrypt(string? plainText)
    {
        if (string.IsNullOrWhiteSpace(plainText))
        {
            return null;
        }

        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var iv = RandomNumberGenerator.GetBytes(IvSize);
        var ciphertext = new byte[plainBytes.Length];
        var tag = new byte[TagSize];

        using var aes = new AesGcm(_key, TagSize);
        aes.Encrypt(iv, plainBytes, ciphertext, tag);

        return Convert.ToBase64String(
            iv.Concat(ciphertext).Concat(tag).ToArray());
    }

    public string? Decrypt(string? cipherText)
    {
        if (string.IsNullOrWhiteSpace(cipherText))
        {
            return null;
        }

        byte[] bytes;
        try
        {
            bytes = Convert.FromBase64String(cipherText);
        }
        catch (FormatException)
        {
            return null;
        }

        if (bytes.Length < IvSize + TagSize)
        {
            return null;
        }

        var iv = bytes[..IvSize];
        var tag = bytes[^TagSize..];
        var ciphertext = bytes[IvSize..^TagSize];
        var plainBytes = new byte[ciphertext.Length];

        try
        {
            using var aes = new AesGcm(_key, TagSize);
            aes.Decrypt(iv, ciphertext, tag, plainBytes);
        }
        catch (CryptographicException)
        {
            return null;
        }

        return Encoding.UTF8.GetString(plainBytes);
    }
}