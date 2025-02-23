using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; // For Session
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using ProjectGSMVC.Areas.Admin.Models; // Reusing TaiKhoanModel for simplicity, adjust if needed
using System.Text.Json;
using System.Text.Json.Serialization;
using ProjectGSMAUI.Api.Modal;
using ProjectGSMAUI.Api.Helper;

namespace ProjectGSMVC.Controllers
{
    public class UserController : Controller
    {
        private readonly HttpClient _httpClient;
        Uri baseAddress = new Uri("https://localhost:7141/api"); // Your API Base URL
        private readonly string _apiUrl = "https://localhost:7141/api/Auth/Login";
        public UserController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = baseAddress;
        }
        [HttpGet]
        public IActionResult Login()
        {
            // 🔹 Nếu đã đăng nhập, chuyển hướng về trang chủ
            if (HttpContext.Session.GetString("UserId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string tenTaiKhoan, string matKhau, string returnUrl)
        {
            if (string.IsNullOrEmpty(tenTaiKhoan) || string.IsNullOrEmpty(matKhau))
            {
                ViewBag.ErrorMessage = "Vui lòng nhập tên tài khoản và mật khẩu.";
                return View();
            }

            try
            {
                var loginData = new
                {
                    tenTaiKhoan = tenTaiKhoan,
                    matKhau = matKhau
                };

                var jsonContent = new StringContent(JsonConvert.SerializeObject(loginData), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync(_apiUrl, jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var loginResult = JsonConvert.DeserializeObject<LoginResponse>(responseContent);

                    if (loginResult != null && loginResult.Success)
                    {
                        // ✅ Lưu UserId và UserName vào Session
                        HttpContext.Session.SetString("UserId", loginResult.UserId);
                        HttpContext.Session.SetString("UserName", loginResult.UserName);

                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home"); // 🔹 Chuyển về trang chủ sau khi đăng nhập
                        }
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Đăng nhập không thành công. Vui lòng kiểm tra lại thông tin.";
                        return View();
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "Lỗi đăng nhập. Vui lòng thử lại sau.";
                    return View();
                }
            }
            catch (HttpRequestException)
            {
                ViewBag.ErrorMessage = "Lỗi kết nối đến máy chủ. Vui lòng thử lại sau.";
                return View();
            }
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromForm] TaiKhoanModel data)
        {
            // ... (Validation and error checking for other fields - unchanged) ...
            var errors = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(data.TenNguoiDung))
            {
                errors["TenNguoiDung"] = "Họ và Tên không được để trống.";
            }

            if (string.IsNullOrEmpty(data.TenTaiKhoan))
            {
                errors["TenTaiKhoan"] = "Tên Tài Khoản không được để trống.";
            }

            if (string.IsNullOrEmpty(data.MatKhau) || data.MatKhau.Contains(" ") || data.MatKhau.Length < 6)
            {
                errors["MatKhau"] = "Mật Khẩu phải có chứa ít nhất 6 kí tự";
            }
            if (data.ReMatKhau != data.MatKhau)
            {
                errors["ReMatKhau"] = "Mật Khẩu nhập lại không trùng khớp";
            }
            if (string.IsNullOrEmpty(data.Email))
            {
                errors["Email"] = "Email không được để trống.";
            }

            if (string.IsNullOrWhiteSpace(data.Sdt) || (data.Sdt.Length != 10 && data.Sdt.Length != 11) || !data.Sdt.All(char.IsDigit))
            {
                errors["Sdt"] = "Số điện thoại không được nhập chữ hay kí tự đặc biệt, độ dài 10-11 số";
            }
            if (data.NgaySinh == default || data.NgaySinh > DateOnly.FromDateTime(DateTime.Today))
            {
                errors["NgaySinh"] = "Ngày bắt đầu không được nhỏ hơn ngày hiện tại.";
            }
            if (string.IsNullOrEmpty(data.Cccd) || data.Cccd.Length != 12 || !data.Cccd.All(char.IsDigit))
            {
                errors["CCCD"] = "CCCD chỉ gồm số và có độ dài là 12 ký tự";
            }
            if (errors.Count > 0)
            {
                foreach (var error in errors)
                {
                    ModelState.AddModelError(error.Key, error.Value);
                }
                return View(data); // Return to view with model state errors
            }


            try
            {
                string? imageBase64String = null; // Initialize as null

                // **Conditional Image Processing**
                if (data.ImageFile != null && data.ImageFile.Length > 0) // Check if ImageFile is not null AND has content
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await data.ImageFile.CopyToAsync(memoryStream);
                        byte[] imageBytes = memoryStream.ToArray();
                        imageBase64String = Convert.ToBase64String(imageBytes);
                    }
                }

                var registerRequest = new TaiKhoanRequest
                {
                    TenNguoiDung = data.TenNguoiDung,
                    MatKhau = data.MatKhau,
                    TenTaiKhoan = data.TenTaiKhoan,
                    Email = data.Email,
                    Sdt = data.Sdt,
                    NgaySinh = data.NgaySinh,
                    Cccd = data.Cccd,
                    GioiTinh = data.GioiTinh == "1" ? true : false,
                    DiaChi = data.DiaChi,
                    Hinh = imageBase64String // Set Hinh to the base64 string (or null if no image)
                };

                var jsonContent = new StringContent(
                    System.Text.Json.JsonSerializer.Serialize(registerRequest, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PostAsync("/api/TaiKhoan/CreateCustomer", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    ViewBag.SuccessMessage = "Đăng ký thành công! Vui lòng đăng nhập.";
                    return View("Login");
                }
                else
                {
                    string apiError = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<APIResponse>(apiError);

                    if (response.StatusCode == System.Net.HttpStatusCode.BadRequest && apiResponse?.ErrorMessage == "Tên tài khoản đã tồn tại. Vui lòng chọn tên tài khoản khác.")
                    {
                        ModelState.AddModelError("TenTaiKhoan", apiResponse.ErrorMessage);
                        return View(data);
                    }
                    else
                    {
                        ModelState.AddModelError("", $"Lỗi đăng ký từ API: {apiError}");
                        return View(data);
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Đã xảy ra lỗi trong quá trình đăng ký: {ex.Message}");
                return View(data);
            }
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Clear all session data
            return RedirectToAction("Index", "Home"); // Redirect to homepage after logout
        }

        // Example action to demonstrate booking redirection (you'll need to adapt this to your booking flow)
        public IActionResult BookTicket()
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                // Store current URL to redirect back after login
                HttpContext.Session.SetString("ReturnUrl", "/User/BookTicket"); // Assuming /User/BookTicket is the booking action URL
                TempData["LoginMessage"] = "Bạn cần đăng nhập để đặt vé."; // Message to display on login page (optional)
                return RedirectToAction("Login");
            }

            // User is logged in, proceed with booking logic
            return View(); // Your booking view
        }
    }

    // Response class for Login API (adjust based on your actual API response)
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string UserId { get; set; } // Or int, depending on your User ID type
        public string UserName { get; set; } // Optional: Username
        public string Message { get; set; } // Optional: Error message
    }
}