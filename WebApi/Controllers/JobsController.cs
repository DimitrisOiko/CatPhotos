using Hangfire;
using Hangfire.Storage;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobsController : ControllerBase
    {
        [HttpGet("{id}")]
        public IActionResult GetJobStatus(string id)
        {
            using var connection = JobStorage.Current.GetConnection();
            var jobData = connection.GetJobData(id);

            if (jobData == null)
                return NotFound(new { Message = "Job not found" });

            return Ok(new
            {
                JobId = id,
                Status = jobData.State
            });
        }
    }
}