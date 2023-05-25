using AutoMapper;
using Azure;
using MagicVilla_API.Datos;
using MagicVilla_API.Models;
using MagicVilla_API.Models.DTO;
using MagicVilla_API.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Runtime.ConstrainedExecution;

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
        //Reemplazamos la variable de tipo DbContext por la interfaz de villa repositorio (IVillaRepositorio)
        private readonly IVillaRepositorio _villaRepo;

        //Inyectamos AutoMapper
        private readonly IMapper _mapper;

        //Agregamos nuestra variable de tipo APIResponse
        protected APIResponse _response; //Por lo general esta variable es de tipo protected


        //Lo inyectamos en el constructor
        public VillaController(ILogger<VillaController> logger, IVillaRepositorio villaRepo, IMapper mapper)
        {
            _logger = logger;
            //_db=db;
            _villaRepo = villaRepo;
            _mapper = mapper;
            _response = new(); //Esta variable no es inyectada, sino que la inicializamos
        }

        //CADA ENDPOINT DEBE TENER SU VERBO
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        //Convertimos nuestro método síncrono en ASÍNCRONO
        //public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()

        //------------------Utilizando APIResponse------------------
        public async Task<ActionResult<APIResponse>> GetVillas()
        {
            //A partir de APIResponse, utilizamos el bloque try catch
            try
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
                //IEnumerable<Villa> villaList = await _db.Villas.ToListAsync();
                //return Ok(_mapper.Map<IEnumerable<VillaDTO>>(villaList));

                //------------------Utilizando la Interfaz IVillaRepositorio------------------
                IEnumerable<Villa> villaList = await _villaRepo.ObtenerTodos();
                //return Ok(_mapper.Map<IEnumerable<VillaDTO>>(villaList));

                //------------------Utilizando APIResponse------------------
                _response.Resultado = _mapper.Map<IEnumerable<VillaDTO>>(villaList);
                _response.StatusCode = HttpStatusCode.OK;

                return Ok(_response);
            }
            catch (Exception e)
            {
                _response.IsExitoso = false;
                _response.ErrorMessages = new List<string>() { e.ToString() };
            }

            return _response;
        }

        [HttpGet("id:int", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //Convertimos nuestro método síncrono en ASÍNCRONO

        //public async Task<ActionResult<VillaDTO>> GetVillaID(int id)
        //------------------Utilizando APIResponse------------------
        public async Task<ActionResult<APIResponse>> GetVillaID(int id)
        //ActionResult<Modelo que usamos>
        {
            //A partir de APIResponse, utilizamos el bloque try catch

            try
            {
                if (id == 0)
                {
                    _logger.LogError("Error al traer Villa con id " + id);
                    //return BadRequest();

                    //------------------Utilizando APIResponse------------------
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsExitoso = false;
                    return BadRequest(_response);
                }

                //------------------Utilizando VillaStore------------------
                //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);

                //------------------Utilizando nuestra BD------------------
                //var villa = await _db.Villas.FirstOrDefaultAsync(v => v.Id == id);

                //------------------Utilizando la Interfaz IVillaRepositorio------------------
                var villa = await _villaRepo.Obtener(v => v.Id == id);

                if (villa == null)
                {
                    //return NotFound();

                    //------------------Utilizando APIResponse------------------
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsExitoso = false;
                    return NotFound(_response);
                }


                //return Ok(villa);

                //------------------Utilizando Auto Mapper------------------
                //return Ok(_mapper.Map<VillaDTO>(villa));

                //------------------Utilizando APIResponse------------------

                _response.Resultado = _mapper.Map<VillaDTO>(villa);
                _response.StatusCode = HttpStatusCode.OK;

                return Ok(_response);
            }
            catch (Exception e)
            {
                _response.IsExitoso = false;
                _response.ErrorMessages = new List<string>() { e.ToString() };
            }

            return _response;
        }

        //POST
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] //=>Error de Servidor
        //Convertimos nuestro método síncrono en ASÍNCRONO
        //public async Task<ActionResult<VillaDTO>> PostVilla([FromBody] VillaCreateDTO createDto)

        //------------------Utilizando APIResponse------------------
        public async Task<ActionResult<APIResponse>> PostVilla([FromBody] VillaCreateDTO createDto)
        {
            try
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
                //if (await _db.Villas.FirstOrDefaultAsync(v => v.Nombre.ToLower() == createDto.Nombre.ToLower()) != null)
                //{
                //    ModelState.AddModelError("NombreExiste", "La Villa con ese Nombre YA existe");
                //    return BadRequest(ModelState);
                //}

                //------------------Utilizando la Interfaz IVillaRepositorio------------------
                if (await _villaRepo.Obtener(v => v.Nombre.ToLower() == createDto.Nombre.ToLower()) != null)
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

                //------------------Utilizando nuestra BD------------------
                //await _db.Villas.AddAsync(modelo);
                //await _db.SaveChangesAsync();

                //Para que la fecha de creación y la de actualización no se almacenen con valores incorrectos, antes de enviar la informacion a nuestro repositorio debemos actualizar dichas fechas. (Si no actualizamos, envía valores incorrectos y al actualizar, cada registro se carga con las fechas correspondientes)
                modelo.FechaCreacion = DateTime.Now;
                modelo.FechaActualizacion = DateTime.Now;

                //------------------Utilizando la Interfaz IVillaRepositorio------------------
                await _villaRepo.Crear(modelo);

                //------------------Utilizando y agregando APIResponse------------------
                _response.Resultado = modelo;
                _response.StatusCode = HttpStatusCode.Created;

                //return CreatedAtRoute("GetVilla", new { id = modelo.Id }, modelo);

                //------------------Utilizando APIResponse------------------
                return CreatedAtRoute("GetVilla", new { id = modelo.Id }, _response);
            }
            catch (Exception e)
            {
                _response.IsExitoso = false;
                _response.ErrorMessages = new List<string>() { e.ToString() };
            }
            return _response;

        }

        //DELETE
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //Convertimos nuestro método síncrono en ASÍNCRONO

        //public async Task<IActionResult> DeleteVilla(int id) //=> Usamos IActionResult en vez de ActionResult<> porque el método no necesita el modelo. Cuándo se trabaja con Delete, se debe retornar un no content

        //------------------Utilizando APIResponse------------------
        //public async Task<IActionResult<APIResponse>> DeleteVilla(int id)  //En caso del DELETE, devuelve un error debido a que las Interfaces no pueden tener un tipo de retorno
        public async Task<IActionResult> DeleteVilla(int id) //Le sacamos el tipo de retorno
        {
            try
            {
                if (id == 0)
                {
                    //return BadRequest();

                    //------------------Utilizando APIResponse------------------
                    _response.IsExitoso = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;

                    return BadRequest(_response);

                }

                //------------------Utilizando VillaStore------------------
                //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);

                //------------------Utilizando nuestra BD------------------
                //var villa = await _db.Villas.FirstOrDefaultAsync(v => v.Id == id);

                //------------------Utilizando la Interfaz IVillaRepositorio------------------

                var villa = await _villaRepo.Obtener(v => v.Id == id);

                if (villa == null)
                {
                    //return NotFound();

                    //------------------Utilizando APIResponse------------------
                    _response.IsExitoso = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                ///------------------Utilizando VillaStore------------------
                //VillaStore.villaList.Remove(villa);

                //------------------Utilizando nuestra BD------------------
                //_db.Villas.Remove(villa);
                //await _db.SaveChangesAsync();

                //------------------Utilizando la Interfaz IVillaRepositorio------------------
                await _villaRepo.Remover(villa);
                //return NoContent();

                //------------------Utilizando APIResponse------------------
                _response.StatusCode = HttpStatusCode.NoContent;
                return Ok(_response); //Cambiamos de NotFound a Ok debido a que NotFound no puede recibir parámetros
            }
            catch (Exception e)
            {
                _response.IsExitoso = false;
                _response.ErrorMessages = new List<string>() { e.ToString() };

            }

            //return _response; //para que podamos hacer uso del return del response, usamos un Bad Request
            return BadRequest(_response);
        }

        //PUT
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        //Convertimos nuestro método síncrono en ASÍNCRONO
        //public async Task<IActionResult> PutVilla(int id, [FromBody] VillaUpdateDTO updateDto)
        //Usamos IActionResult porque devuelve un no content

        //------------------Utilizando APIResponse------------------
        public async Task<IActionResult> PutVilla(int id, [FromBody] VillaUpdateDTO updateDto)  //En caso del UPDATE, devuelve un error debido a que las Interfaces no pueden tener un tipo de retorno
        {

            if (updateDto == null || id != updateDto.Id)
            {

                //return BadRequest();

                //------------------Utilizando APIResponse------------------
                _response.IsExitoso = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
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

            //------------------Utilizando nuestra BD------------------
            //_db.Villas.Update(modelo);
            //await _db.SaveChangesAsync();

            //------------------Utilizando la Interfaz IVillaRepositorio------------------
            await _villaRepo.Actualizar(modelo);

            //return NoContent();
            //------------------Utilizando APIResponse------------------
            _response.StatusCode = HttpStatusCode.NoContent;
            return Ok(_response);
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
            //var villa = await _db.Villas.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);

            //------------------Utilizando la Interfaz IVillaRepositorio------------------
            var villa = await _villaRepo.Obtener(v => v.Id == id, tracked: false);


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

            //------------------Utilizando nuestra BD------------------
            //_db.Villas.Update(modelo);
            //await _db.SaveChangesAsync();

            //------------------Utilizando la Interfaz IVillaRepositorio------------------
            await _villaRepo.Actualizar(modelo);

            //return NoContent();

            //------------------Utilizando APIResponse------------------
            _response.StatusCode = HttpStatusCode.NoContent;
            return Ok(_response);

            //UPDATE Y DELETE NO SON ASÍNCRONOS
        }
    }

}
