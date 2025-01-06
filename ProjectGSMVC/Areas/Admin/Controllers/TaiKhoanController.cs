using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjectGSMVC.Areas.Admin.ViewModels;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProjectGSMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TaiKhoanController : Controller
    {
        private readonly HttpClient _httpClient;

        public TaiKhoanController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public IActionResult Index()
        {
            return View();
        }
        // Danh sách khách hàng
        public async Task<IActionResult> Customer()
        {
            var response = await _httpClient.GetAsync("http://localhost:5030/api/TaiKhoan");
            if (!response.IsSuccessStatusCode) return NotFound();

            var responseData = await response.Content.ReadAsStringAsync();
            var taiKhoans = JsonConvert.DeserializeObject<List<TaiKhoanViewModel>>(responseData);

            // Lọc khách hàng (VaiTro = 1)
            var customers = taiKhoans.Where(t => t.VaiTro == 1).ToList();
            return View(customers);
        }

        // Danh sách admin
        public async Task<IActionResult> Admin()
        {
            var response = await _httpClient.GetAsync("http://localhost:5030/api/TaiKhoan");
            if (!response.IsSuccessStatusCode) return NotFound();

            var responseData = await response.Content.ReadAsStringAsync();
            var taiKhoans = JsonConvert.DeserializeObject<List<TaiKhoanViewModel>>(responseData);

            // Lọc admin (VaiTro = 2)
            var admins = taiKhoans.Where(t => t.VaiTro == 2).ToList();
            return View(admins);
        }

        //// Tạo tài khoản
        //[HttpPost]
        //public async Task<IActionResult> Create([FromForm] TaiKhoanViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest("Thông tin không hợp lệ.");
        //    }

        //    // Tự động tạo mã tài khoản
        //    var response = await _httpClient.GetAsync("http://localhost:5030/api/TaiKhoan");
        //    if (response.IsSuccessStatusCode)
        //    {
        //        var responseData = await response.Content.ReadAsStringAsync();
        //        var taiKhoans = JsonConvert.DeserializeObject<List<TaiKhoanViewModel>>(responseData);

        //        int maxId = taiKhoans
        //            ?.Where(t => t.IdtaiKhoan.StartsWith("TK"))
        //            .Select(t => int.Parse(t.IdtaiKhoan.Substring(2)))
        //            .DefaultIfEmpty(0)
        //            .Max() ?? 0;

        //        model.IdtaiKhoan = $"TK{(maxId + 1):D3}";
        //    }
        //    else
        //    {
        //        model.IdtaiKhoan = "TK001";
        //    }

        //    // Xử lý upload ảnh
        //    if (model.Anh != null)
        //    {
        //        var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/AnhTaiKhoan");
        //        if (!Directory.Exists(imagePath))
        //        {
        //            Directory.CreateDirectory(imagePath);
        //        }

        //        var fileName = $"{model.IdtaiKhoan}_{Path.GetFileName(model.Anh.FileName)}";
        //        var filePath = Path.Combine(imagePath, fileName);

        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await model.Anh.CopyToAsync(stream);
        //        }

        //        model.Hinh = $"/images/AnhTaiKhoan/{fileName}"; // Lưu đường dẫn ảnh
        //    }

        //    // Gửi thông tin lên API
        //    var apiResponse = await _httpClient.PostAsJsonAsync("http://localhost:5030/api/TaiKhoan", model);
        //    if (apiResponse.IsSuccessStatusCode)
        //    {
        //        return RedirectToAction(model.VaiTro == 1 ? "Customer" : "Admin");
        //    }

        //    return BadRequest("Lỗi khi thêm tài khoản.");
        //}

        //// Sửa tài khoản
        //[HttpGet]
        //public async Task<IActionResult> Edit(string id)
        //{
        //    var response = await _httpClient.GetAsync($"http://localhost:5030/api/TaiKhoan/{id}");
        //    if (!response.IsSuccessStatusCode) return NotFound();

        //    var responseData = await response.Content.ReadAsStringAsync();
        //    var model = JsonConvert.DeserializeObject<TaiKhoanViewModel>(responseData);

        //    return PartialView("_EditForm", model); // Sử dụng Partial View cho form sửa
        //}

        //[HttpPost]
        //public async Task<IActionResult> Edit([FromForm] TaiKhoanViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest("Thông tin không hợp lệ.");
        //    }

        //    // Xử lý upload ảnh nếu có
        //    if (model.Anh != null)
        //    {
        //        var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/AnhTaiKhoan");
        //        if (!Directory.Exists(imagePath))
        //        {
        //            Directory.CreateDirectory(imagePath);
        //        }

        //        var fileName = $"{model.IdtaiKhoan}_{Path.GetFileName(model.Anh.FileName)}";
        //        var filePath = Path.Combine(imagePath, fileName);

        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await model.Anh.CopyToAsync(stream);
        //        }

        //        model.Hinh = $"/images/AnhTaiKhoan/{fileName}"; // Lưu đường dẫn ảnh
        //    }

        //    // Gửi yêu cầu API để cập nhật
        //    var apiResponse = await _httpClient.PutAsJsonAsync($"http://localhost:5030/api/TaiKhoan/{model.IdtaiKhoan}", model);
        //    if (apiResponse.IsSuccessStatusCode)
        //    {
        //        return RedirectToAction(model.VaiTro == 1 ? "Customer" : "Admin");
        //    }

        //    return BadRequest("Lỗi khi cập nhật tài khoản.");
        //}

        //// Xóa tài khoản
        //[HttpPost]
        //public async Task<IActionResult> Delete(string id, int role)
        //{
        //    var response = await _httpClient.DeleteAsync($"http://localhost:5030/api/TaiKhoan/{id}");
        //    if (response.IsSuccessStatusCode)
        //    {
        //        return RedirectToAction(role == 1 ? "Customer" : "Admin");
        //    }
        //    return BadRequest("Lỗi khi xóa tài khoản.");
        //}
    }
}
