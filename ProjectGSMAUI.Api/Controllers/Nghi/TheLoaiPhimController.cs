using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectGSMAUI.Api.Data;
using ProjectGSMAUI.Api.Data.Entities;
using ProjectGSMAUI.Api.Services;

namespace ProjectGSMAUI.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TheLoaiPhimController : ControllerBase
    {
        private readonly ITheLoaiPhimService service;
        public TheLoaiPhimController(ITheLoaiPhimService service)
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
                return NotFound(new { Message = "Không tìm thấy thể loại phim." });
            }
            return Ok(data);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] TheLoaiPhim data)
        {
            if (data == null)
            {
                return BadRequest(new { Message = "Dữ liệu không hợp lệ." });
            }

            var response = await this.service.Create(data);
            if (response.ResponseCode != 200)
            {
                return StatusCode(response.ResponseCode, new { Message = response.ErrorMessage });
            }
            return CreatedAtAction(nameof(GetByID), new { id = data.Id }, data);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update(int id, [FromBody] TheLoaiPhim data)
        {
            if (data == null || id != data.Id)
            {
                return BadRequest(new { Message = "Dữ liệu không hợp lệ." });
            }

            var response = await this.service.Update(data, id);
            if (response.ResponseCode != 200)
            {
                return StatusCode(response.ResponseCode, new { Message = response.ErrorMessage });
            }
            return Ok(new { Message = response.Result });
        }

        [HttpDelete("Remove")]
        public async Task<IActionResult> Remove(int id)
        {
            var response = await this.service.Remove(id);
            if (response.ResponseCode != 200)
            {
                return StatusCode(response.ResponseCode, new { Message = response.ErrorMessage });
            }
            return Ok(new { Message = response.Result });
        }
    }
}