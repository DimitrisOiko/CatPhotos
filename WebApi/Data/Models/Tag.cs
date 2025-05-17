namespace WebApi.Data.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public ICollection<Cat> Cats { get; set; } = [];
    }
}
