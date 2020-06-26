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
        {
            if (Session["loginUserName"] == null)
            {
                // セッションが空だったらシステムエラー
                return RedirectToAction("EmployeeError", "Login");
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
                return RedirectToAction("CustomerError", "Login");
            }

            ViewBag.subTotal = subTotal;
            ViewBag.tax = tax;
            ViewBag.total = total;
            return View();
        }
        public ActionResult OrderSerch2(OrderInfo order, DateTime? deliveryFrom, DateTime? deliveryTo, DateTime? orderFrom, DateTime? orderTo,string backResult)
        {
            // セッション確認
            if (Session["loginUserName"] == null)
            {
                // セッションが空だったらシステムエラー
                return RedirectToAction("EmployeeError", "Login");
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
                    if(ul.Count == 0)
                    {
                        ViewBag.ErrorMessage = Properties.Settings.Default.p015_error_NoMatch;
                        return View("OrderSerch1");
                    }

                    ViewBag.customerId = order.customerId.ToString("000000");
                    ViewBag.orderNo = order.orderNo.ToString("00000000");
                    ViewBag.deliveryFrom = deliveryFrom;
                    ViewBag.deliveryTo = deliveryTo;
                    ViewBag.orderFrom = orderFrom;
                    ViewBag.orderTo = orderTo;
                    ViewBag.status = order.status;
                    return View(ul);
                }
                
                else
                {
                    var ul = db.OrderInfoes.WhereIf(order.customerId != 0, e => e.customerId.ToString().Contains(order.customerId.ToString()))
                                           .WhereIf(order.orderNo != 0, e => e.orderNo.ToString().Contains(order.orderNo.ToString()))
                                           .WhereIf(deliveryFrom != null, e => e.deliveryDate >= deliveryFrom & e.deliveryDate <= deliveryTo)
                                           .WhereIf(orderFrom != null, e => e.orderDate >= orderFrom & e.orderDate <= orderTo).ToList();

                    if (ul.Count == 0)
                    {
                        ViewBag.ErrorMessage = Properties.Settings.Default.p015_error_NoMatch;
                        return View("OrderSerch1");
                    }

                    ViewBag.customerId = order.customerId.ToString("000000");
                    ViewBag.orderNo = order.orderNo.ToString("000000");
                    ViewBag.deliveryFrom = deliveryFrom;
                    ViewBag.deliveryTo = deliveryTo;
                    ViewBag.orderFrom = orderFrom;
                    ViewBag.orderTo = orderTo;
                    ViewBag.status = order.status;
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
                return RedirectToAction("EmployeeError", "Login");
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
                return RedirectToAction("EmployeeError", "Login");
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
                        u.stock = u.stock + order.quantity;
                        orderStatus.status = order.status;
                    }
                    else
                    {
                        if (u.stock < order.quantity)
                        {
                            ViewBag.ErrorMessage = Properties.Settings.Default.p017_error_ShortageStock;
                            return View("OrderUpdate1", list);
                        }
                        u.stock = u.stock - order.quantity;
                        orderStatus.status = order.status;
                    }
                }
                db.SaveChanges();
                return View("OrderUpdate2", list);
            }
        }
        public ActionResult OrderCancel1(OrderInfo orderInfo,int orderNo)
        {
            // セッション確認
            if (Session["loginUserName"] == null)
            {
                // セッションが空だったらシステムエラー
                return RedirectToAction("CustomerError", "Login");
            }
            using (var db = new DatabaseEntities())
            {
                var ul = db.OrderInfoes.Where(e => e.orderNo == orderNo);
                var list = ul.ToList();
                foreach(var order in list)
                {
                    if (order.status != "未出荷")
                    {
                        ViewBag.ErrorMessage = Properties.Settings.Default.p011_error_CannotCancel;
                        
                        return StatusCheck();
                    }
                }
                var u = from e in db.OrderInfoes
                        where
                               e.orderNo == orderNo
                        group
                               e by e.orderNo;
                var orderlist = u.ToList();
                return View(orderlist);
            }
        }
        public ActionResult OrderCancel2(string back, string cancel,int customerId,int orderNo)
        {
            // セッション確認
            if (Session["loginUserName"] == null)
            {
                // セッションが空だったらシステムエラー
                return RedirectToAction("CustomerError", "Login");
            }
            using (var db = new DatabaseEntities())
            {
                //戻るボタンが押された
                if (back != null)
                {
                    //顧客IDを元に
                    var ul = from e in db.OrderInfoes
                             where
                                    e.customerId == customerId
                             group
                                    e by e.orderNo;
                    var list = ul.ToList();
                    return View("StatusCheck", list);
                }
                //キャンセルボタンが押された
                if (cancel != null)
                {
                    
                    var ul = db.OrderInfoes.Where(e => e.orderNo == orderNo).ToList();
                        foreach (var orderStatus in ul)
                    {
                        orderStatus.status = "キャンセル";
                    }
                    db.SaveChanges();
                    var order = from e in db.OrderInfoes
                             where
                                    e.customerId == customerId
                             group
                                    e by e.orderNo;
                    var list = order.ToList();
                    return View("StatusCheck",list);
                }
                return View();
            }
        }
        public ActionResult StatusCheck()
        {
            // セッション確認
            if (Session["loginUserName"] == null)
            {
                // セッションが空だったらシステムエラー
                return RedirectToAction("CustomerError", "Login");
            }
            string customerId = (string)Session["loginUserName"];
            int customerIdInt = int.Parse(customerId);
            using (var db = new DatabaseEntities())
            {
                var ul = from e in db.OrderInfoes
                         where
                                e.customerId == customerIdInt
                         group
                                e by e.orderNo;
                var list = ul.ToList();
                return View("StatusCheck",list);
            }
        }
    }
}