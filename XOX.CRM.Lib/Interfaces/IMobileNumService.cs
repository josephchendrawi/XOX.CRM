using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOX.CRM.Lib
{
    public interface IMobileNumService
    {
        List<MobileNumVO> GetAll();
        MobileNumVO Get(long Id);
        long Add(MobileNumVO vo);
        bool Edit(MobileNumVO vo);
        List<AccountVO> GetBatchAssignedUser(string BatchNum);
        //void BatchUnAssignUser(string BatchNum, long AccountId);
        void BatchAssignUser(string BatchNum, List<long> AccountId);
    }
}
