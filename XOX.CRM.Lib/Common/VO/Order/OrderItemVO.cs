using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOX.CRM.Lib
{
    public class OrderItemVO
    {
        public long ROW_ID { get; set; }
        public long? CREATED_BY { get; set; }
        public DateTime? CREATED { get; set; }
        public long? LAST_UPD_BY { get; set; }
        public DateTime? LAST_UPD { get; set; }
        public long ORDER_ID { get; set; }
        public string ORDER_ASSET_NUM { get; set; }
        public long? PROD_ID { get; set; }
        public string STATUS_CD { get; set; }
        public long? CUST_ID { get; set; }
        public long? BILL_ID { get; set; }
        public long? SVC_AC_ID { get; set; }
        public long? PAR_ORDER_ITEM_ID { get; set; }
        public long? OLD_PAR_ORDER_ITEM_ID { get; set; }
        public long? ROOT_ORDER_ITEM_ID { get; set; }
        public string SERVICE_NUM { get; set; }
        public string OLD_SERVICE_NUM { get; set; }
        public string INTEGRATION_ID { get; set; }
        public DateTime? INSTALL_DT { get; set; }
        public int QTY { get; set; }
        public decimal? PROD_PRICE { get; set; }
        public string ACTION_TYPE { get; set; }
        public string PROD_NAME { get; set; }
        public string PROD_TYPE { get; set; }
        public string RADIUS_GROUP_CD { get; set; }
        public decimal? CIP_PRICE { get; set; }
        public string OTC_REMARKS { get; set; }
        public string OTC_TYPE { get; set; }
        public string SUBNET_MASK { get; set; }
        public string ETC_FLG { get; set; }
        public long ROOT_PACKAGE_ID { get; set; }
    }
}
