using Microsoft.AspNetCore.Mvc;
using ProjectGSMAUI.Api.Container;
using ProjectGSMAUI.Api.Helper;
using ProjectGSMAUI.Api.Modal;
using ProjectGSMAUI.Api.Utilities;
using System.ComponentModel.DataAnnotations;
using Facebook;
using Microsoft.AspNetCore.Authentication;
using ProjectGSMAUI.Api.Data.Entities;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;

namespace ProjectGSMAUI.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ITaiKhoanService _taiKhoanService;
        private readonly EmailService _emailService;
        private readonly IConfiguration _configuration;

        public AuthController(ITaiKhoanService taiKhoanService, EmailService emailService, IConfiguration configuration) 
        {
            _taiKhoanService = taiKhoanService;
            _emailService = emailService;
            _configuration = configuration;
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


        [HttpPost("ForgotPasswordRequest")]
        public async Task<IActionResult> ForgotPasswordRequest([FromBody] ForgotPasswordRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool resetRequested = await _taiKhoanService.RequestPasswordResetAsync(model.Email);
            if (!resetRequested)
            {
                return BadRequest(new { Message = "Không tìm thấy tài khoản với email này." }); // Or more generic error for security in production
            }

            return Ok(new { Message = "OTP đã được gửi đến email của bạn. Vui lòng kiểm tra hộp thư đến." });
        }


        [HttpPost("VerifyOTP")]
        public async Task<IActionResult> VerifyOTP([FromBody] VerifyOTPModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool isOTPValid = await _taiKhoanService.VerifyOTPAsync(model.Email, model.OTP);
            if (!isOTPValid)
            {
                return BadRequest(new { Message = "Mã OTP không hợp lệ hoặc đã hết hạn." });
            }

            return Ok(new { Message = "OTP hợp lệ. Vui lòng đặt mật khẩu mới." }); // Indicate OTP is valid
        }


        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool passwordReset = await _taiKhoanService.ResetPasswordAsync(model.Email, model.OTP, model.NewPassword);
            if (!passwordReset)
            {
                return BadRequest(new { Message = "Lỗi đặt lại mật khẩu. Vui lòng thử lại." }); // Generic error
            }

            return Ok(new { Message = "Mật khẩu đã được đặt lại thành công. Vui lòng đăng nhập bằng mật khẩu mới." });
        }


        [HttpPost("FacebookLogin")]
        public IActionResult FacebookLogin()
        {
            // Construct the OAuth URL manually using Facebook SDK classes
            string appId = _configuration["Authentication:Facebook:AppId"]; // Read AppId from configuration
            string redirectUri = Url.Action("FacebookLoginCallback", "Auth", null, "https"); // Construct callback URL

            var client = new FacebookClient();
            Uri loginUrl = client.GetLoginUrl(
                parameters: new
                {
                    client_id = appId,
                    redirect_uri = redirectUri,
                    response_type = "code", // OAuth authorization code flow
                    scope = "email,public_profile" // Request email and public profile permissions
                }
            );

            return Redirect(loginUrl.AbsoluteUri); // Redirect user to Facebook Login URL
        }


        [HttpGet("FacebookLoginCallback")]
        public async Task<IActionResult> FacebookLoginCallback(string code) // Facebook will send back an authorization 'code'
        {
            if (string.IsNullOrEmpty(code))
            {
                return BadRequest(new { Message = "Facebook login failed: Authorization code missing." });
            }

            string appId = _configuration["Authentication:Facebook:AppId"];      // Read AppId from configuration
            string appSecret = _configuration["Authentication:Facebook:AppSecret"]; // Read AppSecret from configuration
            string redirectUri = Url.Action("FacebookLoginCallback", "Auth", null, "https"); // Reconstruct callback URL

            var client = new FacebookClient();
            dynamic tokenResult = null;
            try
            {
                // Exchange the authorization code for an access token
                tokenResult = await client.PostTaskAsync( // Remove <dynamic> type argument
                    "oauth/access_token",
                    new
                    {
                        client_id = appId,
                        client_secret = appSecret,
                        redirect_uri = redirectUri,
                        code = code
                    }
                );
            }
            catch (FacebookOAuthException ex)
            {
                return BadRequest(new { Message = $"Facebook OAuth error: {ex.Message}" });
            }

            string accessToken = (string)tokenResult.access_token;

            dynamic profileResult = null;
            try
            {
                // Get user profile information using the access token
                profileResult = await client.GetTaskAsync<dynamic>(
                    "me",
                    new
                    {
                        access_token = accessToken,
                        fields = "id,name,email" // Request user ID, name, and email
                    }
                );
            }
            catch (FacebookApiException ex)
            {
                return BadRequest(new { Message = $"Facebook API error: {ex.Message}" });
            }

            string facebookId = (string)profileResult.id;
            string name = (string)profileResult.name;
            string email = (string)profileResult.email;


            // **Authenticate or Register User (same logic as before)**
            var existingTaiKhoan = await _taiKhoanService.GetTaiKhoanByFacebookIdAsync(facebookId); // Assuming you have this service method

            TaiKhoan taiKhoan;
            if (existingTaiKhoan != null)
            {
                taiKhoan = existingTaiKhoan; // User exists, log them in
            }
            else
            {
                // Register new user using Facebook profile
                taiKhoan = new TaiKhoan
                {
                    IdtaiKhoan = $"FB-{facebookId}", // Generate a unique ID (adjust as needed)
                    TenTaiKhoan = email ?? facebookId, // Use email as username if available, otherwise Facebook ID
                    TenNguoiDung = name ?? "Facebook User",
                    Email = email,
                    VaiTro = 1, // Default role for Facebook users
                    NgayDangKy = DateOnly.FromDateTime(DateTime.Today),
                    TrangThai = 1,
                    // ... other default fields ...
                    FacebookId = facebookId // Store Facebook ID in TaiKhoan entity
                };
                await _taiKhoanService.CreateTaiKhoanAsync(taiKhoan);
            }


            // **Authentication successful - create and return login response**
            return Ok(new LoginResponseApiModel
            {
                Success = true,
                UserId = taiKhoan.IdtaiKhoan,
                UserName = taiKhoan.TenTaiKhoan,
                Message = "Đăng nhập Facebook thành công."
            });
        }

    }

    public class ForgotPasswordRequestModel
    {
        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; }
    }

    public class VerifyOTPModel
    {
        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "OTP là bắt buộc.")]
        public string OTP { get; set; }
    }

    public class ResetPasswordModel
    {
        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "OTP là bắt buộc.")]
        public string OTP { get; set; }

        [Required(ErrorMessage = "Mật khẩu mới là bắt buộc.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        public string NewPassword { get; set; }
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