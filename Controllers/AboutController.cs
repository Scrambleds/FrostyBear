using Microsoft.AspNetCore.Mvc;

namespace FrostyBear.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
           
            return View();
        }
    }
}
