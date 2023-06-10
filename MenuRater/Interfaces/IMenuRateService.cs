using MenuRater.Models;
using MenuRater.Models.Dtos;

namespace MenuRater.Interfaces
{
    public interface IMenuRateService
    {
        Task<ServiceResponse<List<GetMenuRateDto>>> GetAllMenuRatesAsync();

        Task<ServiceResponse<GetMenuRateDto>> UpdateRating(Guid menuRateId, int rating);
    }
}
