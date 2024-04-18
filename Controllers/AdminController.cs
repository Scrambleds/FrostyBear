using FrostyBear.Models;
using Microsoft.AspNetCore.Mvc;


namespace FrostyBear.Controllers
{
    public class AdminController : Controller
    {

        public readonly FrostyBearContext _db;
        public AdminController(FrostyBearContext db) { _db = db; }
        public IActionResult Index()
        {

            if (HttpContext.Session.GetString("EmployeeUsername") == null)
            {
                return RedirectToAction("AdminLogin");
            }

            return View();
        }
        public IActionResult AdminLogin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AdminLogin(String username, string password)
        {
            if (HttpContext.Session.GetString("CusId") != null)
            {
                HttpContext.Session.Clear();
            }
            var users = from ad in _db.Employees
                        where ad.EmployeeUsername.Equals(username)
                            && ad.EmployeePassword.Equals(password)
                        select ad;

            if (users.Count() == 0)
            {
                TempData["ErrorMessage"] = "Invalid Username or Password";
                return View();
            }

            string EmployeeId;
            string EmployeeUsername;
            string PositionId;

            foreach (var item in users)
            {
                EmployeeId = item.EmployeeId;
                EmployeeUsername = item.EmployeeUsername;
                PositionId = item.PositionId;
                HttpContext.Session.SetString("EmployeeUsername", EmployeeUsername);
                HttpContext.Session.SetString("EmployeeId", EmployeeId);
                HttpContext.Session.SetString("PositionId", PositionId);

            }
            return RedirectToAction("Index","Home");
        }

        public IActionResult AdminLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index","Home");
        }




    }
}