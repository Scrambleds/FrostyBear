using FrostyBear.Models;
using Microsoft.AspNetCore.Mvc;
using FrostyBear.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace FrostyBear.Controllers
{
    public class RegisterController : Controller
    {
        private readonly FrostyBearContext _db;

        public RegisterController(FrostyBearContext db)
        {
            _db = db;
        }

        // GET: /Register
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(Customer Cusobj,string CustomerUsername)
        {
            var cus = from c in _db.Customers
                      where c.CustomerUsername.Equals(CustomerUsername)
                      select c;
            var lastcus = _db.Customers
                                .OrderByDescending(c => c.CustomerId)
                                .Select(c => c.CustomerId)
                                .FirstOrDefault();
            try
            {
                if (ModelState.IsValid)
                {
                    if (cus.ToList().Count() == 0)
                    {
                        string newUserId = lastcus.Substring(0, 1) + (int.Parse(lastcus.Substring(1)) + 1).ToString("D3");
                        Cusobj.CustomerId = newUserId;
                        Cusobj.Startdate = DateOnly.FromDateTime(DateTime.Now);
                        _db.Customers.Add(Cusobj);
                        _db.SaveChanges();
                        return RedirectToAction("Index", "ShopLogin");
                    }
                    else {
                        TempData["ErrorMessage"] = "มีคนใช่ชื่อนี้แล้ว";
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(Cusobj);
            }
            ViewBag.ErrorMessage = "การบันทึกผิดพลาด";
            return View(Cusobj);

        }
    }
    }