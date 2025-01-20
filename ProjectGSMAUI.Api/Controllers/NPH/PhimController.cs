using Microsoft.AspNetCore.Mvc;
using ProjectGSMAUI.Api.Container;
using ProjectGSMAUI.Api.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace ProjectGSMAUI.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhimController : ControllerBase
    {
        private readonly IPhimService _phimService;

        private readonly ILogger<PhimController> _logger;
        public PhimController(IPhimService phimService, ILogger<PhimController> logger)
        {
            _phimService = phimService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetPhims()
        {
            var phims = await _phimService.GetPhimsAsync();
            var result = phims.Select(p => new
            {
                p.Id,
                p.TenPhim,
                p.TheLoai,
                p.ThoiLuong,
                p.DaoDien,
                p.GioiHanDoTuoi,
                p.NgayKhoiChieu,
                p.NgayKetThuc,
                p.SoXuatChieu,
                p.TrangThai,
                p.MoTa,
                HinhAnh = p.HinhAnhs.FirstOrDefault() != null ? new
                {
                    p.HinhAnhs.FirstOrDefault().Id,
                    p.HinhAnhs.FirstOrDefault().ImageData
                } : null
            });
            return Ok(result);
        }

        [HttpGet("GetById")]
        public async Task<ActionResult<object>> GetPhim(int id)
        {
            var phim = await _phimService.GetPhimByIdAsync(id);
            if (phim == null) return NotFound("Phim không tồn tại");
            var result = new
            {
                phim.Id,
                phim.TenPhim,
                phim.TheLoai,
                phim.ThoiLuong,
                phim.DaoDien,
                phim.GioiHanDoTuoi,
                phim.NgayKhoiChieu,
                phim.NgayKetThuc,
                phim.SoXuatChieu,
                phim.TrangThai,
                phim.MoTa,
                HinhAnhs = phim.HinhAnhs.Select(h => new
                {
                    h.Id,
                    h.ImageData
                }).ToList()
            };

            return Ok(result);
        }


        [HttpPost]
        public async Task<ActionResult> CreatePhim([FromForm] Phim phim)
        {
            try
            {
                _logger.LogInformation($"Creating a new movie: {phim.TenPhim}");
                await _phimService.CreatePhimAsync(phim);
                _logger.LogInformation($"Created Movie: {phim.TenPhim}, id = {phim.Id}");
                return CreatedAtAction(nameof(GetPhim), new { id = phim.Id }, phim);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating movie");
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi thêm mới phim. Lỗi: " + ex.Message });
            }


        }

        [HttpPut("UpdatePhim")]
        public async Task<IActionResult> UpdatePhim(int id, [FromForm] Phim phim)
        {
            if (id != phim.Id) return BadRequest("ID không khớp");
            await _phimService.UpdatePhimAsync(phim);
            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhim(int id)
        {
            await _phimService.DeletePhimAsync(id);
            return NoContent();
        }
    }
}