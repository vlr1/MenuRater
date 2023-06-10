using AutoMapper;
using MenuRater.Models;
using MenuRater.Models.Dtos;

namespace MenuRater
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<MenuRate, GetMenuRateDto>();
        }
    }
}
