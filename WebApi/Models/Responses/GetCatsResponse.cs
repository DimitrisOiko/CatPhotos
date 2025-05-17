namespace WebApi.Models.Responses
{
    public class GetCatsResponse
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
        public List<CatResponse> Cats { get; set; } = [];
    }
}
