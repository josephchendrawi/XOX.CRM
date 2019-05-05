using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOX.CRM.Lib
{
    public interface IAssetService
    {
        void AddAsset(OrderItemVO item, long UserId = 1);
        bool ConvertAssetFromPlan(AccountVO account, GetSubscriberProfileResponse mppResponse);
        List<AssetVO> GetAll(int startIdx, int length, ref int TotalCount, string orderBy = "", string orderDirection = "", AssetVO qFilter = null);
    }
}
