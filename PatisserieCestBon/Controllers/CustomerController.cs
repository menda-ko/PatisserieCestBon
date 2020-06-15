using Microsoft.Ajax.Utilities;
using PatisserieCestBon.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PatisserieCestBon.Controllers
{
    public class CustomerController : Controller
    {
        
        public ActionResult CustomerAdd1()
        {
            return View();
        }

        public ActionResult CustomerAdd2(string companyName, string address, string telNo, string customerName, string customerKana, string dept, string email, string password)
        {
            ViewBag.companyName = companyName;
            ViewBag.address = address;
            ViewBag.telNo = telNo;
            ViewBag.customerName = customerName;
            ViewBag.customerKana = customerKana;
            ViewBag.dept = dept;
            ViewBag.email = email;
            ViewBag.password = password;
            return View();
        }

        public ActionResult CustomerAdd3(string companyName, string address, string telNo, string customerName, string customerKana, string dept, string email, string password)
        {

            using (var db = new DatabaseEntities())
            {
                Customer model = new Customer
                {
                    companyName = companyName,
                    address = address,
                    telNo = telNo,
                    customerName = customerName,
                    customerKana = customerKana,
                    dept = dept,
                    email = email,
                    password = password
                };
                db.Customers.Add(model);
                //文字数？によっては動かない場合あり
                db.SaveChanges();
                ViewBag.model = model;
                ViewBag.customerId = model.customerId;
                return View();
            }
        }
    }
}