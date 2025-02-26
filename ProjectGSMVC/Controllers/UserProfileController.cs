using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjectGSMVC.Models;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ProjectGSMVC.Controllers
{
    public class UserProfileController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string apiBaseUrl = "https://localhost:7141/api/TaiKhoan";

        public UserProfileController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = HttpContext.Session.GetString("UserId"); 
                var userName = HttpContext.Session.GetString("UserName");
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Login", "User");

                }

                string apiUrl = $"{apiBaseUrl}/TaiKhoanCustomer?Name={Uri.EscapeDataString(userName)}";

                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var user = JsonConvert.DeserializeObject<TaiKhoanModel>(responseContent);
                    
                    return View(user);
                }
                else
                {
                    ViewBag.ErrorMessage = "API không trả về dữ liệu hợp lệ.";
                    return View(new TaiKhoanModel());
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Lỗi kết nối API: " + ex.Message;
                return View(new TaiKhoanModel());
            }
        }



        [HttpPost]
        public async Task<IActionResult> Update(TaiKhoanModel model)
        {
            if (model == null)
            {
                ViewBag.ErrorMessage = "Dữ liệu không hợp lệ.";
                return View("Index", model);
            }

            try
            {
                // **Xử lý ảnh**
                string? imageBase64String = null;

                if (model.HinhFile != null && model.HinhFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await model.HinhFile.CopyToAsync(memoryStream);
                        byte[] imageBytes = memoryStream.ToArray();
                        imageBase64String = Convert.ToBase64String(imageBytes);
                    }
                }
                else
                {
                    imageBase64String = model.HinhBase64; // Nếu không có ảnh mới, giữ ảnh cũ
                }
 
                var updateData = new
                {
                    Id = model.IdTaiKhoan,
                    TaiKhoanRequest = new
                    {
                        tenTaiKhoan = model.TenTaiKhoan,
                        matKhau= model.MatKhau,
                        tenNguoiDung = model.TenNguoiDung,
                        email = model.Email,
                        sdt = model.Sdt,
                        NgaySinh = model.NgaySinh?.ToString("yyyy-MM-dd"),
                        gioiTinh = model.GioiTinh,
                        diaChi = model.DiaChi,
                        cccd = model.Cccd,
                        Hinh = imageBase64String,

                        
                    }
                };

                string jsonData = JsonConvert.SerializeObject(updateData);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PutAsync($"{apiBaseUrl}/UpdateCustomer2", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
                    return RedirectToAction("Index");
                }
                else
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    ViewBag.ErrorMessage = $"Cập nhật thất bại: {errorResponse}";
                    return View("Index", model);
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Lỗi cập nhật: " + ex.Message;
                return View("Index", model);
            }
        }



    }
}
