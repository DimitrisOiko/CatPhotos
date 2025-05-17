using AutoFixture;
using AutoMapper;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApi.Controllers;
using WebApi.Data.Interfaces;
using WebApi.Data.Models;
using WebApi.Managers.Intefaces;
using WebApi.Models.Responses;

namespace WebApi.Tests.Controllers
{
    public class CatsControllerTests
    {
        private readonly Mock<ICatsManager> _catsManager;
        private readonly Mock<IBackgroundJobClient> _backgroundJobClient;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IRepository> _repository;
        private readonly Fixture _fixture;
        private readonly CatsController _controller;

        public CatsControllerTests()
        {
            _fixture = new Fixture();
            _catsManager = new Mock<ICatsManager>();
            _backgroundJobClient = new Mock<IBackgroundJobClient>();
            _mapper = new Mock<IMapper>();
            _repository = new Mock<IRepository>();
            _controller = new CatsController(
                _catsManager.Object,
                _backgroundJobClient.Object,
                _mapper.Object,
                _repository.Object
            );
        }

        [Fact]
        public void FetchCats_ShouldEnqueueFetchJob()
        {
            // Act
            var result = _controller.FetchCats() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async Task GetCatById_ShouldReturnCat_WhenCatExists()
        {
            // Arrange
            var cat = _fixture.Build<Cat>().Without(x => x.Tags).Create();
            var catResponse = _fixture.Create<CatResponse>();
            _repository.Setup(r => r.GetCatByIdAsync(cat.Id)).ReturnsAsync(cat);
            _mapper.Setup(m => m.Map<CatResponse>(cat)).Returns(catResponse);

            // Act
            var result = await _controller.GetCatById(cat.Id) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(catResponse, result.Value);
        }

        [Fact]
        public async Task GetCatById_ShouldReturnNotFound_WhenCatDoesNotExist()
        {
            // Arrange
            _repository.Setup(r => r.GetCatByIdAsync(It.IsAny<int>())).ReturnsAsync((Cat)null);

            // Act
            var result = await _controller.GetCatById(1) as NotFoundObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
        }

        [Fact]
        public async Task GetCatById_ShouldReturnBadRequest_WhenIdIsInvalid()
        {
            // Act
            var result = await _controller.GetCatById(0) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
        }

        [Fact]
        public async Task GetCatImage_ShouldReturnImage_WhenCatExists()
        {
            // Arrange
            var cat = _fixture.Build<Cat>().With(c => c.Image, Convert.ToBase64String(_fixture.Create<byte[]>())).Without(x => x.Tags).Create();
            _repository.Setup(r => r.GetCatByIdAsync(cat.Id)).ReturnsAsync(cat);

            // Act
            var result = await _controller.GetCatImage(cat.Id) as FileContentResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("image/jpeg", result.ContentType);
            Assert.Equal(Convert.FromBase64String(cat.Image), result.FileContents);
        }

        [Fact]
        public async Task GetCatImage_ShouldReturnNotFound_WhenCatDoesNotExist()
        {
            // Arrange
            _repository.Setup(r => r.GetCatByIdAsync(It.IsAny<int>())).ReturnsAsync((Cat)null);

            // Act
            var result = await _controller.GetCatImage(1) as NotFoundObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
        }

        [Fact]
        public async Task GetAllCats_ShouldReturnAllCats_WhenNoTagIsSpecified()
        {
            // Arrange
            var cats = _fixture.Build<Cat>().Without(x => x.Tags).CreateMany(10).ToList();
            var catResponses = _fixture.CreateMany<CatResponse>(10).ToList();
            _repository.Setup(r => r.GetAllCatsAsync(1, 10)).ReturnsAsync(cats);
            _mapper.Setup(m => m.Map<List<CatResponse>>(cats)).Returns(catResponses);

            // Act
            var result = await _controller.GetAllCats(1, 10) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            var response = result.Value as GetCatsResponse;
            Assert.NotNull(response);
            Assert.Equal(10, response.Total);
            Assert.Equal(catResponses, response.Cats);
        }

        [Fact]
        public async Task GetAllCats_ShouldReturnTaggedCats_WhenTagIsSpecified()
        {
            // Arrange
            var tag = "Playful";
            var cats = _fixture.Build<Cat>().Without(x => x.Tags).CreateMany(5).ToList();
            var catResponses = _fixture.CreateMany<CatResponse>(5).ToList();
            _repository.Setup(r => r.GetCatsByTagAsync(tag, 1, 10)).ReturnsAsync(cats);
            _mapper.Setup(m => m.Map<List<CatResponse>>(cats)).Returns(catResponses);

            // Act
            var result = await _controller.GetAllCats(1, 10, tag) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            var response = result.Value as GetCatsResponse;
            Assert.NotNull(response);
            Assert.Equal(5, response.Total);
            Assert.Equal(catResponses, response.Cats);
        }

        [Fact]
        public async Task GetAllCats_ShouldReturnBadRequest_WhenPageOrPageSizeIsInvalid()
        {
            // Act
            var result = await _controller.GetAllCats(0, 0) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
        }
    }
}
