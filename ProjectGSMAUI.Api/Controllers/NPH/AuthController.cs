using Microsoft.AspNetCore.Mvc;
using ProjectGSMAUI.Api.Container;
using ProjectGSMAUI.Api.Helper;
using ProjectGSMAUI.Api.Modal;
using ProjectGSMAUI.Api.Utilities;

namespace ProjectGSMAUI.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ITaiKhoanService _taiKhoanService;

        public AuthController(ITaiKhoanService taiKhoanService)
        {
            _taiKhoanService = taiKhoanService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var taiKhoan = await _taiKhoanService.GetTaiKhoanByTenTaiKhoanAsync(model.TenTaiKhoan);

                if (taiKhoan == null)
                {
                    return Unauthorized(new { Success = false, Message = "Tên tài khoản không đúng." });
                }

                if (!PasswordHasher.VerifyPassword(model.MatKhau, taiKhoan.MatKhau))
                {
                    return Unauthorized(new { Success = false, Message = "Mật khẩu không đúng." });
                }

                if (taiKhoan.TrangThai != 1)
                {
                    return Unauthorized(new { Success = false, Message = "Tài khoản đã bị vô hiệu hóa." });
                }

                // ✅ Kiểm tra Session có bị null không
                if (HttpContext.Session == null)
                {
                    return StatusCode(500, new { Success = false, Message = "Lỗi: HttpContext.Session bị null!" });
                }

                // ✅ Kiểm tra trước khi lưu vào Session
                if (string.IsNullOrEmpty(taiKhoan.IdtaiKhoan) || string.IsNullOrEmpty(taiKhoan.TenNguoiDung))
                {
                    return StatusCode(500, new { Success = false, Message = "Lỗi: Không thể lấy thông tin User." });
                }

                // ✅ **Lưu thông tin vào Session**
                HttpContext.Session.SetString("UserId", taiKhoan.IdtaiKhoan);
                HttpContext.Session.SetString("UserName", taiKhoan.TenNguoiDung);

                return Ok(new
                {
                    Success = true,
                    UserId = taiKhoan.IdtaiKhoan,
                    UserName = taiKhoan.TenNguoiDung,
                    Message = "Đăng nhập thành công."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Lỗi hệ thống", Error = ex.Message });
            }
        }

    }

    public class LoginRequestModel
    {
        public string TenTaiKhoan { get; set; }
        public string MatKhau { get; set; }
    }

    public class LoginResponseApiModel
    {
        public bool Success { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
    }

}