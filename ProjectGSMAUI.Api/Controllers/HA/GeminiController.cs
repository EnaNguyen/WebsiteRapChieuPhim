using Microsoft.AspNetCore.Mvc;
using ProjectGSMAUI.Api.Modal;
using ProjectGSMAUI.Api.Services;

namespace ProjectGSMAUI.Api.Controllers.HA
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeminiController : ControllerBase
    {
        private readonly IGeminiServices service;
        public GeminiController(IGeminiServices service)
        {
            this.service = service;
        }
        [HttpGet("TraLoi")]
        public async Task<IActionResult> TraLoi(string question)
        {
            var data = await this.service.TraLoi(question);
            return Ok(data);
        }
    }
}
