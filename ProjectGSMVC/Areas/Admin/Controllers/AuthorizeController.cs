using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ProjectGSMAUI.Api.Modal;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using ProjectGSMVC.Areas.Admin.Models;
using Microsoft.AspNetCore.Session;
using System.Net.Http;
using System.Text.Json;
using ProjectGSMAUI.Api.Utilities;
namespace ProjectGSMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuthorizeController : Controller
    {
        private static int LoadingPageTime = 0;

        private readonly HttpClient _client;
        private readonly JwtSettings _jwtSettings;

        public AuthorizeController(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
            _client = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7141/api/")
            };
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var loginData = new
            {
                Username = username,
                Password = password,
                VaiTro = "Admin",
            };

            try
            {
                var jsonData = JsonConvert.SerializeObject(loginData);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync("Authorize/GenerateToken", content);

                if (response.IsSuccessStatusCode)
                {
                    var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(await response.Content.ReadAsStringAsync());
                    HttpContext.Session.SetString("JWTToken", tokenResponse.Token);
                    HttpContext.Session.SetString("GenerateRefreshToken", tokenResponse.RefreshToken);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Error = "Đăng nhập không thành công.";
                    return View();
                }
            }
            catch
            {
                ViewBag.Error = "Lỗi khi kết nối với API.";
                return View();
            }
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(TaiKhoanModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var jsonData = JsonConvert.SerializeObject(model);
                    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                    var response = await _client.PostAsync("Authorize/CreateCustomer", content);

                    if (response.IsSuccessStatusCode)
                    {
                        return Json(new { success = true, message = "Đăng ký thành công!" });
                    }
                    else
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        return Json(new { success = false, message = errorMessage });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Lỗi khi kết nối với API: " + ex.Message });
                }
            }

            return Json(new { success = false, message = "Dữ liệu không hợp lệ." });
        }


    }
}
