using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectGSMAUI.Api.Container;
using ProjectGSMAUI.Api.Data;
using ProjectGSMAUI.Api.Data.Entities;
using ProjectGSMAUI.Api.Helper;
using ProjectGSMAUI.Api.Modal;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
                Hinh = t.Hinh,
                Cccd = t.Cccd?.Trim(),
                VaiTro = t.VaiTro,
                Email = t.Email?.Trim(),
                Sdt = t.Sdt?.Trim(),
            });
            return Ok(result);
        }

        [HttpGet("GetTaiKhoan")]
        public async Task<TaiKhoan> GetTaiKhoan(string id)
        {
            TaiKhoan tk = new TaiKhoan();
            var taiKhoan = await _taiKhoanService.GetTaiKhoanByIdAsync(id);

            if (taiKhoan != null)
            {
                tk = taiKhoan;
            }

            return tk;
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
            /*try
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
            {*/
            return StatusCode(500, $"Lỗi hệ thống:");
            /*}*/
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
                    Sdt = taiKhoan.Sdt, // Nếu SĐT có thể để trống
                    TrangThai = taiKhoan.TrangThai, // Nếu trạng thái có thể để trống
                    Hinh = taiKhoan.Hinh // Nếu hình ảnh có thể để trống
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

        private async Task<bool> TaiKhoanExists(string id)
        {
            var taiKhoan = await _taiKhoanService.GetTaiKhoanByIdAsync(id);
            return taiKhoan != null;
        }




        //Admin
        [HttpGet("TaiKhoanAdmin")]
        public async Task<IActionResult> TaiKhoanAdmin([FromQuery] string Name = null)
        {
            var data = await this._taiKhoanService.TaiKhoanAdmin(Name);
            return Ok(data);
        }
        [HttpPost("CreateAdmin")]
        public async Task<IActionResult> CreateAdmin(TaiKhoanRequest _data)
        {
            var data = await _taiKhoanService.CreateAdmin(_data);
            return Ok(data);
        }
        [HttpGet("GetByTenTaiKhoan")]
        public async Task<IActionResult> GetByTenTaiKhoan(string Name)
        {
            var data = await _taiKhoanService.GetByTenTaiKhoan(Name);
            return Ok(data);
        }
        [HttpGet("GetByEmail")]
        public async Task<IActionResult> GetByEmail(string Name)
        {
            var data = await _taiKhoanService.GetByEmail(Name);
            return Ok(data);
        }
        [HttpPut("UpdateAdmin")]
        public async Task<IActionResult> UpdateAdmin([FromBody] TaiKhoanEdit request)
        {
            string id = request.Id;
            TaiKhoanRequest data = request.TaiKhoanRequest;

            // Gọi phương thức Update hiện tại
            var response = await _taiKhoanService.UpdateAdmin(id, data);

            if (response.ResponseCode == 201)
            {
                return Ok(response.Result);
            }
            else
            {
                return BadRequest(response.ErrorMessage);
            }
        }
        [HttpPut("VoHieuHoa")]
        public async Task<IActionResult> VoHieuHoa([FromBody] string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("ID không hợp lệ.");
            }

            var response = await _taiKhoanService.VoHieuHoa(id);

            if (response.ResponseCode == 201)
            {
                return Ok(response.Result);
            }
            else
            {
                return BadRequest(response.ErrorMessage);
            }
        }




        //Customer
        [HttpGet("TaiKhoanCustomer")]
        public async Task<IActionResult> TaiKhoanCustomer([FromQuery] string Name = null)
        {
            var data = await this._taiKhoanService.TaiKhoanCustomer(Name);
            return Ok(data);
        }
        [HttpPost("CreateCustomer")]
        public async Task<IActionResult> CreateCustomer(TaiKhoanRequest _data)
        {
            var data = await _taiKhoanService.CreateCustomer(_data);
            return Ok(data);
        }
        [HttpPut("UpdateCustomer")]
        public async Task<IActionResult> UpdateCustomer([FromBody] TaiKhoanEdit request)
        {
            string id = request.Id;
            TaiKhoanRequest data = request.TaiKhoanRequest;

            // Gọi phương thức Update hiện tại
            var response = await _taiKhoanService.UpdateCustomer(id, data);

            if (response.ResponseCode == 201)
            {
                return Ok(response.Result);
            }
            else
            {
                return BadRequest(response.ErrorMessage);
            }
        }
    }
}
