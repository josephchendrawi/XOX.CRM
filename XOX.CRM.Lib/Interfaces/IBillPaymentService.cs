using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOX.CRM.Lib
{
    public interface IBillPaymentService
    {
        List<BillPaymentVO> GetAll(int startIdx, int length, ref int TotalCount, string orderBy = "", string orderDirection = "", BillPaymentVO qFilter = null);
        List<DateTime> GetAllCreatedDate();
        BatchWorkLogStatistic GetErrorAndProcessedCountInBatchWorkLog(DateTime BatchDate);
    }
}
