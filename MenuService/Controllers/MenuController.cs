using Microsoft.AspNetCore.Mvc;

namespace MenuService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MenuController : ControllerBase
    {


        private readonly ILogger<MenuController> _logger;

        public MenuController(ILogger<MenuController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("items")]
        public IEnumerable<Menu> Get()
        {
            return new List<Menu>() { 
                new Menu() { Id = Guid.NewGuid(), Description = "tasty", MenuName = "pizza" },
                new Menu() { Id = Guid.NewGuid(), Description = "tasty", MenuName = "hamburger" },
                new Menu() { Id = Guid.NewGuid(), Description = "healthy", MenuName = "broccoli" }
            };
        }
    }
}