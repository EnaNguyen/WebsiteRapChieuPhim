using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjectGSMVC.Areas.Admin.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

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

            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/theloaiphim").Result;

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


        //Thêm thể loại phim mới
        //[HttpGet]
        //public IActionResult Create()
        //{
        //    return View();
        //}
        //[HttpPost]
        //public IActionResult Create(TheLoaiPhimViewModel model)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(model.Id))
        //        {
        //            model.Id = Guid.NewGuid().ToString();
        //        }
        //        string data = JsonConvert.SerializeObject(model);

        //        StringContent content = new StringContent(data, System.Text.Encoding.UTF8, "application/json");

        //        HttpResponseMessage response = _client.PostAsync(_client.BaseAddress + "/theloaiphim/Create", content).Result;

        //        if (response.IsSuccessStatusCode)
        //        {
        //            return Json(new { success = true, message = "Thêm thể loại phim mới thành công!" });
        //        }
        //        else
        //        {
        //            return Json(new { success = false, message = "Thêm thể loại phim thất bại." });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = ex.Message });
        //    }
        //}





        //public IActionResult DeleteConfirmed(string id) 
        //{ 
        //    try 
        //    { 
        //        HttpResponseMessage response = _client.DeleteAsync(_client.BaseAddress + $"/TheLoaiPhim/Remove?id={id}").Result; 
        //        if (response.IsSuccessStatusCode) 
        //        { 
        //            string data = response.Content.ReadAsStringAsync().Result;
        //            TempData["successMessage"] = "Xóa thể loại phim thành công."; 
        //        } 
        //        return RedirectToAction("Index"); 
        //    } 
        //    catch (Exception ex) 
        //    { 
        //        TempData["errorMessage"] = ex.Message; 
        //        return RedirectToAction("Index"); 
        //    } 
        //}
    }
}
