using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using PatisserieCestBon.Models;

namespace PatisserieCestBon.Controllers
{
    public class EmployeeController : Controller
    {
        public ActionResult List()
        {
            // セッション確認
            if (Session["loginUserName"] == null)
            {
                // セッションが空だったらシステムエラー
                return RedirectToAction("EmployeeError", "Login");
            }
            using (var db = new DatabaseEntities())
            {
                //社員テーブルをインスタンス化して担当者一覧画面で表示できるようにする
                var e = db.Employees.ToList();
                ViewBag.E = e;
                return View("EmployeeList");
            }
        }
        public ActionResult Add1()
        {
            // セッション確認
            if (Session["loginUserName"] == null)
            {
                // セッションが空だったらシステムエラー
                return RedirectToAction("EmployeeError", "Login");
            }
            return View("EmployeeAdd1");
        }
        public ActionResult Add2(string empno, string empname, string password1, string password2)
        {
            // セッション確認
            if (Session["loginUserName"] == null)
            {
                // セッションが空だったらシステムエラー
                return RedirectToAction("EmployeeError", "Login");
            }
            //p:入力情報の不正の個数カウント
            int p = 0;
            //b:入力されたパスワードが半角英数記号ならtrue
            bool b = Regex.IsMatch(password1, @"^[!-~]*$");
            //入力されたパスワードが半角英数記号以外の場合
            if (b.Equals(false))
            {
                ViewBag.ErrorCharTypePassword = Properties.Settings.Default.p022_error_CharTypePassword;
                p++;
            }
            //パスワードと確認用パスワードが不一致の場合
            if (!password1.Equals(password2))
            {
                ViewBag.ErrorMatchRetypePassword = Properties.Settings.Default.p022_error_MatchRetypePassword;
                p++;
            }
            //社員番号が入力されていない場合
            if (empno.Equals(""))
            {
                ViewBag.ErrorRecuiredEmpNo = Properties.Settings.Default.p022_error_RecuiredEmpNo;
                p++;
            }
            //担当者氏名が入力されていない場合
            if (empname.Equals(""))
            {
                ViewBag.ErrorRecuiredEmpName = Properties.Settings.Default.p022_error_RecuiredEmpName;
                p++;
            }
            //パスワードが入力されていない場合
            if (password1.Equals(""))
            {
                ViewBag.ErrorRecuiredPassword = Properties.Settings.Default.p022_error_RecuiredPassword;
                p++;
            }
            //確認用パスワードが入力されていない場合
            if (password2.Equals(""))
            {
                ViewBag.ErrorRecuiredRetypePassword = Properties.Settings.Default.p022_error_RecuiredRetypePassword;
                p++;
            }
            //社員番号が数字かどうか、int型に変換できるかどうかで判断
            try
            {
                //社員番号をint型に直す
                int num = int.Parse(empno);
                //社員番号が999以下の場合
                if (num <= 999)
                {
                    ViewBag.ErrorFormatEmployeeId = Properties.Settings.Default.p022_error_FormatEmployeeId;
                    p++;
                }
                //社員番号が10000以上の場合
                if (num >= 10000)
                {
                    ViewBag.ErrorFormatEmployeeId = Properties.Settings.Default.p022_error_FormatEmployeeId;
                    p++;
                }
                //社員テーブルに入力された社員番号と同じ社員番号がすでにあるかどうか
                try
                {
                    using (var db = new DatabaseEntities())
                    {
                        //同じ社員番号を見つけられたらエラー
                        var ul = db.Employees.Single(c => c.empNo.Equals(num));
                        ViewBag.ErrorDuplicatedEmployeeId = Properties.Settings.Default.p022_error_DuplicatedEmployeeId;
                        return View("EmployeeAdd1");
                    }
                }
                //同じ社員番号が見つけられなかった場合(こちらが正常系)
                catch
                {
                    //入力内容に不正が一つもなければ追加確認画面へ遷移
                    if (p == 0)
                    {
                        ViewBag.EmpNo = empno;
                        ViewBag.EmpName = empname;
                        ViewBag.Password = password1;
                        return View("EmployeeAdd2");
                    }
                    //一つでも不正があれば追加入力画面やり直し
                    else
                    {
                        return View("EmployeeAdd1");
                    }
                }
            }
            //社員番号がint型に変換できなかった場合
            catch
            {
                ViewBag.ErrorFormatEmployeeId = Properties.Settings.Default.p022_error_FormatEmployeeId;
                return View("EmployeeAdd1");
            }
        }      
        public ActionResult Add3(int empno, string empname, string password)
        {
            // セッション確認
            if (Session["loginUserName"] == null)
            {
                // セッションが空だったらシステムエラー
                return RedirectToAction("EmployeeError", "Login");
            }
            using (var db = new DatabaseEntities())
            {
                //u:新規追加しようとしている内容の社員テーブルレコード
                var u = new Employee
                {
                    empNo = empno,
                    empName = empname,
                    password = password
                };
                //社員テーブルにuを追加
                db.Employees.Add(u);
                //データベースの変更を保存
                db.SaveChanges();
                //社員一覧表示画面に遷移するにあたり、社員テーブルをインスタンス化
                var e = db.Employees.ToList();
                ViewBag.E = e;
                ViewBag.InfoAddSuccess = Properties.Settings.Default.p021_info_AddSuccess;
                return View("EmployeeList");
            }
        }
        
        public ActionResult Update1(int empno, string empname, string password)
        {
            // セッション確認
            if (Session["loginUserName"] == null)
            {
                // セッションが空だったらシステムエラー
                return RedirectToAction("EmployeeError", "Login");
            }
            ViewBag.EmpNo = empno;
            ViewBag.EmpName = empname;
            ViewBag.Password = password;
            return View("EmployeeUpdate1");
        }
        public ActionResult Update2(int empno, string empname, string password1, string password2)
        {
            // セッション確認
            if (Session["loginUserName"] == null)
            {
                // セッションが空だったらシステムエラー
                return RedirectToAction("EmployeeError", "Login");
            }
            //p:入力情報の不正の個数カウント
            int p = 0;
            ViewBag.EmpNo = empno;
            //b:入力されたパスワードが半角英数記号ならtrue
            bool b = Regex.IsMatch(password1, @"^[!-~]*$");
            //入力されたパスワードが半角英数記号以外の場合
            if (b.Equals(false))
            {
                ViewBag.ErrorCharTypePassword = Properties.Settings.Default.p022_error_CharTypePassword;
                p++;
            }
            //パスワードと確認用パスワードが不一致の場合
            if (!(password1.Equals(password2)))
            {
                ViewBag.ErrorMatchRetypePassword = Properties.Settings.Default.p022_error_MatchRetypePassword;
                p++;
            }
            //社員名が入力されていない場合
            if (empname.Equals(""))
            {
                ViewBag.ErrorRecuiredEmpName = Properties.Settings.Default.p022_error_RecuiredEmpName;
                p++;
            }
            //パスワードが入力されていない場合
            if (password1.Equals(""))
            {
                ViewBag.ErrorRecuiredPassword = Properties.Settings.Default.p022_error_RecuiredPassword;
                p++;
            }
            //確認用パスワードが入力されていない場合
            if (password2.Equals(""))
            {
                ViewBag.ErrorRecuiredRetypePassword = Properties.Settings.Default.p022_error_RecuiredRetypePassword;
                p++;
            }
            //選択した社員名の社員番号が社員テーブルにあるかどうか
            try
            {
                using (var db = new DatabaseEntities())
                {
                    var ul = db.Employees.Single(c => c.empNo.Equals(empno));
                }
            }
            //選択した社員名の社員番号が社員テーブルに無かった場合
            catch
            {
                ViewBag.ErrorAlreadyDeletedEmployee = Properties.Settings.Default.p021_error_AlreadyDeletedEmployee;
                using (var db = new DatabaseEntities())
                {
                    // 社員一覧表示画面に遷移するにあたり、社員テーブルをインスタンス化
                    var e = db.Employees.ToList();
                    ViewBag.E = e;
                    return View("EmployeeList");
                }
            }
            //入力内容に不正が一つもなければ更新確認画面へ遷移
            if (p == 0)
            {
                ViewBag.EmpName = empname;
                ViewBag.Password = password1;
                return View("EmployeeUpdate2");
            }
            //一つでも不正があれば更新入力画面をやり直し
            else
            {
                return View("EmployeeUpdate1");
            }
        }
        public ActionResult Update3(int empno, string empname, string password)
        {
            // セッション確認
            if (Session["loginUserName"] == null)
            {
                // セッションが空だったらシステムエラー
                return RedirectToAction("EmployeeError", "Login");
            }
            using (var db = new DatabaseEntities())
            {
                //選択した社員の社員番号を社員テーブルから探してそのレコード内容を更新
                var u = db.Employees.Find(empno);
                {
                    u.empNo = empno;
                    u.empName = empname;
                    u.password = password;
                    //データベースの変更を保存
                    db.SaveChanges();
                }
                // 社員一覧表示画面に遷移するにあたり、社員テーブルをインスタンス化
                var e = db.Employees.ToList();
                ViewBag.E = e;      
            }
            ViewBag.InfoUpdateSuccess = Properties.Settings.Default.p021_info_UpdateSuccess;
            return View("EmployeeList");
        }
        public ActionResult Delete1(string empno)
        {
            // セッション確認
            if (Session["loginUserName"] == null)
            {
                // セッションが空だったらシステムエラー
                return RedirectToAction("EmployeeError", "Login");
            }
            //チェックボックスにチェックを入れている場合
            try
            {
                //社員番号をint型に直せるはずであるので直す
                int num = int.Parse(empno);
                using (var db = new DatabaseEntities())
                {
                    //チェックした社員の社員番号があるレコードを社員テーブルから探す
                    var ul = db.Employees.Single(c => c.empNo.Equals(num));
                    //そのレコードの社員番号、社員名を削除確認画面用にViewBag
                    ViewBag.EmpNo = ul.empNo;
                    ViewBag.EmpName = ul.empName;   
                }
                return View("EmployeeDelete1");
            }
            //チェックボックスにチェックを入れずに削除を押した場合(社員番号をint型に直せなかった場合)
            catch
            {
                using (var db = new DatabaseEntities())
                {
                    // 社員一覧表示画面に遷移するにあたり、社員テーブルをインスタンス化
                    var e = db.Employees.ToList();
                    ViewBag.E = e;
                }
                ViewBag.ErrorNotChecked = Properties.Settings.Default.p021_error_NotChecked;
                return View("EmployeeList");
            }        
        }
        public ActionResult Delete2(string empno)
        {
            // セッション確認
            if (Session["loginUserName"] == null)
            {
                // セッションが空だったらシステムエラー
                return RedirectToAction("EmployeeError", "Login");
            }
            using (var db = new DatabaseEntities())
            {
                //チェックした社員の社員番号が社員テーブルにあった場合
                try
                {
                    //社員番号をint型に直す
                    int num = int.Parse(empno);
                    //チェックした社員の社員番号があるレコードを見つける
                    var u = db.Employees.Find(num);
                    //そのレコードを取り去る
                    db.Employees.Remove(u);
                    //データベースの変更を保存
                    db.SaveChanges();
                    // 社員一覧表示画面に遷移するにあたり、社員テーブルをインスタンス化
                    var e = db.Employees.ToList();
                    ViewBag.E = e;
                }
                //チェックした社員の社員番号が社員テーブルになかった場合
                catch
                {
                    ViewBag.ErrorAlreadyDeletedEmployee = Properties.Settings.Default.p021_error_AlreadyDeletedEmployee;           
                    // 社員一覧表示画面に遷移するにあたり、社員テーブルをインスタンス化
                    var t = db.Employees.ToList();
                    ViewBag.E = t;
                    return View("EmployeeList");
                }      
            }
            ViewBag.InfoDeleteSuccess = Properties.Settings.Default.p021_info_DeleteSuccess;
            return View("EmployeeList");
        }
    }
}