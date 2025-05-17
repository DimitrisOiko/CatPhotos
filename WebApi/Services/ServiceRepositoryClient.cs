using System.Net.Http;
using WebApi.Models.CatApi;
using WebApi.Services.Interfaces;

namespace WebApi.Services
{
    public class ServiceRepositoryClient (HttpClient httpClient) : IServiceRepositoryClient
    {
        private readonly HttpClient _httpClient = httpClient;

        private readonly string apiKey = "live_qt5zc62b1AOKlb4w1Hn3jkzv3qWJu243VaCRCpEWCa5BpxaFAAGOVuybMRT22way";
        private readonly int records = 25;

        public async Task<List<ApiCatResponse>> GetCatsAsync() => await _httpClient.GetFromJsonAsync<List<ApiCatResponse>>
            ($"https://api.thecatapi.com/v1/images/search?limit={records}&has_breeds=1&api_key={apiKey}") ?? throw new Exception("Could not retrieve cats");

        public async Task<CatDetailsResponse> GetCatDetailsAsync(string catId) => await _httpClient.GetFromJsonAsync<CatDetailsResponse>
            ($"https://api.thecatapi.com/v1/images/{catId}") ?? throw new Exception($"Could not retrieve image for cat: {catId}");

        public async Task<byte[]> GetCatImageAsync(string url) => await _httpClient.GetByteArrayAsync(url);
    }
}