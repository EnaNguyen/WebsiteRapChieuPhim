using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjectGSMAUI.Api.Data.Entities;
using ProjectGSMAUI.MVC.Models;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ProjectGSMAUI.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SanPhamController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl = "https://localhost:7141/api/SanPham";

        public SanPhamController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string searching = null)
        {
            List<SanPham> danhSachSanPham = new List<SanPham>();

            try
            {
                string url = string.IsNullOrEmpty(searching)
                    ? $"{_baseApiUrl}"
                    : $"{_baseApiUrl}?Name={Uri.EscapeDataString(searching)}";

                HttpResponseMessage response = await _httpClient.GetAsync(url);
                
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    danhSachSanPham = JsonConvert.DeserializeObject<List<SanPham>>(data);
                }
                else
                {
                    string apiError = await response.Content.ReadAsStringAsync();
                    return BadRequest(new { message = $"Lỗi từ API: {apiError}" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Đã xảy ra lỗi: {ex.Message}" });
            }

            ViewData["ListSanPham"] = danhSachSanPham;
            ViewData["Searching"] = searching;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] SanPhamModel model)
        {
            try
            {
                var errors = new Dictionary<string, string>();
                if (string.IsNullOrWhiteSpace(model.TenSanPham))
                {
                    errors["TenSanPham"] = "Tên sản phẩm không được để trống.";
                }

                if (model.Gia <= 0)
                {
                    errors["Gia"] = "Giá sản phẩm phải lớn hơn 0.";
                }              
                if (model.SoLuong <= 0)
                {
                     errors["SoLuong"] ="Số lượng phải lớn hơn 0." ;
                }

                if (model.HinhAnh == null)
                {
                    errors["HinhAnh"] =  "Hình ảnh không được để trống.";
                }

                if (model.HinhAnh != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await model.HinhAnh.CopyToAsync(memoryStream);
                        byte[] imageBytes = memoryStream.ToArray();
                        model.HinhAnh64 = Convert.ToBase64String(imageBytes);
                    }
                }
                if(errors.Count>0)
                {
                    return BadRequest(errors);
                }
                var sanPhamMoi = new SanPham()
                {
                    Id = model.Id,
                    TenSanPham = model.TenSanPham,
                    Gia = model.Gia,
                    MoTa = model.MoTa,
                    SoLuong = model.SoLuong,
                    HinhAnh = model.HinhAnh != null ? Convert.FromBase64String(model.HinhAnh64) : null
                };

                var json = JsonConvert.SerializeObject(sanPhamMoi);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_baseApiUrl, content);
                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true });
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return Json(new { success = false, message = error });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Đã xảy ra lỗi trong quá trình thêm mới sản phẩm. Lỗi: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSanPham(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseApiUrl}/GetById?id={id}");
                if (!response.IsSuccessStatusCode)
                {
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm." });
                }
                var content = await response.Content.ReadAsStringAsync();
                var sanPham = JsonConvert.DeserializeObject<SanPham>(content);
                return Json(new { success = true, data = sanPham });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Đã xảy ra lỗi trong quá trình lấy thông tin sản phẩm. Lỗi: {ex.Message}" });
            }
        }


        [HttpPost]
        public async Task<IActionResult> Update([FromForm] SanPhamModel model)
        {
            var errors = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(model.TenSanPham))
            {
                errors["TenSanPham"] = "Tên sản phẩm không được để trống.";
            }

            if (model.Gia <= 0)
            {
                errors["Gia"] = "Giá sản phẩm phải lớn hơn 0.";
            }

            if (model.SoLuong <= 0)
            {
                errors["SoLuong"] = "Số lượng phải lớn hơn 0.";
            }
            if (errors.Count > 0)
            {
                return BadRequest(errors);
            }

            try
            {
                if (model.HinhAnh != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await model.HinhAnh.CopyToAsync(memoryStream);
                        byte[] imageBytes = memoryStream.ToArray();
                        model.HinhAnh64 = Convert.ToBase64String(imageBytes);
                    }
                }

                var sanPhamMoi = new SanPham()
                {
                    Id = model.Id,
                    TenSanPham = model.TenSanPham,
                    Gia = model.Gia,
                    MoTa = model.MoTa,
                    SoLuong = model.SoLuong,
                    HinhAnh = model.HinhAnh != null ? Convert.FromBase64String(model.HinhAnh64) : null
                };

                var json = JsonConvert.SerializeObject(sanPhamMoi);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync(_baseApiUrl+ "/UpdateSanPham", content);
                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true });
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return Json(new { success = false, message = error });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Đã xảy ra lỗi trong quá trình cập nhật sản phẩm. Lỗi: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseApiUrl}/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Xóa sản phẩm thành công." });
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return Json(new { success = false, message = error });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Đã xảy ra lỗi trong quá trình xóa sản phẩm. Lỗi: {ex.Message}" });
            }
        }
    }
}
