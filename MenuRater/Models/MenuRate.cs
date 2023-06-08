using System.ComponentModel.DataAnnotations;

namespace MenuRater.Models
{
    public class MenuRate
    {
        public Guid Id { get; set; }

        [Required]
        public string MenuName { get; set; }
        public string Image { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }
    }
}
