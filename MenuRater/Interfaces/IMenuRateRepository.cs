using MenuRater.Models;

namespace MenuRater.Interfaces
{
    // we probably want to map the DTOs here instead of having them in the service.
    public interface IMenuRateRepository
    {
        MenuRate Find(Guid menuRateId);
        MenuRate Update(MenuRate menuRate, int previousRatingsCount);

        /// <summary>
        /// Adds new menurates from a list of menurates and returns the information about the current ones.
        /// 
        /// </summary>
        /// <param name="newMenus"></param>
        /// <returns></returns>
        List<MenuRate> UpdateMenuRateList(List<MenuRate> newMenus);
    }
}
