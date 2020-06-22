using PatisserieCestBon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PatisserieCestBon.Controllers
{
    public class CartController : Controller
    {
        
        public ActionResult CartCheck()
        {
            // セッション確認
            if (Session["loginUserName"] == null)
            {
                // セッションが空だったらシステムエラー
                return RedirectToAction("CustomerError", "Login");
            }
            using (var db = new DatabaseEntities())
            {                
                var cartList = db.CartInfoes.ToList();

                //小計
                int sum = 0;
                foreach (var cart in cartList)
                {
                    sum += (int)cart.quantity * (int)cart.unitPrice;
                }

                //小計、消費税、合計金額の計算
                ViewBag.subTotal = sum;
                var tax = sum * 0.08;
                ViewBag.tax = tax;
                ViewBag.total = sum + tax;

                ViewBag.cartList = cartList;

                return View();
            }
            
            
        }
    }
}