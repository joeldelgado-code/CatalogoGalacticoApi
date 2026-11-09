
using System.Security.Cryptography;
using System.Text;

namespace CatalogoGalacticoApi.Services;

public class CifradoService : ICifradoService
{
    private const int TamanoNonce = 12;
    private const int TamanoTag = 16;

    private readonly byte[] _clave;

    public CifradoService(IConfiguration configuracion)
    {
        var claveBase64 = configuracion["Cifrado:Clave"];

        if (string.IsNullOrWhiteSpace(claveBase64))
        {
            throw new InvalidOperationException(
                "No se encontró la clave de cifrado.");
        }

        _clave = Convert.FromBase64String(claveBase64);

        if (_clave.Length != 32)
        {
            throw new InvalidOperationException(
                "La clave de cifrado debe tener 32 bytes.");
        }
    }

    public string Cifrar(string texto)
    {
        byte[] nonce = RandomNumberGenerator.GetBytes(TamanoNonce);
        byte[] textoBytes = Encoding.UTF8.GetBytes(texto);
        byte[] textoCifrado = new byte[textoBytes.Length];
        byte[] tag = new byte[TamanoTag];

        using (var aes = new AesGcm(_clave, TamanoTag))
        {
            aes.Encrypt(nonce, textoBytes, textoCifrado, tag);
        }

        byte[] resultado = new byte[
            TamanoNonce + TamanoTag + textoCifrado.Length];

        Buffer.BlockCopy(nonce, 0, resultado, 0, TamanoNonce);
        Buffer.BlockCopy(tag, 0, resultado, TamanoNonce, TamanoTag);
        Buffer.BlockCopy(
            textoCifrado, 0, resultado,
            TamanoNonce + TamanoTag, textoCifrado.Length);

        return Convert.ToBase64String(resultado);
    }

    public string Descifrar(string textoCifrado)
    {
        byte[] datos = Convert.FromBase64String(textoCifrado);

        if (datos.Length < TamanoNonce + TamanoTag)
        {
            throw new CryptographicException(
                "Los datos cifrados no son válidos.");
        }

        byte[] nonce = new byte[TamanoNonce];
        byte[] tag = new byte[TamanoTag];

        int tamanoCifrado = datos.Length - TamanoNonce - TamanoTag;
        byte[] cifrado = new byte[tamanoCifrado];
        byte[] textoOriginal = new byte[tamanoCifrado];

        Buffer.BlockCopy(datos, 0, nonce, 0, TamanoNonce);
        Buffer.BlockCopy(datos, TamanoNonce, tag, 0, TamanoTag);
        Buffer.BlockCopy(
            datos, TamanoNonce + TamanoTag,
            cifrado, 0, tamanoCifrado);

        using (var aes = new AesGcm(_clave, TamanoTag))
        {
            aes.Decrypt(nonce, cifrado, tag, textoOriginal);
        }

        return Encoding.UTF8.GetString(textoOriginal);
    }
}
