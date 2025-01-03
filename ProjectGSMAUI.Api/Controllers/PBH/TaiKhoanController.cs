using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectGSMAUI.Api.Container;
using ProjectGSMAUI.Api.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectGSMAUI.Api.Controllers.PBH
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaiKhoanController : ControllerBase
    {
        private readonly ITaiKhoanService _taiKhoanService;

        public TaiKhoanController(ITaiKhoanService taiKhoanService)
        {
            _taiKhoanService = taiKhoanService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaiKhoan>>> GetTaiKhoans()
        {
            var taiKhoans = await _taiKhoanService.GetTaiKhoansAsync();
            var result = taiKhoans.Select(t => new
            {
                IdtaiKhoan = t.IdtaiKhoan.Trim(),
                TenTaiKhoan = t.TenTaiKhoan.Trim(),
                MatKhau = t.MatKhau.Trim(),
                TenNguoiDung = t.TenNguoiDung.Trim(),
                TrangThai = t.TrangThai,
                Hinh = t.Hinh.Trim(),
                Cccd = t.Cccd.Trim()
            });
            return Ok(result);
            
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaiKhoan>> GetTaiKhoan(string id)
        {
            var taiKhoan = await _taiKhoanService.GetTaiKhoanByIdAsync(id);

            if (taiKhoan == null)
            {
                return NotFound();
            }

            return Ok(taiKhoan);
        }

        [HttpPost]
        public async Task<ActionResult<TaiKhoan>> PostTaiKhoan(TaiKhoan taiKhoan)
        {
            await _taiKhoanService.CreateTaiKhoanAsync(taiKhoan);
            return CreatedAtAction(nameof(GetTaiKhoan), new { id = taiKhoan.IdtaiKhoan }, taiKhoan);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTaiKhoan(string id, TaiKhoan taiKhoan)
        {
            if (id != taiKhoan.IdtaiKhoan)
            {
                return BadRequest();
            }

            try
            {
                await _taiKhoanService.UpdateTaiKhoanAsync(taiKhoan);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await TaiKhoanExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaiKhoan(string id)
        {
            await _taiKhoanService.DeleteTaiKhoanAsync(id);
            return NoContent();
        }

        private async Task<bool> TaiKhoanExists(string id)
        {
            var taiKhoan = await _taiKhoanService.GetTaiKhoanByIdAsync(id);
            return taiKhoan != null;
        }
    }
}
