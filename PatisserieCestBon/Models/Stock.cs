//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはテンプレートから生成されました。
//
//     このファイルを手動で変更すると、アプリケーションで予期しない動作が発生する可能性があります。
//     このファイルに対する手動の変更は、コードが再生成されると上書きされます。
// </auto-generated>
//------------------------------------------------------------------------------

namespace PatisserieCestBon.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Stock
    {
        public decimal itemNo { get; set; }
        public string itemName { get; set; }
        [RegularExpression("[0-9]+", ErrorMessage = "数量は数字で入力してください")]
        public decimal stock1 { get; set; }
        [RegularExpression("\\d{4}/\\d{1,2}/\\d{1,2}", ErrorMessage = "入荷予定日はyyyy-mm-ddの形式で入力してください")]
        public Nullable<System.DateTime> receiptDate { get; set; }
    
        public virtual Item Item { get; set; }
        
    }
}
