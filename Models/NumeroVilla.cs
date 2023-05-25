using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MagicVilla_API.Models
{
    public class NumeroVilla //Esta clase va a definir una relacion de 1 a varios (La clase Villa es la clase padre y NumeroVilla es la clase hija). Una villa puede tener varios numeros de villa (pk - fk)
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        //DatabaseGenerated(DatabaseGeneratedOption.None) => Con esta opción, se genera una Primary Key pero no es automática, sino que nosotros lo cargamos
        public int VillaNo { get; set; } //Id de NumeroVilla. No queremos que SQLServer le asigne su ID automaticamente, sino que lo ahremos de forma manual. Villa 101, 102, etc

        [Required]
        public int VillaId { get; set; } //Relacion con la tabla Villa (FK)

        [ForeignKey("VillaId")]
        public Villa Villa { get; set; } //fk.Navegación. [Relacion con la tabla Villa]

        public string DetalleEspecial { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }


        /*
         Clase
        DTO
        Interfaz
        Repositorio
        Agregar servicio al Program
         */
    }
}
