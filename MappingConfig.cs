using AutoMapper;
using MagicVilla_API.Models;
using MagicVilla_API.Models.DTO;

namespace MagicVilla_API
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            //CreateMap<fuente,destino>
            //Se debe crear un mapeo y su contrario. Reverse map ayuda a ahorrarse esa línea
            CreateMap<Villa, VillaDTO>();
            CreateMap<VillaDTO, Villa>();

            CreateMap<Villa, VillaCreateDTO>().ReverseMap();
            /*ReverseMap() es lo mismo que hacer 
             CreateMap<Villa, VillaCreateDTO>();
             CreateMap<VillaCreateDTO, Villa>();
             */

            CreateMap<NumeroVilla, NumeroVillaDTO>().ReverseMap();
            CreateMap<NumeroVilla, NumeroVillaCreateDTO>().ReverseMap();
            CreateMap<NumeroVilla, NumeroVillaUpdateDTO>().ReverseMap();
         
        }
    }
}
