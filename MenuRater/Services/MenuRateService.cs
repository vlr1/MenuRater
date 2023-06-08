using MenuRater.Data;
using MenuRater.Dtos;
using MenuRater.Interfaces;
using MenuRater.Models;
using Messaging.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace MenuRater.Services
{
    public class MenuRateService : IMenuRateService
    {
        private readonly ILogger<MenuRateService> _logger;
        private readonly DataContext _context;
        private IPublisherServiceFactory _publisherFactory;

        public MenuRateService(ILogger<MenuRateService> logger, DataContext context, IPublisherServiceFactory publisherFactory)
        {
            _logger = logger;
            _context = context;
            _publisherFactory = publisherFactory;
        }

        public async Task<ServiceResponse<List<GetMenuRateDto>>> GetAllMenuRatesAsync()
        {
            try
            {
                //use rmq to fetch some menu items from a different server
                //why? because i can. 
                var message = new HttpRequestMessage() { 
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("https://localhost:5001/Menu/items"),
                };

                var sender = _publisherFactory.GetPublisher(message);
                var result = await sender.CallAsync(message);

                if (result == null) return new ServiceResponse<List<GetMenuRateDto>>("no menus found");

                var menuItems = JsonConvert.DeserializeObject<List<GetMenuRateDto>>(result);

                var menuItemIds = menuItems.Select(x => x.id);
                var existingItems = await _context.MenuRates
                    .Where(x => menuItemIds.Contains(x.Id))
                    .Select(x => x.Id).ToListAsync();

                var newItems = menuItems.Where(x => !existingItems.Contains(x.id)).Select(x => new MenuRate()
                {
                    Id = x.id,
                    Rating = x.rating,
                    Image = x.image,
                    MenuName = x.image,
                    CreatedAt = new DateTime()
                });

                await _context.AddRangeAsync(newItems);
                await _context.SaveChangesAsync();

                var allItems = await _context.MenuRates.Select(x => new GetMenuRateDto(x.Id, x.MenuName, x.Image, x.Rating)).ToListAsync();
                return new ServiceResponse<List<GetMenuRateDto>>(allItems);                
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while fetching menu rates", ex.ToString());
                return new ServiceResponse<List<GetMenuRateDto>>(ex.Message);
            }
        }
    }
}
