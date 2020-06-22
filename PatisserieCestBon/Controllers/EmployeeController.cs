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
        // GET: Employee
        public ActionResult List1()
        {
            using (var db = new DatabaseEntities())
            {
                var e = db.Employees.ToList();
                ViewBag.E = e;
                return View("EmployeeList");
            }
        }
        public ActionResult List2()
        {
            using (var db = new DatabaseEntities())
            {
                var e = db.Employees.ToList();
                ViewBag.E = e;
                return View("EmployeeList");
            }
        }
        public ActionResult Add1()
        {
            return View("EmployeeAdd1");
        }
        public ActionResult Add2(string empno, string empname, string password1, string password2)

        {
            int p = 0;
            bool b = Regex.IsMatch(password1, @"^[!-~]*$");
            if (b.Equals(false))
            {
                ViewBag.ErrorCharTypePassword = "パスワードは半角英数字と記号のみで入力してください。";
            }
            if (!password1.Equals(password2))
            {
                ViewBag.ErrorMatchRetypePassword = "パスワードが確認用パスワードと一致しません。";
                p++;
            }
            if (empno.Equals(""))
            {
                ViewBag.ErrorRecuiredEmpNo = "社員番号を入力してください。";
                p++;
            }
            if (empname.Equals(""))
            {
                ViewBag.ErrorRecuiredEmpName = "担当者氏名を入力してください。";
                p++;
            }
            if (password1.Equals(""))
            {
                ViewBag.ErrorRecuiredPassword = "パスワードを入力してください。";
                p++;
            }
            if (password2.Equals(""))
            {
                ViewBag.ErrorRecuiredRetypePassword = "確認用パスワードを入力してください。";
                p++;
            }
            try
            {
                int num = int.Parse(empno);
                if (num <= 999)
                {
                    ViewBag.ErrorFormatEmployeeId = "社員番号は1000から始まる4桁の数値で入力してください。";
                    p++;
                }
                if (num >= 10000)
                {
                    ViewBag.ErrorFormatEmployeeId = "社員番号は1000から始まる4桁の数値で入力してください。";
                    p++;
                }
                try
                {
                    using (var db = new DatabaseEntities())
                    {
                        var ul = db.Employees.Single(c => c.empNo.Equals(num));
                        ViewBag.ErrorDuplicatedEmployeeId = "入力された社員番号はすでに登録されています。";
                        return View("EmployeeAdd1");
                    }
                }
                catch
                {
                    if (p == 0)
                    {
                        ViewBag.EmpNo = empno;
                        ViewBag.EmpName = empname;
                        ViewBag.Password = password1;
                        return View("EmployeeAdd2");
                    }
                    else
                    {
                        return View("EmployeeAdd1");
                    }
                }
            }
            catch
            {
                ViewBag.ErrorFormatEmployeeId = "社員番号は1000から始まる4桁の数値で入力してください。";
                return View("EmployeeAdd1");
            }
        }
    

        
        public ActionResult Add3(int empno, string empname, string password)
        {
            using (var db = new DatabaseEntities())
            {
                    var u = new Employee
                    {
                        empNo = empno,
                        empName = empname,
                        password = password
                    };
                    db.Employees.Add(u);
                    db.SaveChanges();

                    var e = db.Employees.ToList();
                    ViewBag.E = e;
                    ViewBag.InfoAddSuccess = "担当者情報を登録しました。";
                    return View("EmployeeList");
            }
        }
        
        public ActionResult Update1(int empno, string empname, string password)
        {
            ViewBag.EmpNo = empno;
            ViewBag.EmpName = empname;
            ViewBag.Password = password;
            return View("EmployeeUpdate1");
        }
        public ActionResult Update2(int empno, string empname, string password1, string password2)
        {
            int p = 0;
            ViewBag.EmpNo = empno;
            bool b = Regex.IsMatch(password1, @"^[!-~]*$");
            if (b.Equals(false))
            {
                ViewBag.ErrorCharTypePassword = "パスワードは半角英数字と記号のみで入力してください。";
            }
            if (!(password1.Equals(password2)))
            {
                ViewBag.ErrorMatchRetypePassword = "パスワードが確認用パスワードと一致しません。";
                p++;
            }
            if (empname.Equals(""))
            {
                ViewBag.ErrorRecuiredEmpName = "担当者氏名を入力してください。";
                p++;
            }
            if (password1.Equals(""))
            {
                ViewBag.ErrorRecuiredPassword = "パスワードを入力してください。";
                p++;
            }
            if (password2.Equals(""))
            {
                ViewBag.ErrorRecuiredRetypePassword = "確認用パスワードを入力してください。";
                p++;
            }
            try
            {
                using (var db = new DatabaseEntities())
                {
                    var ul = db.Employees.Single(c => c.empNo.Equals(empno));
                }
            }
            catch
            {
                ViewBag.ErrorAlreadyDeletedEmployee = "該当の担当者情報はすでに削除されています。";
                using (var db = new DatabaseEntities())
                {
                    var e = db.Employees.ToList();
                    ViewBag.E = e;
                    return View("EmployeeList");
                }
            }

            if (p == 0)
            {
                ViewBag.EmpName = empname;
                ViewBag.Password = password1;
                return View("EmployeeUpdate2");
            }
            else
            {
                return View("EmployeeUpdate1");
            }
        }
        public ActionResult Update3(int empno, string empname, string password)
        {
            using (var db = new DatabaseEntities())
            {
                var u = db.Employees.Find(empno);
                {
                    u.empNo = empno;
                    u.empName = empname;
                    u.password = password;
                    db.SaveChanges();
                }
                var e = db.Employees.ToList();
                ViewBag.E = e;      
            }
            ViewBag.InfoUpdateSuccess = "担当者情報を更新しました。";
            return View("EmployeeList");
        }
        public ActionResult Delete1(string empno)
        {

            if (empno == null)
            {
                using (var db = new DatabaseEntities())
                {
                    var e = db.Employees.ToList();
                    ViewBag.E = e;
                }
                ViewBag.ErrorNotChecked = "削除を行う場合は、削除したい担当者を選択してください。";
                return View("EmployeeList");
            }
            else
            {
                try
                {
                    int num = int.Parse(empno);
                    using (var db = new DatabaseEntities())
                    {
                        var ul = db.Employees.Single(c => c.empNo.Equals(num));
                    }
                }
                catch
                {
                    ViewBag.ErrorAlreadyDeletedEmployee = "該当の担当者情報はすでに削除されています。";
                    using (var db = new DatabaseEntities())
                    {
                        var e = db.Employees.ToList();
                        ViewBag.E = e;
                        return View("EmployeeList");
                    }
                }
                ViewBag.EmpNo = empno;
                int num2 = int.Parse(empno);
                using (var db = new DatabaseEntities())
                {
                    var u = db.Employees.Single(e => e.empNo.Equals(num2));
                    ViewBag.EmpName = u.empName;
                }
                return View("EmployeeDelete1");
            }
        }
        public ActionResult Delete2(int empno)
        {
            using (var db = new DatabaseEntities())
            {
                var u = db.Employees.Find(empno);
                db.Employees.Remove(u);
                db.SaveChanges();
                var e = db.Employees.ToList();
                ViewBag.E = e;
            }
            ViewBag.InfoDeleteSuccess = "担当者情報を削除しました。";
            return View("EmployeeList");
        }
    }
}