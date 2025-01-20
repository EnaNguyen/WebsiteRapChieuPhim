using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjectGSMAUI.Api.Data.Entities;
using ProjectGSMVC.Areas.Admin.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;


namespace ProjectGSMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PhimController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl = "https://localhost:7141/api/Phim";
        private readonly string _baseTheLoaiApiUrl = "https://localhost:7141/api/TheLoaiPhim";
        public PhimController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string searching = null)
        {
            List<PhimModel> danhSachPhim = new List<PhimModel>();
            List<TheLoaiPhimViewModel> danhSachTheLoai = new List<TheLoaiPhimViewModel>();

            try
            {
                string url = string.IsNullOrEmpty(searching)
                    ? $"{_baseApiUrl}"
                    : $"{_baseApiUrl}?Name={Uri.EscapeDataString(searching)}";

                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    danhSachPhim = JsonConvert.DeserializeObject<List<PhimModel>>(data);
                }
                else
                {
                    string apiError = await response.Content.ReadAsStringAsync();
                    return BadRequest(new { message = $"Lỗi từ API: {apiError}" });
                }
                response = await _httpClient.GetAsync($"{_baseTheLoaiApiUrl}");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    danhSachTheLoai = JsonConvert.DeserializeObject<List<TheLoaiPhimViewModel>>(data);
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
            ViewData["ListTheLoai"] = danhSachTheLoai;
            ViewData["ListPhim"] = danhSachPhim;
            ViewData["Searching"] = searching;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] PhimModel model)
        {
            try
            {
                var errors = new Dictionary<string, string>();
                if (string.IsNullOrWhiteSpace(model.TenPhim))
                {
                    errors["TenPhim"] = "Tên phim không được để trống.";
                }

                if (model.ThoiLuong <= 0)
                {
                    errors["ThoiLuong"] = "Thời lượng phim phải lớn hơn 0.";
                }
                if (model.GioiHanDoTuoi <= 0)
                {
                    errors["GioiHanDoTuoi"] = "Giới hạn độ tuổi phải lớn hơn 0.";
                }
                if (model.SoXuatChieu <= 0)
                {
                    errors["SoXuatChieu"] = "Số xuất chiếu phải lớn hơn 0.";
                }
                if (model.ImageFiles == null || model.ImageFiles.Count == 0)
                {
                    errors["ImageFiles"] = "Hình ảnh không được để trống.";
                }
                if (errors.Count > 0)
                {
                    return BadRequest(errors);
                }
                var phimMoi = new Phim()
                {
                    Id = model.Id,
                    TenPhim = model.TenPhim,
                    TheLoai = model.TheLoai,
                    ThoiLuong = model.ThoiLuong,
                    DaoDien = model.DaoDien,
                    GioiHanDoTuoi = model.GioiHanDoTuoi,
                    NgayKhoiChieu = model.NgayKhoiChieu,
                    NgayKetThuc = model.NgayKetThuc,
                    SoXuatChieu = model.SoXuatChieu,
                    TrangThai = model.TrangThai,
                    MoTa = model.MoTa,

                };
                if (model.ImageFiles != null && model.ImageFiles.Any())
                {
                    phimMoi.ImageFiles = new List<IFormFile>();
                    foreach (var file in model.ImageFiles)
                    {
                        phimMoi.ImageFiles.Add(file);
                    }
                }


                var json = JsonConvert.SerializeObject(phimMoi, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
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
                return Json(new { success = false, message = $"Đã xảy ra lỗi trong quá trình thêm mới phim. Lỗi: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPhim(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseApiUrl}/GetById?id={id}");
                if (!response.IsSuccessStatusCode)
                {
                    return Json(new { success = false, message = "Không tìm thấy phim." });
                }
                var content = await response.Content.ReadAsStringAsync();
                var phim = JsonConvert.DeserializeObject<Phim>(content);
                return Json(new { success = true, data = phim });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Đã xảy ra lỗi trong quá trình lấy thông tin phim. Lỗi: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromForm] PhimModel model)
        {
            var errors = new Dictionary<string, string>();
            if (string.IsNullOrWhiteSpace(model.TenPhim))
            {
                errors["TenPhim"] = "Tên phim không được để trống.";
            }

            if (model.ThoiLuong <= 0)
            {
                errors["ThoiLuong"] = "Thời lượng phim phải lớn hơn 0.";
            }
            if (model.GioiHanDoTuoi <= 0)
            {
                errors["GioiHanDoTuoi"] = "Giới hạn độ tuổi phải lớn hơn 0.";
            }
            if (model.SoXuatChieu <= 0)
            {
                errors["SoXuatChieu"] = "Số xuất chiếu phải lớn hơn 0.";
            }
            if (errors.Count > 0)
            {
                return BadRequest(errors);
            }
            try
            {
                var phimMoi = new Phim()
                {
                    Id = model.Id,
                    TenPhim = model.TenPhim,
                    TheLoai = model.TheLoai,
                    ThoiLuong = model.ThoiLuong,
                    DaoDien = model.DaoDien,
                    GioiHanDoTuoi = model.GioiHanDoTuoi,
                    NgayKhoiChieu = model.NgayKhoiChieu,
                    NgayKetThuc = model.NgayKetThuc,
                    SoXuatChieu = model.SoXuatChieu,
                    TrangThai = model.TrangThai,
                    MoTa = model.MoTa
                };
                if (model.ImageFiles != null && model.ImageFiles.Any())
                {
                    phimMoi.ImageFiles = new List<IFormFile>();
                    foreach (var file in model.ImageFiles)
                    {
                        phimMoi.ImageFiles.Add(file);
                    }
                }

                var json = JsonConvert.SerializeObject(phimMoi, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync(_baseApiUrl + "/UpdatePhim", content);
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
                return Json(new { success = false, message = $"Đã xảy ra lỗi trong quá trình cập nhật phim. Lỗi: {ex.Message}" });
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
                    return Json(new { success = true, message = "Xóa phim thành công." });
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return Json(new { success = false, message = error });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Đã xảy ra lỗi trong quá trình xóa phim. Lỗi: {ex.Message}" });
            }
        }
    }
}