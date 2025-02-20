using Azure;
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

        public HomeController(IVnPayServices vnPayServices)
        {
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
            try
            {
                HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/Phim/GetAll").Result;
                HttpResponseMessage response1 = _client.GetAsync(_client.BaseAddress + "/Phim/OnBoarding").Result;
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
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = "Lỗi kết nối API: " + ex.Message;
            }
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
            string? idNguoiDung = HttpContext.Session.GetString("UserID");
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
            var vnPayModel = new VnPaymentRequestModel
            {
                Amount = (float)(model.Ghe.Count()) * 100000,
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
                            cot = i+1;
                            if (cot >8)
                            {
                                cot += 2;
                            }
                            else if(cot>4)
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
                var jsonContent1 = new StringContent(
                    System.Text.Json.JsonSerializer.Serialize(creators, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                    Encoding.UTF8,
                    "application/json"
                );

                var response1 = await _client.PostAsync("/api/CheckOut/CreateDatVe", jsonContent1);
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
    }
}
