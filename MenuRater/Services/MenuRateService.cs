using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using MenuRater.Data;
using MenuRater.Dtos;
using MenuRater.Interfaces;
using MenuRater.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MenuRater.Services
{
    public class MenuRateService : IMenuRateService
    {
        private readonly ILogger<MenuRateService> _logger;
        private readonly IDataContextFactory _dataContextFactory;
        private readonly IPublisherServiceFactory _publisherFactory;
        private readonly IMapper _mapper;

        public MenuRateService(ILogger<MenuRateService> logger, IDataContextFactory dataContextFactory, IPublisherServiceFactory publisherFactory, IMapper mapper)
        {
            _logger = logger;
            _dataContextFactory = dataContextFactory;
            _publisherFactory = publisherFactory;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<GetMenuRateDto>>> GetAllMenuRatesAsync()
        {
            try
            {
                var todaysMenu = await FetchTodaysMenuAsync();

                await AddNewMenus(todaysMenu);

                //fetch todays menus with their ratings, some of them may exist already.

                return new ServiceResponse<List<GetMenuRateDto>>(_mapper.Map<List<GetMenuRateDto>>(todaysMenu));
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while fetching menu rates", ex);
                return new ServiceResponse<List<GetMenuRateDto>>(ex.Message);
            }
        }

        private async Task<List<MenuRate>> FetchTodaysMenuAsync()
        {
            var message = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://localhost:7059/Menu/items")
            };

            var sender = _publisherFactory.GetPublisher(message);
            var httpRequestResponse = await sender.CallAsync(message);

            if (httpRequestResponse == null)
            {
                throw new Exception("No menus found");
            }

            return JsonConvert.DeserializeObject<List<MenuRate>>(httpRequestResponse);
        }

        //this could go into repository, so we can use the repository pattern, and the dependency to the dbContext from the service will be removed. Instead IMenuRateRepository will be added here and can be mocked easier.
        private async Task AddNewMenus(List<MenuRate> todaysMenu)
        {
            using var dataContext = _dataContextFactory.Create();

            var existingItemIds = await dataContext.MenuRates.Select(x => x.Id).ToListAsync();
            var newMenus = todaysMenu.Where(x => !existingItemIds.Contains(x.Id));

            foreach (var menu in newMenus)
            {
                menu.Rating = 5;
                menu.CreatedAt = DateTime.UtcNow;
                dataContext.MenuRates.Add(menu);
            }

            await dataContext.SaveChangesAsync();
        }
    }
}
