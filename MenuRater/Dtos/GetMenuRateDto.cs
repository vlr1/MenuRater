namespace MenuRater.Dtos
{
    public record GetMenuRateDto(Guid id, string menuName, string image, int rating);
}
