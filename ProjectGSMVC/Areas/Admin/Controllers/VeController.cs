using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjectGSMAUI.Api.Data.Entities;
using ProjectGSMAUI.MVC.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProjectGSMAUI.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class VeController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl = "https://localhost:7141/api/Ve";
        private readonly string _basePhongApiUrl = "https://localhost:7141/api/Phong";
        private readonly string _basePhimApiUrl = "https://localhost:7141/api/Phim";

        public VeController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string searching = null, DateTime? filterDate = null, int page = 1)
        {
            try
            {
                var (phongList, phimList) = await GetPhongAndPhimLists();

                ViewData["PhongList"] = phongList;
                ViewData["PhimList"] = phimList;
                ViewData["Searching"] = searching;
                ViewData["FilterDate"] = filterDate;
                ViewData["CurrentPage"] = page;
                return View();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Đã xảy ra lỗi: {ex.Message}" });
            }
        }

        private async Task<(List<Phong>, List<Phim>)> GetPhongAndPhimLists()
        {
            List<Phong> phongList = new List<Phong>();
            List<Phim> phimList = new List<Phim>();
            try
            {
                HttpResponseMessage phongResponse = await _httpClient.GetAsync(_basePhongApiUrl);
                if (phongResponse.IsSuccessStatusCode)
                {
                    string phongData = await phongResponse.Content.ReadAsStringAsync();
                    phongList = JsonConvert.DeserializeObject<List<Phong>>(phongData);
                }
                else
                {
                    throw new Exception($"Lỗi khi lấy danh sách phòng từ API: {await phongResponse.Content.ReadAsStringAsync()}");
                }

                HttpResponseMessage phimResponse = await _httpClient.GetAsync(_basePhimApiUrl + "/GetAll");
                if (phimResponse.IsSuccessStatusCode)
                {
                    string phimData = await phimResponse.Content.ReadAsStringAsync();
                    phimList = JsonConvert.DeserializeObject<List<Phim>>(phimData);
                }
                else
                {
                    throw new Exception($"Lỗi khi lấy danh sách phim từ API: {await phimResponse.Content.ReadAsStringAsync()}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Đã xảy ra lỗi trong quá trình lấy thông tin phòng và phim: {ex.Message}");
            }
            return (phongList, phimList);
        }

        [HttpGet]
        public async Task<IActionResult> GetFilteredTickets([FromQuery] string searching = null, [FromQuery] DateTime? filterDate = null, [FromQuery] int page = 1)
        {
            List<Ve> danhSachVe = new List<Ve>();
            List<Phong> phongList = new List<Phong>();
            List<Phim> phimList = new List<Phim>();
            try
            {
                string url = $"{_baseApiUrl}?";

                if (!string.IsNullOrEmpty(searching))
                {
                    url += $"maVe={Uri.EscapeDataString(searching)}&";
                }

                if (filterDate.HasValue)
                {
                    url += $"filterDate={filterDate.Value.ToString("yyyy-MM-dd")}&";
                }

                url += $"page={page}";

                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    danhSachVe = JsonConvert.DeserializeObject<List<Ve>>(data);
                }
                else
                {
                    string apiError = await response.Content.ReadAsStringAsync();
                    return Json(new { success = false, message = $"Lỗi từ API: {apiError}" });
                }

                (phongList, phimList) = await GetPhongAndPhimLists();
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Đã xảy ra lỗi: {ex.Message}" });
            }

            int pageSize = 10;
            int totalPages = (int)Math.Ceiling((double)(danhSachVe?.Count ?? 0) / pageSize);
            var paginationHtml = GeneratePaginationHtml(page, totalPages);

            return Json(new { success = true, tickets = danhSachVe, paginationHtml, phongList = phongList, phimList = phimList });
        }

        private string GeneratePaginationHtml(int currentPage, int totalPages)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("<ul class='pagination'>");

            if (currentPage > 1)
            {
                stringBuilder.Append($"<li class='page-item'><a class='page-link' href='javascript:void(0);' data-page='{currentPage - 1}' aria-label='Previous'><span aria-hidden='true'>«</span></a></li>");
            }
            for (int i = 1; i <= totalPages; i++)
            {
                stringBuilder.Append($"<li class='page-item {(i == currentPage ? "active" : "")}'><a class='page-link' href='javascript:void(0);' data-page='{i}'>{i}</a></li>");
            }
            if (currentPage < totalPages)
            {
                stringBuilder.Append($"<li class='page-item'><a class='page-link' href='javascript:void(0);' data-page='{currentPage + 1}' aria-label='Next'><span aria-hidden='true'>»</span></a></li>");
            }
            stringBuilder.Append("</ul>");
            return stringBuilder.ToString();
        }

        [HttpGet]
        public async Task<IActionResult> GetPhongList()
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(_basePhongApiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    var phongList = JsonConvert.DeserializeObject<List<Phong>>(data);
                    return Json(phongList);
                }
                else
                {
                    return Json(new { success = false, message = $"Lỗi khi lấy danh sách phòng từ API: {await response.Content.ReadAsStringAsync()}" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Đã xảy ra lỗi trong quá trình lấy thông tin phòng. Lỗi: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPhimList()
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(_basePhimApiUrl + "/GetAll");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    var phimList = JsonConvert.DeserializeObject<List<Phim>>(data);
                    return Json(phimList);
                }
                else
                {
                    return Json(new { success = false, message = $"Lỗi khi lấy danh sách phim từ API: {await response.Content.ReadAsStringAsync()}" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Đã xảy ra lỗi trong quá trình lấy thông tin phim. Lỗi: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VeModel model)
        {
            var errors = new Dictionary<string, string>();
            if (model == null)
            {
                model = new VeModel();
            }

            if (model.MaLichChieu.HasValue && model.MaLichChieu.Value <= 0)
            {
                errors["MaLichChieu"] = "Mã lịch chiếu phải lớn hơn 0.";
            }
            if (model.MaPhim.HasValue && model.MaPhim.Value == 0)
            {
                errors["MaPhim"] = "Mã phim không được để trống.";
            }
            if (model.MaPhong.HasValue && model.MaPhong.Value == 0)
            {
                errors["MaPhong"] = "Mã phòng không được để trống.";
            }
            if (string.IsNullOrWhiteSpace(model.MaGhe))
            {
                errors["MaGhe"] = "Mã ghế không được để trống.";
            }
            else
            {
                string ghePattern = @"^[a-lA-L]([1-9]|1[0-6])$";
                if (!Regex.IsMatch(model.MaGhe, ghePattern))
                {
                    errors["MaGhe"] = "Mã ghế phải theo form: chữ cái (a-l) + số (1-16).";
                }
            }

            if (errors.Count > 0)
            {
                return BadRequest(errors);
            }

            try
            {
                var veMoi = new Ve()
                {
                    MaVe = model.MaVe,
                    MaLichChieu = model.MaLichChieu,
                    MaPhong = model.MaPhong,
                    MaPhim = model.MaPhim,
                    TinhTrang = 1,
                    MaGhe = model.MaGhe,
                    ThoiGianTao = model.ThoiGianTao
                };
                var json = JsonConvert.SerializeObject(veMoi);
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
                return Json(new { success = false, message = $"Đã xảy ra lỗi trong quá trình thêm mới vé. Lỗi: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetVe(string id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseApiUrl}/GetById?id={id}");
                if (!response.IsSuccessStatusCode)
                {
                    return Json(new { success = false, message = "Không tìm thấy vé." });
                }

                var content = await response.Content.ReadAsStringAsync();
                var ve = JsonConvert.DeserializeObject<Ve>(content);
                return Json(new { success = true, data = ve });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Đã xảy ra lỗi trong quá trình lấy thông tin vé. Lỗi: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] VeModel model)
        {
            var errors = new Dictionary<string, string>();
            if (model == null)
            {
                model = new VeModel();
            }

            if (model.MaLichChieu.HasValue && model.MaLichChieu.Value <= 0)
            {
                errors["MaLichChieu"] = "Mã lịch chiếu phải lớn hơn 0.";
            }
            if (model.MaPhim.HasValue && model.MaPhim.Value == 0)
            {
                errors["MaPhim"] = "Mã phim không được để trống.";
            }
            if (model.MaPhong.HasValue && model.MaPhong.Value == 0)
            {
                errors["MaPhong"] = "Mã phòng không được để trống.";
            }
            if (string.IsNullOrWhiteSpace(model.MaGhe))
            {
                errors["MaGhe"] = "Mã ghế không được để trống.";
            }
            else
            {
                string ghePattern = @"^[a-lA-L]([1-9]|1[0-6])$";
                if (!Regex.IsMatch(model.MaGhe, ghePattern))
                {
                    errors["MaGhe"] = "Mã ghế phải theo form: chữ cái (a-l) + số (1-16).";
                }
            }

            if (errors.Count > 0)
            {
                return BadRequest(errors);
            }
            try
            {
                var veMoi = new Ve()
                {
                    MaVe = model.MaVe,
                    MaLichChieu = model.MaLichChieu,
                    MaPhong = model.MaPhong,
                    MaPhim = model.MaPhim,
                    TinhTrang = model.TinhTrang.Value,
                    MaGhe = model.MaGhe,
                    ThoiGianTao = model.ThoiGianTao
                };
                var json = JsonConvert.SerializeObject(veMoi);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync(_baseApiUrl + "/UpdateVe/" + model.MaVe, content);

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
                return Json(new { success = false, message = $"Đã xảy ra lỗi trong quá trình cập nhật vé. Lỗi: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseApiUrl}/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Xóa vé thành công." });
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return Json(new { success = false, message = error });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Đã xảy ra lỗi trong quá trình xóa vé. Lỗi: {ex.Message}" });
            }
        }
    }
}