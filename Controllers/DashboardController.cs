using Microsoft.AspNetCore.Mvc;

namespace FrostyBear.Controllers
{
    public class DashboardController : Controller
    {
        
        public IActionResult Index()
        {
            return View();
        }
      
    }
}
