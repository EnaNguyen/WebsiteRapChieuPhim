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

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			return View(); 
		}

		[HttpGet]
		public async Task<IActionResult> GetBillStatistics()
		{
			try
			{
				HttpResponseMessage response = await _client.GetAsync("/Bill_Management/GetAll");

				if (response.IsSuccessStatusCode)
				{
					string data = await response.Content.ReadAsStringAsync();
					var billList = JsonConvert.DeserializeObject<List<BillMViewModels>>(data);

					var statistics = billList.GroupBy(b => b.TinhTrangDisplay)
						.Select(g => new { TrangThai = g.Key, SoLuong = g.Count() })
						.ToList();

					return Json(statistics);
				}
			}
			catch (Exception ex)
			{
				return Json(new { error = "Lỗi khi lấy dữ liệu" });
			}
			return Json(new List<object>());
		}
	}
}
