using System.Linq.Expressions;

namespace MagicVilla_API.Repositorio.IRepositorio
{
    //Todos los modelos pueden utilizar este repositorio
    public interface IRepositorio<T> where T : class //De esta forma, definimos que esta Interfaz será Genérica
    {
        Task Crear(T entidad);
        Task<List<T>> ObtenerTodos(Expression<Func<T, bool>> ? filtro = null); //Expresión Linq
        Task<T> Obtener(Expression<Func<T, bool>> filtro = null, bool tracked = true);
        Task Remover(T entidad);
        //Task<T> Remover(T entidad);
        Task Grabar();
        //Task<T> Grabar();



    }
}
