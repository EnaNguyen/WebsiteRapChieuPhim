using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjectGSMVC.Areas.Admin.Models;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ProjectGSMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class QuanLiHoaDonController : Controller
    {
        private readonly Uri baseAddress = new Uri("https://localhost:7141/api");
        private readonly HttpClient _client;

        public QuanLiHoaDonController()
        {
            _client = new HttpClient { BaseAddress = baseAddress };
        }

        // Lấy danh sách hóa đơn
        [HttpGet]
        public IActionResult Index()
        {
            List<BillMViewModels> billList = new List<BillMViewModels>();

            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/Bill_Management/GetAll").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                billList = JsonConvert.DeserializeObject<List<BillMViewModels>>(data);
            }
            else
            {
                string errorMessage = response.Content.ReadAsStringAsync().Result;
                ViewBag.BadRequest = $"Không thể lấy dữ liệu: {errorMessage}";
            }

            return View(billList);
        }

        // Cập nhật trạng thái thanh toán
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id)
        {
            // Gọi API để lấy thông tin hóa đơn
            HttpResponseMessage response = await _client.GetAsync($"{_client.BaseAddress}/Bill_Management/GetByID?id={id}");

            if (!response.IsSuccessStatusCode)
            {
                return Json(new
                {
                    success = false,
                    message = "Không thể kết nối tới server để lấy thông tin hóa đơn!"
                });
            }

            string data = await response.Content.ReadAsStringAsync();
            var bill = JsonConvert.DeserializeObject<BillMViewModels>(data);

            if (bill == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Không tìm thấy hóa đơn!"
                });
            }

            // Kiểm tra trạng thái thanh toán hiện tại
            if (string.IsNullOrEmpty(bill.TinhTrangDisplay))
            {
                return Json(new
                {
                    success = false,
                    message = "Trạng thái thanh toán không hợp lệ!"
                });
            }

            // Cập nhật trạng thái thanh toán
            int tinhTrang = bill.TinhTrangDisplay == "Chưa thanh toán" ? 1 : 0;
            bill.TinhTrangDisplay = tinhTrang == 1 ? "Đã thanh toán" : "Chưa thanh toán";

            // Gửi dữ liệu cập nhật đến API
            StringContent content = new StringContent(JsonConvert.SerializeObject(bill), Encoding.UTF8, "application/json");
            HttpResponseMessage updateResponse = await _client.PutAsync($"{_client.BaseAddress}/Bill_Management/Update?id={id}", null);

            if (updateResponse.IsSuccessStatusCode)
            {
                return Json(new
                {
                    success = true,
                    message = "Cập nhật trạng thái thành công!",
                    tinhTrang = bill.TinhTrangDisplay
                });
            }
            else
            {
                string errorMessage = await updateResponse.Content.ReadAsStringAsync();
                return Json(new
                {
                    success = false,
                    message = $"Không thể cập nhật trạng thái: {errorMessage}"
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            ////// Gọi API để lấy chi tiết hóa đơn
            HttpResponseMessage response = await _client.GetAsync($"{_client.BaseAddress}/Bill_Management/GetDetailsByID?id={id}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();

                var bill = JsonConvert.DeserializeObject<BillMViewModels>(data); // Deserialize dữ liệu vào đối tượng

                // Kiểm tra xem BillMViewModels có dữ liệu chi tiết hóa đơn không
                if (bill.DetailBillItems != null && bill.DetailBillItems.Any())
                {
                    return PartialView("Details", bill);
                }
                else
                {
                    return NotFound(); // Không có chi tiết hóa đơn
                }
            }
            else
            {
                return NotFound(); // Nếu API không thành công
            }
        }




    }
}
