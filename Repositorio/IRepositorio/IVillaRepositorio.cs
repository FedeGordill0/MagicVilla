using MagicVilla_API.Models;

namespace MagicVilla_API.Repositorio.IRepositorio
{
    public interface IVillaRepositorio : IRepositorio<Villa> //Heredamos el Repositorio genérico y trabajamos sobre la entidad Villa
    {
        Task<Villa> Actualizar(Villa entidad);
    }
}
