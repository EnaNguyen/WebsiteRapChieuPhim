﻿using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProjectGSMAUI.Api.Data.Entities;
using ProjectGSMAUI.Api.Helper;
using ProjectGSMAUI.Api.Modal;
using ProjectGSMVC.Areas.Admin.Models;
using ProjectGSMVC.Models;
using ProjectGSMVC.Services;
using ProjectGSMVC.ViewModels;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ErrorViewModel = ProjectGSMVC.Models.ErrorViewModel;

namespace ProjectGSMVC.Controllers
{
    public class HomeController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:7141/api");
        private readonly HttpClient _client;
        private readonly IVnPayServices _vnPayServices;
        private readonly ILogger<HomeController> _logger; // ✅ Khai báo Logger
        private readonly HttpClient _httpClient;
        public HomeController(ILogger<HomeController> logger, IVnPayServices vnPayServices, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
            _vnPayServices = vnPayServices;
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
        }
        public static DatVeModel Temp = new DatVeModel();
        public static string IDNguoiDung = "";
        [HttpGet]
        public IActionResult Index()
        {
            List<PhimVM> phimList = new List<PhimVM>();
            List<PhimView> pl = new List<PhimView>();
            List<ComboView> Combo = new List<ComboView>();
            try
            {
                HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/Phim/GetAll").Result;
                HttpResponseMessage response1 = _client.GetAsync(_client.BaseAddress + "/Phim/OnBoarding").Result;
                HttpResponseMessage response2 = _client.GetAsync(_client.BaseAddress + "/CheckOut/ListCombo").Result;
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    phimList = JsonConvert.DeserializeObject<List<PhimVM>>(data);

                }
                else
                {
                    ViewData["ErrorMessage"] = "Không thể lấy danh sách phim.";
                }
                // Đảm bảo Videos không bị null
                string data1 = response1.Content.ReadAsStringAsync().Result;
                pl = JsonConvert.DeserializeObject<List<PhimView>>(data1);
                string data2 = response2.Content.ReadAsStringAsync().Result;
                Combo = JsonConvert.DeserializeObject<List<ComboView>>(data2);
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = "Lỗi kết nối API: " + ex.Message;
            }
            ViewData["ListCombo"] = Combo;
            ViewData["ListPhim"] = pl;
            return View(phimList);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpGet]
        public async Task<IActionResult> GetLichChieuByDate(int id, DateOnly date)
        {
            var apiUrl = $"https://localhost:7141/api/LichChieu/GetLichChieuByDate?id={id}&date={date}";
            var response = await _client.GetAsync(apiUrl);
            if (!response.IsSuccessStatusCode)
            {
                return BadRequest("Không thể lấy dữ liệu lịch chiếu.");
            }
            string data1 = response.Content.ReadAsStringAsync().Result;
            var pl = JsonConvert.DeserializeObject<List<LichChieuView>>(data1);

            return Ok(pl);
        }
        [HttpPost("MuaVe")]
        public async Task<IActionResult> MuaVe([FromBody] DatVeModel model)
        {
            string? idNguoiDung = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(idNguoiDung))
            {
                idNguoiDung = "AD001";
            }
            var apiURL = $"https://localhost:7141/api/TaiKHoan/GetTaiKhoan?id={idNguoiDung}";
            var response2 = await _client.GetAsync(apiURL);
            string data2 = response2.Content.ReadAsStringAsync().Result;
            var pl = JsonConvert.DeserializeObject<TaiKhoan>(data2);
            if (!response2.IsSuccessStatusCode)
            {
                return BadRequest("Không thể lấy dữ liệu Người Dùng");
            }
            Temp = model;
            IDNguoiDung = idNguoiDung;
            float TongTien = 0;
            TongTien = (float)model.TongTien;
            var vnPayModel = new VnPaymentRequestModel
            {
                Amount = TongTien,
                CreatedDate = DateTime.Now,
                Description = $"{pl.Sdt} {pl.TenNguoiDung}",
                FullName = pl.TenNguoiDung,
                OrderId = (new Random().Next(1000, 100000)).ToString()
            };
            var Check = _vnPayServices.CreatePaymentUrl(HttpContext, vnPayModel);
            return Ok(_vnPayServices.CreatePaymentUrl(HttpContext, vnPayModel));
        }
        [HttpGet]
        public async Task<IActionResult> GetVeByDate(int LichChieu)
        {
            string Check = "ABCDEFGHIJKL";
            List<string> MaGhe = new List<string>();
            var apiURL = $"https://localhost:7141/api/Ve/GetVeByDate?LichChieu={LichChieu}";
            var response2 = await _client.GetAsync(apiURL);
            if (response2.IsSuccessStatusCode)
            {
                string data2 = response2.Content.ReadAsStringAsync().Result;
                var pl = JsonConvert.DeserializeObject<List<string>>(data2);
                foreach (var item in pl)
                {
                    int cot = 0;
                    string cotTemp = item.Substring(0, 1);
                    string hang = item.Substring(1).Trim();
                    for (int i = 0; i < Check.Length; i++)
                    {
                        if (cotTemp[0] == Check[i])
                        {
                            cot = i + 1;
                            if (cot > 8)
                            {
                                cot += 2;
                            }
                            else if (cot > 4)
                            {
                                cot++;
                            }
                            break;
                        }
                    }
                    MaGhe.Add(hang + "_" + cot.ToString().Trim());
                }
            }
            return Ok(MaGhe);
        }


        public async Task<IActionResult> PaymentCallBack()
        {
            var response2 = _vnPayServices.PaymentExcute(Request.Query);
            if (response2 == null || response2.VnPayResponseCode != "00")
            {
                string error = "Thanh toán VNPay không thành công. Vui lòng thử lại.";
                return BadRequest(new { message = $"Lỗi từ API của VNPay: {error}" });
            }
            var model = Temp;
            var idNguoiDung = IDNguoiDung;
            var jsonContent = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(model, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                Encoding.UTF8,
                "application/json"
            );
            var response = await _client.PostAsync("/api/Ve/MuaVe", jsonContent);
            List<DatVeCreator> creators = new List<DatVeCreator>();
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var veResponse = System.Text.Json.JsonSerializer.Deserialize<List<string>>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                Console.WriteLine(veResponse);
                if (veResponse != null)
                {
                    foreach (var item in veResponse)
                    {
                        DatVeCreator data1 = new DatVeCreator()
                        {
                            MaKhachHang = idNguoiDung,
                            MaVe = item,
                            NgayDat = DateOnly.FromDateTime(DateTime.Now),
                            TrangThai = 0,
                        };
                        creators.Add(data1);
                    }
                }
                int MaGia = 0;
                if (!string.IsNullOrEmpty(model.Ma))
                {
                    var response6 = await _client.GetAsync($"/api/Voucher/Used?ma={model.Ma}");
                    if (response6.IsSuccessStatusCode)
                    {
                        var jsonString = await response6.Content.ReadAsStringAsync();
                        var apiResponse = JsonConvert.DeserializeObject<APIResponse>(jsonString);
                        string MaDaNhap = apiResponse.Data.ToString();
                        MaGia = int.Parse(MaDaNhap);
                    }
                    else
                    {
                        TempData["NoData"] = "Mã không hợp lệ";
                        return RedirectToAction("Index");
                    }
                }
                HoaDonCreator creator = new HoaDonCreator();
                creator.TongTien= model.TongTien??0;
                creator.ListCT = creators;
                creator.MaGiamGia = MaGia;
                var jsonContent1 = new StringContent(
                    System.Text.Json.JsonSerializer.Serialize(creator, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                    Encoding.UTF8,
                    "application/json"
                );
                var response1 = await _client.PostAsync("/api/CheckOut/CreateDatVe", jsonContent1);
                if (response1.IsSuccessStatusCode)
                {
                    var responseBody2 = await response1.Content.ReadAsStringAsync();
                    var veResponse2 = System.Text.Json.JsonSerializer.Deserialize<int>(responseBody2, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if(model.SanPham!=null && model.SanPham.Count>0)
                    {
                        foreach (var item in model.SanPham)
                        {
                            item.MaHoaDon = veResponse2;
                        }
                        var jsonContent3 = new StringContent(
                           System.Text.Json.JsonSerializer.Serialize(model.SanPham, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                           Encoding.UTF8,
                           "application/json");
                        var response3 = await _client.PostAsync("/api/CheckOut/OrderSanPham", jsonContent3);

                    }

                    if (model.Combo!=null &&model.Combo.Count > 0)
                    {
                        foreach (var item in model.Combo)
                        {
                            item.MaHoaDon = veResponse2;
                        }
                        var jsonContent4 = new StringContent(
                        System.Text.Json.JsonSerializer.Serialize(model.Combo, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                        Encoding.UTF8,
                        "application/json");
                        var response4 = await _client.PostAsync("/api/CheckOut/OrderCombo", jsonContent4);
                    }   
                    if(!string.IsNullOrEmpty(model.Ma))
                    {
                        var response5 = await _client.GetAsync($"/api/Voucher/StatusUpdate?ma={model.Ma.Trim()}");
                    }    
                }

                TempData["SuccessMessage"] = "Thanh toán VNPay thành công! Vui lòng check Email của bạn để kiểm tra lại mã vé";
                return RedirectToAction("Index");
            }
            else
            {
                string apiError = await response.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = $"Đã xảy ra lỗi: {apiError}";
                return RedirectToAction("Index");
            }

        }
        [HttpGet("GetUserBillHistory")]
        public async Task<IActionResult> GetUserBillHistory()
        {
            string? idNguoiDung = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(idNguoiDung))
            {
                _logger.LogWarning("⚠️ Session bị mất! Không thể lấy UserId.");
                return RedirectToAction("Login", "User"); // Chuyển hướng đến trang đăng nhập
            }

            _logger.LogInformation("✅ Session tồn tại, UserId: {UserId}", idNguoiDung);

            var apiURL = $"https://localhost:7141/api/Bill_Management/GetBillsByCustomerId?id={idNguoiDung}";
            var response = await _httpClient.GetAsync(apiURL);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("❌ Không thể lấy lịch sử hóa đơn từ API.");
                TempData["ErrorMessage"] = "Không thể lấy lịch sử giao dịch.";
                return RedirectToAction("Index"); // Quay lại trang chính
            }

            string data = await response.Content.ReadAsStringAsync();
            var billHistory = JsonConvert.DeserializeObject<List<ProjectGSMAUI.Api.Modal.BillHistoryModal>>(data);

            return View("GetUserBillHistory", billHistory);
        }
        [HttpGet]
        public async Task<DetailMovie> Detail(int id)
        {

            HttpResponseMessage response = await _client.GetAsync(_client.BaseAddress + $"/Phim/GetByID?id={id}");

            if (response.IsSuccessStatusCode)
            {
                var phimData = await response.Content.ReadAsStringAsync();
                var phim = JsonConvert.DeserializeObject<DetailMovie>(phimData);

                if (phim == null)
                {
                    return null;
                }
                return phim;
            }
            else
            {
                return null;
            }
        }
        [HttpGet("Discount")]
        public async Task<IActionResult> Discount(string Ma)
        {
            if (!string.IsNullOrEmpty(Ma))
            {
                var response = await _client.GetAsync($"/api/Voucher/Used?ma={Ma}");
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<APIResponse>(jsonString);
                    string percent = apiResponse.Result;
                    string MaDaNhap = apiResponse.Data.ToString();
                    return Ok(percent);
                }
                else
                {
                    TempData["NoData"] = "Mã không hợp lệ";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                TempData["NoData"] = "Không thể lấy lịch sử giao dịch.";
                return RedirectToAction("Index");
            }    

        }
    }
}
