using Microsoft.AspNetCore.Mvc;
using ProjectGSMAUI.Api.Container;
using ProjectGSMAUI.Api.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectGSMAUI.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SanPhamController : ControllerBase
    {
        private readonly ISanPham _sanPhamService;

        public SanPhamController(ISanPham sanPhamService)
        {
            _sanPhamService = sanPhamService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SanPham>>> GetSanPhams()
        {
            return Ok(await _sanPhamService.GetSanPhamsAsync());
        }

        [HttpGet("GetById")]
        public async Task<ActionResult<SanPham>> GetSanPham(int id)
        {
            var sanPham = await _sanPhamService.GetSanPhamByIdAsync(id);
            if (sanPham == null) return NotFound("Sản phẩm không tồn tại");
            return Ok(sanPham);
        }

        [HttpPost]
        public async Task<ActionResult> CreateSanPham(SanPham sanPham)
        {
            await _sanPhamService.CreateSanPhamAsync(sanPham);
            return CreatedAtAction(nameof(GetSanPham), new { id = sanPham.Id }, sanPham);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSanPham(int id, SanPham sanPham)
        {
            if (id != sanPham.Id) return BadRequest("ID không khớp");
            await _sanPhamService.UpdateSanPhamAsync(sanPham);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSanPham(int id)
        {
            await _sanPhamService.DeleteSanPhamAsync(id);
            return NoContent();
        }
    }
}
