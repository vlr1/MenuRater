using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using MenuRater.Data;
using MenuRater.Dtos;
using MenuRater.Interfaces;
using MenuRater.Models;
using MenuRater.Services;
using System.Linq.Expressions;
using Assert = Xunit.Assert;

namespace MenuRater.Tests.Services
{
    public class MenuRateServiceTests
    {
        private MenuRateService _menuRateService;
        private Mock<ILogger<MenuRateService>> _loggerMock;
        private Mock<IDataContextFactory> _contextMock;
        private Mock<IPublisherServiceFactory> _publisherFactoryMock;

        public MenuRateServiceTests()
        {
            _loggerMock = new Mock<ILogger<MenuRateService>>();
            _contextMock = new Mock<IDataContextFactory>();
            _publisherFactoryMock = new Mock<IPublisherServiceFactory>();
            _menuRateService = new MenuRateService(_loggerMock.Object, _contextMock.Object, _publisherFactoryMock.Object);
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
            var httpRequestResponse = "[{\"Id\":\"c31e46de-29f2-4c57-9cc5-0c25faebe1c9\",\"MenuName\":\"Menu 1\",\"Description\":\"Description 1\"},{\"Id\":\"3f09fe78-eb9b-4b06-8e3a-87a13e9e1dbd\",\"MenuName\":\"Menu 2\",\"Description\":\"Description 2\"}]";
            var expectedMenuRates = todaysMenu.Select(x => new GetMenuRateDto { Id = x.Id, MenuName = x.MenuName, Description = x.Description }).ToList();

            var senderMock = new Mock<IPublisherService>();
            senderMock.Setup(s => s.CallAsync(It.IsAny<HttpRequestMessage>())).ReturnsAsync(httpRequestResponse);
            _publisherFactoryMock.Setup(f => f.GetPublisher(It.IsAny<HttpRequestMessage>())).Returns(senderMock.Object);

            var contextMock = new Mock<DataContext>();
            var context = _contextMock.Setup(x => x.Create()).Returns(contextMock.Object);

            contextMock.Setup(c => c.MenuRates.Where(It.IsAny<Expression<Func<MenuRate, bool>>>())).Returns<Expression<Func<MenuRate, bool>>>(predicate =>
            {
                var queryableData = todaysMenu.Select(x => new MenuRate { Id = x.Id, MenuName = x.MenuName, Description = x.Description }).AsQueryable();
                return queryableData.Where(predicate);
            });

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
            var httpRequestResponse = "";
            var senderMock = new Mock<IPublisherService>();
            senderMock.Setup(s => s.CallAsync(It.IsAny<HttpRequestMessage>())).ReturnsAsync(httpRequestResponse);
            _publisherFactoryMock.Setup(f => f.GetPublisher(It.IsAny<HttpRequestMessage>())).Returns(senderMock.Object);

            // Act
            var result = await _menuRateService.GetAllMenuRatesAsync();

            // Assert
            Assert.False(result.Success);
            Assert.Equal("no menus found", result.Message);
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
            Assert.Equal("Some exception", result.Message);
            Assert.Null(result.Data);
        }
    }
}
