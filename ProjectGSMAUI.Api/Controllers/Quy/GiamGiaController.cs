using Microsoft.AspNetCore.Mvc;
using ProjectGSMAUI.Api.Container;
using ProjectGSMAUI.Api.Services;
using Microsoft.AspNetCore.Http;

namespace ProjectGSMAUI.Api.Controllers.Quy
{
    [Route("api/[controller]")]
    [ApiController]
    public class GiamGiaController : ControllerBase
    {
        private readonly IGiamGiaServices service;
        public GiamGiaController(IGiamGiaServices service)
        {
            this.service = service;
        }
        [HttpGet]
        public IActionResult Get()
        {
            var data = service.GetAll();
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }
    }
}
