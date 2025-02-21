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

            // Authentication successful
            return Ok(new LoginResponseApiModel
            {
                Success = true,
                UserId = taiKhoan.IdtaiKhoan, 
                UserName = taiKhoan.TenNguoiDung, 
                Message = "Đăng nhập thành công."
            });
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