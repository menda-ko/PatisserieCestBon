using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PatisserieCestBon.Models;
using System.Web.Security;

namespace PatisserieCestBon.Controllers
{
    [Authorize]     /* ★[Authorize]属性
                                 認証されたユーザのみアクセス可能にする属性。
    			               （LoginControllerクラス全体に対して付与される） */
    public class LoginController : Controller
    {
        private DatabaseEntities db = new DatabaseEntities();
        // その１　フロントエンド
        public ActionResult CustomerMenu()
        {
            string session = (string)Session["loginusername"];
            if (session == null)
            {
                // セッションが空だったらログイン画面にリダイレクト
                return Redirect("CustomerLogin");
            }
            // 顧客メニュー画面を表示
            return View();
        }

        [AllowAnonymous]    /* ★[AllowAnonymous]属性
                                                認証されていないユーザでもアクセス可能にする属性。
        				                        メソッドに個別に付与することができ、付与されたメソッドは
        				                        クラス全体に付与された[Authorize]属性を無視する。 */
        public ActionResult CustomerLogin()
        {
            // 認証済であればメニュー画面を表示
            if (Request.IsAuthenticated)
            {
                return View("CustomerMenu");
            }
            // 認証されていない場合、ログイン画面を表示
            return View();
        }

        // 認証を行うメソッド
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CustomerLogin([Bind(Include = "customerId, password")] Customer model)
        {
            string session = (string)Session["loginusername"];
            if (session == null)
            {
                // セッションが空だったら認証処理開始
                if (ModelState.IsValid && model.customerId != null && model.password != null)
                {
                    /* IDとパスワード両方入力されていた場合、DBと照合開始。
                     * 一致するレコードがあれば認証成功 → メニュー画面を表示 */
                    if (ValidateUser(model))
                    {
                        FormsAuthentication.SetAuthCookie(userName: model.customerName, createPersistentCookie: false);
                        /* userName … 認証済みユーザの名前
                         * create～～ … 複数のブラウザーセッションにわたって保存される
                         *                     永続的なクッキーを作成する場合はtrue。それ以外の場合はfalse */
                        Session["loginusername"] = model.customerName;      // セッションに認証済みユーザのIDを格納
                        return RedirectToAction(actionName: "CustomerMenu");
                    }
                    // IDまたはパスワードが誤っていた場合は画面遷移せずエラーメッセージを表示
                    ViewBag.Message = PatisserieCestBon.Properties.Settings.Default.p001_error_Auth;
                    return View(model);
                }
                // IDまたはパスワードが空欄だった場合は画面遷移せずエラーメッセージを表示
                ViewBag.Message = PatisserieCestBon.Properties.Settings.Default.p001_error_Recuired;
                return View(model);
            }
            // セッションが空でなければメニュー画面にリダイレクト
            return RedirectToAction(actionName: "CustomerMenu");
        }

        private bool ValidateUser(Customer model)
        {
            var user = db.Customers
                .Where(u => u.customerId == model.customerId && u.password == model.password)
                .FirstOrDefault();      // FirstOrDefault() … 要素が見つからない場合は規定値（ここではnull）を返す
            return user != null;
        }

    public ActionResult CustomerLogout()
    {
        if (Session != null)
        {
            Session.Remove("loginusername");  // セッションのキーを削除
        }
        FormsAuthentication.SignOut();
        return Redirect("CustomerLogin");
    }

        /* その２　バックオフィス
         *  フロントエンドとほぼ同じなのでコメントは省略 */
    public ActionResult EmployeeMenu()
        {
            string session = (string)Session["loginusername"];
            if (session == null)
            {
                return Redirect("EmployeeLogin");
            }
            return View();
        }
        
        [AllowAnonymous]
        public ActionResult EmployeeLogin()
        {
            if (Request.IsAuthenticated)
            {
                return View("EmployeeMenu");
            }
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EmployeeLogin([Bind(Include = "empNo, password")] Employee model)
        {
            string session = (string)Session["loginusername"];
            if (session == null)
            {
                if (ModelState.IsValid && model.empNo != null && model.password != null)
                {
                    if (ValidateUser(model))
                    {
                        FormsAuthentication.SetAuthCookie(userName: model.empName, createPersistentCookie: false);
                        Session["loginusername"] = model.empName;
                        return RedirectToAction(actionName: "EmployeeMenu");
                    }
                    ViewBag.Message = PatisserieCestBon.Properties.Settings.Default.p013_error_Auth;
                    return View(model);
                }
                ViewBag.Message = PatisserieCestBon.Properties.Settings.Default.p013_error_Recuired;
                return View(model);
            }
            return RedirectToAction(actionName: "EmployeeMenu");
        }

        private bool ValidateUser(Employee model)
        {
            var user = db.Employees
                .Where(u => u.empNo == model.empNo && u.password == model.password)
                .FirstOrDefault();
            return user != null;
        }

        public ActionResult EmployeeLogout()
        {
            if (Session != null)
            {
                Session.Remove("loginusername");
            }
            FormsAuthentication.SignOut();
            return Redirect("EmployeeLogin");
        }
    }
}