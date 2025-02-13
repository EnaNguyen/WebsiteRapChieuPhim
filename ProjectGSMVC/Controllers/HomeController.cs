using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjectGSMAUI.Api.Data.Entities;
using ProjectGSMVC.Areas.Admin.Models;
using ProjectGSMVC.Models;
using System.Diagnostics;
using System.Net.Http;
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
            try
            {
                HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/Phim/GetAll").Result;

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
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = "Lỗi kết nối API: " + ex.Message;
            }

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
    }
}
