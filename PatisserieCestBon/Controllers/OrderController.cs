using Microsoft.Ajax.Utilities;
using PatisserieCestBon.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using PatisserieCestBon.Models;

namespace PatisserieCestBon.Controllers
{
    public static class LinqExtension
    {
        public static IQueryable<TSource> WhereIf<TSource>
                            (this IQueryable<TSource> Source, bool Condition, Expression<Func<TSource, bool>> Predicate)
        {
            if (Condition)
                return Source.Where(Predicate);
            else
                return Source;
        }
    }
    public class OrderController : Controller
    {
        public ActionResult OrderSerch1()
        if (Session["loginUserName"] == null)
            {
                // セッションが空だったらシステムエラー
                return Redirect("EmployeeError");
            }
            return View();
        }
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
        public ActionResult OrderSerch2(OrderInfo order, DateTime? deliveryFrom, DateTime? deliveryTo, DateTime? orderFrom, DateTime? orderTo)
        {
            // セッション確認
            if (Session["loginUserName"] == null)
            {
                // セッションが空だったらシステムエラー
                return Redirect("EmployeeError");
            }
            if (!ModelState.IsValid)
            {
                return View("OrderSerch1");
            }
            using (var db = new DatabaseEntities())
            {
                //検索条件のステータスがすべてじゃない
                if (order.status != "すべて")
                {
                    var ul = db.OrderInfoes.WhereIf(order.customerId != 0, e => e.customerId.ToString().Contains(order.customerId.ToString()))
                                           .WhereIf(order.orderNo != 0, e => e.orderNo.ToString().Contains(order.orderNo.ToString()))
                                           .WhereIf(deliveryFrom != null, e => e.deliveryDate >= deliveryFrom & e.deliveryDate <= deliveryTo)
                                           .WhereIf(orderFrom != null, e => e.orderDate >= orderFrom & e.orderDate <= orderTo)
                                           .WhereIf(order.status != null, e => e.status == order.status).ToList();
                    return View(ul);
                }
                else
                {
                    var ul = db.OrderInfoes.WhereIf(order.customerId != 0, e => e.customerId.ToString().Contains(order.customerId.ToString()))
                                           .WhereIf(order.orderNo != 0, e => e.orderNo.ToString().Contains(order.orderNo.ToString()))
                                           .WhereIf(deliveryFrom != null, e => e.deliveryDate >= deliveryFrom & e.deliveryDate <= deliveryTo)
                                           .WhereIf(orderFrom != null, e => e.orderDate >= orderFrom & e.orderDate <= orderTo).ToList();
                    ViewBag.order = order;
                    ViewBag.deliveryFrom = deliveryFrom;
                    ViewBag.deliveryTo = deliveryTo;
                    ViewBag.orderFrom = orderFrom;
                    ViewBag.orderTo = orderTo;
                    return View(ul);
                }
            }
        }
        public ActionResult OrderUpdate1(OrderInfo order)
        {
            // セッション確認
            if (Session["loginUserName"] == null)
            {
                // セッションが空だったらシステムエラー
                return Redirect("EmployeeError");
            }
            using (var db = new DatabaseEntities())
            {
                var ul = db.OrderInfoes.Where(e => e.orderNo == order.orderNo & e.orderSeqNo == order.orderSeqNo).ToList();
                return View(ul);
            }
        }
        public ActionResult OrderUpdate2(OrderInfo order)
        {
            // セッション確認
            if (Session["loginUserName"] == null)
            {
                // セッションが空だったらシステムエラー
                return Redirect("EmployeeError");
            }
            using (var db = new DatabaseEntities())
            {
                var ul = db.OrderInfoes.Where(e => e.orderNo == order.orderNo & e.orderSeqNo == order.orderSeqNo);
                var list = ul.ToList();
                var u = db.Stocks.Find(order.itemNo);
                foreach (var orderStatus in list)
                {
                    if (order.status == "未出荷")
                    {
                        u.stock1 = u.stock1 + order.quantity;
                        orderStatus.status = order.status;
                    }
                    else
                    {
                        if (u.stock1 < order.quantity)
                        {
                            ModelState.AddModelError(string.Empty, PatisserieCestBon.Properties.Settings.Default.p017_error_ShortageStock);
                            return View("OrderUpdate1", list);
                        }
                        u.stock1 = u.stock1 - order.quantity;
                        orderStatus.status = order.status;
                    }
                }
                db.SaveChanges();
                return View("OrderUpdate2", list);
            }
        }
        public ActionResult OrderCancel1(OrderInfo orderInfo,Decimal groupKey)
        {
            // セッション確認
            if (Session["loginUserName"] == null)
            {
                // セッションが空だったらシステムエラー
                return Redirect("CustomerError");
            }
            using (var db = new DatabaseEntities())
            {
                var ul = from e in db.OrderInfoes
                         where
                               e.orderNo == groupKey
                         group
                               e by e.orderNo;
                var list = ul.ToList();
                return View(ul);
            }
        }
        public ActionResult OrderCancel2(OrderInfo orderInfo, string back, string cancel)
        {
            // セッション確認
            if (Session["loginUserName"] == null)
            {
                // セッションが空だったらシステムエラー
                return Redirect("CustomerError");
            }
            using (var db = new DatabaseEntities())
            {
                //戻るボタンが押された
                if (back != null)
                {
                    //顧客IDを元に
                    var ul = db.OrderInfoes.Where(e => e.customerId == orderInfo.customerId).ToList();
                    return View("StatusCheck", ul);
                }
                //キャンセルボタンが押された
                if (cancel != null)
                {
                    var ul = db.OrderInfoes.Where(e => e.orderNo == orderInfo.orderNo).ToList();
                    foreach (var orderStatus in ul)
                    {
                        orderStatus.status = "キャンセル";
                    }
                    db.SaveChanges();
                    return View("StatusCheck", ul);
                }
                return View();
            }
        }
        public ActionResult StatusCheck(OrderInfo orderInfo)
        {
            // セッション確認
            if (Session["loginUserName"] == null)
            {
                // セッションが空だったらシステムエラー
                return Redirect("CustomerError");
            }
            using (var db = new DatabaseEntities())
            {
                var ul = from e in db.OrderInfoes
                         where
                                e.customerId == orderInfo.customerId
                         group
                                e by e.orderNo;
                var list = ul.ToList();
                return View(list);
            }
        }
    }
}