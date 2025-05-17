using WebApi.Models.CatApi;

namespace WebApi.Services.Interfaces
{
    public interface IServiceRepositoryClient
    {
        Task<List<ApiCatResponse>> GetCatsAsync();

        Task<CatDetailsResponse> GetCatDetailsAsync(string catId);

        Task<byte[]> GetCatImageAsync(string url);
    }
}
