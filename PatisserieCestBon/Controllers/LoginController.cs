﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PatisserieCestBon.Models;
using System.Web.Security;

namespace PatisserieCestBon.Controllers
{
    public class LoginController : Controller
    {
        private DatabaseEntities db = new DatabaseEntities();
        // その１　フロントエンド
        // メニュー画面表示
        public ActionResult CustomerMenu()
        {
            if (Session["loginUserName"] == null)
            {
                // セッションが空だったらシステムエラー
                return RedirectToAction("CustomerError", "Login");
            }
            // 顧客メニュー画面を表示
            return View();
        }

        //ログイン画面表示
        public ActionResult CustomerLogin()
        {
            // 認証済であればメニュー画面を表示
            if (Request.IsAuthenticated)
            {
                return View("CustomerMenu");
            }
            // 認証されていない場合、ログイン画面を表示
            return View("CustomerLogin");
        }

        // 認証を行うメソッド
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CustomerLogin(string inputCustomerId, string inputPassword)
        {
                string sessionId = (string)Session["loginUserName"];
                if (sessionId == null)
                {
                try
                {
                    // セッションが空だったら認証処理開始
                    if (!string.IsNullOrWhiteSpace(inputCustomerId) && !string.IsNullOrWhiteSpace(inputPassword))
                    {
                        /* IDとパスワード両方入力されていた場合、DBと照合開始（ValidateUser）
                         * 一致するレコードがあれば認証成功 → メニュー画面を表示 */
                        if (ValidateCustomer(inputCustomerId, inputPassword))
                        {
                            FormsAuthentication.SetAuthCookie(userName: inputCustomerId, createPersistentCookie: false);
                            /* userName … 認証済みユーザの名前
                             * create～～ … 複数のブラウザーセッションにわたって保存される
                             *                     永続的なクッキーを作成する場合はtrue。それ以外の場合はfalse */
                            Session["loginUserName"] = inputCustomerId;      // セッションに認証済みユーザの名前を格納
                            return RedirectToAction(actionName: "CustomerMenu");
                        }
                        // IDまたはパスワードが誤っていた場合は画面遷移せずエラーメッセージを表示
                        ViewBag.ErrorMessage = Properties.Settings.Default.p001_error_Auth;
                        return View("CustomerLogin");
                    }
                }
                // 例外発生時はシステムエラー画面へ
                catch
                {
                    return View("CustomerError");
                }
            // IDまたはパスワードが空欄だった場合は画面遷移せずエラーメッセージを表示
            ViewBag.ErrorMessage = Properties.Settings.Default.p001_error_Recuired;
                    return View("CustomerLogin");
                }
                // セッションが空でなければメニュー画面にリダイレクト
                return RedirectToAction(actionName: "CustomerMenu");
            }

        private bool ValidateCustomer(string inputCustomerId, string inputPassword)
        {
            foreach (var customer in db.Customers)
            {
                /* DBに登録されている顧客IDをすべて6桁0埋めした文字列に加工して照合。 
                * 入力されたIDとパスワードともに一致するものがあれば認証完了 */
                string customerIdString = customer.customerId.ToString("000000");
                if (customerIdString == inputCustomerId && customer.password == inputPassword)
                {
                    return customer != null;
                }
            }
            // ID・パスワード両方一致するレコードがなければ認証失敗
            return false;
        }
        
        // ログアウト処理
        public ActionResult CustomerLogout()
    {
        if (Session != null)
        {
            Session.Remove("loginUserName");  // セッションのキーを削除
        }
        FormsAuthentication.SignOut();
        return Redirect("CustomerLogin");
    }

        /* その２　バックオフィス
         *  フロントエンドとほぼ同じなのでコメントは省略 */
    public ActionResult EmployeeMenu()
        {
            if (Session["loginUserName"] == null)
            {
                return RedirectToAction("EmployeeError", "Login");
            }
            return View();
        }
        
        public ActionResult EmployeeLogin()
        {
            if (Request.IsAuthenticated)
            {
                return View("EmployeeMenu");
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EmployeeLogin(string inputEmpNo, string inputPassword)
        {
                string sessionId = (string)Session["loginUserName"];
                if (sessionId == null)
                {
                try
                {
                    if (!string.IsNullOrWhiteSpace(inputEmpNo) && !string.IsNullOrWhiteSpace(inputPassword))
                    {
                        if (ValidateEmployee(inputEmpNo, inputPassword))
                        {
                            FormsAuthentication.SetAuthCookie(userName: inputEmpNo, createPersistentCookie: false);
                            Session["loginUserName"] = inputEmpNo;
                            return RedirectToAction(actionName: "EmployeeMenu");
                        }
                        ViewBag.ErrorMessage = Properties.Settings.Default.p013_error_Auth;
                        return View("EmployeeLogin");
                    }
                }
                catch
                {
                    return View("EmployeeError");
                }
                    ViewBag.ErrorMessage = Properties.Settings.Default.p013_error_Recuired;
                    return View("EmployeeLogin");
                }
                return RedirectToAction(actionName: "EmployeeMenu");
        }

        private bool ValidateEmployee(string inputEmpNo, string inputPassword)
        {
            foreach (var employee in db.Employees)
            {
                string customerIdString = employee.empNo.ToString();
                if (customerIdString == inputEmpNo && employee.password == inputPassword)
                {
                    return employee != null;
                }
            }
            return false;
        }
        public ActionResult EmployeeLogout()
        {
            if (Session != null)
            {
                Session.Remove("loginUserName");
            }
            FormsAuthentication.SignOut();
            return Redirect("EmployeeLogin");
        }

        // システムエラー画面に飛ばす処理
        public ActionResult CustomerError()
        {
            return View("CustomerError");
        }
        public ActionResult EmployeeError()
        {
            return View("EmployeeError");
        }
    }
}