using Microsoft.Ajax.Utilities;
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
        // ★商品一覧表示（バックオフィス）ここから★
        public ActionResult List()
        {
            // Itemテーブルから、削除フラグがFalseのもののみ取得
            var itemList = db.Items
                .Where(i => i.deleteFlag.Equals(false));
            ViewBag.ItemList = itemList;
            return View();
        }
        // ★商品一覧表示（バックオフィス）ここまで★

        // ★商品追加登録（Add1～Add3）ここから★
        // Add1 … 商品追加入力画面へ
        public ActionResult Add1()
        {
            return View();
        }
        // Add2 … 商品追加確認画面へ
        public ActionResult Add2(int itemNo, string itemName, string size, string photoUrl, int unitPrice,
            string assortment, string category)
        {
            /* // エラーメッセージを格納するリスト
            List<string> errorMessageList = new List<string>();
            // 必須項目が入力されていない場合「～を入力してください」のエラーメッセージをリストに格納
            if (itemNo == null)
            {
                errorMessageList.Add(PatisserieCestBon.Properties.Settings.Default.p032_error_RecuiredItemNo);
            }
            if (string.IsNullOrWhiteSpace(itemName))
            {
                errorMessageList.Add(PatisserieCestBon.Properties.Settings.Default.p032_error_RecuiredItemName);
            }
            if (string.IsNullOrWhiteSpace(size))
            {
                errorMessageList.Add(PatisserieCestBon.Properties.Settings.Default.p032_error_RecuiredSize);
            }
            if (string.IsNullOrWhiteSpace(photoUrl)) // photoUrlは必須？NULL可？
            {
                errorMessageList.Add(PatisserieCestBon.Properties.Settings.Default.p032_error_RecuiredPhotoUrl);
            }
            if (string.IsNullOrWhiteSpace(unitPrice))
            {
                errorMessageList.Add(PatisserieCestBon.Properties.Settings.Default.p032_error_RecuiredUnitPrice);
            }
            if (errorMessageList != null)
            {
                // エラーメッセージリストがnullでない場合、画面遷移せずエラーメッセージを表示
                    foreach (var errorMessage in errorMessageList)
                {
                    ViewBag.ErrorMessageList = errorMessage;
                };
                return View("Add1");
            }
            */
            // 確認画面に表示するために値を渡す
            var item = new Item()
            {
                itemNo = itemNo
                , itemName = itemName
                , size = size
                , photoUrl = photoUrl
                , unitPrice = unitPrice
                , assortment = assortment
                , category = category
            };
            ViewBag.model = item;
            Session["newItem"] = item;
            return View();
        }
        // Add3 … Itemテーブルに商品を登録
        public ActionResult Add3(int itemNo, string itemName, string size, string photoUrl, int unitPrice,
            string assortment, string category)
        {
                var item = new Item()
                {
                    itemNo = itemNo
                    ,itemName = itemName
                    ,size = size
                    ,photoUrl = photoUrl
                    ,unitPrice = unitPrice
                    ,assortment = assortment
                    ,category = category
                };
                db.Items.Add(item);
                db.SaveChanges();
                ViewBag.InfoMessage = PatisserieCestBon.Properties.Settings.Default.p031_info_AddSuccess;
            var itemList = db.Items
                .Where(i => i.deleteFlag.Equals(false));
            ViewBag.ItemList = itemList;
            return View("List");
            }
        // ★商品追加登録（Add1～Add3）ここまで★
    }
}