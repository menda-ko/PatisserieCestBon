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
    
    public partial class OrderInfo
    {
        public decimal orderNo { get; set; }
        public decimal orderSeqNo { get; set; }
        public decimal itemNo { get; set; }
        public string itemName { get; set; }
        public decimal quantity { get; set; }
        public System.DateTime deliveryDate { get; set; }
        public System.DateTime orderDate { get; set; }
        public string status { get; set; }
        public decimal customerId { get; set; }
    
        public virtual Item Item { get; set; }
    }
}
