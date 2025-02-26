using Microsoft.AspNetCore.Mvc;

namespace ProjectGSMVC.Areas.Admin.Controllers
{
    public class ComboController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
