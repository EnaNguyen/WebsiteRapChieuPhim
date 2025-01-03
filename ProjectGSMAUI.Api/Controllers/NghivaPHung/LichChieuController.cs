using Microsoft.AspNetCore.Mvc;
using ProjectGSMAUI.Api.Services;
using System.Threading.Tasks;

namespace ProjectGSMAUI.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LichChieuController : ControllerBase
    {
        private readonly ILichChieuService service;

        public LichChieuController(ILichChieuService service)
        {
            this.service = service;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> Get()
        {
            var response = await this.service.GetAll();
            if (response.ResponseCode != 200)
            {
                return StatusCode(response.ResponseCode, new { Message = response.ErrorMessage });
            }
            return Ok(response.Data);
        }

        [HttpGet("GetByID")]
        public async Task<IActionResult> GetByID(int id)
        {
            var data = await this.service.GetByID(id);
            if (data == null)
            {
                return NotFound(new { Message = "Không tìm thấy lịch chiếu phim." });
            }
            return Ok(data);
        }

        [HttpPost("Generate")]
        public async Task<IActionResult> GenerateSchedule()
        {
            var response = await this.service.GenerateSchedule();
            if (response.ResponseCode != 200)
            {
                return StatusCode(response.ResponseCode, new { Message = response.ErrorMessage });
            }
            return Ok(response.Data);
        }
    }
}
