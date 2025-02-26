using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectGSMAUI.Api.Data.Entities;
using ProjectGSMAUI.Api.Modal;
using ProjectGSMAUI.Api.Services;

namespace ProjectGSMAUI.Api.Controllers.VHun
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComboController : ControllerBase
    {
        private readonly ICombo _comboService;

        public ComboController(ICombo comboService)
        {
            _comboService = comboService;
        }

        // Lấy tất cả Combo
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _comboService.GetAll();
            return Ok(result);
        }

        // Lấy Combo theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _comboService.GetById(id);
            if (result == null)
                return NotFound("Không tìm thấy combo!");
            return Ok(result);
        }

        // Tạo Combo mới
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ComboModal comboModal)
        {
            if (comboModal == null)
                return BadRequest("Dữ liệu combo không hợp lệ!");

            var response = await _comboService.Create(comboModal);
            return StatusCode(response.ResponseCode, response);
        }


        // Cập nhật Combo
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ComboModal comboModal)
        {
            if (comboModal == null)
                return BadRequest("Dữ liệu combo không hợp lệ!");

            var response = await _comboService.Update(id, comboModal);
            return StatusCode(response.ResponseCode, response);
        }

        // Xóa Combo
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _comboService.Delete(id);
            return StatusCode(response.ResponseCode, response);
        }
    }
}

