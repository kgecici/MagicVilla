using AutoMapper;
using MagicVilla_VillaAPI.Models;

namespace MagicVilla_VillaAPI;

public class MappingConfig : Profile
{
    public MappingConfig()
    {
        CreateMap<Villa, VillaDTO>().ReverseMap();
        CreateMap<Villa, VillaCreateDTO>().ReverseMap();
        CreateMap<Villa, VillaUpdateDTO>().ReverseMap();
        CreateMap<VillaDTO, VillaCreateDTO>().ReverseMap();
        CreateMap<VillaDTO, VillaUpdateDTO>().ReverseMap();
        
    }
}