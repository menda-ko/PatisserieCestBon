using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PatisserieCestBon.Models;

namespace PatisserieCestBon.Controllers
{
    public class StockController : Controller
    {
        // GET: Stock
        public ActionResult StockList()
        {
            // セッション確認
            if (Session["loginUserName"] == null)
            {
                // セッションが空だったらシステムエラー
                return RedirectToAction("EmployeeError", "Login");
            }
            using (var db = new DatabaseEntities())
            {
                var ul = db.Stocks.ToList();
                ViewBag.stock = ul;
                return View(ul);
            }
        }
        public ActionResult StockUpdate1(Stock stock)
        {
            // セッション確認
            if (Session["loginUserName"] == null)
            {
                // セッションが空だったらシステムエラー
                return RedirectToAction("EmployeeError", "Login");
            }
            using (var db = new DatabaseEntities())
            {
                var u = db.Stocks.Find(stock.itemNo);
                ViewBag.stock = u;
                return View(u);
            }
        }
        public ActionResult StockUpdate2(Stock stock,string receipt, string decrease, string update)
        {
            // セッション確認
            if (Session["loginUserName"] == null)
            {
                // セッションが空だったらシステムエラー
                return RedirectToAction("EmployeeError", "Login");
            }
            using (var db = new DatabaseEntities())
            {
                if (receipt != null)
                {
                    var u = db.Stocks.Find(stock.itemNo);
                    
                    u.stock = u.stock + stock.stock;
                    db.SaveChanges();
                    ViewBag.stock = u;
                    ModelState.AddModelError(string.Empty, PatisserieCestBon.Properties.Settings.Default.p019_info_UpdateSuccess);
                    return Redirect("StockList");
                }
                if(decrease != null)
                {
                    var u = db.Stocks.Find(stock.itemNo);
                    if (u.stock == 0)
                    {
                        ModelState.AddModelError(string.Empty, PatisserieCestBon.Properties.Settings.Default.p020_error_StockAlreadyZero);
                        ViewBag.stock = u;
                        return View("StockUpdate1", stock);
                    }
                    if (u.stock < stock.stock)
                    {
                        ModelState.AddModelError(string.Empty, PatisserieCestBon.Properties.Settings.Default.p020_error_UpdateStock);
                        ViewBag.stock = u;
                        return View("StockUpdate1", stock);
                    }
                    else
                    {
                        u.stock = u.stock - stock.stock;
                        db.SaveChanges();
                        ViewBag.stock = u;
                        return Redirect("StockList");
                    }
                }
                if (update != null)
                {
                    var u = db.Stocks.Find(stock.itemNo);
                    u.receiptDate = stock.receiptDate;
                    db.SaveChanges();
                    ViewBag.stock = u;
                    return Redirect("StockList");
                }
                return Redirect("StockList");
            }
        }
    }
}