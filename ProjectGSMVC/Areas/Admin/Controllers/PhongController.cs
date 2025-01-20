using Microsoft.AspNetCore.Mvc;
using ProjectGSMAUI.Api.Container;
using ProjectGSMAUI.MVC.Models;

namespace ProjectGSMAUI.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PhongController : Controller
    {
        private readonly IPhongService _phongService;

        public PhongController(IPhongService phongService)
        {
            _phongService = phongService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var phongs = await _phongService.GetPhongsAsync();
                var phongModels = phongs.Select(p => new PhongModel
                {
                    Id = p.Id,
                    TenPhong = p.TenPhong,
                    SoLuongGhe = p.SoLuongGhe,
                    TinhTrang = p.TinhTrang

                }).ToList();

                return View(phongModels);
            }
            catch (Exception ex)
            {
                // Log the error here
                return BadRequest(new { message = $"Đã xảy ra lỗi: {ex.Message}" });
            }
        }

        // You can add more actions for create, edit, delete, etc. here later
    }
}