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
            using (var db = new DatabaseEntities())
            {
                var ul = db.Stocks.ToList();
                return View(ul);
            }
        }
        public ActionResult StockUpdate1(Stock stock)
        {
            using(var db = new DatabaseEntities())
            {
                var u = db.Stocks.Find(stock.itemNo);
                ViewBag.stock = u;
                return View(u);
            }
        }
        public ActionResult StockUpdate2(Stock stock)
        {
            using(var db = new DatabaseEntities())
            {
                var u = db.Stocks.Find(stock.itemNo);
                u.stock1 = stock.stock1;
                db.SaveChanges(); 
                return Redirect("Stock/StockList");
            }
        }
    }
}