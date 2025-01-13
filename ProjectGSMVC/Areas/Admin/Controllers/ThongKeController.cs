using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjectGSMVC.Areas.Admin.Models;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ProjectGSMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ThongKeController : Controller
    {

        private readonly Uri baseAddress = new Uri("http://localhost:5030/api");
        private readonly HttpClient _client;

        public ThongKeController()
        {
            _client = new HttpClient { BaseAddress = baseAddress };
        }
        // Lấy danh sách thống kê
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<BillMViewModels> billList = new List<BillMViewModels>();

            try
            {
               
                HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/Bill_Management/GetAll").Result;
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    billList = JsonConvert.DeserializeObject<List<BillMViewModels>>(data);

                    // Return data as JSON
                    return Json(new { success = true, data = billList });
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    return Json(new { success = false, message = $"Không thể lấy dữ liệu: {errorMessage}" });
                }
            }
            catch (Exception ex)
            {
                // Log exception (optional) and return error message
                return Json(new { success = false, message = $"Lỗi xảy ra: {ex.Message}" });
            }
        }


    }
}