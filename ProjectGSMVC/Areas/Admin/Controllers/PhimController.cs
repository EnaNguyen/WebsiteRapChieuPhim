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
using ProjectGSMAUI.Api.Modal;
using static System.Net.WebRequestMethods;
using System.Drawing;

namespace ProjectGSMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PhimController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl = "https://localhost:7141/api/Phim/GetAll";
        private readonly string _baseTheLoaiApiUrl = "https://localhost:7141/api/TheLoaiPhim";
        public PhimController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string searching = null)
        {
            List<PhimView> dsPhim = new List<PhimView>();
            List<TheLoaiPhim> dsTheLoai = new List<TheLoaiPhim>();
            try
            {
                string url = string.IsNullOrEmpty(searching) ? $"{_baseApiUrl}"
                      : $"{_baseApiUrl}?Name={Uri.EscapeDataString(searching)}";
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    dsPhim = JsonConvert.DeserializeObject<List<PhimView>>(data);
                }
                else
                {
                    string apiError = await response.Content.ReadAsStringAsync();
                    return BadRequest(new { message = $"Lỗi từ API: {apiError}" });
                }
                string url2 = "https://localhost:7141/api/TheLoaiPhim";
                HttpResponseMessage response1 = await _httpClient.GetAsync(url2);
                if (response1.IsSuccessStatusCode)
                {
                    string data = await response1.Content.ReadAsStringAsync();
                    dsTheLoai = JsonConvert.DeserializeObject<List<TheLoaiPhim>>(data);
                }
                else
                {
                    string apiError = await response1.Content.ReadAsStringAsync();
                    return BadRequest(new { message = $"Lỗi từ API: {apiError}" });
                }
                ViewData["ListTheLoai"] = dsTheLoai;
                ViewData["ListPhim"] = dsPhim;
            }
            catch (Exception ex)
            {

            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UploadFiles(IFormFile files)
        {
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> CreateMovie([FromBody] CreateMovie model)
        {
            var errors = new Dictionary<string, string>();
            if (string.IsNullOrWhiteSpace(model.PhimDatas.TenPhim))
            {
                errors["TenPhim"] = "Tên phim không được để trống.";
            }
            
            if (model.PhimDatas.ThoiLuong==null||model.PhimDatas.ThoiLuong <= 0)
            {
                errors["ThoiLuong"] = "Thời lượng phim phải lớn hơn 0.";
            }
            if (string.IsNullOrWhiteSpace(model.PhimDatas.DaoDien))
            {
                errors["DaoDien"] = "Đạo diễn không được để trống";
            }            
            if (model.PhimDatas.SoSuatChieu==null || model.PhimDatas.SoSuatChieu < 0 || model.PhimDatas.SoSuatChieu > 42)
            {
                errors["SoSuatChieu"] = "Số xuất chiếu phải lớn hơn 0 và nhỏ hơn 42 trong 1 tuần";
            }
            if (model.PhimDatas.NgayKhoiChieu == null||model.PhimDatas.NgayKhoiChieu<DateOnly.FromDateTime(DateTime.Now))
            {
                errors["NgayKhoiChieu"] = "Ngày khởi chiếu phải bắt đầu sau hôm nay";
            }
            if(model.PhimDatas.NgayKetThuc ==null ||model.PhimDatas.NgayKetThuc < model.PhimDatas.NgayKhoiChieu)        
            {
                errors["NgayKetThuc"] = "Phim phải chiếu được ít nhất 1 ngày";
            }               
            if (model.Videos.Count < 1)
            {
                errors["Videos"] = "Video không được để trống";
            }
            if (string.IsNullOrWhiteSpace(model.PhimDatas.MoTa))
            {
                errors["MoTa"] = "Đạo diễn không được để trống";
            }
            if (errors.Count > 0)
            {
                return BadRequest(errors);
            }
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://localhost:7141/api/Phim/CreatePhim", content);

            if (response.IsSuccessStatusCode)
            {
                return Ok("Thêm phim thành công!");
            }
            else
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                return BadRequest($"Lỗi API: {errorResponse}");
            }
        }



        //[HttpGet]
        //public async Task<IActionResult> Index(string searching = null)
        //{
        //    List<PhimModel> danhSachPhim = new List<PhimModel>();
        //    List<TheLoaiPhimViewModel> danhSachTheLoai = new List<TheLoaiPhimViewModel>();

        //    try
        //    {
        //        string url = string.IsNullOrEmpty(searching)
        //            ? $"{_baseApiUrl}"
        //            : $"{_baseApiUrl}?Name={Uri.EscapeDataString(searching)}";

        //        HttpResponseMessage response = await _httpClient.GetAsync(url);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            string data = await response.Content.ReadAsStringAsync();
        //            danhSachPhim = JsonConvert.DeserializeObject<List<PhimModel>>(data);
        //        }
        //        else
        //        {
        //            string apiError = await response.Content.ReadAsStringAsync();
        //            return BadRequest(new { message = $"Lỗi từ API: {apiError}" });
        //        }
        //        response = await _httpClient.GetAsync($"{_baseTheLoaiApiUrl}");
        //        if (response.IsSuccessStatusCode)
        //        {
        //            string data = await response.Content.ReadAsStringAsync();
        //            danhSachTheLoai = JsonConvert.DeserializeObject<List<TheLoaiPhimViewModel>>(data);
        //        }
        //        else
        //        {
        //            string apiError = await response.Content.ReadAsStringAsync();
        //            return BadRequest(new { message = $"Lỗi từ API: {apiError}" });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { message = $"Đã xảy ra lỗi: {ex.Message}" });
        //    }
        //    ViewData["ListTheLoai"] = danhSachTheLoai;
        //    ViewData["ListPhim"] = danhSachPhim;
        //    ViewData["Searching"] = searching;
        //    return View();
        //}

        //[HttpPost]
        //public async Task<IActionResult> Create([FromForm] PhimModel model)
        //{
        //    try
        //    {
        //        var errors = new Dictionary<string, string>();
        //        if (string.IsNullOrWhiteSpace(model.TenPhim))
        //        {
        //            errors["TenPhim"] = "Tên phim không được để trống.";
        //        }

        //        if (model.ThoiLuong <= 0)
        //        {
        //            errors["ThoiLuong"] = "Thời lượng phim phải lớn hơn 0.";
        //        }
        //        if (model.GioiHanDoTuoi <= 0)
        //        {
        //            errors["GioiHanDoTuoi"] = "Giới hạn độ tuổi phải lớn hơn 0.";
        //        }
        //        if (model.SoXuatChieu <= 0)
        //        {
        //            errors["SoXuatChieu"] = "Số xuất chiếu phải lớn hơn 0.";
        //        }
        //        if (model.ImageFiles == null || model.ImageFiles.Count == 0)
        //        {
        //            errors["ImageFiles"] = "Hình ảnh không được để trống.";
        //        }
        //        if (errors.Count > 0)
        //        {
        //            return BadRequest(errors);
        //        }
        //        var phimMoi = new Phim()
        //        {
        //            Id = model.Id,
        //            TenPhim = model.TenPhim,
        //            TheLoai = model.TheLoai,
        //            ThoiLuong = model.ThoiLuong,
        //            DaoDien = model.DaoDien,
        //            GioiHanDoTuoi = model.GioiHanDoTuoi,
        //            NgayKhoiChieu = model.NgayKhoiChieu,
        //            NgayKetThuc = model.NgayKetThuc,
        //            SoSuatChieu = model.SoXuatChieu,
        //            TrangThai = model.TrangThai,
        //            MoTa = model.MoTa,

        //        };
        //        if (model.ImageFiles != null && model.ImageFiles.Any())
        //        {
        //            phimMoi.ImageData = new List<IFormFile>();
        //            foreach (var file in model.ImageFiles)
        //            {
        //                phimMoi.ImageData.Add(file);
        //            }
        //        }


        //        var json = JsonConvert.SerializeObject(phimMoi, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        //        var content = new StringContent(json, Encoding.UTF8, "application/json");

        //        var response = await _httpClient.PostAsync(_baseApiUrl, content);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            return Json(new { success = true });
        //        }
        //        else
        //        {
        //            var error = await response.Content.ReadAsStringAsync();
        //            return Json(new { success = false, message = error });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = $"Đã xảy ra lỗi trong quá trình thêm mới phim. Lỗi: {ex.Message}" });
        //    }
        //}

        [HttpGet]
        public async Task<IActionResult> GetPhim(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"https://localhost:7141/api/Phim/GetById?id={id}");
                if (!response.IsSuccessStatusCode)
                {
                    return Json(new { success = false, message = "Không tìm thấy phim." });
                }
                var content = await response.Content.ReadAsStringAsync();
                var phim = JsonConvert.DeserializeObject<DetailMovie>(content);
                return Json(new { success = true, data = phim });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Đã xảy ra lỗi trong quá trình lấy thông tin phim. Lỗi: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] UpdateMovieRequest Abc)
        {
            var errors = new Dictionary<string, string>();
            var model = Abc.Model;
            if (string.IsNullOrWhiteSpace(model.PhimDatas.TenPhim))
            {
                errors["TenPhim"] = "Tên phim không được để trống.";
            }

            if (model.PhimDatas.ThoiLuong == null || model.PhimDatas.ThoiLuong <= 0)
            {
                errors["ThoiLuong"] = "Thời lượng phim phải lớn hơn 0.";
            }
            if (string.IsNullOrWhiteSpace(model.PhimDatas.DaoDien))
            {
                errors["DaoDien"] = "Đạo diễn không được để trống";
            }
            if (model.PhimDatas.SoSuatChieu == null || model.PhimDatas.SoSuatChieu < 0 || model.PhimDatas.SoSuatChieu > 42)
            {
                errors["SoSuatChieu"] = "Số xuất chiếu phải lớn hơn 0 và nhỏ hơn 42 trong 1 tuần";
            }
            if (model.PhimDatas.NgayKhoiChieu == null || model.PhimDatas.NgayKhoiChieu < DateOnly.FromDateTime(DateTime.Now))
            {
                errors["NgayKhoiChieu"] = "Ngày khởi chiếu phải bắt đầu sau hôm nay";
            }
            if (model.PhimDatas.NgayKetThuc == null || model.PhimDatas.NgayKetThuc < model.PhimDatas.NgayKhoiChieu)
            {
                errors["NgayKetThuc"] = "Phim phải chiếu được ít nhất 1 ngày";
            }
            if (model.Videos.Count < 1)
            {
                errors["Videos"] = "Video không được để trống";
            }
            if (string.IsNullOrWhiteSpace(model.PhimDatas.MoTa))
            {
                errors["MoTa"] = "Đạo diễn không được để trống";
            }           
            if (errors.Count > 0)
            {
                return BadRequest(errors);
            }
            if (model.HinhAnhs.Count>0)
            {
                foreach (var image in model.HinhAnhs)
                {
                    if(image.ImageData.StartsWith("data:"))
                    {
                        int commaIndex = image.ImageData.IndexOf(",");
                        if (commaIndex >= 0)
                        {
                            string result = image.ImageData.Substring(commaIndex + 1);
                            image.ImageData = result;
                        }
                    }    
                }
            }            
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"https://localhost:7141/api/Phim/Update?Id={Abc.IdPhim}", content);

            if (response.IsSuccessStatusCode)
            {
                return Ok("Sửa phim thành công!");
            }
            else
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                return BadRequest($"Lỗi API: {errorResponse}");
            }
        }

        public class UpdateMovieRequest
        {
            public CreateMovie Model { get; set; }
            public int IdPhim { get; set; }
        }
        //[HttpPost]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    try
        //    {
        //        var response = await _httpClient.DeleteAsync($"{_baseApiUrl}/{id}");
        //        if (response.IsSuccessStatusCode)
        //        {
        //            return Json(new { success = true, message = "Xóa phim thành công." });
        //        }
        //        else
        //        {
        //            var error = await response.Content.ReadAsStringAsync();
        //            return Json(new { success = false, message = error });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = $"Đã xảy ra lỗi trong quá trình xóa phim. Lỗi: {ex.Message}" });
        //    }
        //}
        //[HttpGet]
        //public async Task<IActionResult> Index(string searching = null)
        //{

        //}
    }
}