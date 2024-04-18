using FrostyBear.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FrostyBear.Controllers
{
    public class ShopLoginController : Controller
    {
        private readonly FrostyBearContext _db;

        public ShopLoginController(FrostyBearContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string CUsername, string CPassword)
        {
            if (HttpContext.Session.GetString("EmployeeId") != null)
            {
                HttpContext.Session.Clear();
            }
            var cus = from c in _db.Customers
                      where c.CustomerUsername.Equals(CUsername)
                      && c.CustomerPassword.Equals(CPassword)
                      select c;

            if (cus.ToList().Count() == 0)
            {
                TempData["ErrorMessage"] = "ไม่พบข้อมูล";
                return RedirectToAction("Index");
            }

            string CusId;
            string CusName;

            foreach (var item in cus)
            {
                CusId = item.CustomerId;
                CusName = item.CustomerUsername;

                HttpContext.Session.SetString("CusId", CusId);
                HttpContext.Session.SetString("CusName", CusName);

                var theRecord = _db.Customers.Find(CusId);
                theRecord.Lastlogin = DateOnly.FromDateTime(DateTime.Now);

                _db.Entry(theRecord).State = EntityState.Modified;
            }

            _db.SaveChanges();
            return RedirectToAction("Check", "Cart");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
