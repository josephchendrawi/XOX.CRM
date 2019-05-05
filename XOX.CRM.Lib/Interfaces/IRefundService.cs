using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOX.CRM.Lib
{
    public interface IRefundService
    {
        long Add(AddRefundVO AddRefundVO, long UserId = 0);
        List<RefundVO> GetAll(int startIdx, int length, ref int TotalCount, string orderBy = "", string orderDirection = "", DateTime? From = null, DateTime? To = null);
    }
}
