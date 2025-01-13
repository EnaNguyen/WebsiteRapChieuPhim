using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjectGSMAUI.Api.Data.Entities;
using ProjectGSMAUI.Api.Modal;
using ProjectGSMVC.Areas.Admin.Models;
using ProjectGSMVC.Areas.Admin.ViewModels;
using ProjectGSMVC.Areas.Admin.ViewModels;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using ProjectGSMAUI.Api.Utilities;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using ProjectGSMAUI.Api.Helper;
using Microsoft.DotNet.MSIdentity.Shared;
namespace ProjectGSMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TaiKhoanController : Controller
    {
        private static int LoadingPageTime = 0;
        Uri baseAddress = new Uri("https://localhost:7141/api");
        private readonly HttpClient _httpClient;

        public TaiKhoanController(HttpClient httpClient)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
        }
        public IActionResult Index()
        {
            return View();
        }
        //Customer
        [HttpGet]
        public async Task<IActionResult> Customer(string searching = null)
        {
            string url = string.IsNullOrEmpty(searching)
         ? $"{_httpClient.BaseAddress}/TaiKhoan/TaiKhoanCustomer"
         : $"{_httpClient.BaseAddress}/TaiKhoan/TaiKhoanCustomer?Name={Uri.EscapeDataString(searching)}";

            HttpResponseMessage response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var AdminAccountList = JsonConvert.DeserializeObject<List<TaiKhoan>>(data);
                ViewData["ListAdminAccount"] = AdminAccountList;
                ViewData["Searching"] = searching;
                if (searching != null)
                 {
                     LoadingPageTime += 1;
                     if (AdminAccountList.Count == 0)
                     {
                         return PartialView("SearchingAdmin", null);
                     }
                     return PartialView("SearchingAdmin", AdminAccountList);
                 }
                 else
                 {
                     if (LoadingPageTime == 0)
                     {

                         ViewData["ListAdminAccount"] = AdminAccountList;

                         return View();
                     }
                     LoadingPageTime = 0;
                     return PartialView("SearchingAdmin", AdminAccountList);
                 }
            }
            else
            {
                return BadRequest(new { message = "Không thể tải dữ liệu từ API" });
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromForm] TaiKhoanModel data)
        {
            LoadingPageTime = 0;
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
                string url = $"{_httpClient.BaseAddress}/TaiKhoan/GetByTenTaiKhoan?Name={Uri.EscapeDataString(data.TenTaiKhoan)}";

                HttpResponseMessage response = _httpClient.GetAsync(url).Result;
                if (!response.IsSuccessStatusCode)
                {
                    errors["TenTaiKhoan"] = "Tên Tài Khoản đã Tồn tại";
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
                string url = $"{_httpClient.BaseAddress}/TaiKhoan/GetByEmail?Name={Uri.EscapeDataString(data.Email)}";

                HttpResponseMessage response = _httpClient.GetAsync(url).Result;
                if (!response.IsSuccessStatusCode)
                {
                    errors["Email"] = "Email này đã Tồn tại";
                }
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
                    data.Hinh = Convert.ToBase64String(imageBytes);
                }

                var giamGiaRequest = new TaiKhoanRequest
                {
                    TenNguoiDung = data.TenNguoiDung,
                    MatKhau = PasswordHasher.HashPassword(data.MatKhau),
                    TenTaiKhoan = data.TenTaiKhoan,
                    Email = data.Email,
                    Sdt = data.Sdt,
                    NgaySinh = data.NgaySinh,
                    Cccd = data.Cccd,
                    GioiTinh = data.GioiTinh == "1" ? true : false,
                    DiaChi = data.DiaChi,
                    Hinh = data.Hinh
                };

                var jsonContent = new StringContent(
                    System.Text.Json.JsonSerializer.Serialize(giamGiaRequest, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PostAsync("/api/TaiKhoan/CreateCustomer", jsonContent);

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

        public async Task<IActionResult> UpdateCustomer(string id, TaiKhoanModel data)
        {
            LoadingPageTime = 0;
            var errors = new Dictionary<string, string>();
            var DuLieu = await _httpClient.GetAsync(_httpClient.BaseAddress + "/TaiKhoan/GetTaiKhoan?" + $"id={id.Trim()}");
            var jsonResponse = await DuLieu.Content.ReadAsStringAsync();
            var giamGia = JsonConvert.DeserializeObject<TaiKhoanRequest>(jsonResponse);
            if (string.IsNullOrEmpty(data.TenNguoiDung))
            {
                errors["TenNguoiDung"] = "Họ và Tên không được để trống.";
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
                if (giamGia.Email != data.Email.Trim())
                {
                    string url = $"{_httpClient.BaseAddress}/TaiKhoan/GetByEmail?Name={Uri.EscapeDataString(data.Email)}";

                    HttpResponseMessage response = _httpClient.GetAsync(url).Result;
                    if (!response.IsSuccessStatusCode)
                    {
                        errors["Email"] = "Email này đã Tồn tại";
                    }
                }
            }
            if (string.IsNullOrWhiteSpace(data.Sdt) || (data.Sdt.Trim().Length != 10 && data.Sdt.Trim().Length != 11) || !data.Sdt.Trim().All(char.IsDigit))
            {
                errors["Sdt"] = "Số điện thoại không được nhập chữ hay kí tự đặc biệt, độ dài 10-11 số";
            }
            if (data.NgaySinh == default || data.NgaySinh > DateOnly.FromDateTime(DateTime.Today))
            {
                errors["NgaySinh"] = "Ngày bắt đầu không được nhỏ hơn ngày hiện tại.";
            }
            if (string.IsNullOrEmpty(data.Cccd) || data.Cccd.Trim().Length != 12 || !data.Cccd.Trim().All(char.IsDigit))
            {
                errors["CCCD"] = "CCCD chỉ gồm số và có độ dài là 12 ký tự";
            }

            if (string.IsNullOrEmpty(data.TenTaiKhoan))
            {
                errors["TenTaiKhoan"] = "Tên Tài Khoản không được để trống.";
            }
            if (errors.Count > 0)
            {
                return BadRequest(errors);
            }
            try
            {
                TaiKhoanRequest giamGiaRequest = new TaiKhoanRequest();
                if (data.ImageFile == null || data.ImageFile.Length == 0)
                {
                    var Temp = new TaiKhoanRequest
                    {
                        TenNguoiDung = data.TenNguoiDung,
                        MatKhau = PasswordHasher.HashPassword(data.MatKhau),
                        TenTaiKhoan = data.TenTaiKhoan,
                        Email = data.Email,
                        Sdt = data.Sdt,
                        NgaySinh = data.NgaySinh,
                        Cccd = data.Cccd,
                        GioiTinh = data.GioiTinh == "1" ? true : false,
                        DiaChi = data.DiaChi,
                        Hinh = giamGia.Hinh,
                    };
                    giamGiaRequest = Temp;
                }
                else
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await data.ImageFile.CopyToAsync(memoryStream);
                        byte[] imageBytes = memoryStream.ToArray();
                        data.Hinh = Convert.ToBase64String(imageBytes);

                    }
                    var Temp = new TaiKhoanRequest
                    {
                        TenNguoiDung = data.TenNguoiDung,
                        MatKhau = PasswordHasher.HashPassword(data.MatKhau),
                        TenTaiKhoan = data.TenTaiKhoan,
                        Email = data.Email,
                        Sdt = data.Sdt,
                        NgaySinh = data.NgaySinh,
                        Cccd = data.Cccd,
                        GioiTinh = data.GioiTinh == "1" ? true : false,
                        DiaChi = data.DiaChi,
                        Hinh = data.Hinh,
                    };
                    giamGiaRequest = Temp;
                }
                var updateRequest = new TaiKhoanEdit
                {
                    Id = id.Trim(),
                    TaiKhoanRequest = giamGiaRequest
                };
                var jsonContent = new StringContent(
                   System.Text.Json.JsonSerializer.Serialize(updateRequest, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                   Encoding.UTF8,
                   "application/json"
               );

                var response2 = await _httpClient.PutAsync("/api/TaiKhoan/UpdateCustomer", jsonContent);

                if (response2.IsSuccessStatusCode)
                {
                    return Json(new { success = true });
                }
                else
                {
                    string apiError = await response2.Content.ReadAsStringAsync();
                    return BadRequest(new { message = $"Lỗi từ API: {apiError}" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Đã xảy ra lỗi trong quá trình xử lý: {ex.Message}");
            }
        }




        //Admin
        [HttpGet]
        public async Task<IActionResult> Admin(string searching = null)    
        {
            string url = string.IsNullOrEmpty(searching)
         ? $"{_httpClient.BaseAddress}/TaiKhoan/TaiKhoanAdmin"
         : $"{_httpClient.BaseAddress}/TaiKhoan/TaiKhoanAdmin?Name={Uri.EscapeDataString(searching)}";

            HttpResponseMessage response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var AdminAccountList = JsonConvert.DeserializeObject<List<TaiKhoan>>(data);
                ViewData["ListAdminAccount"] = AdminAccountList;
                ViewData["Searching"] = searching;
                if (searching != null)
                 {
                     LoadingPageTime += 1;
                     if (AdminAccountList.Count == 0)
                     {
                         return PartialView("SearchingAdmin", null);
                     }
                     return PartialView("SearchingAdmin", AdminAccountList);
                 }
                 else
                 {
                     if (LoadingPageTime == 0)
                     {

                         ViewData["ListAdminAccount"] = AdminAccountList;

                         return View();
                     }
                     LoadingPageTime = 0;
                     return PartialView("SearchingAdmin", AdminAccountList);
                 }
            }
            else
            {
                return BadRequest(new { message = "Không thể tải dữ liệu từ API" });
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateAdmin([FromForm] TaiKhoanModel data)
        {
            LoadingPageTime = 0;
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
                string url = $"{_httpClient.BaseAddress}/TaiKhoan/GetByTenTaiKhoan?Name={Uri.EscapeDataString(data.TenTaiKhoan)}";

                HttpResponseMessage response = _httpClient.GetAsync(url).Result;
                if (!response.IsSuccessStatusCode)
                {
                    errors["TenTaiKhoan"] = "Tên Tài Khoản đã Tồn tại";
                }
            }
            if(string.IsNullOrEmpty(data.MatKhau)||data.MatKhau.Contains(" ")||data.MatKhau.Length<6)
            {
                errors["MatKhau"] = "Mật Khẩu phải có chứa ít nhất 6 kí tự";
            }    
            if(data.ReMatKhau!=data.MatKhau)
            {
                errors["ReMatKhau"] = "Mật Khẩu nhập lại không trùng khớp";
            }
            if (string.IsNullOrEmpty(data.Email))
            {
                errors["Email"] = "Email không được để trống.";
            }

            else
            {
                string url = $"{_httpClient.BaseAddress}/TaiKhoan/GetByEmail?Name={Uri.EscapeDataString(data.Email)}";

                HttpResponseMessage response = _httpClient.GetAsync(url).Result;
                if (!response.IsSuccessStatusCode)
                {
                    errors["Email"] = "Email này đã Tồn tại";
                }
            }
            if(string.IsNullOrWhiteSpace(data.Sdt)||(data.Sdt.Length!=10&&data.Sdt.Length!=11)|| !data.Sdt.All(char.IsDigit))
            {
                errors["Sdt"] = "Số điện thoại không được nhập chữ hay kí tự đặc biệt, độ dài 10-11 số";
            }    
            if (data.NgaySinh == default || data.NgaySinh > DateOnly.FromDateTime(DateTime.Today))
            {
                errors["NgaySinh"] = "Ngày bắt đầu không được nhỏ hơn ngày hiện tại.";
            }
            if (string.IsNullOrEmpty(data.Cccd) || data.Cccd.Length!=12 || !data.Cccd.All(char.IsDigit))
            {
                errors["CCCD"] = "CCCD chỉ gồm số và có độ dài là 12 ký tự";
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
                    data.Hinh = Convert.ToBase64String(imageBytes);
                }

                var giamGiaRequest = new TaiKhoanRequest
                {
                    TenNguoiDung = data.TenNguoiDung,
                    MatKhau = PasswordHasher.HashPassword(data.MatKhau),
                    TenTaiKhoan = data.TenTaiKhoan,
                    Email = data.Email,
                    Sdt = data.Sdt,
                    NgaySinh = data.NgaySinh,
                    Cccd = data.Cccd,
                    GioiTinh = data.GioiTinh=="1"?true:false,
                    DiaChi = data.DiaChi,
                    Hinh = data.Hinh
                };

                var jsonContent = new StringContent(
                    System.Text.Json.JsonSerializer.Serialize(giamGiaRequest, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PostAsync("/api/TaiKhoan/CreateAdmin", jsonContent);

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
        public async Task<IActionResult> GetTaiKhoan(string id)
        {
            LoadingPageTime = 0;
            try
            {
                var response = await _httpClient.GetAsync(_httpClient.BaseAddress + "/TaiKhoan/GetTaiKhoan?" + $"id={id.Trim()}");

                if (!response.IsSuccessStatusCode)
                {
                    string apiError = await response.Content.ReadAsStringAsync();
                    return BadRequest(new { message = $"Lỗi từ API: {apiError}" });
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();


                var Tk = JsonConvert.DeserializeObject<TaiKhoan>(jsonResponse);

                if (Tk  == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy dữ liệu giảm giá." });
                }
                return Json(new
                {
                    success = true,
                    data = new
                    {
                        Tk.IdtaiKhoan,
                        Tk.TenTaiKhoan,
                        Tk.MatKhau,
                        Tk.TenNguoiDung,
                        Tk.Email,
                        Tk.Sdt,
                        Tk.VaiTro,
                        Tk.NgaySinh,
                        Tk.NgayDangKy,
                        Tk.TrangThai,
                        Tk.DiemTichLuy,
                        Hinh= Tk.Hinh != null ? Convert.ToBase64String(Tk.Hinh) : null,
                        Tk.Cccd,
                        Tk.GioiTinh,
                        Tk.DiaChi,
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateAdmin(string id, TaiKhoanModel data)
        {
            LoadingPageTime = 0;
            var errors = new Dictionary<string, string>();
            var DuLieu = await _httpClient.GetAsync(_httpClient.BaseAddress + "/TaiKhoan/GetTaiKhoan?" + $"id={id.Trim()}");
            var jsonResponse = await DuLieu.Content.ReadAsStringAsync();
            var giamGia = JsonConvert.DeserializeObject<TaiKhoanRequest>(jsonResponse);
            if (string.IsNullOrEmpty(data.TenNguoiDung))
            {
                errors["TenNguoiDung"] = "Họ và Tên không được để trống.";
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
                if(giamGia.Email!=data.Email.Trim())
                {
                    string url = $"{_httpClient.BaseAddress}/TaiKhoan/GetByEmail?Name={Uri.EscapeDataString(data.Email)}";

                    HttpResponseMessage response = _httpClient.GetAsync(url).Result;
                    if (!response.IsSuccessStatusCode)
                    {
                        errors["Email"] = "Email này đã Tồn tại";
                    }
                }                   
            }
            if (string.IsNullOrWhiteSpace(data.Sdt) || (data.Sdt.Trim().Length != 10 && data.Sdt.Trim().Length != 11) || !data.Sdt.Trim().All(char.IsDigit))
            {
                errors["Sdt"] = "Số điện thoại không được nhập chữ hay kí tự đặc biệt, độ dài 10-11 số";
            }
            if (data.NgaySinh == default || data.NgaySinh > DateOnly.FromDateTime(DateTime.Today))
            {
                errors["NgaySinh"] = "Ngày bắt đầu không được nhỏ hơn ngày hiện tại.";
            }
            if (string.IsNullOrEmpty(data.Cccd) || data.Cccd.Trim().Length != 12 || !data.Cccd.Trim().All(char.IsDigit))
            {
                errors["CCCD"] = "CCCD chỉ gồm số và có độ dài là 12 ký tự";
            }
          
            if (string.IsNullOrEmpty(data.TenTaiKhoan))
            {
                errors["TenTaiKhoan"] = "Tên Tài Khoản không được để trống.";
            }
            if (errors.Count > 0)
            {
                return BadRequest(errors);
            }         
            try
            {
                TaiKhoanRequest giamGiaRequest = new TaiKhoanRequest();
                if (data.ImageFile == null || data.ImageFile.Length == 0)
                {
                    var Temp = new TaiKhoanRequest
                    {
                        TenNguoiDung = data.TenNguoiDung,
                        MatKhau = PasswordHasher.HashPassword(data.MatKhau),
                        TenTaiKhoan = data.TenTaiKhoan,
                        Email = data.Email,
                        Sdt = data.Sdt,
                        NgaySinh = data.NgaySinh,
                        Cccd = data.Cccd,
                        GioiTinh = data.GioiTinh == "1" ? true : false,
                        DiaChi = data.DiaChi,
                        Hinh = giamGia.Hinh,
                    };
                    giamGiaRequest = Temp;
                }
                else
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await data.ImageFile.CopyToAsync(memoryStream);
                        byte[] imageBytes = memoryStream.ToArray();
                        data.Hinh = Convert.ToBase64String(imageBytes);
                        
                    }
                    var Temp = new TaiKhoanRequest
                    {
                        TenNguoiDung = data.TenNguoiDung,
                        MatKhau = PasswordHasher.HashPassword(data.MatKhau),
                        TenTaiKhoan = data.TenTaiKhoan,
                        Email = data.Email,
                        Sdt = data.Sdt,
                        NgaySinh = data.NgaySinh,
                        Cccd = data.Cccd,
                        GioiTinh = data.GioiTinh == "1" ? true : false,
                        DiaChi = data.DiaChi,
                        Hinh = data.Hinh,
                    };
                    giamGiaRequest = Temp;
                }
                var updateRequest = new TaiKhoanEdit
                {
                    Id = id.Trim(),
                    TaiKhoanRequest = giamGiaRequest
                };
                var jsonContent = new StringContent(
                   System.Text.Json.JsonSerializer.Serialize(updateRequest, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                   Encoding.UTF8,
                   "application/json"
               );

                var response2 = await _httpClient.PutAsync("/api/TaiKhoan/UpdateAdmin", jsonContent);

                if (response2.IsSuccessStatusCode)
                {
                    return Json(new { success = true });
                }
                else
                {
                    string apiError = await response2.Content.ReadAsStringAsync();
                    return BadRequest(new { message = $"Lỗi từ API: {apiError}" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Đã xảy ra lỗi trong quá trình xử lý: {ex.Message}");
            }
        }
        [HttpPost]
        public async Task<IActionResult> XoaTaiKhoan(string id)
        {
            LoadingPageTime = 0;
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new { message = "ID không hợp lệ." });
            }

            var jsonContent = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(id.Trim(), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PutAsync("/api/TaiKhoan/VoHieuHoa", jsonContent);          
            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                if (responseData == "0")
                {
                    return Json(new { success = true, message = "Tài khoản đã bị vô hiệu hóa." });
                }
                else
                {
                    return Json(new { success = true, message = "Tài khoản đã được kích hoạt lại." });
                }                       
            }
            else
            {
                var apiError = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Error: {apiError}");
                return BadRequest(new { message = $"Lỗi từ API: {apiError}" });
            }
        }
    }
}
