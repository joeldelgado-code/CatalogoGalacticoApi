
namespace CatalogoGalacticoApi.Services;

public interface ICifradoService
{
    string Cifrar(string texto);
    string Descifrar(string textoCifrado);
}
