using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOX.CRM.Lib
{
    public interface IUserService
    {
        long UserLogin(string username, string password, ref string usergroup);

        List<UserGroupVO> UserGroupGetAll();
        UserGroupVO UserGroupGet(int id);
        bool UserGroupEdit(UserGroupVO vo);
        long UserGroupAdd(UserGroupVO vo);

        List<ModuleVO> ModuleGetAll();
        ModuleVO ModuleGet(int id);
        bool ModuleEdit(ModuleVO vo);
        long ModuleAdd(ModuleVO vo);

        List<ModuleVO> GroupAccessGet(int UserGroupId);
        bool GroupAccessEdit(long UserGroupId, List<ModuleVO> Modules);

        List<UserVO> UserGetAll(long UserGroupId = 0);
        UserVO UserGet(int id);
        bool UserEdit(UserVO vo);
        long UserAdd(UserVO vo);

        void UserUnlock(long UserId);
        void UserLock(long UserId);
    }
}
