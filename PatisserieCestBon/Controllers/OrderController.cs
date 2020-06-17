using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PatisserieCestBon.Models;

namespace PatisserieCestBon.Controllers
{
    public class OrderController : Controller
    {
        // GET: Order
        public ActionResult OrderCancel1()
        {

            return View();
        }
        public ActionResult OrderCancel2()
        {
            return View();
        }
        public ActionResult StatusCheck()
        {
            using (var db = new DatabaseEntities())
            {
                var ul = db.OrderInfoes.Where(e => e.customerId == 1);
                var list = ul.ToList();

                return View(list);
            }
        }
    }
}