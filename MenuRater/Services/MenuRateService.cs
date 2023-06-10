using AutoMapper;
using MenuRater.Interfaces;
using MenuRater.Models;
using MenuRater.Models.Dtos;
using Newtonsoft.Json;

namespace MenuRater.Services
{
    public class MenuRateService : IMenuRateService
    {
        private readonly ILogger<MenuRateService> _logger;
        private readonly IMenuRateRepository _menuRateRepository;
        private readonly IPublisherServiceFactory _publisherFactory;
        private readonly IMapper _mapper;

        public MenuRateService(ILogger<MenuRateService> logger, IMenuRateRepository menuRateRepository, IPublisherServiceFactory publisherFactory, IMapper mapper)
        {
            _logger = logger;
            _menuRateRepository = menuRateRepository;
            _publisherFactory = publisherFactory;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<GetMenuRateDto>>> GetAllMenuRatesAsync()
        {
            try
            {
                var todaysMenu = await FetchTodaysMenuAsync();

                if (todaysMenu.Any()) {
                    var updateResult = _menuRateRepository.UpdateMenuRateList(todaysMenu);
                    return new ServiceResponse<List<GetMenuRateDto>>(_mapper.Map<List<GetMenuRateDto>>(updateResult)); 
                }
                return new ServiceResponse<List<GetMenuRateDto>>("No menus available for today");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while fetching menu rates. \nException Message: {ex.Message} \nException: {ex}");
                return new ServiceResponse<List<GetMenuRateDto>>(ex.Message);
            }
        }

        public async Task<ServiceResponse<GetMenuRateDto>> UpdateRating(Guid menuRateId, int rating)
        {
            try
            {
                var menuRate = _menuRateRepository.Find(menuRateId);
                if (menuRate == null) throw new InvalidOperationException("Selected menu not found, try again later");

                var prevRatingsCount = menuRate.RatingsCount;
                menuRate.RatingsCount += 1;
                menuRate.TotalRating += rating;

                var updated = _menuRateRepository.Update(menuRate, prevRatingsCount);
                return new ServiceResponse<GetMenuRateDto>(_mapper.Map<GetMenuRateDto>(updated));
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while fetching menu rates. \nException Message: {ex.Message} \nException: {ex}");
                return new ServiceResponse<GetMenuRateDto>(ex.Message);
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
            var responseContent = await sender.CallAsync(message);

            if (responseContent == null || responseContent == string.Empty)
                throw new ServiceUnavailableException("Menu Service did not respond back with a valid result");

            //this converts directly to menurate db object - not great.
            var menuRates = JsonConvert.DeserializeObject<List<MenuRate>>(responseContent);
            return menuRates ?? throw new InvalidResponseException($"Unexpected content from response {responseContent}");
        }
    }

    internal class InvalidResponseException : Exception
    {
        public InvalidResponseException(string? message) : base(message) { }
    }

    internal class ServiceUnavailableException : Exception
    {
        public ServiceUnavailableException(string? message) : base(message) { }
    }
}
