using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOX.CRM.API.ServiceModel.Types
{
    public class Product
    {
        public bool Is_Package { get; set; }
        public long Parent_Package_ID { get; set; }
        public long ProductId { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string Level { get; set; }
        public string Type { get; set; }
        public decimal? Price { get; set; }
        public string PriceType { get; set; }
        public string Quota { get; set; }
        public bool VasFlag { get; set; }
        public string ExtProductName { get; set; }

        public List<Product> Child { get; set; }

        public string GST_CD { get; set; }
        public int GST_PT { get; set; }
        public string DISPLAY_FLG { get; set; }
    }

    public class Plan
    {
        public string planInfo { get; set; }
        public decimal CreditLimit { get; set; }
        public string SignUpChannel { get; set; }
        public decimal Prime { get; set; }
        public decimal InitFreeOnNetCalls { get; set; }
        public decimal InitFreeOffNetCalls { get; set; }
        public decimal InitFreeOnNetSms { get; set; }
        public decimal InitFreeOffNetSms { get; set; }
        public string DataPack { get; set; }
        public decimal InitFnFOnNetCalls { get; set; }
        public decimal InitFnFOffNetCalls { get; set; }
        public decimal InitFnFOnNetSms { get; set; }
        public decimal InitFnFOffNetSms { get; set; }
        public decimal InitFnFData { get; set; }
        public decimal InitFreeData { get; set; }
        public decimal ContractPeriod { get; set; }
        public decimal Deposit { get; set; }
        public string PrdecimaledBill { get; set; }
        public string AutoDebit { get; set; }
        public string PrintedBill { get; set; }
    }

}
