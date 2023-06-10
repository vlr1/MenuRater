using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using MenuRater.Data;
using MenuRater.Interfaces;
using MenuRater.Models;
using MenuRater.Services;
using System.Linq.Expressions;
using Assert = Xunit.Assert;
using AutoMapper;
using Newtonsoft.Json;
using MenuRater.Repository;
using MenuRater.Models.Dtos;

namespace MenuRater.Tests.Services
{
    public class MenuRateServiceTests
    {
        private MenuRateService _menuRateService;
        private Mock<ILogger<MenuRateService>> _loggerMock;
        private Mock<IMenuRateRepository> _menuRateRepository;
        private Mock<IPublisherServiceFactory> _publisherFactoryMock;
        private Mock<IMapper> _mapper;

        public MenuRateServiceTests()
        {
            _loggerMock = new Mock<ILogger<MenuRateService>>();
            _menuRateRepository = new Mock<IMenuRateRepository>();
            _publisherFactoryMock = new Mock<IPublisherServiceFactory>();
            _mapper = new Mock<IMapper>();
            _menuRateService = new MenuRateService(_loggerMock.Object, _menuRateRepository.Object, _publisherFactoryMock.Object, _mapper.Object);
        }

        [Fact]
        public async Task GetAllMenuRatesAsync_WhenMenuItemsExist_ReturnsServiceResponseWithMenuRates()
        {
            // Arrange
            var todaysMenu = new List<GetMenuRateDto>
            {
                new GetMenuRateDto { Id = Guid.NewGuid(), MenuName = "Menu 1", Description = "Description 1" },
                new GetMenuRateDto { Id = Guid.NewGuid(), MenuName = "Menu 2", Description = "Description 2" }
            };
            var httpRequestResponse = new[]
            {
                new { Id = "c31e46de-29f2-4c57-9cc5-0c25faebe1c9", MenuName = "Menu 1", Description = "Description 1" },
                new { Id = "3f09fe78-eb9b-4b06-8e3a-87a13e9e1dbd", MenuName = "Menu 2", Description = "Description 2" }
            };

            var expectedMenuRates = todaysMenu.Select(x => new GetMenuRateDto { Id = x.Id, MenuName = x.MenuName, Description = x.Description }).ToList();

            var senderMock = new Mock<IPublisherService>();
            senderMock.Setup(s => s.CallAsync(It.IsAny<HttpRequestMessage>())).ReturnsAsync(JsonConvert.SerializeObject(httpRequestResponse));
            _publisherFactoryMock.Setup(f => f.GetPublisher(It.IsAny<HttpRequestMessage>())).Returns(senderMock.Object);

            _menuRateRepository.Setup(x => x.UpdateMenuRateList(It.IsAny<List<MenuRate>>())).Returns(new List<MenuRate>() {
                new MenuRate() { Id = Guid.Parse(httpRequestResponse[0].Id), Description = httpRequestResponse[0].Description, MenuName = httpRequestResponse[0].MenuName },
                new MenuRate() { Id = Guid.Parse(httpRequestResponse[1].Id), Description = httpRequestResponse[1].Description, MenuName = httpRequestResponse[1].MenuName }
            });

            _mapper.Setup(x => x.Map<List<GetMenuRateDto>>(It.IsAny<List<MenuRate>>())).Returns(expectedMenuRates);

            // Act
            var result = await _menuRateService.GetAllMenuRatesAsync();

            // Assert
            Assert.True(result.Success);
            Assert.Equal(expectedMenuRates.Count, result.Data.Count);
            Assert.True(expectedMenuRates.All(e => result.Data.Any(d => d.Id == e.Id && d.MenuName == e.MenuName && d.Description == e.Description)));
        }

        [Fact]
        public async Task GetAllMenuRatesAsync_WhenMenuItemsDoNotExist_ReturnsServiceResponseWithError()
        {
            // Arrange
            var httpRequestResponse = Array.Empty<object>();
            var senderMock = new Mock<IPublisherService>();
            senderMock.Setup(s => s.CallAsync(It.IsAny<HttpRequestMessage>())).ReturnsAsync(JsonConvert.SerializeObject(httpRequestResponse));
            _publisherFactoryMock.Setup(f => f.GetPublisher(It.IsAny<HttpRequestMessage>())).Returns(senderMock.Object);

            // Act
            var result = await _menuRateService.GetAllMenuRatesAsync();

            // Assert
            Assert.False(result.Success);
            Assert.Equal("No menus available for today", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task GetAllMenuRatesAsync_WhenAnExceptionOccurs_ReturnsServiceResponseWithError()
        {
            // Arrange
            var senderMock = new Mock<IPublisherService>();
            senderMock.Setup(s => s.CallAsync(It.IsAny<HttpRequestMessage>())).ThrowsAsync(new Exception("Service offline"));
            _publisherFactoryMock.Setup(f => f.GetPublisher(It.IsAny<HttpRequestMessage>())).Returns(senderMock.Object);

            // Act
            var result = await _menuRateService.GetAllMenuRatesAsync();

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Service offline", result.Message);
            Assert.Null(result.Data);
        }
    }
}
