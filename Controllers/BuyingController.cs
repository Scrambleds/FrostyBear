using FrostyBear.Models;
using FrostyBear.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Dynamic;
using System.Globalization;

namespace KuShop.Controllers
{
    public class BuyingController : Controller
    {

        //สร้าง Field สำหรับใช้งาน DBContext ที่กำหนด
        private readonly FrostyBearContext _db;

        //สร้าง Constructor สำหรับตัว Controller เมื่อเริ่มต้นให้ใช้ Obj ของ DBContext
        public BuyingController(FrostyBearContext db)
        { _db = db; }

        public IActionResult Index()
        {

            if (HttpContext.Session.GetString("EmployeeUsername") == null)
            {
                
                return RedirectToAction("Shop", "Home");
            }
            DateOnly thedate = DateOnly.FromDateTime(DateTime.Now.Date);
            var pd = from b in _db.Buyings
                     join s in _db.Suppliers on b.SupId equals s.SupId into join_b_s
                     from b_s in join_b_s.DefaultIfEmpty()
                     where b.BuyDate == thedate
                     select new BuyVM
                     {
                         BuyId = b.BuyId,
                         SupId = b.SupId,
                         SupName = b_s.SupName,
                         BuyDate = b.BuyDate,
                         EmployeeId = b.EmployeeId,
                         BuyDocId = b.BuyDocId,
                         Saleman = b.Saleman,
                         BuyQty = b.BuyQty,
                         BuyMoney = b.BuyMoney,
                         BuyRemark = b.BuyRemark
                     };
            return View(pd);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(DateOnly thedate)
        {
            if (HttpContext.Session.GetString("EmployeeUsername") == null)
            {
                
                return RedirectToAction("Shop", "Home");
            }
            var pd = from b in _db.Buyings
                     join s in _db.Suppliers on b.SupId equals s.SupId into join_b_s
                     from b_s in join_b_s.DefaultIfEmpty()
                     where b.BuyDate == thedate
                     select new BuyVM
                     {
                         BuyId = b.BuyId,
                         SupId = b.SupId,
                         SupName = b_s.SupName,
                         BuyDate = b.BuyDate,
                         EmployeeId = b.EmployeeId,
                         BuyDocId = b.BuyDocId,
                         Saleman = b.Saleman,
                         BuyQty = b.BuyQty,
                         BuyMoney = b.BuyMoney,
                         BuyRemark = b.BuyRemark
                     };
            return View(pd);
        }

        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("EmployeeUsername") == null)
            {
                
                return RedirectToAction("Shop", "Home");
            }
            //GenID เพื่อไปแสดงใน View
            string theId;
            int rowCount;
            int i = 0;
            CultureInfo us = new CultureInfo("en-US");
            do
            {
                //สร้าง id จากปีและเดือนปัจจุบัน และต่อด้วย String 000x
                i++;
                theId = string.Concat(DateTime.Now.ToString("'BUY'yyyyMMdd", us), i.ToString("00"));
                //ทำการตรวจสอบว่ามี id ที่ซ้ำกันหรือไม่ วนจนกว่าจะไม่ซ่ำ
                var buying = from b in _db.Buyings
                             where b.BuyId.Equals(theId)
                             select b;
                rowCount = buying.ToList().Count;
            }
            while (rowCount != 0);
            //ส่งค่า ID ให้ View
            ViewBag.BuyId = theId;
            //อ่านข้อมูลจากตารางลง SelectList แล้วใส่ข้อมูลลงตัว ViewData
            ViewData["Sup"] = new SelectList(_db.Suppliers, "SupId", "SupName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Buying obj)
        {
            if (HttpContext.Session.GetString("EmployeeUsername") == null)
            {
                
                return RedirectToAction("Shop", "Home");
            }
            try
            {
                if (ModelState.IsValid)
                {
                    _db.Buyings.Add(obj);
                    _db.SaveChanges();
                    return RedirectToAction("Show", "Buying", new { buyid = obj.BuyId });
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Recording error";
                return View(obj);
            }
            TempData["ErrorMessage"] = "Recording error";
            return View(obj);
        }

        public IActionResult Show(string buyid)
        {
            if (HttpContext.Session.GetString("EmployeeUsername") == null)
            {
                return RedirectToAction("Shop", "Home");
            }
            //ตรวจสอบMaster
            if (buyid == null)
            {
                TempData["ErrorMessage"] = "Document number must be specified.";
                return RedirectToAction("Index");
            }
            //ได้ข้อมูล Buying เป็นส่วน Master
            var buying = (from b in _db.Buyings
                          where b.BuyId == buyid
                          select b).ToList();
            if (buying.ToList().Count == 0)
            {
                TempData["ErrorMessage"] = "No information found";
                return RedirectToAction("Index");
            }

            foreach (var s in buying)
            {
                // Access the Supplier using a separate database query
                ViewBag.SupName = _db.Suppliers.FirstOrDefault(sp => sp.SupId == s.SupId)?.SupName;
            }

            //เลือกข้อมูลของตะกร้าที่ระบุ 
            //สร้าง ViewModel ชื่อ CidVM เพื่อแสดงชื่อสินค้าของ CartDtl (ส่วน Detail)
            var buydtl = from bd in _db.BuyDtls
                         join p in _db.Products on bd.PdId equals p.ProductId into join_bd_p
                         from bd_p in join_bd_p.DefaultIfEmpty()
                         where bd.BuyId == buyid
                         select new BdVM
                         {
                             BuyId = bd.BuyId,
                             PdId = bd.PdId,
                             PdName = bd_p.ProductName,
                             BdtlMoney = bd.BdtlMoney,
                             BdtlPrice = bd.BdtlPrice,
                             BdtlQty = bd.BdtlQty
                         };
            ViewBag.theid = buyid;
            //สร้าง dynamic model
            //เพื่อส่งข้อมูลให้ View แบบสองตารางพร้อมกัน
            dynamic DyModel = new ExpandoObject();
            //แบ่งเป็นส่วน Master รับข้อมูลจาก Obj cart
            DyModel.Master = buying;
            //ส่วน Detail รับข้อมูลจาก Obj cartdtl
            DyModel.Detail = buydtl;
            //ส่ง Dynamic Model ไปที่ View
            return View(DyModel);
        }

        public IActionResult Edit(string buyid)
        {
            if (HttpContext.Session.GetString("EmployeeUsername") == null)
            {
                
                return RedirectToAction("Shop", "Home");
            }
            if (buyid == null)
            {
                TempData["ErrorMessage"] = "Document number must be specified.";
                return RedirectToAction("Index");
            }

            var obj = _db.Buyings.Find(buyid);
            if (obj == null)
            {
                TempData["ErrorMessage"] = "No information found";
                return RedirectToAction("Index");
            }
            ViewData["SupId"] = new SelectList(_db.Suppliers, "SupId", "SupName", obj.SupId);
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Buying obj)
        {
            if (HttpContext.Session.GetString("EmployeeUsername") == null)
            {
                
                return RedirectToAction("Shop", "Home");
            }
            try
            {
                if (ModelState.IsValid)
                {
                    _db.Buyings.Update(obj);
                    _db.SaveChanges();
                    return RedirectToAction("Show", "Buying", new { buyid = obj.BuyId });
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Recording error";
                return View(obj);
            }
            TempData["ErrorMessage"] = "Recording error";
            return View(obj);
        }

        public IActionResult Delete(string buyid)
        {
            if (HttpContext.Session.GetString("EmployeeUsername") == null)
            {
                
                return RedirectToAction("Shop", "Home");
            }
            ////////////////////////
            // ลบ Master
            var master = _db.Buyings.Find(buyid);
            _db.Buyings.Remove(master);
            _db.SaveChanges();

            ////////////////////////
            //ลบ Master แล้วต้องลบ Detail ด้วย
            //เลือกข้อมูลเป็น Set ต้องใช้ linq
            var detail = from bd in _db.BuyDtls
                         where bd.BuyId == buyid
                         select bd;
            var detailList = detail.ToList();

            // ทำการลูปคำสั่งลบข้อมูล
            foreach (var item in detailList)
            {
                _db.BuyDtls.Remove(item);

                // ปิด DataReader ก่อนที่จะดำเนินการดึงข้อมูลจากฐานข้อมูลอีกครั้ง
                _db.SaveChanges(); // หรือใช้วิธีอื่นๆ ในการปิด DataReader และทำการ Commit การเปลี่ยนแปลงที่เกิดขึ้นในฐานข้อมูล

                ////////////
                // ดำเนินการอื่นๆ เช่น การปรับปรุงยอดสินค้าใน Stock
                var pd = _db.Products.Find(item.PdId);
                pd.ProductStock = pd.ProductStock - item.BdtlQty;
            }
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult CreateDtl(string buyid)
        {
            if (HttpContext.Session.GetString("EmployeeUsername") == null)
            {
                
                return RedirectToAction("Shop", "Home");
            }
            if (buyid == null)
            {
                TempData["ErrorMessage"] = "Document number must be specified.";
                return RedirectToAction("Show", new { buyid = buyid });
            }
            //สร้าง SelectList เพื่อส่งข้อมูล Product ไปที่ View ให้เลือก
            ViewData["PdId"] = new SelectList(_db.Products, "ProductId", "ProductName");
            //ส่งหมายเลขเอกสารเพื่อแสดงผล
            ViewBag.BuyId = buyid;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateDtl(BuyDtl obj)
        {
            if (HttpContext.Session.GetString("EmployeeUsername") == null)
            {
                
                return RedirectToAction("Shop", "Home");
            }
            try
            {
                if (ModelState.IsValid)
                {
                    _db.BuyDtls.Add(obj);
                    _db.SaveChanges();

                    ///////////////
                    //ส่วนปรับยอด Master
                    //
                    //หายอดรวม Detail
                    var buymoney = _db.BuyDtls.Where(a => a.BuyId == obj.BuyId).Sum(b => b.BdtlMoney);
                    var buyqty = _db.BuyDtls.Where(a => a.BuyId == obj.BuyId).Sum(b => b.BdtlQty);
                    //Update Buying
                    var buy = _db.Buyings.Find(obj.BuyId);
                    buy.BuyQty = buyqty;
                    buy.BuyMoney = buymoney;
                    _db.SaveChanges();

                    ////////////
                    //Update Stock สินค้า
                    var pd = _db.Products.Find(obj.PdId);
                    //ปรับค่า Stock ปัจจุบัน(เพิ่ม) และ วันที่ขายวันสุดท้ายจากระบบ
                    pd.ProductStock = pd.ProductStock + obj.BdtlQty;
                    pd.PdLastBuy = buy.BuyDate;
                    pd.ProductCost = obj.BdtlPrice;
                    //_db.Entry(pd).State = EntityState.Modified;
                    _db.SaveChanges();

                    return RedirectToAction("Show", "Buying", new { buyid = obj.BuyId });
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Recording error";
                return RedirectToAction("Show", "Buying", new { buyid = obj.BuyId });
            }
            TempData["ErrorMessage"] = "Recording error";
            return RedirectToAction("Show", "Buying", new { buyid = obj.BuyId });
        }
        public IActionResult DeleteDtl(string pdid, string buyid)
        {
            if (HttpContext.Session.GetString("EmployeeUsername") == null)
            {
                
                return RedirectToAction("Shop", "Home");
            }
            ////////////////////////
            ///หา Detail
            var obj = _db.BuyDtls.Find(buyid, pdid);

            ////////////
            //Update Stock สินค้า
            var pd = _db.Products.Find(obj.PdId);
            //ปรับค่า Stock ปัจจุบัน(ลบ)
            pd.ProductStock = pd.ProductStock - obj.BdtlQty;
            //pd.PdLastBuy = buy.BuyDate;
            //pd.PdCost = obj.BdtlPrice;
            _db.SaveChanges();

            /////ลบ BuyDtls
            _db.BuyDtls.Remove(obj);
            _db.SaveChanges();

            ///////////////
            //ส่วนปรับยอด Master
            //
            //หายอดรวม Detail
            var buymoney = _db.BuyDtls.Where(a => a.BuyId == buyid).Sum(b => b.BdtlMoney);
            var buyqty = _db.BuyDtls.Where(a => a.BuyId == buyid).Sum(b => b.BdtlQty);
            //Update Buying
            var buy = _db.Buyings.Find(buyid);
            buy.BuyQty = buyqty;
            buy.BuyMoney = buymoney;
            _db.SaveChanges();

            return RedirectToAction("Show", "Buying", new { buyid = buyid });
        }
    }
}