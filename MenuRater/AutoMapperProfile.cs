using AutoMapper;
using MenuRater.Dtos;
using MenuRater.Models;

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
