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

        public UserController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = baseAddress;
        }

        [HttpGet]
        public IActionResult Login()
        {
            // Check if user is already logged in
            if (HttpContext.Session.GetString("UserId") != null)
            {
                return RedirectToAction("Index", "Home"); // Redirect to homepage if already logged in
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
                var loginData = new Dictionary<string, string>
                {
                    { "tenTaiKhoan", tenTaiKhoan },
                    { "matKhau", matKhau }
                };

                var jsonContent = new StringContent(JsonConvert.SerializeObject(loginData), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _httpClient.PostAsync("/api/Auth/Login", jsonContent); // Assuming your login API endpoint is /api/Auth/Login

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var loginResult = JsonConvert.DeserializeObject<LoginResponse>(responseContent); // Assuming an API response with UserId and success

                    if (loginResult != null && loginResult.Success)
                    {
                        HttpContext.Session.SetString("UserId", loginResult.UserId); // Store UserId in session
                        HttpContext.Session.SetString("UserName", loginResult.UserName); // Optionally store username too

                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl); // Redirect back to booking page or original URL
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home"); // Redirect to homepage after login
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
            catch (HttpRequestException ex)
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
            
            var errors = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(data.TenNguoiDung))
            {
                errors["TenNguoiDung"] = "Họ và Tên không được để trống.";
            }

            if (string.IsNullOrEmpty(data.TenTaiKhoan))
            {
                errors["TenTaiKhoan"] = "Tên Tài Khoản không được để trống.";
            }
            else
            {
                HttpResponseMessage usernameCheckResponse = await _httpClient.GetAsync($"/api/TaiKhoan/GetByTenTaiKhoan?Name={Uri.EscapeDataString(data.TenTaiKhoan)}");
                if (!usernameCheckResponse.IsSuccessStatusCode) 
                {
                    errors["TenTaiKhoan"] = "Tên Tài Khoản đã tồn tại.";
                }
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
            else
            {
                HttpResponseMessage emailCheckResponse = await _httpClient.GetAsync($"/api/TaiKhoan/GetByEmail?Name={Uri.EscapeDataString(data.Email)}");
                if (!emailCheckResponse.IsSuccessStatusCode) 
                {
                    errors["Email"] = "Email đã tồn tại.";
                }
            }

            if (string.IsNullOrWhiteSpace(data.Sdt) || (data.Sdt.Length != 10 && data.Sdt.Length != 11) || !data.Sdt.All(char.IsDigit))
            {
                errors["Sdt"] = "Số điện thoại không hợp lệ";
            }
            if (data.NgaySinh == default || data.NgaySinh > DateOnly.FromDateTime(DateTime.Today))
            {
                errors["NgaySinh"] = "Ngày sinh không không hợp lệ.";
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
                return View(data); 
            }


            try
            {
                string? imageBase64String = null; 

                
                if (data.ImageFile != null && data.ImageFile.Length > 0) 
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
                    Hinh = imageBase64String 
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

        

        [HttpGet]
        public IActionResult ForgotPasswordRequest()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPasswordRequest(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ViewBag.ErrorMessage = "Vui lòng nhập email.";
                return View();
            }

            try
            {
                var forgotPasswordRequest = new Dictionary<string, string>
                {
                    { "email", email }
                };

                var jsonContent = new StringContent(JsonConvert.SerializeObject(forgotPasswordRequest), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _httpClient.PostAsync("/api/Auth/ForgotPasswordRequest", jsonContent); // API endpoint

                if (response.IsSuccessStatusCode)
                {
                    ViewBag.SuccessMessage = "OTP đã được gửi đến email của bạn. Vui lòng kiểm tra hộp thư đến.";
                    ViewBag.Email = email; // Pass email to VerifyOTP view
                    return View("VerifyOTP"); // Redirect to VerifyOTP view
                }
                else
                {
                    string apiError = await response.Content.ReadAsStringAsync();
                    ViewBag.ErrorMessage = $"Lỗi yêu cầu OTP: {apiError}";
                    return View(); // Stay on ForgotPasswordRequest view with error message
                }
            }
            catch (HttpRequestException ex)
            {
                ViewBag.ErrorMessage = "Lỗi kết nối đến máy chủ. Vui lòng thử lại sau.";
                return View();
            }
        }


        [HttpGet]
        public IActionResult VerifyOTP()
        {
            string email = ViewBag.Email; 
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("ForgotPasswordRequest"); 
            }
            ViewBag.Email = email; 
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerifyOTP(string email, string otp)
        {
            if (string.IsNullOrEmpty(otp))
            {
                ViewBag.ErrorMessage = "Vui lòng nhập mã OTP.";
                ViewBag.Email = email; 
                return View();
            }

            try
            {
                var verifyOTPRequest = new Dictionary<string, string>
                {
                    { "email", email },
                    { "otp", otp }
                };

                var jsonContent = new StringContent(JsonConvert.SerializeObject(verifyOTPRequest), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _httpClient.PostAsync("/api/Auth/VerifyOTP", jsonContent); // API endpoint

                if (response.IsSuccessStatusCode)
                {
                    ViewBag.SuccessMessage = "OTP hợp lệ. Vui lòng đặt mật khẩu mới.";
                    ViewBag.Email = email; // Pass email to ResetPassword view
                    ViewBag.OTP = otp;       // Pass OTP to ResetPassword view (for hidden field)
                    return View("ResetPassword"); // Redirect to ResetPassword view
                }
                else
                {
                    string apiError = await response.Content.ReadAsStringAsync();
                    ViewBag.ErrorMessage = $"Mã OTP không hợp lệ: {apiError}";
                    ViewBag.Email = email; // Keep email in ViewBag
                    return View(); // Stay on VerifyOTP view with error message
                }
            }
            catch (HttpRequestException ex)
            {
                ViewBag.ErrorMessage = "Lỗi kết nối đến máy chủ. Vui lòng thử lại sau.";
                ViewBag.Email = email; // Keep email in ViewBag
                return View();
            }
        }


        [HttpGet]
        public IActionResult ResetPassword(string email, string otp) 
        {
            ViewBag.Email = email; // Pass email to ResetPassword view
            ViewBag.OTP = otp;     // Pass OTP to ResetPassword view
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(string email, string otp, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                ViewBag.ErrorMessage = "Vui lòng nhập mật khẩu mới và xác nhận mật khẩu.";
                ViewBag.Email = email; // Keep email in ViewBag
                ViewBag.OTP = otp;     // Keep OTP in ViewBag
                return View();
            }

            if (newPassword != confirmPassword)
            {
                ViewBag.ErrorMessage = "Mật khẩu xác nhận không khớp.";
                ViewBag.Email = email; // Keep email in ViewBag
                ViewBag.OTP = otp;     // Keep OTP in ViewBag
                return View();
            }

            try
            {
                var resetPasswordRequest = new Dictionary<string, string>
                {
                    { "email", email },
                    { "otp", otp },
                    { "newPassword", newPassword }
                };

                var jsonContent = new StringContent(JsonConvert.SerializeObject(resetPasswordRequest), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _httpClient.PostAsync("/api/Auth/ResetPassword", jsonContent); // API endpoint

                if (response.IsSuccessStatusCode)
                {
                    ViewBag.SuccessMessage = "Mật khẩu đã được đặt lại thành công. Vui lòng đăng nhập bằng mật khẩu mới.";
                    return View("Login"); // Redirect to login page with success message
                }
                else
                {
                    string apiError = await response.Content.ReadAsStringAsync();
                    ViewBag.ErrorMessage = $"Lỗi đặt lại mật khẩu: {apiError}";
                    ViewBag.Email = email; // Keep email in ViewBag
                    ViewBag.OTP = otp;     // Keep OTP in ViewBag
                    return View(); // Stay on ResetPassword view with error message
                }
            }
            catch (HttpRequestException ex)
            {
                ViewBag.ErrorMessage = "Lỗi kết nối đến máy chủ. Vui lòng thử lại sau.";
                ViewBag.Email = email; // Keep email in ViewBag
                ViewBag.OTP = otp;     // Keep OTP in ViewBag
                return View();
            }
        }

        [HttpPost]
        public IActionResult FacebookLogin()
        {
            // **Simply call the API's FacebookLogin endpoint - the API now handles the redirect to Facebook**
            return RedirectToAction("FacebookLogin", "Auth", new { Area = "", }); // Redirect to API's FacebookLogin
        }


        [HttpGet("FacebookLoginCallback")] // Route for callback from API after Facebook auth
        public async Task<IActionResult> FacebookLoginCallback()
        {
            // Call API's FacebookLoginCallback endpoint to finalize login and get login response
            HttpResponseMessage response = await _httpClient.GetAsync("/api/Auth/FacebookLoginCallback");

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var loginResult = JsonConvert.DeserializeObject<LoginResponse>(responseContent);

                if (loginResult != null && loginResult.Success)
                {
                    HttpContext.Session.SetString("UserId", loginResult.UserId);
                    HttpContext.Session.SetString("UserName", loginResult.UserName);
                    return RedirectToAction("Index", "Home"); // Redirect to homepage after Facebook login
                }
            }

            // Facebook login failed at API level
            ViewBag.ErrorMessage = "Đăng nhập Facebook không thành công. Vui lòng thử lại sau.";
            return View("Login"); // Return to login page with error message
        }

    }

   
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string UserId { get; set; } // Or int, depending on your User ID type
        public string UserName { get; set; } // Optional: Username
        public string Message { get; set; } // Optional: Error message
    }
}