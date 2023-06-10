namespace MenuRater.Models.Dtos
{
    public class GetMenuRateDto
    {
        public Guid Id { get; set; }
        public string MenuName { get; set; }
        public string Description { get; set; }
        public double Rating { get; set; }
    }
}
