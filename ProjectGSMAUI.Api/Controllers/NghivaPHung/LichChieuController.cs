using Microsoft.AspNetCore.Mvc;
using ProjectGSMAUI.Api.Services;
using ProjectGSMAUI.Api.Data.Entities;
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

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var response = await this.service.GetAll();
            if (response.ResponseCode != 200)
            {
                return StatusCode(response.ResponseCode, new { Message = response.ErrorMessage });
            }
            return Ok(response.Data);
        }

        [HttpGet("{id}")]
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

        [HttpPost("Add")]
        public async Task<IActionResult> AddSchedule([FromBody] LichChieu schedule)
        {
            var response = await this.service.AddSchedule(schedule);
            if (response.ResponseCode != 200)
            {
                return StatusCode(response.ResponseCode, new { Message = response.ErrorMessage });
            }
            return Ok(response.Data);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            var response = await this.service.DeleteSchedule(id);
            if (response.ResponseCode != 200)
            {
                return StatusCode(response.ResponseCode, new { Message = response.ErrorMessage });
            }
            return Ok(response.Result);
        }

        [HttpPost("GenerateWithAdjustments")]
        public async Task<IActionResult> GenerateScheduleWithAdjustments([FromQuery] int phimCanTangId, [FromQuery] int soSuatCanTang)
        {
            var response = await this.service.GenerateScheduleWithAdjustments(phimCanTangId, soSuatCanTang);
            if (response.ResponseCode != 200)
            {
                return StatusCode(response.ResponseCode, new { Message = response.ErrorMessage });
            }
            return Ok(response.Data);
        }
       
    }
}
