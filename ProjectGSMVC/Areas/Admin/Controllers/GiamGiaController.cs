using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjectGSMVC.Areas.Admin.ViewModel;
using System.Net.Http;
using System.IO;
using ProjectGSMVC.Areas.Admin.Models;
using System.Text;
using ProjectGSMAUI.Api.Modal;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using ProjectGSMAUI.Api.Data.Entities;
namespace ProjectGSMVC.Areas.Admin.Controllers

{
	[Area("Admin")]
	public class GiamGiaController : Controller
	{
		Uri baseAddress = new Uri("https://localhost:7141/api");
		private readonly HttpClient _httpClient;
		public GiamGiaController()
		{
			_httpClient = new HttpClient();
			_httpClient.BaseAddress = baseAddress;
		}
        [HttpGet]
        public IActionResult Index(string searching = null)
        {
            List<GiamGiaViewModel> giamGiaList = new List<GiamGiaViewModel>();
            string url = string.IsNullOrEmpty(searching)
                ? $"{_httpClient.BaseAddress}/GiamGia/GetAll"
                : $"{_httpClient.BaseAddress}/GiamGia/GetAll?Name={Uri.EscapeDataString(searching)}";

            HttpResponseMessage response = _httpClient.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                giamGiaList = JsonConvert.DeserializeObject<List<GiamGiaViewModel>>(data);
            }
            else
            {
                string apiError =  response.Content.ToString();
                return BadRequest(new { message = $"Lỗi từ API: {apiError}" });
            }
            ViewData["ListGiamGia"] = giamGiaList;
            ViewData["Searching"] = searching; // Lưu từ khóa tìm kiếm để hiển thị lại trên giao diện
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] GiamGiaModel data)
        {
            var errors = new Dictionary<string, string>();

            // Kiểm tra các trường dữ liệu
            if (string.IsNullOrEmpty(data.TenGiamGia))
            {
                errors["TenGiamGia"] = "Tên giảm giá không được để trống.";
            }

            if (data.NgayBatDau == default || data.NgayBatDau < DateOnly.FromDateTime(DateTime.Today))
            {
                errors["NgayBatDau"] = "Ngày bắt đầu không được nhỏ hơn ngày hiện tại.";
            }

            if (data.NgayKetThuc == default || data.NgayBatDau > data.NgayKetThuc)
            {
                errors["NgayKetThuc"] = "Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu.";
            }

            if (data.GiaTri <= 0 || data.GiaTri > 100)
            {
                errors["GiaTri"] = "Giá trị giảm giá phải nằm trong khoảng 1% đến 100%.";
            }

            if (data.SoLuong <= 0)
            {
                errors["SoLuong"] = "Số lượng phải lớn hơn 0.";
            }

            if (data.ImageFile == null || data.ImageFile.Length == 0)
            {
                errors["ImageFile"] = "Hình ảnh không được để trống.";
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

                var giamGiaRequest = new GiamGiaRequest
                {
                    TenGiamGia = data.TenGiamGia,
                    NgayBatDau = data.NgayBatDau,
                    NgayKetThuc = data.NgayKetThuc,
                    MoTa = data.MoTa,
                    GiaTri = data.GiaTri,
                    SoLuong = data.SoLuong,
                    ImageFile = data.ImageBase64
                };

                var jsonContent = new StringContent(
                    System.Text.Json.JsonSerializer.Serialize(giamGiaRequest, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PostAsync("/api/GiamGia/Create", jsonContent);

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
        public async Task<IActionResult> GetGiamGia(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync(_httpClient.BaseAddress+"/GiamGia/GetByID?"+ $"id={id}");

                if (!response.IsSuccessStatusCode)
                {
                    string apiError = await response.Content.ReadAsStringAsync();
                    return BadRequest(new { message = $"Lỗi từ API: {apiError}" });
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();


                var giamGia = JsonConvert.DeserializeObject<ActiveGiamGia>(jsonResponse);

                if (giamGia == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy dữ liệu giảm giá." });
                }
                List<Coupon> CouponList = new List<Coupon>();
                string url =  $"{_httpClient.BaseAddress}/Voucher/GetByGiamGia?Id={Uri.EscapeDataString(giamGia.MaGiamGia.ToString())}";

                HttpResponseMessage response1 = _httpClient.GetAsync(url).Result;
                if (response1.IsSuccessStatusCode)
                {
                    string data1 = response1.Content.ReadAsStringAsync().Result;
                    CouponList = JsonConvert.DeserializeObject<List<Coupon>>(data1);
                }
                return Json(new
                {
                    success = true,
                    data = new
                    {
                        giamGia.TenGiamGia,
                        giamGia.NgayBatDau,
                        giamGia.NgayKetThuc,
                        giamGia.MoTa,
                        giamGia.GiaTri,
                        giamGia.SoLuongDaDung,
                        giamGia.SoLuongConLai,
                        HinhAnh = giamGia.HinhAnh != null ? Convert.ToBase64String(giamGia.HinhAnh) : null,
                        couponList = CouponList
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, [FromForm] GiamGiaModel data)
        {
            var errors = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(data.TenGiamGia))
            {
                errors["TenGiamGia"] = "Tên giảm giá không được để trống.";
            }

            if (data.NgayBatDau == default || data.NgayBatDau < DateOnly.FromDateTime(DateTime.Today))
            {
                errors["NgayBatDau"] = "Ngày bắt đầu không được nhỏ hơn ngày hiện tại.";
            }

            if (data.NgayKetThuc == default || data.NgayBatDau > data.NgayKetThuc)
            {
                errors["NgayKetThuc"] = "Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu.";
            }

            if (data.GiaTri <= 0 || data.GiaTri > 100)
            {
                errors["GiaTri"] = "Giá trị giảm giá phải nằm trong khoảng 1% đến 100%.";
            }

            if (data.SoLuong <= 0)
            {
                errors["SoLuong"] = "Số lượng phải lớn hơn 0.";
            }
            if (errors.Count > 0)
            {
                return BadRequest(errors);
            }
            try
            {
                GiamGiaRequest giamGiaRequest = new GiamGiaRequest();
                if (data.ImageFile == null || data.ImageFile.Length == 0)
                {
                    var DuLieu = await _httpClient.GetAsync(_httpClient.BaseAddress + "/GiamGia/GetByID?" + $"id={id}");
                    var jsonResponse = await DuLieu.Content.ReadAsStringAsync();

                    var giamGia = JsonConvert.DeserializeObject<ActiveGiamGia>(jsonResponse);
                    var Temp = new GiamGiaRequest
                    {
                        TenGiamGia = data.TenGiamGia,
                        NgayBatDau = data.NgayBatDau,
                        NgayKetThuc = data.NgayKetThuc,
                        MoTa = data.MoTa,
                        GiaTri = data.GiaTri,
                        SoLuong = data.SoLuong,
                        ImageFile = Convert.ToBase64String(giamGia.HinhAnh)
                    };
                    giamGiaRequest = Temp;
                }
                else
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
                    var Temp = new GiamGiaRequest
                    {
                        TenGiamGia = data.TenGiamGia,
                        NgayBatDau = data.NgayBatDau,
                        NgayKetThuc = data.NgayKetThuc,
                        MoTa = data.MoTa,
                        GiaTri = data.GiaTri,
                        SoLuong = data.SoLuong,
                        ImageFile = data.ImageBase64
                    };
                    giamGiaRequest = Temp;
                }            
                var updateRequest = new GiamGiaEdit
                {
                    Id = id,
                    GiamGiaRequest = giamGiaRequest
                };

                var jsonContent = new StringContent(
                    System.Text.Json.JsonSerializer.Serialize(updateRequest, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PutAsync($"/api/GiamGia/Update", jsonContent);

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
        public async Task<IActionResult> Xoa(int id)
        {
            string url = $"{_httpClient.BaseAddress}/GiamGia/Remove?ID={Uri.EscapeDataString(id.ToString())}";
            HttpResponseMessage response = _httpClient.DeleteAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "Xóa giảm giá thành công!" });
            }
            else
            {
                string apiError = await response.Content.ReadAsStringAsync();
                return BadRequest(new { message = $"Lỗi từ API: {apiError}" });
            }
        }

    }
}
