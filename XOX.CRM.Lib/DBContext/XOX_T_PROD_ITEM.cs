//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace XOX.CRM.Lib.DBContext
{
    using System;
    using System.Collections.Generic;
    
    public partial class XOX_T_PROD_ITEM
    {
        public long ROW_ID { get; set; }
        public Nullable<long> CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED { get; set; }
        public Nullable<long> LAST_UPD_BY { get; set; }
        public Nullable<System.DateTime> LAST_UPD { get; set; }
        public long PROD_ITEM_ID { get; set; }
        public long PROD_ID { get; set; }
        public Nullable<long> PAR_ITEM_ID { get; set; }
        public Nullable<long> ROOT_ITEM_ID { get; set; }
        public string ACTIVE_FLG { get; set; }
        public string REQ_FLG { get; set; }
        public string ITEM_ATR1 { get; set; }
        public string ITEM_ATR2 { get; set; }
    }
}
