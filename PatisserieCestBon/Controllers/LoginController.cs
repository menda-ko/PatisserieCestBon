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
            string sessionId = (string)Session["loginUserName"];
            if (sessionId == null)
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
            return View("CustomerLogin");
        }

        // 認証を行うメソッド
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CustomerLogin(string inputCustomerId, string inputPassword)
        {
            string sessionId = (string)Session["loginUserName"];
            if (sessionId == null)
            {
                // セッションが空だったら認証処理開始
                if (!string.IsNullOrWhiteSpace(inputCustomerId) && !string.IsNullOrWhiteSpace(inputPassword))
                {
                    /* IDとパスワード両方入力されていた場合、DBと照合開始（ValidateUser）
                     * 一致するレコードがあれば認証成功 → メニュー画面を表示 */
                    if (ValidateUser(inputCustomerId, inputPassword))
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
                // IDまたはパスワードが空欄だった場合は画面遷移せずエラーメッセージを表示
                ViewBag.ErrorMessage = Properties.Settings.Default.p001_error_Recuired;
                return View("CustomerLogin");
            }
            // セッションが空でなければメニュー画面にリダイレクト
            return RedirectToAction(actionName: "CustomerMenu");
        }

        private bool ValidateUser(string inputCustomerId, string inputPassword)
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
            string sessionId = (string)Session["loginUserName"];
            if (sessionId == null)
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
            string sessionId = (string)Session["loginUserName"];
            if (sessionId == null)
            {
                if (ModelState.IsValid && model.empNo.Equals(null) && model.password.Equals(null))
                {
                    if (ValidateUser(model))
                    {
                        FormsAuthentication.SetAuthCookie(userName: model.empName, createPersistentCookie: false);
                        Session["loginUserName"] = model.empName;
                        return RedirectToAction(actionName: "EmployeeMenu");
                    }
                    ViewBag.ErrorMessage = Properties.Settings.Default.p013_error_Auth;
                    return View(model);
                }
                ViewBag.ErrorMessage = Properties.Settings.Default.p013_error_Recuired;
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
                Session.Remove("loginUserName");
            }
            FormsAuthentication.SignOut();
            return Redirect("EmployeeLogin");
        }
    }
}