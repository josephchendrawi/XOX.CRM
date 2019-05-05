using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOX.CRM.Lib
{
    public class ProductVO
    {
        public long ROW_ID { get; set; }
        public long? CREATED_BY { get; set; }
        public DateTime? CREATED { get; set; }
        public long? LAST_UPD_BY { get; set; }
        public DateTime? LAST_UPD { get; set; }
        public long PROD_ITEM_ID { get; set; }
        public long PAR_ITEM_ID { get; set; }
        public long ROOT_ITEM_ID { get; set; }
        public string PRD_DESC { get; set; }
        public bool VAS_FLG { get; set; }
        public string PRD_LVL { get; set; }
        public decimal? PRD_PRICE { get; set; }
        public string PRD_CATEGORY { get; set; }
        public string PRD_TYPE { get; set; }
        public string PRD_PRICE_TYPE { get; set; }
        public string QUOTA { get; set; }
        public string EXT_PROD_NAME { get; set; }
        public string GST_CD { get; set; }
        public int GST_PT { get; set; }
        public string DISPLAY_FLG { get; set; }

        public bool Is_Package { get; set; }
        public long Parent_Package_ID { get; set; }
    }
}