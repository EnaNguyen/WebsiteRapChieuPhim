using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectGSMAUI.Api.Container;
using ProjectGSMAUI.Api.Data;
using ProjectGSMAUI.Api.Data.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProjectGSMAUI.Api.Controllers.PBH
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaiKhoanController : ControllerBase
    {
        private readonly ITaiKhoanService _taiKhoanService;
        private readonly string _imagePath;
        private readonly ApplicationDbContext _context;

        public TaiKhoanController(ITaiKhoanService taiKhoanService, ApplicationDbContext context)
        {
            _taiKhoanService = taiKhoanService;
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/AnhTaiKhoan");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaiKhoan>>> GetTaiKhoans()
        {
            var taiKhoans = await _taiKhoanService.GetTaiKhoansAsync();
            var result = taiKhoans.Select(t => new
            {
                IdtaiKhoan = t.IdtaiKhoan.Trim(),
                TenTaiKhoan = t.TenTaiKhoan.Trim(),
                MatKhau = t.MatKhau?.Trim(),
                TenNguoiDung = t.TenNguoiDung?.Trim(),
                TrangThai = t.TrangThai,
                Hinh = t.Hinh?.Trim(),
                Cccd = t.Cccd?.Trim(),
                VaiTro = t.VaiTro,
                Email = t.Email?.Trim(),
                Sdt = t.Sdt?.Trim(),
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

        [HttpGet("GenerateId")]
        public ActionResult<string> GenerateId()
        {
            try
            {
                var newId = $"TK{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}"; // VD: TK1A2B3C4D
                return Ok(newId);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi hệ thống: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<TaiKhoan>> PostTaiKhoan([FromForm] TaiKhoan taiKhoan)
        {
            if (string.IsNullOrEmpty(taiKhoan.TenTaiKhoan) || taiKhoan.TenTaiKhoan.Length < 3)
            {
                return BadRequest(new { ErrorMessage = "Tên tài khoản không được để trống và phải có ít nhất 3 ký tự." });
            }
            if (string.IsNullOrEmpty(taiKhoan.MatKhau) || !Regex.IsMatch(taiKhoan.MatKhau, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*])[A-Za-z\d!@#$%^&*]{8,}$"))
            {
                return BadRequest(new { ErrorMessage = "Mật khẩu không hợp lệ. Mật khẩu phải có ít nhất 8 ký tự, một chữ cái hoa, một chữ cái thường, một số và một ký tự đặc biệt." });
            }
            if (string.IsNullOrEmpty(taiKhoan.Email) || !Regex.IsMatch(taiKhoan.Email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
            {
                return BadRequest(new { ErrorMessage = "Email không hợp lệ." });
            }
            if (!string.IsNullOrEmpty(taiKhoan.Sdt) && !Regex.IsMatch(taiKhoan.Sdt, @"^\+?\d+$"))
            {
                return BadRequest(new { ErrorMessage = "Số điện thoại chỉ được phép nhập các chữ số và có thể bắt đầu bằng dấu '+'. Không nhập ký tự đặc biệt khác." });
            }
            if (taiKhoan.TrangThai < 0 || taiKhoan.TrangThai > 1)
            {
                return BadRequest(new { ErrorMessage = "Trạng thái không hợp lệ." });
            }
            if (taiKhoan.Anh != null && !new[] { ".jpg", ".jpeg", ".png" }.Contains(Path.GetExtension(taiKhoan.Anh.FileName).ToLower()))
            {
                return BadRequest(new { ErrorMessage = "Hình ảnh chỉ hỗ trợ định dạng .jpg, .jpeg, .png." });
            }
            if (!string.IsNullOrEmpty(taiKhoan.Cccd) && (!Regex.IsMatch(taiKhoan.Cccd, @"^\d{12}$")))
            {
                return BadRequest(new { ErrorMessage = "CCCD chỉ được phép nhập 12 chữ số." });
            }
            if (taiKhoan.NgaySinh != null && (DateTime.Now.Year - taiKhoan.NgaySinh.Value.Year) < 18)
            {
                return BadRequest(new { ErrorMessage = "Người dùng phải trên 18 tuổi." });
            }
            if (await _context.TaiKhoans.AnyAsync(t => t.TenTaiKhoan == taiKhoan.TenTaiKhoan))
            {
                return BadRequest(new { ErrorMessage = "Tên tài khoản đã tồn tại." });
            }

            if (await _context.TaiKhoans.AnyAsync(t => t.Email == taiKhoan.Email))
            {
                return BadRequest(new { ErrorMessage = "Email đã tồn tại." });
            }

            try
            {
                if (string.IsNullOrEmpty(taiKhoan.IdtaiKhoan))
                {
                    taiKhoan.IdtaiKhoan = $"TK{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";
                }

                taiKhoan.TrangThai = 1;

                if (taiKhoan.Anh != null && taiKhoan.Anh.Length > 0)
                {
                    string fileName = $"{taiKhoan.IdtaiKhoan}_{Path.GetFileName(taiKhoan.Anh.FileName)}";
                    string filePath = Path.Combine(_imagePath, fileName);

                    if (!Directory.Exists(_imagePath))
                    {
                        Directory.CreateDirectory(_imagePath);
                    }

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await taiKhoan.Anh.CopyToAsync(stream);
                    }

                    taiKhoan.Hinh = $"/images/AnhTaiKhoan/{fileName}";
                }

                await _taiKhoanService.CreateTaiKhoanAsync(taiKhoan);

                return CreatedAtAction(nameof(GetTaiKhoan), new { id = taiKhoan.IdtaiKhoan }, taiKhoan);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi hệ thống: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTaiKhoan(string id, [FromForm] TaiKhoan taiKhoan)
        {
            if (id != taiKhoan.IdtaiKhoan)
            {
                return BadRequest(new { ErrorMessage = "ID không khớp với dữ liệu." });
            }

            if (string.IsNullOrEmpty(taiKhoan.TenTaiKhoan) || string.IsNullOrEmpty(taiKhoan.Email))
            {
                return BadRequest(new { ErrorMessage = "Tên tài khoản hoặc email không được để trống." });
            }

            try
            {
                var updatedTaiKhoan = new TaiKhoan
                {
                    IdtaiKhoan = taiKhoan.IdtaiKhoan,
                    TenTaiKhoan = taiKhoan.TenTaiKhoan,
                    Email = taiKhoan.Email,
                    Sdt = taiKhoan.Sdt,
                    TrangThai = taiKhoan.TrangThai,
                    Anh = taiKhoan.Anh
                };

                await _taiKhoanService.UpdateTaiKhoanAsync(updatedTaiKhoan);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ErrorMessage = "Lỗi hệ thống: " + ex.Message });
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaiKhoan(string id)
        {
            await _taiKhoanService.DeleteTaiKhoanAsync(id);
            return NoContent();
        }
    }
}
