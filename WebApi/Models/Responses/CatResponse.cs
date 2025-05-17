namespace WebApi.Models.Responses
{
    public class CatResponse
    {
        public int Id { get; set; }
        public string CatId { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public DateTime Created { get; set; }
        public ICollection<TagResponse> Tags { get; set; } = [];
    }
}
