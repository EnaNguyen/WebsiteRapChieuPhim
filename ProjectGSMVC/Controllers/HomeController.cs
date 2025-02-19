using Azure;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjectGSMAUI.Api.Data.Entities;
using ProjectGSMAUI.Api.Helper;
using ProjectGSMAUI.Api.Modal;
using ProjectGSMVC.Areas.Admin.Models;
using ProjectGSMVC.Models;
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


        public HomeController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
        }
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
            string? idNguoiDung = HttpContext.Session.GetString("IDNguoiDung");
            if (string.IsNullOrEmpty(idNguoiDung))
            {
                idNguoiDung = "AD001";
            }
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
                return Json(new { success = true });
            }
            else
            {
                string apiError = await response.Content.ReadAsStringAsync();
                return BadRequest(new { message = $"Lỗi từ API: {apiError}" });
            }
        }
    }
}
