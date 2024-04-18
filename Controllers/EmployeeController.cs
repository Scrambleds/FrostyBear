using FrostyBear.Models;
using FrostyBear.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FrostyBear.Controllers
{
    public class EmployeeController : Controller
    {

        public readonly FrostyBearContext _db;

        public EmployeeController(FrostyBearContext db)
        { _db = db; }


        public IActionResult Index()
        {
            var emvm = from e in _db.Employees
                       join ep in _db.Positions on e.PositionId equals ep.PositionId into join_e_ep
                       from e_ep in join_e_ep.DefaultIfEmpty()

                       select new EmVM
                       {
                           EmployeeId = e.EmployeeId,
                           EmployeeName = e.EmployeeName,
                           EmployeeContact = e.EmployeeContact,
                           EmployeeUsername = e.EmployeeUsername,
                           EmployeePassword = e.EmployeePassword,
                           PositionId = e.PositionId,
                           PositionName = e_ep.PositionName,
                       };

            if (emvm == null) return NotFound();
            return View(emvm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(string? stext)
        {
            if (stext == null)
            {
                return RedirectToAction("Index");
            }

            var emvm = from e in _db.Employees
                       join ep in _db.Positions on e.PositionId equals ep.PositionId into join_e_ep
                       from e_ep in join_e_ep.DefaultIfEmpty()
                       where e_ep.PositionName.Contains(stext)||
                       e.EmployeeName.Contains(stext)||
                       e.EmployeeUsername.Contains(stext)

                       select new EmVM
                       {
                           EmployeeId = e.EmployeeId,
                           EmployeeName = e.EmployeeName,
                           EmployeeContact = e.EmployeeContact,
                           EmployeeUsername = e.EmployeeUsername,
                           EmployeePassword = e.EmployeePassword,
                           PositionId = e.PositionId,
                           PositionName = e_ep.PositionName,
                       };

            ViewBag.stext = stext;
            return View(emvm);
        }

        [HttpPost]
        public IActionResult Create()
        {
            ViewData["PositionName"] = new SelectList(_db.Positions, "PositionId", "PositionName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Employee obj, IFormFile imgfiles)
        {
            var lastemp = _db.Employees
                                .OrderByDescending(e => e.EmployeeId)
                                .Select(e => e.EmployeeId)
                                .FirstOrDefault();
            try
            {
                if (ModelState.IsValid)
                {
                    string newempId = lastemp.Substring(0, 1) + (int.Parse(lastemp.Substring(1)) + 1).ToString("D3");
                    obj.EmployeeId = newempId;
                    if (imgfiles != null && imgfiles.Length > 0)
                    {
                        var FileName = newempId;
                        var FileExtension = ".png";
                        var SaveFileName = FileName + FileExtension;
                        var SavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\imagem");
                        var SaveFilePath = Path.Combine(SavePath, SaveFileName);

                        using (FileStream fs = System.IO.File.Create(SaveFilePath))
                        {
                            imgfiles.CopyTo(fs);
                            fs.Flush();
                        }
                    }
                    _db.Employees.Add(obj);
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(obj);
            }
            ViewBag.ErrorMessage = "การบันทึกผิดพลาด";
            return View(obj);
        }

        public IActionResult Edit(string id)
        {
            if (id == null)
            {
                ViewBag.ErrorMessage = "ระบุ id";
                return RedirectToAction("Index");
            }
            var obj = _db.Employees.Find(id);
            if (obj == null)
            {
                ViewBag.ErrorMessage = "ไม่พบข้อมูล";
                return RedirectToAction("Index");
            }
            ViewData["Positions"] = new SelectList(_db.Positions, "PositionId", "PositionName", obj.EmployeeId);
            ViewBag.imgfile = "/imagem/" + obj.EmployeeId + ".png";
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Employee obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _db.Employees.Update(obj);
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
            ViewData["Positions"] = new SelectList(_db.Positions, "PositionId", "PositionName", obj.EmployeeId);
            return View(obj);
        }

        public IActionResult Delete(String id)
        {
            if (id == null)
            {
                ViewBag.ErrorMessage = "ระบุ id";
                return RedirectToAction("Index");
            }
            var obj = _db.Employees.Find(id);
            if (obj == null)
            {
                ViewBag.ErrorMessage = "ไม่พบข้อมูล";
                return RedirectToAction("Index");
            }
            ViewData["Positions"] = new SelectList(_db.Positions, "PositionId", "PositionName", obj.EmployeeId);
            ViewBag.imgfile = "/imagem/" + obj.EmployeeId + ".png";
            return View(obj);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(string EmployeeId)
        {
            try
            {
                var obj = _db.Employees.Find(EmployeeId);
                if (obj == null)
                {
                    ViewBag.ErrorMessage = "ไม่พบข้อมูล";
                    return RedirectToAction("Index");
                }
                _db.Employees.Remove(obj);
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
            var FileName = theid;
            //var FileExtension = Path.GetExtension(imgfiles.FileName);
            var FileExtension = ".png";
            var SaveFileName = FileName + FileExtension;
            var SavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\imagem");
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
            var fileName = id.ToString() + ".png";
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\imagem");
            var filePath = Path.Combine(imagePath, fileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            return RedirectToAction("Edit", new { id = id });
        }

    }
}
