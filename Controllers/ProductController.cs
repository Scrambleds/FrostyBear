    using FrostyBear.Models;
using Microsoft.AspNetCore.Mvc;
using FrostyBear.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using System.Threading;
using Microsoft.AspNetCore.Http;
namespace FrostyBear.Controllers
{
    public class ProductController : Controller
    {
        public readonly FrostyBearContext _db;

        public ProductController(FrostyBearContext db)
        { _db = db; }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("EmployeeUsername") == null)
            {
                
                return RedirectToAction("Shop", "Home");
            }

            var pdvm = from p in _db.Products
                       join pt in _db.Categories on p.CategoryId equals pt.CategoryId into join_p_pt
                       from p_pt in join_p_pt.DefaultIfEmpty()
                       join b in _db.Brands on p.BrandId equals b.BrandId into join_p_b
                       from p_b in join_p_b.DefaultIfEmpty()

                       select new PdVM
                       {
                           ProductId = p.ProductId,
                           ProductName = p.ProductName,
                           CategoryName = p_pt.CategoryName,
                           BrandName = p_b.BrandName,
                           ProductPrice = p.ProductPrice,
                           ProductCost = p.ProductCost,
                           ProductStock = p.ProductStock,
                           Detail = p.Detail
                       };

            if (pdvm == null) return NotFound();
            return View(pdvm);
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

            var pdvm = from p in _db.Products
                       join pt in _db.Categories on p.CategoryId equals pt.CategoryId into join_p_pt
                       from p_pt in join_p_pt.DefaultIfEmpty()
                       join b in _db.Brands on p.BrandId equals b.BrandId into join_p_b
                       from p_b in join_p_b.DefaultIfEmpty()
                       where p.ProductName.Contains(stext) ||
                             p_b.BrandName.Contains(stext) ||
                             p_pt.CategoryName.Contains(stext)
                       select new PdVM
                       {
                           ProductId = p.ProductId,
                           ProductName = p.ProductName,
                           CategoryName = p_pt.CategoryName,
                           BrandName = p_b.BrandName,
                           ProductPrice = p.ProductPrice,
                           ProductCost = p.ProductCost,
                           ProductStock = p.ProductStock,
                           Detail = p.Detail
                       };

            ViewBag.stext = stext;
            return View(pdvm);
        }

        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("EmployeeUsername") == null)
            {
                
                return RedirectToAction("Shop", "Home");
            }
            ViewData["Categories"] = new SelectList(_db.Categories, "CategoryId", "CategoryName");
            ViewData["Brand"] = new SelectList(_db.Brands, "BrandId", "BrandName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product obj,IFormFile imgfiles)
        {
            if (HttpContext.Session.GetString("EmployeeUsername") == null)
            {
                
                return RedirectToAction("Shop", "Home");
            }

            var lastprod = _db.Products
                                .OrderByDescending(p => p.ProductId)
                                .Select(p => p.ProductId)
                                .FirstOrDefault();
            try
            {
                if (ModelState.IsValid)
                {
                    string newprodId = lastprod.Substring(0, 1) + (int.Parse(lastprod.Substring(1)) + 1).ToString("D3");
                    obj.ProductId = newprodId;
                    if (imgfiles != null && imgfiles.Length > 0)
                    {
                        var FileName = newprodId;
                        var FileExtension = ".png";
                        var SaveFileName = FileName + FileExtension;
                        var SavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images");
                        var SaveFilePath = Path.Combine(SavePath, SaveFileName);

                        using (FileStream fs = System.IO.File.Create(SaveFilePath))
                        {
                            imgfiles.CopyTo(fs);
                            fs.Flush();
                        }
                    }
                    _db.Products.Add(obj);  
                    _db.SaveChanges(); 
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(obj);
            }
            ViewBag.ErrorMessage = "Recording error";
            return View(obj);
        }

        public IActionResult Edit(string id)
        {
            if (HttpContext.Session.GetString("EmployeeUsername") == null)
            {
                
                return RedirectToAction("Shop", "Home");
            }
            if (id == null)
            {
                ViewBag.ErrorMessage = "Please enter id";
                return RedirectToAction("Index");
            }
            var obj = _db.Products.Find(id);
            if (obj == null)
            {
                ViewBag.ErrorMessage = "No information found";
                return RedirectToAction("Index");
            }
            ViewData["Categories"] = new SelectList(_db.Categories, "CategoryId", "CategoryName", obj.ProductId);
            ViewData["Brand"] = new SelectList(_db.Brands, "BrandId", "BrandName", obj.BrandId);
            ViewBag.imgfile = "/images/" + obj.ProductId + ".png";
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Product obj)
        {
            if (HttpContext.Session.GetString("EmployeeUsername") == null)
            {
                
                return RedirectToAction("Shop", "Home");
            }
            try
            {
                if (ModelState.IsValid)
                {
                    _db.Products.Update(obj);
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(obj);
            }
            ViewBag.ErrorMessage = "Recording error";
            ViewData["Categories"] = new SelectList(_db.Categories, "CategoryId", "CategoryName", obj.ProductId);
            ViewData["Brand"] = new SelectList(_db.Brands, "BrandId", "BrandName", obj.BrandId);
            return View(obj);
        }

        public IActionResult Delete(String id)
        {
            if (HttpContext.Session.GetString("EmployeeUsername") == null)
            {
                
                return RedirectToAction("Shop", "Home");
            }
            if (id == null)
            {
                ViewBag.ErrorMessage = "Please enter id";
                return RedirectToAction("Index");
            }
            var obj = _db.Products.Find(id);
            if (obj == null)
            {
                ViewBag.ErrorMessage = "No information found";
                return RedirectToAction("Index");
            }
            ViewBag.pdtName = _db.Categories.FirstOrDefault(pt => pt.CategoryId == obj.CategoryId);
            ViewBag.brandName = _db.Brands.FirstOrDefault(br => br.BrandId == obj.BrandId);
            ViewBag.imgfile = "/images/" + obj.ProductId + ".png";
            return View(obj);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(string ProductId)
        {
            if (HttpContext.Session.GetString("EmployeeUsername") == null)
            {
                
                return RedirectToAction("Shop", "Home");
            }
            try
            {
                var obj = _db.Products.Find(ProductId);
                if (obj == null)
                {
                    ViewBag.ErrorMessage = "No information found";
                    return RedirectToAction("Index");
                }
                _db.Products.Remove(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [Route("/api/categories")]
        public IActionResult GetCategories()
        {
            if (HttpContext.Session.GetString("EmployeeUsername") == null)
            {
                
                return RedirectToAction("Shop", "Home");
            }
            var categories = _db.Categories.ToList();
            return Json(categories);
        }

        [HttpGet]
        [Route("/api/products")]
        public IActionResult GetProducts(int categoryId)
        {
            if (HttpContext.Session.GetString("EmployeeUsername") == null)
            {
                
                return RedirectToAction("Shop", "Home");
            }
            var products = _db.Products.Where(p => p.CategoryId == categoryId).ToList();
            return Json(products);
        }

        [HttpPost]
        public ActionResult GetStock(string PrefixPdId)
        {
            if (HttpContext.Session.GetString("EmployeeUsername") == null)
            {
                
                return RedirectToAction("Shop", "Home");
            }
            var stockRemain = (from p in _db.Products
                               where p.ProductId.StartsWith(PrefixPdId)
                               select p.ProductStock).FirstOrDefault();
            return Content(stockRemain.ToString());
        }
        public IActionResult ImgUpload(IFormFile imgfiles, string theid)
        {
            if (HttpContext.Session.GetString("EmployeeUsername") == null)
            {
                
                return RedirectToAction("Shop", "Home");
            }
            var FileName = theid;
            //var FileExtension = Path.GetExtension(imgfiles.FileName);
            var FileExtension = ".png";
            var SaveFileName = FileName + FileExtension;
            var SavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images");
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
            var fileName = id.ToString() + ".png";
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images");
            var filePath = Path.Combine(imagePath, fileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            return RedirectToAction("Edit", new { id = id });
        }
    }
}