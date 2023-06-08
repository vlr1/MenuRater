using MenuRater.Dtos;
using MenuRater.Models;

namespace MenuRater.Interfaces
{
    public interface IMenuRateService
    {
        Task<ServiceResponse<List<GetMenuRateDto>>> GetAllMenuRatesAsync();
    }
}
