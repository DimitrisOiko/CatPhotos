using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using WebApi.Data.Interfaces;
using WebApi.Data.Models;
using WebApi.Managers;
using WebApi.Models.CatApi;
using WebApi.Services.Interfaces;

namespace WebApi.Tests.Managers
{
    public class CatsManagerTests
    {
        private readonly Mock<IRepository> _repo;
        private readonly Mock<IServiceRepositoryClient> _client;
        private readonly Mock<ILogger<CatsManager>> _logger;
        private readonly Fixture _fixture;
        private readonly CatsManager _manager;

        public CatsManagerTests()
        {
            _fixture = new Fixture();
            _repo = new Mock<IRepository>();
            _client = new Mock<IServiceRepositoryClient>();
            _logger = new Mock<ILogger<CatsManager>>();
            _manager = new CatsManager(_client.Object, _logger.Object, _repo.Object);
        }

        [Fact]
        public async Task FetchAndSaveCatsAsync_ShouldFetchAndSave25UniqueCats()
        {
            // Arrange
            var cats = _fixture.CreateMany<ApiCatResponse>(25).ToList();
            _client.Setup(c => c.GetCatsAsync()).ReturnsAsync(cats);

            _repo.Setup(r => r.CatExistsAsync(It.IsAny<string>())).ReturnsAsync(false);

            var details = _fixture.Create<CatDetailsResponse>();
            _client.Setup(c => c.GetCatDetailsAsync(It.IsAny<string>())).ReturnsAsync(details);

            var image = _fixture.Create<byte[]>();
            _client.Setup(c => c.GetCatImageAsync(It.IsAny<string>())).ReturnsAsync(image);

            _repo.Setup(r => r.FindExistingTagsAsync(It.IsAny<HashSet<string>>())).ReturnsAsync([]);

            _repo.Setup(r => r.AddCat(It.IsAny<Cat>()));
            _repo.Setup(r => r.AddTags(It.IsAny<List<Tag>>()));
            _repo.Setup(r => r.SaveChanges());
            _repo.Setup(r => r.PersistTransaction());

            // Act
            await _manager.FetchAndSaveCatsAsync();

            // Assert
            _repo.Verify(r => r.AddCat(It.IsAny<Cat>()), Times.Exactly(25));
            _repo.Verify(r => r.AddTags(It.IsAny<List<Tag>>()), Times.Exactly(25));
            _repo.Verify(r => r.SaveChanges(), Times.Exactly(25));
        }

        [Fact]
        public async Task FetchAndSaveCatsAsync_ShouldSkipExistingCats()
        {
            // Arrange
            var cats = _fixture.CreateMany<ApiCatResponse>(25).ToList();
            _client.Setup(c => c.GetCatsAsync()).ReturnsAsync(cats);

            _repo.Setup(r => r.CatExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
            // Act
            await _manager.FetchAndSaveCatsAsync();

            // Assert
            _repo.Verify(r => r.AddCat(It.IsAny<Cat>()), Times.Never);
            _repo.Verify(r => r.SaveChanges(), Times.Never);
        }

        [Fact]
        public async Task FetchAndSaveCatsAsync_ShouldThrowErrorBecauseOfBreed()
        {
            // Arrange
            var cats = _fixture.CreateMany<ApiCatResponse>(25).ToList();
            _client.Setup(c => c.GetCatsAsync()).ReturnsAsync(cats);

            _repo.Setup(r => r.CatExistsAsync(It.IsAny<string>())).ReturnsAsync(false);

            var details = _fixture.Build<CatDetailsResponse>().Without(x => x.Breeds).Create();
            _client.Setup(c => c.GetCatDetailsAsync(It.IsAny<string>())).ReturnsAsync(details);

            var image = _fixture.Create<byte[]>();
            _client.Setup(c => c.GetCatImageAsync(It.IsAny<string>())).ReturnsAsync(image);

            // Act
            await _manager.FetchAndSaveCatsAsync();

            // Assert
            _repo.Verify(r => r.SaveChanges(), Times.Never);
            _repo.Verify(r => r.RollbackTransaction(), Times.Exactly(25));
        }
    }
}
