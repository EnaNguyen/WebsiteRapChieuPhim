using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using ProjectGSMVC.Areas.Admin.Models;
using ProjectGSMAUI.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ProjectGSMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PhimController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:7141/api");
        private readonly HttpClient _httpClient;

        public PhimController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
        }
        private readonly PhimController _context;

        public PhimController(PhimController context) // Inject the DbContext here
        {
            _context = context;
        }

        


        [HttpGet]
        public IActionResult Index(string searching = null)
        {
            List<PhimViewModel> phimList = new List<PhimViewModel>();
            string url = string.IsNullOrEmpty(searching)
                ? $"{_httpClient.BaseAddress}/Phim/GetAll"
                : $"{_httpClient.BaseAddress}/Phim/GetAll?Name={Uri.EscapeDataString(searching)}";

            HttpResponseMessage response = _httpClient.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                phimList = JsonConvert.DeserializeObject<List<PhimViewModel>>(data);
            }
            else
            {
                string apiError = response.Content.ToString();
                return BadRequest(new { message = $"Lỗi từ API: {apiError}" });
            }
            ViewData["ListPhim"] = phimList;
            ViewData["Searching"] = searching;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] PhimViewModel data)
        {
            var errors = new Dictionary<string, string>();

            // Validation
            if (string.IsNullOrEmpty(data.TenPhim))
            {
                errors["TenPhim"] = "Tên phim không được để trống.";
            }

            if (string.IsNullOrEmpty(data.TheLoai))
            {
                errors["TheLoai"] = "Thể loại không được để trống.";
            }

            if (data.ThoiLuong <= 0)
            {
                errors["ThoiLuong"] = "Thời lượng phải lớn hơn 0 phút.";
            }

            if (string.IsNullOrEmpty(data.DaoDien))
            {
                errors["DaoDien"] = "Đạo diễn không được để trống.";
            }

            if (data.GioiHanDoTuoi <= 0)
            {
                errors["GioiHanDoTuoi"] = "Giới hạn độ tuổi phải lớn hơn 0.";
            }

            if (data.NgayKhoiChieu == null || data.NgayKhoiChieu < DateOnly.FromDateTime(DateTime.Today))
            {
                errors["NgayKhoiChieu"] = "Ngày khởi chiếu không hợp lệ.";
            }

            if (data.NgayKetThuc == null || data.NgayKhoiChieu > data.NgayKetThuc)
            {
                errors["NgayKetThuc"] = "Ngày kết thúc phải sau ngày khởi chiếu.";
            }

            if (data.ImageFile == null || data.ImageFile.Length == 0)
            {
                errors["ImageFile"] = "Poster phim không được để trống.";
            }

            if (errors.Count > 0)
            {
                return BadRequest(errors);
            }

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await data.ImageFile.CopyToAsync(memoryStream);
                    byte[] imageBytes = memoryStream.ToArray();
                    data.ImageBase64 = Convert.ToBase64String(imageBytes);
                }

                var jsonContent = new StringContent(
                    JsonConvert.SerializeObject(data),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PostAsync("/api/Phim/Create", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true });
                }
                else
                {
                    string apiError = await response.Content.ReadAsStringAsync();
                    return BadRequest(new { message = $"Lỗi từ API: {apiError}" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Đã xảy ra lỗi trong quá trình xử lý: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPhim(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/Phim/GetByID?id={id}");

                if (!response.IsSuccessStatusCode)
                {
                    string apiError = await response.Content.ReadAsStringAsync();
                    return BadRequest(new { message = $"Lỗi từ API: {apiError}" });
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var phim = JsonConvert.DeserializeObject<PhimViewModel>(jsonResponse);

                if (phim == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy thông tin phim." });
                }

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        phim.Id,
                        phim.TenPhim,
                        phim.TheLoai,
                        phim.ThoiLuong,
                        phim.DaoDien,
                        phim.GioiHanDoTuoi,
                        phim.NgayKhoiChieu,
                        phim.NgayKetThuc,
                        phim.SoSuatChieu,
                        phim.TrangThai,
                        phim.MoTa,
                        HinhAnh = phim.ImageBase64,
                        phim.HinhAnhs,
                        phim.LichChieus,
                        phim.Videos
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, [FromForm] PhimViewModel data)
        {
            var errors = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(data.TenPhim))
            {
                errors["TenPhim"] = "Tên phim không được để trống.";
            }

            if (string.IsNullOrEmpty(data.TheLoai))
            {
                errors["TheLoai"] = "Thể loại không được để trống.";
            }

            if (data.ThoiLuong <= 0)
            {
                errors["ThoiLuong"] = "Thời lượng phải lớn hơn 0 phút.";
            }

            if (string.IsNullOrEmpty(data.DaoDien))
            {
                errors["DaoDien"] = "Đạo diễn không được để trống.";
            }

            if (data.GioiHanDoTuoi <= 0)
            {
                errors["GioiHanDoTuoi"] = "Giới hạn độ tuổi phải lớn hơn 0.";
            }

            if (data.NgayKhoiChieu == null || data.NgayKhoiChieu < DateOnly.FromDateTime(DateTime.Today))
            {
                errors["NgayKhoiChieu"] = "Ngày khởi chiếu không hợp lệ.";
            }

            if (data.NgayKetThuc == null || data.NgayKhoiChieu > data.NgayKetThuc)
            {
                errors["NgayKetThuc"] = "Ngày kết thúc phải sau ngày khởi chiếu.";
            }

            if (errors.Count > 0)
            {
                return BadRequest(errors);
            }

            try
            {
                if (data.ImageFile != null && data.ImageFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await data.ImageFile.CopyToAsync(memoryStream);
                        byte[] imageBytes = memoryStream.ToArray();
                        data.ImageBase64 = Convert.ToBase64String(imageBytes);
                    }
                }
                else
                {
                    var existingPhim = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/Phim/GetByID?id={id}");
                    var jsonResponse = await existingPhim.Content.ReadAsStringAsync();
                    var currentPhim = JsonConvert.DeserializeObject<PhimViewModel>(jsonResponse);
                    data.ImageBase64 = currentPhim.ImageBase64;
                }

                data.Id = id;
                var jsonContent = new StringContent(
                    JsonConvert.SerializeObject(data),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PutAsync($"/api/Phim/Update", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true });
                }
                else
                {
                    string apiError = await response.Content.ReadAsStringAsync();
                    return BadRequest(new { message = $"Lỗi từ API: {apiError}" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Đã xảy ra lỗi trong quá trình xử lý: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            string url = $"{_httpClient.BaseAddress}/Phim/Remove?ID={Uri.EscapeDataString(id.ToString())}";
            HttpResponseMessage response = _httpClient.DeleteAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "Xóa phim thành công!" });
            }
            else
            {
                string apiError = await response.Content.ReadAsStringAsync();
                return BadRequest(new { message = $"Lỗi từ API: {apiError}" });
            }
        }
    }
}