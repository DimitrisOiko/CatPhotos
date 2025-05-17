namespace WebApi.Models.CatApi
{
    public class CatDetailsResponse
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public List<Breed> Breeds { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
