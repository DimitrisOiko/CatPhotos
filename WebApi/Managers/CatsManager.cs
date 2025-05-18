using System.Data;
using WebApi.Data.Interfaces;
using WebApi.Data.Models;
using WebApi.Managers.Intefaces;
using WebApi.Services.Interfaces;

namespace WebApi.Managers
{
    public class CatsManager(IServiceRepositoryClient client, ILogger<CatsManager> logger, IRepository repository) : ICatsManager
    {
        private readonly ILogger<CatsManager> _logger = logger;
        private readonly IRepository _repository = repository;
        private readonly IServiceRepositoryClient _client = client;

        public async Task FetchAndSaveCatsAsync()
        {
            var response = await _client.GetCatsAsync();
            foreach (var cat in response)
            {
                if (await _repository.CatExistsAsync(cat.Id))
                    continue;

                _repository.BeginTransaction(IsolationLevel.Serializable);
                try
                {
                    var catDetails = await _client.GetCatDetailsAsync(cat.Id);
                    var catImage = await DownloadImageAsBase64Async(cat.Url);
                    if (catDetails.Breeds is null)
                        continue;

                    HashSet<string> uniqueCatTags = [];
                    foreach (var breed in catDetails.Breeds)
                    {
                        if (string.IsNullOrEmpty(breed.Temperament))
                            continue;

                        foreach (var tag in breed.Temperament.Split(",").Select(tag => tag.Trim().ToLower()).ToList())
                            uniqueCatTags.Add(tag);
                    }
                    var existingTagsForCat = await _repository.FindExistingTagsAsync(uniqueCatTags);

                    List<Tag> tagsToBeAddedOnDb = uniqueCatTags
                        .Except(existingTagsForCat.Select(t => t.Name))
                        .Select(tag => new Tag { Name = tag, Created = DateTime.UtcNow })
                        .ToList();

                    var newCat = new Cat
                    {
                        CatId = cat.Id,
                        Width = cat.Width,
                        Height = cat.Height,
                        Image = catImage,
                        Created = DateTime.Now,
                        Tags = [.. existingTagsForCat, .. tagsToBeAddedOnDb]
                    };
                    await _repository.AddTags(tagsToBeAddedOnDb);
                    await _repository.AddCat(newCat);
                    _repository.SaveChanges();
                    _repository.PersistTransaction();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Unexpected error while saving cat with ID {cat.Id}.");
                    _repository.RollbackTransaction();
                }
            }
        }

        private async Task<string> DownloadImageAsBase64Async(string imageUrl)
        {
            var imageBytes = await _client.GetCatImageAsync(imageUrl);
            return Convert.ToBase64String(imageBytes);
        }
    }
}
