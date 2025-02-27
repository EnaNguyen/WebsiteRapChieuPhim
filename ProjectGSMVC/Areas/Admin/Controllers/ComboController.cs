using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProjectGSMAUI.Api.Data.Entities;
using ProjectGSMAUI.MVC.Models;
using ProjectGSMVC.Areas.Admin.Models;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ProjectGSMAUI.Api.Modal;
namespace ProjectGSMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ComboController : Controller
    {
        private readonly HttpClient _client;
        private readonly string _baseApiUrl = "https://localhost:7141/api/Combo";

        public ComboController()
        {
            _client = new HttpClient();
        }

        // ✅ Hiển thị danh sách Combo
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var response = await _client.GetAsync(_baseApiUrl);
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage = "Không thể lấy danh sách Combo.";
                return View(new List<ComboViewModel>());
            }

            string data = await response.Content.ReadAsStringAsync();
            var comboList = JsonConvert.DeserializeObject<List<ComboViewModel>>(data);
            return View(comboList);
        }

        // ✅ Hiển thị chi tiết Combo
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var response = await _client.GetAsync($"{_baseApiUrl}/{id}");

            if (!response.IsSuccessStatusCode)
                return NotFound("Không tìm thấy Combo!");

            string data = await response.Content.ReadAsStringAsync();
            var combo = JsonConvert.DeserializeObject<ComboViewModel>(data);

            if (combo == null)
                return NotFound("Combo không tồn tại!");

            return PartialView("Details", combo);
        }



        // ✅ Tạo mới Combo (GET)

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var response = await _client.GetAsync("https://localhost:7141/api/SanPham");
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var sanPhams = JsonConvert.DeserializeObject<List<SanPhamModel>>(data);
                ViewBag.SanPhams = sanPhams;
            }
            else
            {
                ViewBag.SanPhams = new List<SanPhamModel>(); 
            }

            return View();
        }


         //✅ Tạo mới Combo(POST)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ComboCreate combo)
        {
            StringContent content = new StringContent(JsonConvert.SerializeObject(combo), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("https://localhost:7141/api/Combo/Create", content);

            if (!response.IsSuccessStatusCode)
                return BadRequest("Lỗi khi tạo Combo!");

            return Ok("Combo đã được thêm!");

        }

        // ✅ Sửa Combo (GET)
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _client.GetAsync($"{_baseApiUrl}/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound("Không tìm thấy Combo!");

            string data = await response.Content.ReadAsStringAsync();
            var combo = JsonConvert.DeserializeObject<ComboViewModel>(data);

            // Lấy danh sách sản phẩm
            var productResponse = await _client.GetAsync("https://localhost:7141/api/SanPham");
            if (productResponse.IsSuccessStatusCode)
            {
                string productData = await productResponse.Content.ReadAsStringAsync();
                var sanPhams = JsonConvert.DeserializeObject<List<SanPhamModel>>(productData);
                ViewBag.SanPhams = sanPhams;
            }

            return View(combo);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, [FromBody] ComboCreate combo)
        {
            StringContent content = new StringContent(JsonConvert.SerializeObject(combo), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("https://localhost:7141/api/Combo/{id}", content);

            if (!response.IsSuccessStatusCode)
            {
                string errorMsg = await response.Content.ReadAsStringAsync();
                return BadRequest($"Lỗi khi cập nhật Combo: {errorMsg}");
            }

            return Ok(new { success = true, message = "Combo đã được cập nhật!" });
        }



        // ✅ Xóa Combo
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _client.DeleteAsync($"{_baseApiUrl}/{id}");

            if (!response.IsSuccessStatusCode)
                return Json(new { success = false, message = "Lỗi khi xóa Combo!" });

            return Json(new { success = true, message = "Combo đã được xóa!" });
        }
       




    }
}
