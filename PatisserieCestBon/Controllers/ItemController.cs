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
            // セッション確認。ログイン時にセットされたキーが残っていれば処理続行
            // キーがnullの場合、システムエラーページに飛ばす
            if (Session["loginUserName"] == null)
            {
                return View("EmployeeError");
            }
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
            // セッション確認。ログイン時にセットされたキーが残っていれば処理続行
            // キーがnullの場合、システムエラーページに飛ばす
            if (Session["loginUserName"] == null)
            {
                return View("EmployeeError");
            }
            return View("Add1");
        }
        // Add2 … 商品追加確認画面へ
        public ActionResult Add2(string itemNo, string itemName, string size, string photoUrl, string unitPrice,
            string assortment, string category)
        {
            if (Session["loginUserName"] == null)
            {
                return RedirectToAction("EmployeeError", "Login");
            }
            // エラーメッセージ格納リスト作成
            List<string> errorMessageList = new List<string>();
            // 入力内容チェック
            // 商品番号（必須）
            if (string.IsNullOrWhiteSpace(itemNo))
            {
                errorMessageList.Add(Properties.Settings.Default.p032_error_RecuiredItemNo);
            }
            // 商品番号が入力されていた場合、桁数とフォーマットのチェック
            else if (Regex.IsMatch(itemNo, "^[0-9]{4}$") == false)
            {
                errorMessageList.Add(Properties.Settings.Default.p032_error_FormatItemNo);
            }
            // 商品名（必須）
            if (string.IsNullOrWhiteSpace(itemName))
            {
                errorMessageList.Add(Properties.Settings.Default.p032_error_RecuiredItemName);
            }
            // 商品画像URL（必須）
            if (string.IsNullOrWhiteSpace(photoUrl))
            {
                errorMessageList.Add(Properties.Settings.Default.p032_error_RecuiredPhotoUrl);
            }
            /* 商品画像URLが入力されていた場合、フォーマットチェック
             * /Content/images/XXXXX.拡張子 のような形で記入 */
            else if (Regex.IsMatch(photoUrl, "^/[a-zA-Z_0-9]+/[a-zA-Z_0-9]+/[a-zA-Z_0-9]+[.][a-z]+$") == false)
            {
                errorMessageList.Add(Properties.Settings.Default.p032_error_FormatPhotoUrl);
            }
            // 単価（必須）
            if (string.IsNullOrWhiteSpace(unitPrice))
            {
                errorMessageList.Add(Properties.Settings.Default.p032_error_RecuiredUnitPrice);
            }
            else
            {
                // 単価が入力されていた場合、整数かどうかのチェック
                if (Regex.IsMatch(unitPrice, "^[0-9]+$") == false)
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
                // 単価を文字列から数値に変換。商品番号は文字列のまま
                int.TryParse(unitPrice, out int unitPriceInt);
                ViewBag.itemNo = itemNo;
                ViewBag.itemName = itemName;
                ViewBag.size = size;
                ViewBag.photoUrl = photoUrl;
                ViewBag.unitPrice = unitPriceInt;
                ViewBag.assortment = assortment;
                ViewBag.category = category;
                return View("Add2");
            }
        }
        // Add3 … 商品テーブルと在庫テーブルにレコードを登録
        public ActionResult Add3(string itemNo, string itemName, string size, string photoUrl, int unitPrice,
            string assortment, string category)
        {
            if (Session["loginUserName"] == null)
            {
                return RedirectToAction("EmployeeError", "Login");
            }
            // 商品番号をintに変換（のちの重複チェック・DB登録の際に使用）
            int.TryParse(itemNo, out int itemNoInt);
            // エラーメッセージを格納するリストを作成
            List<string> errorMessageList = new List<string>();
            // 商品番号または商品名の重複チェック
            var itemDupulicate = db.Items
                .Where(i => (i.itemNo.Equals(itemNoInt)) | (i.itemName.Equals(itemName)));
            // どちらかが重複していた場合は入力画面に戻ってエラーメッセージ表示
            if (itemDupulicate.Count() > 0)
            {
                errorMessageList.Add(Properties.Settings.Default.p032_error_DupulicatedItem);
                ViewBag.ErrorMessageList = errorMessageList;
                // エラーメッセージリストの要素数を渡す
                int lengthOfErrorMessageList = errorMessageList.Count();
                ViewBag.LengthOfErrorMessageList = lengthOfErrorMessageList;
                return Add2(itemNo, itemName, size, photoUrl, unitPrice.ToString(), assortment, category);
            }
            var item = new Item()
                {
                    itemNo = itemNoInt
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
                itemNo = itemNoInt
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
            if (Session["loginUserName"] == null)
            {
                return RedirectToAction("EmployeeError", "Login");
            }
            // 更新対象の商品情報をDBから取得して入力画面に渡す
            ViewBag.Item = db.Items.Find(id);
            return View("Update1");
        }
        // Update2 … 更新入力画面で入力した情報を確認画面に渡す
        public ActionResult Update2(int id, string itemName, string size, string photoUrl, string unitPrice,
            string assortment, string category)
        {
            if (Session["loginUserName"] == null)
            {
                return RedirectToAction("EmployeeError", "Login");
            }
            // エラーメッセージ格納リスト作成
            List<string> errorMessageList = new List<string>();
            // 入力内容チェック
            // 商品名（必須）
            if (string.IsNullOrWhiteSpace(itemName))
            {
                errorMessageList.Add(Properties.Settings.Default.p034_error_RecuiredItemName);
            }
            // 商品画像URL（必須）
            if (string.IsNullOrWhiteSpace(photoUrl))
            {
                errorMessageList.Add(Properties.Settings.Default.p034_error_RecuiredPhotoUrl);
            }
            /* 商品画像URLが入力されていた場合、フォーマットチェック
             * /Content/images/XXXXX.拡張子 のような形で記入 */
            else if (Regex.IsMatch(photoUrl, "^/[a-zA-Z_0-9]+/[a-zA-Z_0-9]+/[a-zA-Z_0-9]+[.][a-z]+$") == false)
            {
                errorMessageList.Add(Properties.Settings.Default.p034_error_FormatPhotoUrl);
            }
            // 単価（必須）
            if (string.IsNullOrWhiteSpace(unitPrice))
            {
                errorMessageList.Add(Properties.Settings.Default.p034_error_RecuiredUnitPrice);
            }
            else
            {
                // 単価が入力されていた場合、整数かどうかのチェック
                if(Regex.IsMatch(unitPrice, "^[0-9]+$") == false)
                {
                    errorMessageList.Add(Properties.Settings.Default.p034_error_FormatUnitPrice);
                }
            }
            // 寸法（必須）
            if (string.IsNullOrWhiteSpace(size))
            {
                errorMessageList.Add(Properties.Settings.Default.p034_error_RecuiredSize);
            }
            /* 寸法が入力されていた場合、フォーマットをチェック
             * 0～9の数字n桁、半角英字x、0～9の数字n桁 が連結された形であることを確認 */
            else if (Regex.IsMatch(size, "^[0-9]+[x][0-9]+$") == false)
            {
                errorMessageList.Add(Properties.Settings.Default.p034_error_FormatSize);
            }
            // ここまででエラーがあった場合、エラーメッセージのリストとその要素数を渡して入力画面に戻る
            int errorMessageListCount = errorMessageList.Count();
            if (errorMessageListCount > 0)
            {
                ViewBag.LengthOfErrorMessageList = errorMessageListCount;
                ViewBag.ErrorMessageList = errorMessageList;
                return Update1(id);
            }
            // エラーがなければ更新確認画面に値を渡す
            else
            {
                ViewBag.itemNo = id;
                ViewBag.itemName = itemName;
                ViewBag.size = size;
                ViewBag.photoUrl = photoUrl;
                ViewBag.unitPrice = unitPrice;
                ViewBag.assortment = assortment;
                ViewBag.category = category;
                return View("Update2");
            }
        }
        // Update3 … DBを更新し一覧画面に戻る
        public ActionResult Update3(int id, string itemName, string size, string photoUrl, string unitPrice,
            string assortment, string category)
        {
            if (Session["loginUserName"] == null)
            {
                return RedirectToAction("EmployeeError", "Login");
            }
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
                    .Where(i => i.itemName.Equals(itemName) && i.itemNo != id);
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
                // 単価を数値に変換
                int.TryParse(unitPrice, out int unitPriceInt);
                item.itemName = itemName;
                item.size = size;
                item.photoUrl = photoUrl;
                item.unitPrice = unitPriceInt;
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
            if (Session["loginUserName"] == null)
            {
                return RedirectToAction("EmployeeError", "Login");
            }
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
            if (Session["loginUserName"] == null)
            {
                return RedirectToAction("EmployeeError", "Login");
            }
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
        public ActionResult Catalog()
        {
            // セッション確認。ログイン時にセットされたキーが残っていれば処理続行
            // キーがnullの場合、システムエラーページに飛ばす
            if (Session["loginUserName"] == null)
            {
                return View("CustomerError");
            }
            // 未出荷の受注レコード全件取得
            var orderList = db.OrderInfoes
               // .Select(o => new { o.itemNo, o.quantity, o.status })
                .Where(o => o.status == "未出荷")
                .OrderBy(o => o.itemNo);
            // 未出荷受注全件をビューに渡す
            ViewBag.OrderList = orderList;
            // 商品の在庫全件取得
            var stockList = db.Stocks
              //  .Select(s => new { s.itemNo, s.stock })
                .OrderBy(s => s.itemNo);
            // 削除フラグがFalseの商品全件取得
            var itemList = db.Items
                .Where(i => i.deleteFlag.Equals(false))
                .OrderBy(i => i.itemNo);
            // カテゴリーごとの商品情報取得
            // ①シュー
            var creampuffItemList = itemList
                .Where(i => i.category == "シュー")
                .OrderBy(i => i.itemNo);
            // カテゴリが「シュー」の商品が1件以上あった場合、商品情報と在庫情報を商品番号で結合
            if (creampuffItemList != null)
            {
                var creampuffStockList = creampuffItemList
                    .Join(
                    stockList, i => i.itemNo, s => s.itemNo, (i, s)
                    => new
                    {
                        i.itemNo,
                        i.itemName,
                        i.photoUrl,
                        i.unitPrice,
                        i.size,
                        i.assortment,
                        i.category,
                        s.stock
                    }
                    )
                    .OrderBy(i => i.itemNo);
                // カテゴリ「シュー」の商品・在庫情報リストをビューに渡す
                ViewBag.CreamPuffList = creampuffStockList;
            }
            return View();
        }
    }
}