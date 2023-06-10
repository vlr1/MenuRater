using MenuRater.Data;
using MenuRater.Interfaces;
using MenuRater.Models;
using Microsoft.EntityFrameworkCore;

namespace MenuRater.Repository
{
    public class MenuRateRepository : IMenuRateRepository
    {
        private readonly DataContext _dataContext;
        public MenuRateRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public MenuRate? Find(Guid menuRateId) => _dataContext.MenuRates.SingleOrDefault(x => x.Id == menuRateId);

        public MenuRate Update(MenuRate menuRate, int previousRatingsCount)
        {
            try
            {
                // we could just use menuRate.RatingsCount - 1 instead of passing previousRatingsCount
                // optimistic locking is better with dateTime so using something like LastUpdatedAt is better approach.
                var existingRecord = _dataContext.MenuRates.SingleOrDefault(x => x.Id == menuRate.Id && x.RatingsCount == previousRatingsCount);
                if (existingRecord == null) throw new Exception("Data record not found, it may have been updated already by another instance, try again");
                
                existingRecord.TotalRating = menuRate.TotalRating;
                existingRecord.RatingsCount = menuRate.RatingsCount;
                _dataContext.SaveChanges();
                return existingRecord;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                //log the exception and give a good response back.
                throw ex;
            }
        }

        public List<MenuRate> UpdateMenuRateList(List<MenuRate> newMenus)
        {
            var newMenusIds = newMenus.Select(x => x.Id);
            var alreadyExistingMenus = _dataContext.MenuRates.Where(x => newMenusIds.Contains(x.Id));

            var menusToAdd = newMenus.Except(alreadyExistingMenus);

            foreach(var menu in menusToAdd)
            {
                menu.CreatedAt = DateTime.Now;
                _dataContext.MenuRates.Add(menu);
            }

            _dataContext.SaveChanges();

            var newMenuIds = newMenus.Select(x => x.Id);
            var result = _dataContext.MenuRates
                .Where(x => newMenuIds.Contains(x.Id))
                .ToList();

            return result;
        }
    }
}
