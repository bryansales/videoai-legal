using Microsoft.AspNetCore.Mvc;
using VideoAI.Api.Services;

namespace VideoAI.Api.Controllers
{
    [ApiController]
    [Route("api/video")]
    public class VideoController : ControllerBase
    {
        private readonly IRoutingService _router;
        public VideoController(IRoutingService router) => _router = router;
        [HttpPost("create")]
        public IActionResult Create([FromBody] VideoRequest request) => Accepted(new { JobId = _router.StartJob(request) });
        [HttpGet("status/{jobId}")]
        public IActionResult Status(string jobId) => Ok(new { JobId = jobId, Status = _router.GetStatus(jobId) });
    }
}
