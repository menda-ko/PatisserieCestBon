using PatisserieCestBon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PatisserieCestBon.Controllers
{
    public class ItemController : Controller
    {
        private DatabaseEntities db = new DatabaseEntities();
        // 商品一覧表示（バックオフィス）
        public ActionResult List()
        {
            // Itemテーブルから、削除フラグがFalseのもののみ取得
            var itemList = db.Items
                .Where(i => i.deleteFlag.Equals(false));
            ViewBag.ItemList = itemList;
            return View();
        }

        // 商品追加登録機能（Add1～Add3）
        // Add1 … 商品追加入力画面へ
        public ActionResult Add1()
        {
            return View();
        }
        /* Add2 … 商品追加確認画面へ
        public ActionResult Add2(int itemNo, string itemName,
            string size, string photoUrl, string unitPrice,
            string assortment, string category)
        {
            /*            if (itemNo == null)
                        {
                            ViewBag.ErrorMessage = PatisserieCestBon.Properties.Settings.Default.p032_error_RecuiredItemNo;
                        }
                        if (itemName == null)
                        {
                            ViewBag.ErrorMessage = PatisserieCestBon.Properties.Settings.Default.p032_error_RecuiredItemName
                        }
                        return View();
                    } 
            return View();
    }*/
    }
}