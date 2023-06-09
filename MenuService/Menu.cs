namespace MenuService
{
    public class Menu
    {
        public Guid Id { get; set; }
        public string MenuName { get; set; }
        public string Description { get; set; }
        public byte[] Image { get; set; }
    }
}
