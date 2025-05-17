namespace WebApi.Data.Models
{
    public class Cat
    {
        public int Id { get; set; }
        public string CatId { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Image { get; set; }
        public DateTime Created { get; set; }
        public ICollection<Tag> Tags { get; set; } = [];
    }
}
