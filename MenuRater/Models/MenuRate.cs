using System.ComponentModel.DataAnnotations;

namespace MenuRater.Models
{
    public class MenuRate
    {
        public Guid Id { get; set; }

        [Required]
        public string MenuName { get; set; }
        public string Description { get; set; }

        [Range(1, 5)]
        public double TotalRating { get; set; }
        public DateTime CreatedAt { get; set; }
        public int RatingsCount { get; set; } = 0;

        public double Rating
        {
            get { return RatingsCount > 0 ? TotalRating / RatingsCount : 0; }
        }
    }
}
