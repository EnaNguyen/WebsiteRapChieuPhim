using Microsoft.AspNetCore.Mvc;
using ProjectGSMAUI.Api.Container;
using ProjectGSMAUI.Api.Data.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using ProjectGSMAUI.Api.Modal;
namespace ProjectGSMAUI.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VeController : ControllerBase
    {
        private readonly IVeService _veService;
        private readonly ILogger<VeController> _logger;
        public VeController(IVeService veService, ILogger<VeController> logger)
        {
            _veService = veService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetVes(string maVe = null, DateTime? filterDate = null, int page = 1)
        {
            var ves = await _veService.GetFilteredVesAsync(maVe, filterDate);
            var result = ves.Select(v => new
            {
                v.MaVe,
                v.MaLichChieu,
                v.MaPhong,
                v.MaPhim,
                v.TinhTrang,
                v.MaGhe,
                v.ThoiGianTao,
                LichChieu = v.MaLichChieuNavigation != null ? new
                {
                    v.MaLichChieuNavigation.MaLichChieu,
                    v.MaLichChieuNavigation.NgayChieu,
                    v.MaLichChieuNavigation.GioChieu,
                    v.MaLichChieuNavigation.GiaVe,
                    v.MaLichChieuNavigation.TinhTrang
                } : null,
                Ghe = v.MaGheNavigation != null ? new
                {
                    v.MaGheNavigation.MaGhe,
                    v.MaGheNavigation.SoHang,
                    v.MaGheNavigation.SoCot
                } : null,
                Phim = v.MaPhimNavigation != null ? new
                {
                    v.MaPhimNavigation.Id,
                    v.MaPhimNavigation.TenPhim
                } : null
            });
            return Ok(result);
        }

        [HttpGet("GetById")]
        public async Task<ActionResult<object>> GetVe(string id)
        {
            var ve = await _veService.GetVeByIdAsync(id);
            if (ve == null) return NotFound("Vé không tồn tại");
            var result = new
            {
                ve.MaVe,
                ve.MaLichChieu,
                ve.MaPhong,
                ve.MaPhim,
                ve.TinhTrang,
                ve.MaGhe,
                ve.ThoiGianTao,
                LichChieu = ve.MaLichChieuNavigation != null ? new
                {
                    ve.MaLichChieuNavigation.MaLichChieu,
                    ve.MaLichChieuNavigation.NgayChieu,
                    ve.MaLichChieuNavigation.GioChieu,
                    ve.MaLichChieuNavigation.GiaVe,
                    ve.MaLichChieuNavigation.TinhTrang
                } : null,
                Ghe = ve.MaGheNavigation != null ? new
                {
                    ve.MaGheNavigation.MaGhe,
                    ve.MaGheNavigation.SoHang,
                    ve.MaGheNavigation.SoCot
                } : null,
                Phim = ve.MaPhimNavigation != null ? new
                {
                    ve.MaPhimNavigation.Id,
                    ve.MaPhimNavigation.TenPhim
                } : null
            };
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult> CreateVe([FromBody] Ve ve)
        {
            try
            {
                _logger.LogInformation($"Creating a new ticket: {ve.MaVe}");
                await _veService.CreateVeAsync(ve);
                _logger.LogInformation($"Created ticket: {ve.MaVe}");
                return CreatedAtAction(nameof(GetVe), new { id = ve.MaVe }, ve);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating ticket");
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi thêm mới vé. Lỗi: " + ex.Message });
            }
        }

        [HttpPut("UpdateVe/{id}")]
        public async Task<IActionResult> UpdateVe(string id, [FromBody] Ve ve)
        {
            await _veService.UpdateVeAsync(id, ve);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVe(string id)
        {
            await _veService.DeleteVeAsync(id);
            return NoContent();
        }
        [HttpPost("MuaVe")]
        public async Task<IActionResult> MuaVe(DatVeModel model)
        {
            var data = await _veService.MuaVe(model);
            return Ok(data);
        }
    }
}