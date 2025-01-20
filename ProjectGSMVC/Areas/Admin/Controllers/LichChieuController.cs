using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjectGSMVC.Areas.Admin.Models;

namespace ProjectGSMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LichChieuController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:7141/api");
        private readonly HttpClient _client;
        public LichChieuController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<LichChieuViewModel> lichChieuPhim = new List<LichChieuViewModel>();

            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/lichchieu").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                lichChieuPhim = JsonConvert.DeserializeObject<List<LichChieuViewModel>>(data);
            }
            else
            {
                string errorMessage = response.Content.ReadAsStringAsync().Result;
                ViewData["ErrorMessage"] = "Không thể lấy danh sách lịch chiếu phim.";
            }
            return View(lichChieuPhim);
        }
    }
}
