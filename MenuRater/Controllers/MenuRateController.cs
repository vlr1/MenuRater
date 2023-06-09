using AutoMapper;
using MenuRater.Dtos;
using MenuRater.Interfaces;
using MenuRater.Models;
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

            return menuRates?.Data.Count > 0
                ? Ok(menuRates)
                : NotFound(menuRates);
        }
    }
}