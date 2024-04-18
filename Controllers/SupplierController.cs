using FrostyBear.Models;
using FrostyBear.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FrostyBear.Controllers
{
    public class SupplierController : Controller
    {

        public readonly FrostyBearContext _db;
        public SupplierController(FrostyBearContext db)
        { _db = db; }

        public IActionResult Index()
        {
            var svm = from s in _db.Suppliers
                      select new SVM
                      {
                          SupId = s.SupId,
                          SupName = s.SupName,
                          SupContact = s.SupContact,
                          SupTel = s.SupTel,
                          SupEmail = s.SupEmail,
                          SupAdd = s.SupAdd,
                          SupRemark = s.SupRemark,
                      };
            if (svm == null) return NotFound();
            return View(svm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(string? stext)
        {
            if (stext == null)
            {
                return RedirectToAction("Index");
            }


            var svm = from s in _db.Suppliers
                      where s.SupName.Contains(stext) ||
                      s.SupTel.Contains(stext) ||
                      s.SupEmail.Contains(stext) ||
                      s.SupAdd.Contains(stext)
                      select new SVM
                      {
                          SupId = s.SupId,
                          SupName = s.SupName,
                          SupContact = s.SupContact,
                          SupTel = s.SupTel,
                          SupEmail = s.SupEmail,
                          SupAdd = s.SupAdd,
                          SupRemark = s.SupRemark,
                      };

            ViewBag.stext = stext;
            return View(svm);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Supplier obj)
        {
            var lastsp = _db.Suppliers
                                .OrderByDescending(e => e.SupId)
                                .Select(e => e.SupId)
                                .FirstOrDefault();
            try
            {
                if (ModelState.IsValid)
                {
                    string newspId = lastsp.Substring(0, 1) + (int.Parse(lastsp.Substring(1)) + 1).ToString("D3");
                    obj.SupId = newspId;
                    _db.Suppliers.Add(obj);
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
            var obj = _db.Suppliers.Find(id);

            if (obj == null)
            {
                ViewBag.ErrorMessage = "ไม่พบข้อมูล";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Supplier obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _db.Suppliers.Update(obj);
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
            if (id == null)
            {
                ViewBag.ErrorMessage = "ระบุ id";
                return RedirectToAction("Index");
            }
            var obj = _db.Suppliers.Find(id);

            if (obj == null)
            {
                ViewBag.ErrorMessage = "ไม่พบข้อมูล";
                return RedirectToAction("Index");
            }
            return View(obj);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(string SupId)
        {
            try
            {
                //SupId
                var obj = _db.Suppliers.Find(SupId);
                if (obj == null)
                {
                    ViewBag.ErrorMessage = "ไม่พบข้อมูล";
                    return RedirectToAction("Index");
                }
                _db.Suppliers.Remove(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}