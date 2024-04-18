using FrostyBear.Models;
using FrostyBear.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FrostyBear.Controllers
{
    public class AdmincusController : Controller
    {
        public readonly FrostyBearContext _db;

        public AdmincusController(FrostyBearContext db)
        { _db = db; }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("EmployeeUsername") == null)
            {

                return RedirectToAction("Shop", "Home");
            }
            var adcvm = from c in _db.Customers
                        select new AdcVM
                        {
                            CustomerId = c.CustomerId,
                            CustomerName = c.CustomerName,
                            CustomerAddress = c.CustomerAddress,
                            CustomerContact = c.CustomerContact,
                            CustomerUsername = c.CustomerUsername,
                            CustomerPassword = c.CustomerPassword,
                            Startdate = c.Startdate,
                            Lastlogin = c.Lastlogin
                        };

            if (adcvm == null) return NotFound();
            return View(adcvm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(string? stext)
        {
            if (HttpContext.Session.GetString("EmployeeUsername") == null)
            {

                return RedirectToAction("Shop", "Home");
            }
            if (stext == null)
            {
                return RedirectToAction("Index");
            }


            var adcvm = from c in _db.Customers
                        where c.CustomerName.Contains(stext) ||
                      c.CustomerUsername.Contains(stext) ||
                      c.CustomerContact.Contains(stext) ||
                      c.CustomerAddress.Contains(stext)
                        select new AdcVM
                        {
                            CustomerId = c.CustomerId,
                            CustomerName = c.CustomerName,
                            CustomerAddress = c.CustomerAddress,
                            CustomerContact = c.CustomerContact,
                            CustomerUsername = c.CustomerUsername,
                            CustomerPassword = c.CustomerPassword,
                            Startdate = c.Startdate,
                            Lastlogin = c.Lastlogin
                        };

            ViewBag.stext = stext;
            return View(adcvm);
        }
        public IActionResult Edit(string id)
        {
            if (HttpContext.Session.GetString("EmployeeUsername") == null)
            {

                return RedirectToAction("Shop", "Home");
            }
            if (id == null)
            {
                ViewBag.ErrorMessage = "ระบุ id";
                return RedirectToAction("Index");
            }
            var obj = _db.Customers.Find(id);
            if (obj == null)
            {
                ViewBag.ErrorMessage = "ไม่พบข้อมูล";
                return RedirectToAction("Index");
            }
            ViewBag.imgfile = "/imagcus/" + obj.CustomerId + ".jpg";
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Customer obj)
        {
            if (HttpContext.Session.GetString("EmployeeUsername") == null)
            {

                return RedirectToAction("Shop", "Home");
            }
            try
            {
                if (ModelState.IsValid)
                {
                    _db.Customers.Update(obj);
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(obj);
            }
            ViewBag.ErrorMessage = "การแก้ไขผิดพลาด";
            return View(obj);
        }

        public IActionResult Delete(String id)
        {
            if (HttpContext.Session.GetString("EmployeeUsername") == null)
            {

                return RedirectToAction("Shop", "Home");
            }
            if (HttpContext.Session.GetString("EmployeeUsername") == null)
            {

                return RedirectToAction("Shop", "Home");
            }
            if (id == null)
            {
                ViewBag.ErrorMessage = "ระบุ id";
                return RedirectToAction("Index");
            }
            var obj = _db.Customers.Find(id);
            if (obj == null)
            {
                ViewBag.ErrorMessage = "ไม่พบข้อมูล";
                return RedirectToAction("Index");
            }
            ViewBag.imgfile = "/imagcus/" + obj.CustomerId + ".jpg";
            return View(obj);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(string CustomerId)
        {
            if (HttpContext.Session.GetString("EmployeeUsername") == null)
            {

                return RedirectToAction("Shop", "Home");
            }
            try
            {
                var obj = _db.Customers.Find(CustomerId);
                if (obj == null)
                {
                    ViewBag.ErrorMessage = "ไม่พบข้อมูล";
                    return RedirectToAction("Index");
                }
                _db.Customers.Remove(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return RedirectToAction("Index");
            }
        }

        public IActionResult ImgUpload(IFormFile imgfiles, string theid)
        {
            if (HttpContext.Session.GetString("EmployeeUsername") == null)
            {

                return RedirectToAction("Shop", "Home");
            }
            var FileName = theid;
            //var FileExtension = Path.GetExtension(imgfiles.FileName);
            var FileExtension = ".jpg";
            var SaveFileName = FileName + FileExtension;
            var SavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\imagcus");
            var SaveFilePath = Path.Combine(SavePath, SaveFileName);
            using (FileStream fs = System.IO.File.Create(SaveFilePath))
            {
                imgfiles.CopyTo(fs);
                fs.Flush();
            }

            return RedirectToAction("Edit", new { id = theid });
        }

        public IActionResult ImgDelete(string id)
        {
            if (HttpContext.Session.GetString("EmployeeUsername") == null)
            {

                return RedirectToAction("Shop", "Home");
            }
            var fileName = id.ToString() + ".jpg";
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\imagcus");
            var filePath = Path.Combine(imagePath, fileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            return RedirectToAction("Edit", new { id = id });
        }
    }
}
