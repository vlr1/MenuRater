using AutoMapper;
using MenuRater.Interfaces;
using MenuRater.Models;
using MenuRater.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace MenuRater.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MenuRateController : ControllerBase
    {
        private readonly IMenuRateService _menuRateService;

        public MenuRateController(IMenuRateService menuRateService)
        {
            _menuRateService = menuRateService;
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<GetMenuRateDto>>>> GetMenuRates()
        {
            var menuRates = await _menuRateService.GetAllMenuRatesAsync();

            return menuRates?.Data?.Count > 0
                ? Ok(menuRates)
                : NotFound(menuRates);
        }

        [HttpPut]
        public async Task<ActionResult<ServiceResponse<GetMenuRateDto>>> UpdateMenuRate(Guid id, int rating)
        {
            var updatedMenu = await _menuRateService.UpdateRating(id, rating);
            return updatedMenu.Success ? Ok(updatedMenu) : NotFound(updatedMenu);
        }
    }
}