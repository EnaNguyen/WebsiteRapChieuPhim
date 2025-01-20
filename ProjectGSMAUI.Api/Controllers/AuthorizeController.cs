using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProjectGSMAUI.Api.Data;
using ProjectGSMAUI.Api.Modal;
using ProjectGSMAUI.Api.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ProjectGSMAUI.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtSettings jwtSettings;
        private readonly IRefreshHandler refresh;
        public AuthorizeController(ApplicationDbContext context, IOptions<JwtSettings> options,IRefreshHandler refresh)
        {
            _context = context;
            this.jwtSettings = options.Value;
            this.refresh = refresh; 
        }
        [HttpPost("GenerateToken")]
        public async Task<IActionResult> GenerateToken([FromBody] TaiKhoanCredential taiKhoanCredential)
        {
            string hashedPassword = HashPassword(taiKhoanCredential.Password);
            var user = await _context.TaiKhoans.FirstOrDefaultAsync(item => item.TenTaiKhoan == taiKhoanCredential.Username && item.MatKhau == hashedPassword);
            if (user == null)
            {
                return Unauthorized();
            }
            else
            {
                string role = user.VaiTro switch
                {
                    0 => "KhachHang",
                    1 => "NhanVien",
                    2 => "Admin",
                    null => "Unknown"
                };
                var tokenhandler = new JwtSecurityTokenHandler();
                var tokenkey = Encoding.UTF8.GetBytes(this.jwtSettings.securitykey);
                var tokendesc = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, user.TenTaiKhoan),
                        new Claim(ClaimTypes.Role, role)
                    }),
                    Expires = DateTime.UtcNow.AddSeconds(30),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenkey), SecurityAlgorithms.HmacSha256)
                };
                var token = tokenhandler.CreateToken(tokendesc);
                var finaltoken = tokenhandler.WriteToken(token);
                return Ok(new TokenResponse()
                {
                    Token = finaltoken,
                    RefreshToken = await this.refresh.GenerateToken(taiKhoanCredential.Username)
                });
            }
        }
        [HttpPost("GenerateRefreshToken")]

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2")); 
                }
                return builder.ToString();
            }
        }
    }
}
