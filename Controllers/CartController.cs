using FrostyBear.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Dynamic;
using FrostyBear.ViewModels;
using Microsoft.Identity.Client;
using System;

namespace FrostyBear.Controllers
{
    public class CartController : Controller
    {
        private readonly FrostyBearContext _db;

        public CartController(FrostyBearContext db)
        { _db = db; }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("CusId") == null)
            {
                return RedirectToAction("Index", "ShopLogin");
            }
            return View();
        }
        public IActionResult AddDtl(string pdid)
        {
           

            //ตรวจสอบ Login??
            if (HttpContext.Session.GetString("CusId") == null)
            {
              
                return RedirectToAction("Index", "ShopLogin"); //ยังไม่ได้สร้าง ต้องไปสร้างก่อน
            }
            //ตรวจสอบว่ามี pdid ส่งมา
            if (pdid == null)
            {
                TempData["ErrorMessage"] = "Must specify product code";
                return RedirectToAction("Index", "Home");
            }
            //ตรวจสอบว่ามีตะกร้า?? ถ้าไม่มีก็สร้างตะกร้า
            if (HttpContext.Session.GetString("CartId") == null)
            {
                return RedirectToAction("Add", new { pdid = pdid }); //ยังไม่ได้สร้าง ต้องไปสร้างก่อน
            }

            //ถ้ามีแล้วบันทึกข้อมูลสินค้าลงตะกร้า
            //หาข้อมูลสินค้า - ราคาขาย
            var pd = _db.Products.Find(pdid);

            string CartId = HttpContext.Session.GetString("CartId");

            var cdtl = from cd in _db.CartDtls
                       where cd.CartId.Equals(CartId)
                       && cd.ProductId.Equals(pdid)
                       select cd;
            if (cdtl.Count() == 0)
            {
                CartDtl obj = new CartDtl();
                obj.CartId = CartId;
                obj.ProductId = pdid;
                obj.CdtlQty = 1;
                obj.CdtlPrice = pd.ProductPrice;
                obj.CdtlMoney = pd.ProductPrice * 1;
                _db.Entry(obj).State = EntityState.Added;
            }
            else
            {
                foreach (var obj in cdtl)
                {
                    obj.CdtlQty = obj.CdtlQty + 1;
                    obj.CdtlMoney = pd.ProductPrice * (obj.CdtlQty);
                    _db.Entry(obj).State = EntityState.Modified;
                }
            }
            _db.SaveChanges();

            //เมื่อ Detail เปลี่ยน Master ก็เปลี่ยน
            //หายอดรวมของ Detail
            var CartMoney = _db.CartDtls.Where(a => a.CartId == CartId).Sum(a => a.CdtlMoney);
            var CartQty = _db.CartDtls.Where(a => a.CartId == CartId).Sum(a => a.CdtlQty);

            //Update db
            var cart = _db.Carts.Find(CartId);
            cart.CartQty = CartQty;
            cart.CartMoney = CartMoney;
            _db.SaveChanges();

            //Update Session เพื่อแสดงผล
            HttpContext.Session.SetString("CartQty", CartQty.ToString());
            HttpContext.Session.SetString("CartMoney", CartMoney.ToString());

            return RedirectToAction("Index", "Home");

        }

        public IActionResult Add(string pdid)
        {
            if (HttpContext.Session.GetString("CusId") == null)
            {
                return RedirectToAction("Index", "ShopLogin");
            }
            //Gen CartId
            string theId;
            int rowCount = 0;
            int i = 0;
            string today;
            string CusId = HttpContext.Session.GetString("CusId");

            CultureInfo us = new CultureInfo("en-US");
            do
            {
                //สร้าง id จากปีเดิอนปัจจุบันต่อด้วยลำดับ รูปแบบ 000x
                i++;
                today = DateTime.Now.ToString("'CT'yyyyMMdd");
                theId = string.Concat(today, i.ToString("0000"));
                //ทำการตรวจสอบว่ามี CartId นี้แล้วหรือยัง
                var cart = from ct in _db.Carts
                           where ct.CartId.Equals(theId)
                           select ct;
                rowCount = cart.Count();
            } while (rowCount != 0);
            //ทำการบันทึกลง Table Cart
            try
            {
                //สร้าง Obj Cart และกำหนด Field ต่างๆ
                Cart obj = new Cart();
                obj.CartId = theId;
                obj.CustomerId = CusId;
                obj.CartDate = DateOnly.FromDateTime(DateTime.Now.Date);
                obj.CartQty = 0;
                obj.CartMoney = 0;
                //กำหนดสถานะทำงานเป็น Add และสั่งบันทึก
                _db.Entry(obj).State = EntityState.Added;
                _db.SaveChanges();

                //บันทึกลง Session Cart
                HttpContext.Session.SetString("CartId", theId);
                HttpContext.Session.SetString("CartQty", "0");
                HttpContext.Session.SetString("CartMoney", "0");

                return RedirectToAction("AddDtl", new { pdid = pdid });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Recording error";
                return RedirectToAction("Index", "Home");
            }


        }

        public IActionResult Show(string cartid)
        {
            if (HttpContext.Session.GetString("CusId") == null)
            {
                return RedirectToAction("Index", "ShopLogin");
            }
            //Master
            //ตรวจสอบว่าเป็นตะกร้าของลูกค้าที่ใช้งานอยู่หรือไม่
            //ได้ข้อมูล Cart เป็นส่วน Master
            string cusid = HttpContext.Session.GetString("CusId");
            var cart = from ct in _db.Carts
                       where ct.CartId == cartid &&
                           ct.CustomerId == cusid
                       select ct;
            if (cart == null)
            {
                TempData["ErrorMessage"] = "ไม่พบตะกร้าที่ระบุ";
                return RedirectToAction("Index", "Home");
            }

            //Detail เลือกข้อมูลของตะกร้า+สร้าง ViewModel CtdVM แสดงชื่อสินค้า
            var cartdtl = from ctd in _db.CartDtls
                          join p in _db.Products on ctd.ProductId equals p.ProductId
                              into join_ctd_p
                          from ctd_p in join_ctd_p.DefaultIfEmpty()
                          where ctd.CartId == cartid
                          select new CtdVM
                          {
                              CartId = ctd.CartId,
                              ProductId = ctd.ProductId,
                              ProductName = ctd_p.ProductName,
                              CdtlMoney = ctd.CdtlMoney,
                              CdtlPrice = ctd.CdtlPrice,
                              CdtlQty = ctd.CdtlQty
                          };
            //สร้าง Dynamic model เพื่อส่งข้อมูลให้ View เป็นสองตารางพร้อมกัน
            dynamic DyModel = new ExpandoObject();
            //ระบุส่วน Master รับข้อมูลจาก Obj cart
            DyModel.Master = cart;
            //ระบุส่วน Detail รับข้อมูลจาก Obj cartdtl
            DyModel.Detail = cartdtl;
            //ส่ง Dynamic Model ไปที่ View
            return View(DyModel);
        }
        public IActionResult Check()
        {
            if (HttpContext.Session.GetString("CusId") == null)
            {
                return RedirectToAction("Index", "ShopLogin");
            }
            // ตรวจสอบตะกร้า ของลูกค้าปัจจุบีน ที่ยังไม่ได้ทำการ CF - ถ้ามีค้างให้ใช้ CartId นั้น
            string cusid = HttpContext.Session.GetString("CusId");
            var cart = from ct in _db.Carts
                       where ct.CustomerId.Equals(cusid) && ct.CartCf != "Y"
                       select ct;
            int rowCount = cart.Count();
            // ถ้ามีตะกร้าค้าง
            if (rowCount > 0)
            {
                Cart obj = new Cart();
                foreach (var item in cart)
                {
                    obj = item;
                }
                //กำหนด Session ต่างๆของตะกร้า
                HttpContext.Session.SetString("CartId", obj.CartId);
                HttpContext.Session.SetString("CartQty", obj.CartQty.ToString());
                HttpContext.Session.SetString("CartMoney", obj.CartMoney.ToString());
            }
            return RedirectToAction("Shop", "Home");
        }
        public IActionResult Delete(string cartid)
        {
            if (HttpContext.Session.GetString("CusId") == null)
            {
                return RedirectToAction("Index", "ShopLogin");
            }
            //การลบตะกร้า คือลบทั้งเอกสาร ดั้งนั้นต้องลบตัวMaster และ Detail ด้วย
            //ลบส่วน Detail
            //เลือกรายการที่อยู่ในตะกร้า
            var detail = from ctd in _db.CartDtls
                         where ctd.CartId.Equals(cartid)
                         select ctd;
            //วน Loop ไล่ลบที่ละรายการ
            foreach (var item in detail)
            {
                _db.CartDtls.Remove(item);
            }
            _db.SaveChanges();

            //ลบส่วน Master
            //หาเอกสารที่ระบุ
            var master = _db.Carts.Find(cartid);
            if (master == null)
            {
                TempData["ErrorMessage"] = "ไม่พบตะกร้า";
                return RedirectToAction("Show", "Cart", new { cartid = cartid });
            }
            _db.Carts.Remove(master);
            _db.SaveChanges();

            //ลบตะกร้าแล้ว ลบ Session ด้วย
            HttpContext.Session.Remove("CartId");
            HttpContext.Session.Remove("CartQty");
            HttpContext.Session.Remove("CartMoney");

            TempData["SuccessMessage"] = "ยกเลิกคำสั่งซื้อแล้ว";
            return RedirectToAction("Shop", "Home");
        }
        public IActionResult DeleteDtl(string pdid, string cartid) 
        {
            if (HttpContext.Session.GetString("CusId") == null)
            {
                return RedirectToAction("Index", "ShopLogin");
            }
            if (HttpContext.Session.GetString("CusId") == null)
            {
                return RedirectToAction("Index", "ShopLogin");
            }
            //ลบ Detail แต่ไม่ต้องวน Loop เพราะเลือกสินค้ามารายการเดียว
            var obj = _db.CartDtls.Find(cartid, pdid);
            if (obj == null)
            {
                TempData["ErrorMessage"] = "ไม่พบข้อมูล";
                return RedirectToAction("Show", "Cart", new { cartid = cartid });
            }
            _db.CartDtls.Remove(obj);
            _db.SaveChanges();

            //เมื่อ Detail เปลี่ยน ทำการปรับยอดของ Master
            //เหมือนกับ AddDtl
            var cartmoney = _db.CartDtls.Where(a => a.CartId == cartid).Sum(a => a.CdtlMoney);
            var cartqty = _db.CartDtls.Where(a => a.CartId == cartid).Sum(a => a.CdtlQty);

            //ถ้าจำนวนสินค้าเป็น 0 ก็ลบตะกร้าทิ้ง
            if (cartqty == 0)
            {
                //ลบ Master
                var master = _db.Carts.Find(cartid);
                _db.Carts.Remove(master);
                _db.SaveChanges();

                //ลบตะกร้าแล้ว ลบSession ด้วย
                HttpContext.Session.Remove("CartId");
                HttpContext.Session.Remove("CartQty");
                HttpContext.Session.Remove("CartMoney");

                TempData["SuccessMessage"] = "ยกเลิกคำสั่งซื้อแล้ว";
                return RedirectToAction("Shop", "Home");
            }
            else
            {
                //Update Cart
                var cart = _db.Carts.Find(cartid);
                cart.CartQty = cartqty;
                cart.CartMoney = cartmoney;
                _db.SaveChanges();

                //Update Session
                HttpContext.Session.SetString("CartMoney", cartmoney.ToString());
                HttpContext.Session.SetString("CartQty", cartqty.ToString());

                return RedirectToAction("Show", "Cart", new { cartid = cartid });
            }
        }
        public IActionResult Confirm(string cartid)
        {
            if (HttpContext.Session.GetString("CusId") == null)
            {
                return RedirectToAction("Index", "ShopLogin");
            }
            if (HttpContext.Session.GetString("CusId") == null)
            {
                return RedirectToAction("Index", "ShopLogin");
            }
            // Fetch all products related to the cart details
            var productIds = _db.CartDtls.Where(ctd => ctd.CartId == cartid).Select(ctd => ctd.ProductId).ToList();
            var products = _db.Products.Where(p => productIds.Contains(p.ProductId)).ToList();

            foreach (var detail in _db.CartDtls.Where(ctd => ctd.CartId.Equals(cartid)))
            {
                // Find the corresponding product in the fetched products list
                var product = products.FirstOrDefault(p => p.ProductId == detail.ProductId);
                if (product != null)
                {
                    // Update Stock and LastSale date
                    product.ProductStock -= detail.CdtlQty;
                    product.PdLastSale = DateOnly.FromDateTime(DateTime.Now.Date); // Or use your preferred DateOnly type

                    // Update EntityState
                    _db.Entry(product).State = EntityState.Modified;
                }
            }

            _db.SaveChanges();

            // Update cart confirmation status
            var master = _db.Carts.Find(cartid);
            if (master != null)
            {
                master.CartCf = "Y";
                _db.Entry(master).State = EntityState.Modified;
                _db.SaveChanges();
            }

            // Clear session
            HttpContext.Session.Remove("CartId");
            HttpContext.Session.Remove("CartQty");
            HttpContext.Session.Remove("CartMoney");

            TempData["SuccessMessage"] = "ยืนยันคำสั่งซื้อแล้ว";
            return RedirectToAction("Shop", "Home");
        }
        public IActionResult List(string cusid) 
        {
            if (HttpContext.Session.GetString("CusId") == null)
            { 
                return RedirectToAction("Index", "ShopLogin");
            }
            //เลือกตะกร้าทั้งหมด ที่เป็นของลูกค้าที่ระบุ
            var cart = from c in _db.Carts
                       where c.CustomerId.Equals(cusid)
                       orderby c.CartId descending
                       select c;
            return View(cart);
        }
    }
}