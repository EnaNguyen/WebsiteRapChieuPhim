using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjectGSMVC.Areas.Admin.Models;
using Microsoft.Extensions.Logging;

namespace ProjectGSMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TheLoaiPhimController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:7141/api");
        private readonly HttpClient _client;

        public TheLoaiPhimController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<TheLoaiPhimViewModel> theLoaiPhimList = new List<TheLoaiPhimViewModel>();

            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/theloaiphim/Get").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                theLoaiPhimList = JsonConvert.DeserializeObject<List<TheLoaiPhimViewModel>>(data);
            }
            else
            {
                string errorMessage = response.Content.ReadAsStringAsync().Result;
                ViewData["ErrorMessage"] = "Không thể lấy danh sách thể loại phim.";
            }


            return View(theLoaiPhimList);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost] 
        public IActionResult Create(TheLoaiPhimViewModel model)
        {
            try
            {
                string data = JsonConvert.SerializeObject(model);

                StringContent content = new StringContent(data, System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage response = _client.PostAsync(_client.BaseAddress + "/theloaiphim/Create", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Thêm thể loại phim mới thành công!";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex) 
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
            return View(model);
        }
    }
}
