using AutoMapper;
using MagicVilla_API.Datos;
using MagicVilla_API.Models;
using MagicVilla_API.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_API.Controllers
{
    //ENDPOINT: METODO DE C#
    //Ruta api/Nombre del controlador => api/Villa
    [Route("api/[controller]")]

    //Controlador de tipo API
    [ApiController]

    public class VillaController : ControllerBase
    //Hereda de ControllerBase
    //UNA API RETORNA DATOS
    {
        //Servicio LOGGER => Una de las acciones de Logger es mostrar información en la consola acerca de la petición realizada
        private readonly ILogger<VillaController> _logger;

        //Inyectamos nuestro dbContext
        private readonly ApplicationDbContext _db;

        //Inyectamos AutoMapper
        private readonly IMapper _mapper;


        //Lo inyectamos en el constructor
        public VillaController(ILogger<VillaController> logger, ApplicationDbContext db, IMapper mapper)
        {
            _logger = logger;
            _db = db;
            _mapper = mapper;
        }

        //CADA ENDPOINT DEBE TENER SU VERBO
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        //Convertimos nuestro método síncrono en ASÍNCRONO
        public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
        {
            _logger.LogInformation("Obtener todas las villas"); //=> Muestra este mensaje en la consola
                                                                //return new List<VillaDTO> { //=> Retorna estos dos nuevos objetos hardcodeados
                                                                //    new VillaDTO{Id = 1, Nombre = "Vista a la Piscina"},
                                                                //    new VillaDTO{Id = 2, Nombre = "Vista a la Playa"},
                                                                //};


            //------------------Utilizando VillaStore (Simulación de datos cargados)------------------
            //return Ok(VillaStore.villaList);

            //------------------Utilizando nuestra BD------------------
            //return Ok(await _db.Villas.ToListAsync());
            //ToList(): Método Sincrónico
            //ToListAsync(): Método Asincrónico


            //------------------Utilizando Auto Mapper------------------
            //Creamos una lista para poder utilizar el Auto Mapper
            IEnumerable<Villa> villaList = await _db.Villas.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<VillaDTO>>(villaList));


        }

        [HttpGet("id:int", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //Convertimos nuestro método síncrono en ASÍNCRONO
        public async Task<ActionResult<VillaDTO>> GetVillaID(int id)
        //ActionResult<Modelo que usamos>
        {
            if (id == 0)
            {
                _logger.LogError("Error al traer Villa con id " + id);
                return BadRequest();
            }

            //------------------Utilizando VillaStore------------------
            //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);

            //------------------Utilizando nuestra BD------------------
            var villa = await _db.Villas.FirstOrDefaultAsync(v => v.Id == id);

            if (villa == null)
            {
                return NotFound();
            }

            //return Ok(villa);

            //------------------Utilizando Auto Mapper------------------
            return Ok(_mapper.Map<VillaDTO>(villa));

        }

        //POST
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] //=>Error de Servidor
        //Convertimos nuestro método síncrono en ASÍNCRONO
        public async Task<ActionResult<VillaDTO>> PostVilla([FromBody] VillaCreateDTO createDto)
        {
            //Controlar si el modelo es válido
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //------------------Utilizando VillaStore------------------
            //if (VillaStore.villaList.FirstOrDefault(v => v.Nombre.ToLower() == villaDto.Nombre.ToLower()) != null)
            //{
            //ModelState.AddModelError("NombreExiste", "La Villa con ese Nombre YA existe");
            //  return BadRequest(ModelState);
            //}

            //------------------Utilizando nuestra BD------------------
            if (await _db.Villas.FirstOrDefaultAsync(v => v.Nombre.ToLower() == createDto.Nombre.ToLower()) != null)
            {
                ModelState.AddModelError("NombreExiste", "La Villa con ese Nombre YA existe");
                return BadRequest(ModelState);
            }

            if (createDto == null)
            {
                return BadRequest(createDto);
            }

            //Hacemos esta validacion debido a que estamos generando un nuevo registro, no necesitando el id
            //Actualización:Al tener un DTO para crear, y al no necesitar el ID, no necesitamos realizar esta validación
            //if (villaDto.Id > 0)
            //{
            //    return StatusCode(StatusCodes.Status500InternalServerError);
            //}

            //------------------Utilizando VillaStore------------------
            //villaDto.Id = VillaStore.villaList.OrderByDescending(v => v.Id).FirstOrDefault().Id + 1;
            //VillaStore.villaList.Add(villaDto);
            //return CreatedAtRoute("GetVilla", new { id = villaDto.Id }, villaDto);

            //------------------Utilizando nuestra BD------------------
            //Villa modelo = new Villa()
            //{
            //    Nombre = villaDto.Nombre,
            //    Detalle = villaDto.Detalle,
            //    ImagenUrl = villaDto.ImagenUrl,
            //    Ocupantes = villaDto.Ocupantes,
            //    Tarifa = villaDto.Tarifa,
            //    MetrosCuadrados = villaDto.MetrosCuadrados,
            //    Amenidad = villaDto.Amenidad

            //};

            //------------------Utilizando Auto Mapper------------------
            //Utilizando Auto Mapper, nos ahorramos la creación del objeto de arriba
            Villa modelo = _mapper.Map<Villa>(createDto);

            await _db.Villas.AddAsync(modelo);
            await _db.SaveChangesAsync();

            return CreatedAtRoute("GetVilla", new { id = modelo.Id }, createDto);
        }

        //DELETE
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //Convertimos nuestro método síncrono en ASÍNCRONO
        public async Task<IActionResult> DeleteVilla(int id) //=> Usamos IActionResult en vez de ActionResult<> porque el método no necesita el modelo. Cuándo se trabaja con Delete, se debe retornar un no content
        {

            if (id == 0)
            {
                return BadRequest();
            }

            //------------------Utilizando VillaStore------------------
            //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);

            //------------------Utilizando nuestra BD------------------
            var villa = await _db.Villas.FirstOrDefaultAsync(v => v.Id == id);

            if (villa == null)
            {
                return NotFound();
            }

            ///------------------Utilizando VillaStore------------------
            //VillaStore.villaList.Remove(villa);

            //------------------Utilizando nuestra BD------------------
            _db.Villas.Remove(villa);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        //PUT
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        //Convertimos nuestro método síncrono en ASÍNCRONO
        public async Task<IActionResult> PutVilla(int id, [FromBody] VillaUpdateDTO updateDto)
        //Usamos IActionResult porque devuelve un no content
        {
            if (updateDto == null || id != updateDto.Id)
            {
                return BadRequest();
            }

            //------------------Utilizando VillaStore------------------
            //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
            //villa.Nombre = villaDto.Nombre;
            //villa.Ocupantes = villaDto.Ocupantes;
            //villa.MetrosCuadrados = villaDto.MetrosCuadrados;

            //------------------Utilizando nuestra BD------------------
            //Villa modelo = new Villa()
            //{
            //    Id = villaDto.Id,
            //    Nombre = villaDto.Nombre,
            //    Detalle = villaDto.Detalle,
            //    ImagenUrl = villaDto.ImagenUrl,
            //    Ocupantes = villaDto.Ocupantes,
            //    Tarifa = villaDto.Tarifa,
            //    MetrosCuadrados = villaDto.MetrosCuadrados,
            //    Amenidad = villaDto.Amenidad

            //};

            //------------------Utilizando Auto Mapper------------------
            //Utilizando Auto Mapper, nos ahorramos la creación del objeto de arriba
            Villa modelo = _mapper.Map<Villa>(updateDto);

            _db.Villas.Update(modelo);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        //PATCH
        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PatchVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDto)
        //Usamos IActionResult porque devuelve un no content
        {
            if (patchDto == null || id == 0)
            {
                return BadRequest();
            }

            //------------------Utilizando VillaStore------------------
            //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);

            //------------------Utilizando nuestra BD------------------
            //Utilizando AsNoTracking => Permite consultar un registro sin que se trackee evitando que se genere un error
            var villa = await _db.Villas.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);

       

            //VillaUpdateDTO VillaUpdateDTO = new()
            //{
            //    Id = villa.Id,
            //    Nombre = villa.Nombre,
            //    Detalle = villa.Detalle,
            //    ImagenUrl = villa.ImagenUrl,
            //    Ocupantes = villa.Ocupantes,
            //    Tarifa = villa.Tarifa,
            //    MetrosCuadrados = villa.MetrosCuadrados,
            //    Amenidad = villa.Amenidad

            //};

            //------------------Utilizando Auto Mapper------------------
            //Utilizando Auto Mapper, nos ahorramos la creación del objeto de arriba

            VillaUpdateDTO villaDto = _mapper.Map<VillaUpdateDTO>(villa);

            if (villa == null) return BadRequest();

            //patchDto.ApplyTo(villa, ModelState);
            patchDto.ApplyTo(villaDto, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Villa modelo = new()
            //{
            //    Id = villaDto.Id,
            //    Nombre = villaDto.Nombre,
            //    Detalle = villaDto.Detalle,
            //    ImagenUrl = villaDto.ImagenUrl,
            //    Ocupantes = villaDto.Ocupantes,
            //    Tarifa = villaDto.Tarifa,
            //    MetrosCuadrados = villaDto.MetrosCuadrados,
            //    Amenidad = villaDto.Amenidad

            //};

            //------------------Utilizando Auto Mapper------------------
            //Utilizando Auto Mapper, nos ahorramos la creación del objeto de arriba
            Villa modelo = _mapper.Map<Villa>(villaDto);

            _db.Villas.Update(modelo);
            await _db.SaveChangesAsync();

            return NoContent();

            //UPDATE Y DELETE NO SON ASÍNCRONOS
        }
    }

}
