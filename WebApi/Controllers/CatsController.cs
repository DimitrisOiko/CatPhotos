using AutoMapper;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using WebApi.Data.Interfaces;
using WebApi.Managers.Intefaces;
using WebApi.Models.Responses;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatsController(ICatsManager catsManager, IBackgroundJobClient backgroundJobClient, IMapper mapper, IRepository repository) : ControllerBase
    {
        private readonly ICatsManager _catsManager = catsManager;
        private readonly IBackgroundJobClient _backgroundJobClient = backgroundJobClient;
        private readonly IMapper _mapper = mapper;
        private readonly IRepository _repository = repository;

        [HttpPost("fetch")]
        public IActionResult FetchCats()
        {
            string jobId = _backgroundJobClient.Enqueue(() => _catsManager.FetchAndSaveCatsAsync());

            return Ok(new { Message = "Fetch job started", JobId = jobId });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCatById(int id)
        {
            if (id <= 0)
                return BadRequest(new { Message = "Id must be greater than 0." });

            var cat = await _repository.GetCatByIdAsync(id);
            if (cat == null)
                return NotFound(new { Message = "Cat not found." });

            return Ok( _mapper.Map<CatResponse>(cat) );
        }

        [HttpGet("{id}.jpg")]
        public async Task<IActionResult> GetCatImage(int id)
        {
            if (id <= 0)
                return BadRequest(new { Message = "Id must be greater than 0." });

            var cat = await _repository.GetCatByIdAsync(id);
            if (cat == null)
                return NotFound(new { Message = "Cat not found." });

            return File(Convert.FromBase64String(cat.Image), "image/jpeg");
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCats([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string tag = null)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest(new { Message = "Page and PageSize must be greater than 0." });

            if (!string.IsNullOrEmpty(tag))
            {
                var taggedCats = await _repository.GetCatsByTagAsync(tag, page, pageSize);
                return Ok(new GetCatsResponse 
                {
                    Page = page,
                    PageSize = pageSize,
                    Total = taggedCats.Count,
                    Cats = _mapper.Map<List<CatResponse>>(taggedCats) 
                });
            }

            var cats = await _repository.GetAllCatsAsync(page, pageSize);
            return Ok(new GetCatsResponse
            {
                Page = page,
                PageSize = pageSize,
                Total = cats.Count,
                Cats = _mapper.Map<List<CatResponse>>(cats)
            });
        }
    }
}