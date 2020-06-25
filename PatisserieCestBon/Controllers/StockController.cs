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
        public ActionResult StockUpdate2(int? quantity,DateTime? receiptDate, string receipt, string decrease, string update,decimal itemNo)
        {

            // セッション確認
            if (Session["loginUserName"] == null)
            {
                // セッションが空だったらシステムエラー
                return RedirectToAction("EmployeeError", "Login");
            }
            using (var db = new DatabaseEntities())
            {
                var stockList = db.Stocks.ToList();
                if (receipt != null)
                {
                    var u = db.Stocks.Find(itemNo);
                    u.stock = u.stock + quantity;
                    db.SaveChanges();
                    ViewBag.stock = u;
                    
                    ViewBag.InfoMessage = Properties.Settings.Default.p019_info_UpdateSuccess;
                    return View("StockList", stockList);
                }
                if(decrease != null)
                {
                    var u = db.Stocks.Find(itemNo);
                    if (u.stock == 0)
                    {
                        ViewBag.stock = u;
                        ViewBag.ErrorMessage = Properties.Settings.Default.p020_error_StockAlreadyZero;
                        return View("StockUpdate1",u);
                    }
                    if (u.stock < quantity)
                    {
                        ViewBag.stock = u;
                        ViewBag.ErrorMessage = Properties.Settings.Default.p020_error_UpdateStock;
                        return View("StockUpdate1",u);
                    }
                    else
                    {
                        u.stock = u.stock - quantity;
                        db.SaveChanges();
                        ViewBag.stock = u;
                        ViewBag.InfoMessage = Properties.Settings.Default.p019_info_UpdateSuccess;
                        return View("StockList", stockList);
                    }
                }
                if (update != null)
                {
                    var u = db.Stocks.Find(itemNo);
                    u.receiptDate = receiptDate;
                    db.SaveChanges();
                    ViewBag.stock = u;
                    ViewBag.InfoMessage = Properties.Settings.Default.p019_info_UpdateSuccess;
                    return View("StockList",stockList);
                }
                return Redirect("StockList");
            }
        }
    }
}