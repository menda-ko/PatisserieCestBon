using Microsoft.Ajax.Utilities;
using PatisserieCestBon.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
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

        public ActionResult CustomerAdd2(string companyName, string address, string telNo, string customerName, string customerKana, string dept, string email, string password, string checkPass)
        {

            ViewBag.companyName = companyName;
            ViewBag.address = address;
            ViewBag.telNo = telNo;
            ViewBag.customerName = customerName;
            ViewBag.customerKana = customerKana;
            ViewBag.dept = dept;
            ViewBag.email = email;
            ViewBag.password = password;
            ViewBag.checkPass = checkPass;

            //会社名の未入力チェック
            int compNameCheck = 0;
            if (companyName == "")
            {
                ViewBag.RecuiredCompanyName = PatisserieCestBon.Properties.Settings.Default.p002_error_RecuiredCompanyName;
                compNameCheck = 1;
                ViewBag.compNameCheck = compNameCheck;
            }

            //住所の未入力チェック
            int addressCheck = 0;
            if (address == "")
            {
                ViewBag.RecuiredAddress = PatisserieCestBon.Properties.Settings.Default.p002_error_RecuiredAddress;
                addressCheck = 1;
                ViewBag.addressCheck = addressCheck;
            }

            //電話番号の未入力チェック
            int telNoCheck = 0;
            if (telNo == "")
            {
                ViewBag.RecuiredTelNo = PatisserieCestBon.Properties.Settings.Default.p002_error_RecuiredTelNo;
                telNoCheck = 1;
                ViewBag.telNoCheck = telNoCheck;
            }

            //氏名(漢字)の未入力チェック
            int customerNameCheck = 0;
            if (customerName == "")
            {
                ViewBag.RecuiredCustomerName = PatisserieCestBon.Properties.Settings.Default.p002_error_RecuiredCustomerName;
                customerNameCheck = 1;
                ViewBag.customerNameCheck = customerNameCheck;
            }

            //氏名(かな)の未入力チェック
            int customerKanaCheck = 0;
            if (customerKana == "")
            {
                ViewBag.RecuiredCustomerKana = PatisserieCestBon.Properties.Settings.Default.p002_error_RecuiredCustomerKana;
                customerKanaCheck = 1;
                ViewBag.customerKanaCheck = customerKanaCheck;
            }

            //パスワードの未入力チェック
            int passwordCheck = 0;
            if (password == "")
            {
                ViewBag.RequiredPassword = PatisserieCestBon.Properties.Settings.Default.p002_error_RecuiredPassword;
                passwordCheck = 1;
                ViewBag.passwordCheck = passwordCheck;
            }

            //パスワード(確認)の未入力チェック
            int retypePasswordCheck = 0;
            if (checkPass == "")
            {
                ViewBag.RequiredRetypePassword = PatisserieCestBon.Properties.Settings.Default.p002_error_RequiredRetypePassword;
                retypePasswordCheck = 1;
                ViewBag.retypePasswordCheck = retypePasswordCheck;
            }

            //パスワードとパスワード(確認)が一致するかチェック
            int matchPasswordCheck = 0;
            if (checkPass != password)
            {
                ViewBag.MatchRetypePassword = PatisserieCestBon.Properties.Settings.Default.p002_error_MatchRetypePassword;
                matchPasswordCheck = 1;
                ViewBag.matchPasswordCheck = matchPasswordCheck;
            }

            //電話番号の正規表現
            int formatTelNoCheck = 0;
            if (!Regex.IsMatch(telNo, @"^[0-9]{2,4}-[0-9]{4}-[0-9]{4}$"))
            {
                ViewBag.formatTelNo = PatisserieCestBon.Properties.Settings.Default.p002_error_FormatTelNo;
                formatTelNoCheck = 1;
                ViewBag.formatTelNoCheck = formatTelNoCheck;
            }

            //メールアドレスの正規表現
            int formatEmailCheck = 0;
            if (!Regex.IsMatch(email, @"^[a-zA-Z0-9.]{1,20}@[a-zA-Z0-9.]{2,9}$"))
            {
                ViewBag.formatEmail = PatisserieCestBon.Properties.Settings.Default.p002_error_FormatEmail;
                formatEmailCheck = 1;
                ViewBag.formatEmailCheck = formatEmailCheck;
            }

            //氏名(かな)の正規表現
            int formatKanaCheck = 0;
            if (!Regex.IsMatch(customerKana, @"^[ぁ-ん]+$"))
            {
                ViewBag.TypeKana = PatisserieCestBon.Properties.Settings.Default.p002_error_CharTypeKana;
                formatKanaCheck = 1;
                ViewBag.formatKanaCheck = formatKanaCheck;
            }

            //メールアドレスの入力文字チェック
            int typeEmailCheck = 0;
            if (!Regex.IsMatch(email, "^[A-Za-z0-9-.@]+$"))
            {
                ViewBag.TypeEmail = PatisserieCestBon.Properties.Settings.Default.p002_error_CharTypeEmail;
                typeEmailCheck = 1;
                ViewBag.typeEmailCheck = typeEmailCheck;
            }

            //電話番号の入力文字チェック
            int typeTelCheck = 0;
            if (!Regex.IsMatch(telNo, "^[0-9-]+$"))
            {
                ViewBag.TypeTelNo = PatisserieCestBon.Properties.Settings.Default.p002_error_CharTypeTelNo;
                typeTelCheck = 1;
                ViewBag.typeTelCheck = typeTelCheck;
            }

            //入力項目に不備があった際に入力画面に遷移させる処理
            if (compNameCheck == 1 || addressCheck == 1 || telNoCheck == 1 || customerNameCheck == 1 ||
           customerKanaCheck == 1 || passwordCheck == 1 || retypePasswordCheck == 1 || matchPasswordCheck == 1 ||
           formatTelNoCheck == 1 || formatEmailCheck == 1 || typeEmailCheck == 1 || typeTelCheck == 1)
            {
                return View("CustomerAdd1");
            }
            return View();
        }

        public ActionResult CustomerAdd3(string companyName, string address, string telNo, string customerName, string customerKana, string dept, string email, string password)
        {

            using (var db = new DatabaseEntities())
            {
                //更新処理
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

                //文字数によっては動かない場合あり
                db.SaveChanges();
                ViewBag.model = model;
                var id = model.customerId;
                ViewBag.customerId = id.ToString("000000");
                return View();

            }
        }

        public ActionResult List()
        {
            if (Session["loginUserName"] == null)
            {
                // セッションが空だったらシステムエラー
                return Redirect("EmployeeError");
            }

            using (var db = new DatabaseEntities())
            {

                ViewBag.List = db.Customers.ToList();

                //更新・削除成功時のメッセージを表示させないための値
                ViewBag.updateCheck = 0;
                ViewBag.deleteCheck = 0;
                ViewBag.alreadyDelete = 0;

                return View();
            }

        }
        public ActionResult Update1(int customerId)
        {
            if (Session["loginUserName"] == null)
            {
                // セッションが空だったらシステムエラー
                return Redirect("EmployeeError");
            }

            using (var db = new DatabaseEntities())
            {
                //更新入力画面の初期値の設定
                ViewBag.model = db.Customers.Find(customerId);
                ViewBag.customerId = customerId.ToString("000000");
                ViewBag.companyName = db.Customers.Find(customerId).companyName;
                ViewBag.address = db.Customers.Find(customerId).address;
                ViewBag.telNo = db.Customers.Find(customerId).telNo;
                ViewBag.customerName = db.Customers.Find(customerId).customerName;
                ViewBag.customerKana = db.Customers.Find(customerId).customerKana;
                ViewBag.dept = db.Customers.Find(customerId).dept;
                ViewBag.email = db.Customers.Find(customerId).email;
                ViewBag.password = db.Customers.Find(customerId).password;
                ViewBag.checkPass = db.Customers.Find(customerId).password;
                
                return View();
            }
        }

        public ActionResult Update2(int customerId, string companyName, string address, string telNo, string customerName, string customerKana, string dept, string email, string password, string checkPass)
        {
            if (Session["loginUserName"] == null)
            {
                // セッションが空だったらシステムエラー
                return Redirect("EmployeeError");
            }

            ViewBag.customerId = customerId.ToString("000000");
            ViewBag.companyName = companyName;
            ViewBag.address = address;
            ViewBag.telNo = telNo;
            ViewBag.customerName = customerName;
            ViewBag.customerKana = customerKana;
            ViewBag.dept = dept;
            ViewBag.email = email;
            ViewBag.password = password;
            ViewBag.checkPass = checkPass;

            //エラーメッセージを非表示にする処理
            int inappropriateCheck = 0;
            ViewBag.Inappropriate = PatisserieCestBon.Properties.Settings.Default.p028_error_Inappropriate;

            //会社名の未入力チェック
            int compNameCheck = 0;
            if (companyName == "")
            {
                ViewBag.RecuiredCompanyName = PatisserieCestBon.Properties.Settings.Default.p028_error_RecuiredCompanyName;
                compNameCheck = 1;
                ViewBag.compNameCheck = compNameCheck;
            }

            //住所の未入力チェック
            int addressCheck = 0;
            if (address == "")
            {
                ViewBag.RecuiredAddress = PatisserieCestBon.Properties.Settings.Default.p028_error_RecuiredAddress;
                addressCheck = 1;
                ViewBag.addressCheck = addressCheck;
            }

            //電話番号の未入力チェック
            int telNoCheck = 0;
            if (telNo == "")
            {
                ViewBag.RecuiredTelNo = PatisserieCestBon.Properties.Settings.Default.p028_error_RecuiredTelNo;
                telNoCheck = 1;
                ViewBag.telNoCheck = telNoCheck;
            }

            //氏名(漢字)の未入力チェック
            int customerNameCheck = 0;
            if (customerName == "")
            {
                ViewBag.RecuiredCustomerName = PatisserieCestBon.Properties.Settings.Default.p028_error_RecuiredCustomerName;
                customerNameCheck = 1;
                ViewBag.customerNameCheck = customerNameCheck;
            }

            //氏名(かな)の未入力チェック
            int customerKanaCheck = 0;
            if (customerKana == "")
            {
                ViewBag.RecuiredCustomerKana = PatisserieCestBon.Properties.Settings.Default.p028_error_RecuiredCustomerKana;
                customerKanaCheck = 1;
                ViewBag.customerKanaCheck = customerKanaCheck;
            }

            //パスワードの未入力チェック
            int passwordCheck = 0;
            if (password == "")
            {
                ViewBag.RequiredPassword = PatisserieCestBon.Properties.Settings.Default.p028_error_RecuiredPassword;
                passwordCheck = 1;
                ViewBag.passwordCheck = passwordCheck;
            }

            //パスワード(確認)の未入力チェック
            int retypePasswordCheck = 0;
            if (checkPass == "")
            {
                ViewBag.RequiredRetypePassword = PatisserieCestBon.Properties.Settings.Default.p028_error_RequiredRetypePassword;
                retypePasswordCheck = 1;
                ViewBag.retypePasswordCheck = retypePasswordCheck;
            }

            //パスワードとパスワード(確認)が一致するかチェック
            int matchPasswordCheck = 0;
            if (checkPass != password)
            {
                ViewBag.MatchRetypePassword = PatisserieCestBon.Properties.Settings.Default.p028_error_MatchRetypePassword;
                matchPasswordCheck = 1;
                ViewBag.matchPasswordCheck = matchPasswordCheck;
            }

            //電話番号の正規表現
            int formatTelNoCheck = 0;
            if (!Regex.IsMatch(telNo, @"^[0-9]{2,4}-[0-9]{4}-[0-9]{4}$"))
            {
                ViewBag.formatTelNo = PatisserieCestBon.Properties.Settings.Default.p028_error_FormatTelNo;
                formatTelNoCheck = 1;
                ViewBag.formatTelNoCheck = formatTelNoCheck;
            }

            //メールアドレスの正規表現
            int formatEmailCheck = 0;
            if (!Regex.IsMatch(email, @"^[a-zA-Z0-9.]{1,20}@[a-zA-Z0-9.]{2,9}$"))
            {
                ViewBag.formatEmail = PatisserieCestBon.Properties.Settings.Default.p028_error_FormatEmail;
                formatEmailCheck = 1;
                ViewBag.formatEmailCheck = formatEmailCheck;
            }

            //氏名(かな)の正規表現
            int formatKanaCheck = 0;
            if (!Regex.IsMatch(customerKana, @"^[ぁ-ん]+$"))
            {
                ViewBag.TypeKana = PatisserieCestBon.Properties.Settings.Default.p028_error_CharTypeKana;
                formatKanaCheck = 1;
                ViewBag.formatKanaCheck = formatKanaCheck;
            }

            //メールアドレスの入力文字チェック
            int typeEmailCheck = 0;
            if (!Regex.IsMatch(email, "^[A-Za-z0-9-.@]+$"))
            {
                ViewBag.TypeEmail = PatisserieCestBon.Properties.Settings.Default.p028_error_CharTypeEmail;
                typeEmailCheck = 1;
                ViewBag.typeEmailCheck = typeEmailCheck;
            }

            //電話番号の入力文字チェック
            int typeTelCheck = 0;
            if (!Regex.IsMatch(telNo, "^[0-9-]+$"))
            {
                ViewBag.TypeTelNo = PatisserieCestBon.Properties.Settings.Default.p028_error_CharTypeTelNo;
                typeTelCheck = 1;
                ViewBag.typeTelCheck = typeTelCheck;
            }

            if(formatTelNoCheck == 1 || formatEmailCheck == 1 || typeEmailCheck == 1 || typeTelCheck == 1)
            {
                //不正な値チェック
                inappropriateCheck = 1;
                ViewBag.inappropriateCheck = inappropriateCheck;
            }
            
            //入力項目に不備があった際に入力画面に遷移させる処理
            if (compNameCheck == 1 || addressCheck == 1 || telNoCheck == 1 || customerNameCheck == 1 ||
               customerKanaCheck == 1 || passwordCheck == 1 || retypePasswordCheck == 1 || matchPasswordCheck == 1 ||
               formatTelNoCheck == 1 || formatEmailCheck == 1 || typeEmailCheck == 1 || typeTelCheck == 1)
            {
                return View("Update1");
            }
            return View();
        }

        public ActionResult Update3(int customerId, string companyName, string address, string telNo, string customerName, string customerKana, string dept, string email, string password)
        {
            if (Session["loginUserName"] == null)
            {
                // セッションが空だったらシステムエラー
                return Redirect("EmployeeError");
            }

            using (var db = new DatabaseEntities())
            {
                var u = db.Customers.Find(customerId);
                u.companyName = companyName;
                u.address = address;
                u.telNo = telNo;
                u.customerName = customerName;
                u.customerKana = customerKana;
                u.dept = dept;
                u.email = email;
                u.password = password;

                //入力した文字数によっては動かない場合あり
                db.SaveChanges();
                ViewBag.updateSuccess = PatisserieCestBon.Properties.Settings.Default.p027_info_UpdateSuccess;
                ViewBag.updateCheck = 1;

                //顧客一覧を表示用のViewBag
                ViewBag.List = db.Customers.ToList();

                return View("List");
            }
        }

        public ActionResult Delete1(int customerId)
        {
            if (Session["loginUserName"] == null)
            {
                // セッションが空だったらシステムエラー
                return Redirect("EmployeeError");
            }

            using (var db = new DatabaseEntities())
            {

                ViewBag.model = db.Customers.Find(customerId);

                return View();
            }

        }

        public ActionResult Delete2(int customerId)
        {
            if (Session["loginUserName"] == null)
            {
                // セッションが空だったらシステムエラー
                return Redirect("EmployeeError");
            }

            using (var db = new DatabaseEntities())
            {
                try
                {
                    //削除処理
                    var u = db.Customers.Find(customerId);
                    db.Customers.Remove(u);
                    db.SaveChanges();
                    ViewBag.deleteSuccess = PatisserieCestBon.Properties.Settings.Default.p027_info_DeleteSuccess;
                    ViewBag.deleteCheck = 1;

                }
                catch(Exception)
                {
                    //削除されている顧客を再び削除しようとしたときに走る処理
                    ViewBag.alreadyDeleteMes = PatisserieCestBon.Properties.Settings.Default.p027_error_AlreadyDeletedCustomer;
                    ViewBag.alreadyDelete = 1;
                }
                finally
                {
                    //顧客一覧を表示用のViewBag
                    ViewBag.List = db.Customers.ToList();
                }

                return View("List");

            }

        }
    }
}