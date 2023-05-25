using System.Net;

namespace MagicVilla_API.Models
{
    public class APIResponse
    {
        //creamos esta clase para retornar una respuesta por parte de cada petición
        public HttpStatusCode StatusCode { get; set; } //Código de estado
        public bool IsExitoso { get; set; } = true; //Que la espuesta de la API sea exitosa
        public List<string> ErrorMessages { get; set; } //Lista con todos los errores que pueden aparecer
        public object Resultado { get; set; } //Al utilizar object podemos almacenar, en este caso, cualquier lista de objetos
    }
}
