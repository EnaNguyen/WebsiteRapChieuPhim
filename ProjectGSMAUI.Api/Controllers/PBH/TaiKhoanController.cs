using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectGSMAUI.Api.Container;
using ProjectGSMAUI.Api.Data;
using ProjectGSMAUI.Api.Data.Entities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                // Tạo ID tự động bằng GUID
                var newId = $"TK{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}"; // VD: TK1A2B3C4D
                return Ok(newId);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                Console.WriteLine($"Lỗi: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(500, $"Lỗi hệ thống: {ex.Message}");
            }
        }
        [HttpPost]
        public async Task<ActionResult<TaiKhoan>> PostTaiKhoan([FromForm] TaiKhoan taiKhoan)
        {
            try
            {
                // Tự động tạo ID nếu không có
                if (string.IsNullOrEmpty(taiKhoan.IdtaiKhoan))
                {
                    taiKhoan.IdtaiKhoan = $"TK{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";
                }

                // Thiết lập trạng thái mặc định là 1
                taiKhoan.TrangThai = 1;

                // Xử lý lưu ảnh nếu có
                if (taiKhoan.Anh != null && taiKhoan.Anh.Length > 0)
                {
                    string fileName = $"{taiKhoan.IdtaiKhoan}_{Path.GetFileName(taiKhoan.Anh.FileName)}";
                    string filePath = Path.Combine(_imagePath, fileName);

                    // Tạo thư mục nếu chưa tồn tại
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

                // Lưu tài khoản vào database
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
            Console.WriteLine($"ID từ URL: {id}");
            Console.WriteLine($"ID từ Body: {taiKhoan.IdtaiKhoan}");

            if (id != taiKhoan.IdtaiKhoan)
            {
                Console.WriteLine("ID không khớp.");
                return BadRequest(new
                {
                    ErrorMessage = "ID không khớp với dữ liệu."
                });
            }

            // Chỉ kiểm tra các trường cần thiết
            if (string.IsNullOrEmpty(taiKhoan.TenTaiKhoan) || string.IsNullOrEmpty(taiKhoan.Email))
            {
                Console.WriteLine("Dữ liệu không hợp lệ.");
                return BadRequest(new
                {
                    ErrorMessage = "Tên tài khoản hoặc email không được để trống."
                });
            }

            try
            {
                // Cập nhật thông tin tài khoản
                var updatedTaiKhoan = new TaiKhoan
                {
                    IdtaiKhoan = taiKhoan.IdtaiKhoan,
                    TenTaiKhoan = taiKhoan.TenTaiKhoan,
                    Email = taiKhoan.Email,
                    Sdt = taiKhoan.Sdt, // Nếu SĐT có thể để trống
                    TrangThai = taiKhoan.TrangThai, // Nếu trạng thái có thể để trống
                    Anh = taiKhoan.Anh // Nếu hình ảnh có thể để trống
                };

                await _taiKhoanService.UpdateTaiKhoanAsync(updatedTaiKhoan);
                Console.WriteLine("Cập nhật thành công.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi hệ thống: {ex.Message}");
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

        private async Task<bool> TaiKhoanExists(string id)
        {
            var taiKhoan = await _taiKhoanService.GetTaiKhoanByIdAsync(id);
            return taiKhoan != null;
        }
    }
}
