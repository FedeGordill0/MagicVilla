using AutoMapper;
using Azure;
using MagicVilla_API.Datos;
using MagicVilla_API.Models;
using MagicVilla_API.Models.DTO;
using MagicVilla_API.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Runtime.ConstrainedExecution;

namespace MagicVilla_API.Controllers
{

    [Route("api/[controller]")]


    [ApiController]

    public class NumeroVillaController : ControllerBase

    {

        private readonly ILogger<NumeroVillaController> _logger;


        private readonly ApplicationDbContext _db;

        private readonly IVillaRepositorio _villaRepo;

        private readonly INumeroVillaRepositorio _numeroVillaRepo;


        private readonly IMapper _mapper;


        protected APIResponse _response;



        public NumeroVillaController(ILogger<NumeroVillaController> logger, IVillaRepositorio villaRepo, INumeroVillaRepositorio numeroVillaRepo, IMapper mapper)
        {
            _logger = logger;

            _villaRepo = villaRepo;
            _numeroVillaRepo = numeroVillaRepo;
            _mapper = mapper;
            _response = new();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetNumeroVillas()
        {
            try
            {
                _logger.LogInformation("Obtener todos los números de villas");

                IEnumerable<NumeroVilla> numeroVillaList = await _numeroVillaRepo.ObtenerTodos();

                _response.Resultado = _mapper.Map<IEnumerable<NumeroVillaDTO>>(numeroVillaList);
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

        [HttpGet("id:int", Name = "GetNumeroVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<APIResponse>> GetNumeroVillaID(int id)
        {
            try
            {
                if (id == 0)
                {
                    _logger.LogError("Error al traer el número de Villa con id " + id);

                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsExitoso = false;
                    return BadRequest(_response);
                }

                var numeroVilla = await _numeroVillaRepo.Obtener(v => v.VillaNo == id);

                if (numeroVilla == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsExitoso = false;
                    return NotFound(_response);
                }

                _response.Resultado = _mapper.Map<NumeroVillaDTO>(numeroVilla);
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
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<APIResponse>> PostNumeroVilla([FromBody] NumeroVillaCreateDTO createDto)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                //Validamos que no exista el mismo número de Villa
                if (await _numeroVillaRepo.Obtener(v => v.VillaNo == createDto.VillaNo) != null)
                {
                    ModelState.AddModelError("NombreExiste", "El numero de Villa YA existe");
                    return BadRequest(ModelState);
                }

                //Validamos si existe o no el ID padre
                //villa repo ovbtiene el id del padre, y comparamos si dicho id existe o no dentro del hijo
                if (await _villaRepo.Obtener(v => v.Id == createDto.VillaId) == null)
                {
                    ModelState.AddModelError("ClaveForanea", "El Id de la Villa no existe!");
                    return BadRequest(ModelState);
                }

                if (createDto == null)
                {
                    return BadRequest(createDto);
                }


                NumeroVilla modelo = _mapper.Map<NumeroVilla>(createDto);


                modelo.FechaCreacion = DateTime.Now;
                modelo.FechaActualizacion = DateTime.Now;

                await _numeroVillaRepo.Crear(modelo);

                _response.Resultado = modelo;
                _response.StatusCode = HttpStatusCode.Created;

                return CreatedAtRoute("GetNumeroVilla", new { id = modelo.VillaNo }, _response);
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
        
        public async Task<IActionResult> DeleteNumeroVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.IsExitoso = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;

                    return BadRequest(_response);

                }

                var numeroVilla = await _numeroVillaRepo.Obtener(v => v.VillaNo == id);

                if (numeroVilla == null)
                {
                    _response.IsExitoso = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                await _numeroVillaRepo.Remover(numeroVilla);

                _response.StatusCode = HttpStatusCode.NoContent;
                return Ok(_response);
            }
            catch (Exception e)
            {
                _response.IsExitoso = false;
                _response.ErrorMessages = new List<string>() { e.ToString() };

            }

            return BadRequest(_response);
        }

        //PUT
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> PutNumeroVilla(int id, [FromBody] NumeroVillaUpdateDTO updateDto)
        {

            if (updateDto == null || id != updateDto.VillaNo)
            {
                _response.IsExitoso = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            //Validamos si existe o no el ID del padre (Mismo caso que el post)
            if (await _villaRepo.Obtener(v => v.Id == updateDto.VillaId) == null)
            {
                ModelState.AddModelError("ClaveForanea", "El Id de la Villa no existe!");
                return BadRequest(ModelState);
            }


            NumeroVilla modelo = _mapper.Map<NumeroVilla>(updateDto);

           
            await _numeroVillaRepo.Actualizar(modelo);

           
            _response.StatusCode = HttpStatusCode.NoContent;
            return Ok(_response);
        }

        
    }

}
