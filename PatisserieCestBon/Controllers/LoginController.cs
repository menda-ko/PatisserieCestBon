using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PatisserieCestBon.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult CustomerMenu()
        {
            // 顧客メニュー画面を表示
            return View();
        }

        public ActionResult EmployeeMenu()
        {
            // 担当者メニュー画面を表示
            return View();
        }
    }
}