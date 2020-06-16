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
            return View("List");
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
            //登録成功のメッセージ
                ViewBag.InfoMessage = PatisserieCestBon.Properties.Settings.Default.p031_info_AddSuccess;
            // 商品一覧に戻るため、同じコントローラ内のList()メソッドを呼び出し
            return List();
            }
        // ★商品追加登録（Add1～Add3）ここまで★

        // ★商品更新（Update1～Update3）ここから★
        public ActionResult Update1(int id)
        {
            // 更新対象の商品情報をDBから取得して入力画面に渡す
            ViewBag.Item = db.Items.Find(id);
            return View();
        }
        public ActionResult Update2(int id, string itemName, string size, string photoUrl, int unitPrice,
            string assortment, string category)
        {
            // 更新入力画面で入力した情報を確認力画面に渡す
            var item = new Item()
            {
                itemNo = id
                ,itemName = itemName
                ,size = size
                ,photoUrl = photoUrl
                ,unitPrice = unitPrice
                ,assortment = assortment
                ,category = category
            };
            ViewBag.model = item;
            return View();
        }
        public ActionResult Update3(int id, string itemName, string size, string photoUrl, int unitPrice,
            string assortment, string category)
        {
            // 更新確認画面から送信された内容でDBを更新する
            var item = db.Items.Find(id);
            item.itemName = itemName;
            item.size = size;
            item.photoUrl = photoUrl;
            item.unitPrice = unitPrice;
            item.assortment = assortment;
            item.category = category;
            db.SaveChanges();
            // 更新成功のメッセージ
            ViewBag.InfoMessage = PatisserieCestBon.Properties.Settings.Default.p031_info_UpdateSuccess;
            // 商品一覧に戻るため、同じコントローラ内のList()メソッドを呼び出し
            return List();
        }

        // ★商品削除（Delete1～Delete2）ここから★
        public ActionResult Delete1(int[] itemNoList)
        {
            if (itemNoList == null)
            {
                // チェックボックスに1つもチェックが入っていない場合のエラーメッセージ
                ViewBag.ErrorMessage = PatisserieCestBon.Properties.Settings.Default.p031_error_NotChecked;
                // 商品一覧に戻るため、同じコントローラ内のList()メソッドを呼び出し
                return List();
            }
            else 
            {
                List<Item> deleteItemList = new List<Item>();
                foreach (var itemNo in itemNoList)
                {
                    deleteItemList.Add(db.Items.Find(itemNo));
                    ViewBag.DeleteItemList = deleteItemList;
                }
                return View();
            }
        }
        public ActionResult Delete2(int[] itemNoList)
        {
                foreach (var itemNo in itemNoList)
            {
                var item = db.Items.Find(itemNo);
                if(item.deleteFlag == true)
                {
                    // 削除しようとした商品がすでに削除されていた場合、エラーメッセージを一覧に表示
                    ViewBag.ErrorMessage = PatisserieCestBon.Properties.Settings.Default.p031_error_AlreadyDeletedItem;
                    return List();
                }
                item.deleteFlag = true;
            }
            db.SaveChanges();
            // 削除成功のメッセージ
            ViewBag.InfoMessage = PatisserieCestBon.Properties.Settings.Default.p031_info_DeleteSuccess;
            // 商品一覧に戻るため、同じコントローラ内のList()メソッドを呼び出し
            return List();
        }
        // ★商品削除（Delete1～Delete2）ここまで★
    }
}