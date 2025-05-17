using AutoMapper;
using WebApi.Data.Models;
using WebApi.Models.Responses;

namespace WebApi.Mapping
{
    public class MapProfile : Profile
    {
        public MapProfile() 
        {
            CreateMap<Cat, CatResponse>();                
            CreateMap<Tag, TagResponse>();
        }
    }
}