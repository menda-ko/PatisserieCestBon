using Microsoft.Ajax.Utilities;
using PatisserieCestBon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
            /* 商品テーブルから、削除フラグがFalseのもののみ取得
             * このメソッドはAdd～Deleteの正常終了時や一部のエラー時にも呼び出される */
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
            return View("Add1");
        }
        // Add2 … 商品追加確認画面へ
        public ActionResult Add2(int itemNo, string itemName, string size, string photoUrl, int unitPrice,
            string assortment, string category)
        {
            // エラーメッセージ格納リスト作成
            List<string> errorMessageList = new List<string>();
            // 入力内容チェック
            // 商品番号（必須）
            if (itemNo == null)
            {
                errorMessageList.Add(Properties.Settings.Default.p032_error_RecuiredItemNo);
            }
            // 商品番号が入力されていた場合、桁数とフォーマットのチェック
            else 
            {
                // 商品番号を文字列化して桁数と整数かどうかのチェック
                string itemNoString = itemNo.ToString();
                if (itemNoString.Length != 4 || int.TryParse(itemNoString, out int itemNoIntCheck) == false)
                {
                    errorMessageList.Add(Properties.Settings.Default.p032_error_FormatItemNo);
                }
            }
            // 商品名（必須）
            if (string.IsNullOrWhiteSpace(itemName))
            {
                errorMessageList.Add(Properties.Settings.Default.p032_error_RecuiredItemName);
            }
            // 単価（必須）
            if (unitPrice == null)
            {
                errorMessageList.Add(Properties.Settings.Default.p032_error_RecuiredUnitPrice);
            }
            else
            {
                // 単価を文字列化して整数かどうかのチェック
                string unitPriceString = unitPrice.ToString();
                if (int.TryParse(unitPriceString, out int unitPriceIntCheck) == false)
                {
                    errorMessageList.Add(Properties.Settings.Default.p032_error_FormatUnitPrice);
                }
            }
            // 寸法（必須）
            if (string.IsNullOrWhiteSpace(size))
            {
                errorMessageList.Add(Properties.Settings.Default.p032_error_RecuiredSize);
            }
            /* 寸法が入力されていた場合、フォーマットをチェック
             * 「半角数字x半角数字」の形式*/
            else if (Regex.IsMatch(size, "^[0-9]+[x][0-9]+$") == false)
            {
                errorMessageList.Add(Properties.Settings.Default.p032_error_FormatSize);
            }
            // ここまででエラーがあった場合、エラーメッセージリストとその件数を渡して入力画面に戻る
            int errorMessageListCount = errorMessageList.Count();
            if (errorMessageListCount > 0)
            {
                ViewBag.LengthOfErrorMessageList = errorMessageListCount;
                ViewBag.ErrorMessageList = errorMessageList;
                return View("Add1");
            }
            // ここまででエラーがない場合、確認画面に表示するために値を渡す
            else
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
                ViewBag.model = item;
                return View("Add2");
            }
        }
        // Add3 … 商品テーブルと在庫テーブルにレコードを登録
        public ActionResult Add3(int itemNo, string itemName, string size, string photoUrl, int unitPrice,
            string assortment, string category)
        {
            // エラーメッセージを格納するリストを作成
            List<string> errorMessageList = new List<string>();
            // 商品番号または商品名の重複チェック
            var itemDupulicate = db.Items
                .Where(i => (i.itemNo.Equals(itemNo)) | (i.itemName.Equals(itemName)));
            // どちらかが重複していた場合は入力画面に戻ってエラーメッセージ表示
            if (itemDupulicate.Count() > 0)
            {
                errorMessageList.Add(Properties.Settings.Default.p032_error_DupulicatedItem);
                ViewBag.ErrorMessageList = errorMessageList;
                // エラーメッセージリストの要素数を渡す
                int lengthOfErrorMessageList = errorMessageList.Count();
                ViewBag.LengthOfErrorMessageList = lengthOfErrorMessageList;
                return Add2(itemNo, itemName, size, photoUrl, unitPrice, assortment, category);
            }
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
            /* 在庫テーブルにもレコードを登録する
             （数量・入荷予定日はそれぞれデフォルトで0と・NULLが入る） */
            var newItemStock = new Stock()
            {
                itemNo = itemNo
                ,itemName = itemName
            };
            db.Stocks.Add(newItemStock);
            db.SaveChanges();
            //登録成功のメッセージ
                ViewBag.InfoMessage = Properties.Settings.Default.p031_info_AddSuccess;
            // 商品一覧に戻るため、同じコントローラ内のList()メソッドを呼び出し
            return List();
            }
        // ★商品追加登録（Add1～Add3）ここまで★

        // ★商品更新（Update1～Update3）ここから★
        // Update1 … 更新入力画面へ
        public ActionResult Update1(int id)
        {
            // 更新対象の商品情報をDBから取得して入力画面に渡す
            ViewBag.Item = db.Items.Find(id);
            return View("Update1");
        }
        // Update2 … 更新入力画面で入力した情報を確認画面に渡す
        public ActionResult Update2(int id, string itemName, string size, string photoUrl, int unitPrice,
            string assortment, string category)
        {
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
            return View("Update2");
        }
        // Update3 … DBを更新し一覧画面に戻る
        public ActionResult Update3(int id, string itemName, string size, string photoUrl, int unitPrice,
            string assortment, string category)
        {
            // エラーメッセージを格納するリストを作成
            List<string> errorMessageList = new List<string>();
            var item = db.Items.Find(id);
            if (item.deleteFlag == true)
            {
                // 更新しようとした商品がすでに削除されていた（＝削除フラグがTrue）場合のエラーメッセージ
                ViewBag.ErrorMessage = Properties.Settings.Default.p031_error_AlreadyDeletedItem;
                // 商品一覧に戻るため、同じコントローラ内のList()メソッドを呼び出し
                return List();
            }
            else
            {
                // 削除フラグがFalseだった場合、商品名の重複チェック
                var itemNameDupulicate = db.Items
                    .Where(i => i.itemName.Equals(itemName));
                // 商品名が重複していた場合は入力画面に戻ってエラーメッセージ表示
                if (itemNameDupulicate.Count() > 0)
                {
                    errorMessageList.Add(Properties.Settings.Default.p034_error_DupulicatedItem);
                    ViewBag.ErrorMessageList = errorMessageList;
                    // エラーメッセージリストの要素数を渡す
                    int lengthOfErrorMessageList = errorMessageList.Count();
                    ViewBag.LengthOfErrorMessageList = lengthOfErrorMessageList;
                    return Update2(id, itemName, size, photoUrl, unitPrice, assortment, category);
                }
                // ここまででエラーがなかった場合、送信された内容でDBを更新する
                item.itemName = itemName;
                item.size = size;
                item.photoUrl = photoUrl;
                item.unitPrice = unitPrice;
                item.assortment = assortment;
                item.category = category;
                db.SaveChanges();
                // 更新成功のメッセージ
                ViewBag.InfoMessage = Properties.Settings.Default.p031_info_UpdateSuccess;
                // 商品一覧に戻るため、同じコントローラ内のList()メソッドを呼び出し
                return List();
            }
        }

        // ★商品削除（Delete1～Delete2）ここから★
        // Delete1 … 削除確認画面へ
        public ActionResult Delete1(int[] itemNoList)
        {
            // チェックボックスに1つもチェックが入っていない場合
            if (itemNoList == null)
            {
                // エラーメッセージ
                ViewBag.ErrorMessage = Properties.Settings.Default.p031_error_NotChecked;
                // 商品一覧に戻るため、同じコントローラ内のList()メソッドを呼び出し
                return List();
            }
            else
            {
                // 確認画面に表示するための削除商品リストを作成
                List<Item> deleteItemList = new List<Item>();
                foreach (var itemNo in itemNoList)
                {
                    // 在庫が1以上でないかチェック
                    var stockCheck = db.Stocks
                        .Where(s => s.itemNo.Equals(itemNo) & s.stock > 0);
                    // 未出荷の受注がないかチェック
                    var orderCheck = db.OrderInfoes
                        .Where(o => o.itemNo.Equals(itemNo) & o.status == "未出荷");
                    // 在庫が1以上あった場合、または未出荷の受注が1件以上あった場合は削除できないのでエラー
                    if ((stockCheck.Count() > 0) || (orderCheck.Count() > 0))
                    {
                        // エラーメッセージ
                        ViewBag.ErrorMessage = Properties.Settings.Default.p031_error_ThereIsStockOrOrder;
                        // 商品一覧に戻るため、同じコントローラ内のList()メソッドを呼び出し
                        return List();
                    }
                    else
                    {
                        // 在庫0かつ未出荷の受注がない商品の場合、削除商品リストに追加
                        deleteItemList.Add(db.Items.Find(itemNo));
                    }
                }
                ViewBag.DeleteItemList = deleteItemList;
                return View("Delete1");
            }
        }
        // Delete2 … 商品の削除（商品テーブルの削除フラグをTrueに更新）
        public ActionResult Delete2(int[] itemNoList)
        {
                foreach (var itemNo in itemNoList)
            {
                var item = db.Items.Find(itemNo);
                if(item.deleteFlag == true)
                {
                    // 削除しようとした商品がすでに削除されていた（＝削除フラグがTrue）場合のエラーメッセージ
                    ViewBag.ErrorMessage = Properties.Settings.Default.p031_error_AlreadyDeletedItem;
                    // 削除確認画面に戻る
                    return Delete1(itemNoList);
                }
                item.deleteFlag = true;
            }
            db.SaveChanges();
            // 削除成功のメッセージ
            ViewBag.InfoMessage = Properties.Settings.Default.p031_info_DeleteSuccess;
            // 商品一覧に戻るため、同じコントローラ内のList()メソッドを呼び出し
            return List();
        }
        // ★商品削除（Delete1～Delete2）ここまで★
    }
}