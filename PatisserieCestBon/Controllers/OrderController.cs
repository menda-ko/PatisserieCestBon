using Microsoft.Ajax.Utilities;
using PatisserieCestBon.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PatisserieCestBon.Controllers
{
    public class OrderController : Controller
    {
        //cartListの型が分からない
        public ActionResult Add(int subTotal, int tax, int total)
        {
            // セッション確認
            if (Session["loginUserName"] == null)
            {
                // セッションが空だったらシステムエラー
                return Redirect("CustomerError");
            }

            ViewBag.subTotal = subTotal;
            ViewBag.tax = tax;
            ViewBag.total = total;
            return View();
        }
    }
}