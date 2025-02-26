using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProjectGSMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ThongKeController : Controller
    {
        private readonly HttpClient _client;
        private readonly string _baseApiUrl = "https://localhost:7141/api/ThongKe";

        public ThongKeController(HttpClient client)
        {
            _client = client;
        }

        public IActionResult Index()
        {
            return View();
        }

        // Lấy danh sách phim để chọn
        [HttpGet]
        public async Task<IActionResult> GetMovieList()
        {
            var response = await _client.GetAsync($"{_baseApiUrl}/GetMovieList");

            if (!response.IsSuccessStatusCode)
            {
                return Json(new { success = false, message = "Không thể lấy danh sách phim!" });
            }

            var data = await response.Content.ReadAsStringAsync();
            var movies = JsonConvert.DeserializeObject<List<string>>(data);
            return Json(new { success = true, movies });
        }

        // Lấy doanh thu theo ngày
        [HttpGet]
        public async Task<IActionResult> GetRevenueByDate(string date)
        {
            var response = await _client.GetAsync($"{_baseApiUrl}/GetRevenueByDate?date={date}");

            if (!response.IsSuccessStatusCode)
            {
                return Json(new { success = false, message = "Không thể lấy dữ liệu thống kê!" });
            }

            var data = await response.Content.ReadAsStringAsync();
            var revenueData = JsonConvert.DeserializeObject<Dictionary<string, int>>(data);
            return Json(new { success = true, data = revenueData });
        }

        // Lấy doanh thu theo phim
        [HttpGet]
        public async Task<IActionResult> GetRevenueByMovie(string movie)
        {
            var response = await _client.GetAsync($"{_baseApiUrl}/GetRevenueByMovie?movie={movie}");

            if (!response.IsSuccessStatusCode)
            {
                return Json(new { success = false, message = "Không thể lấy dữ liệu doanh thu theo phim!" });
            }

            var data = await response.Content.ReadAsStringAsync();
            var revenueData = JsonConvert.DeserializeObject<Dictionary<string, int>>(data);
            return Json(new { success = true, data = revenueData });
        }

        // Lấy số vé bán theo phim
        [HttpGet]
        public async Task<IActionResult> GetTicketSalesByMovie(string movie)
        {
            var response = await _client.GetAsync($"{_baseApiUrl}/GetTicketSalesByMovie?movie={movie}");

            if (!response.IsSuccessStatusCode)
            {
                return Json(new { success = false, message = "Không thể lấy dữ liệu vé bán theo phim!" });
            }

            var data = await response.Content.ReadAsStringAsync();
            var ticketData = JsonConvert.DeserializeObject<Dictionary<string, int>>(data);
            return Json(new { success = true, data = ticketData });
        }
    }
}
