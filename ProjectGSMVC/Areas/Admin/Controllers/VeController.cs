using Microsoft.AspNetCore.Mvc;

namespace ProjectGSMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class VeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
