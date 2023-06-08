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
        private readonly ILogger<MenuRateController> _logger;
        private readonly IMenuRateService _menuRateService;
        private readonly IMapper _mapper;

        public MenuRateController(ILogger<MenuRateController> logger, IMenuRateService menuRateService, IMapper mapper)
        {
            _logger = logger;
            _menuRateService = menuRateService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<GetMenuRateDto>>>> GetMenuRates()
        {
            var menuRates = await _menuRateService.GetAllMenuRatesAsync();

            if (menuRates.Data == null || menuRates.Data.Count == 0)
            {
                return NotFound(new ServiceResponse<List<ServiceResponse<GetMenuRateDto>>>("No results found"));
            }

            return Ok(new ServiceResponse<List<GetMenuRateDto>>(_mapper.Map<List<GetMenuRateDto>>(menuRates)));
        }
    }
}