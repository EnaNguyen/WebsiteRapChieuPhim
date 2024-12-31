using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ProjectGSMAUI.Api.Data;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;

namespace ProjectGSMAUI.Api.Helper
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly ApplicationDbContext _context;

        public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ApplicationDbContext context)
            : base(options, logger, encoder)
        {
            _context = context;
        }

        protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("Authorization header missing");
            }

            try
            {
                var headerValue = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                if (headerValue.Scheme != "basic" || string.IsNullOrEmpty(headerValue.Parameter))
                {
                    return AuthenticateResult.Fail("Invalid Authorization Header");
                }

                var bytes = Convert.FromBase64String(headerValue.Parameter);
                string credential = Encoding.UTF8.GetString(bytes);
                string[] array = credential.Split(':');
                if (array.Length != 2)
                {
                    return AuthenticateResult.Fail("Invalid credentials format");
                }
                string username = array[0];
                string password = array[1];
                string hashedPassword = HashPassword(password);
                var user = await _context.TaiKhoans.FirstOrDefaultAsync(item => item.TenTaiKhoan == username && item.MatKhau == hashedPassword);
                if (user == null)
                {
                    return AuthenticateResult.Fail("Invalid username or password");
                }

                var claims = new[] { new Claim(ClaimTypes.Name, user.TenTaiKhoan) };
                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return AuthenticateResult.Success(ticket);
            }
            catch (FormatException)
            {
                return AuthenticateResult.Fail("Invalid Authorization Header Format");
            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail($"Authentication failed: {ex.Message}");
            }
        }

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
