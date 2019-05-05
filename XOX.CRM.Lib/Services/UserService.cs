using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XOX.CRM.Lib.Common.Constants;
using XOX.CRM.Lib.DBContext;

namespace XOX.CRM.Lib
{
    public class UserService : IUserService
    {
        public static bool IsAuthenticated()
        {
            using (var DBContext = new CRMDbContext())
            {
                long UserId = long.Parse(Thread.CurrentPrincipal.Identity.Name);

                var result = from d in DBContext.XOX_T_USER
                             where d.ROW_ID == UserId && d.ACTIVE_FLG == XOXConstants.Active
                             select d;

                if (result.Count() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static bool IsAuthorized(string module, string access)
        {
            try
            {
                using (var DBContext = new CRMDbContext())
                {
                    long UserId = long.Parse(Thread.CurrentPrincipal.Identity.Name);

                    var useraccess = from d in DBContext.XOX_T_USER
                                     join e in DBContext.XOX_T_USER_GROUP on d.ROW_ID equals e.USER_ID
                                     join f in DBContext.XOX_T_GROUP_ACCESS on e.GROUP_ID equals f.GROUP_ID
                                     join g in DBContext.XOX_T_MODULE on f.MODULE_ID equals g.ROW_ID
                                     where d.ROW_ID == UserId && g.MODULE_CD.ToUpper() == module.ToUpper()
                                     select f;

                    if (useraccess.Count() > 0)
                    {
                        if (access.ToUpper() == "VIEW")
                        {
                            useraccess = useraccess.Where(p => p.IS_VIEWABLE == 1);
                        }
                        else if (access.ToUpper() == "EDIT")
                        {
                            useraccess = useraccess.Where(p => p.IS_EDITABLE == 1);
                        }
                        else if (access.ToUpper() == "ADD")
                        {
                            useraccess = useraccess.Where(p => p.IS_ADDABLE == 1);
                        }
                        else if (access.ToUpper() == "DELETE")
                        {
                            useraccess = useraccess.Where(p => p.IS_DELETABLE == 1);
                        }
                        else if (access.ToUpper() == "APPROVE")
                        {
                            useraccess = useraccess.Where(p => p.IS_APPROVABLE == 1);
                        }
                        else if (access.ToUpper() == "REJECT")
                        {
                            useraccess = useraccess.Where(p => p.IS_REJECTABLE == 1);
                        }

                        if (useraccess.Count() > 0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static List<UserVO> GetAll()
        {
            List<UserVO> List = new List<UserVO>();
            using (var DBContext = new CRMDbContext())
            {
                var result = from d in DBContext.XOX_T_USER
                             where d.ACTIVE_FLG == XOXConstants.Active
                             select d;

                foreach (var v in result)
                {
                    List.Add(new UserVO()
                    {
                        Id = v.ROW_ID,
                        Username = v.USERNAME,
                        ActiveFlag = v.ACTIVE_FLG
                    });
                }
            }

            return List;
        }

        public static string GetName(long id)
        {
            using (var DBContext = new CRMDbContext())
            {
                var result = from d in DBContext.XOX_T_USER
                                where d.ROW_ID == id
                                select d;

                if (result.Count() > 0)
                    return result.First().STAFF_NAME;
                else
                    return "N/A";                
            }
        }

        public long UserLogin(string username, string password, ref string usergroup)
        {
            var encrypted = CalculateMD5Hash(password);
            using (var DBContext = new CRMDbContext())
            {
                var user = from d in DBContext.XOX_T_USER
                           where d.USERNAME == username
                           select d;

                if (user.Where(m => m.ACTIVE_FLG == XOXConstants.Active).Count() <= 0)
                {
                    throw new Exception("This User is locked.");
                }

                if (user.Count() > 0)
                {
                    if (user.Where(m => m.PASSWORD == encrypted).Count() > 0)
                    {
                        user = user.Where(m => m.PASSWORD == encrypted);

                        var XOX_T_USER_GROUP = from d in DBContext.XOX_T_USER_GROUP
                                               join e in DBContext.XOX_T_GROUP on d.GROUP_ID equals e.ROW_ID
                                               where d.USER_ID == user.FirstOrDefault().ROW_ID
                                               select e;

                        usergroup = XOX_T_USER_GROUP.First().GROUP_CD;


                        //reset login failed attempt
                        user.FirstOrDefault().LOGIN_FAILED_ATTEMPT = 0;
                        DBContext.SaveChanges();

                        return user.First().ROW_ID;
                    }
                    else
                    {
                        //record login failed attempt
                        user.FirstOrDefault().LOGIN_FAILED_ATTEMPT = (user.FirstOrDefault().LOGIN_FAILED_ATTEMPT ?? 0) + 1;
                        DBContext.SaveChanges();

                        //lock user if login failed attempt exceed allowed value
                        ICommonService CommonService = new CommonService();
                        var AllowedLoginAttempt = CommonService.GetLookupValueByName("Allowed Login Attempt", "Login");
                        if (user.FirstOrDefault().LOGIN_FAILED_ATTEMPT >= int.Parse(AllowedLoginAttempt))
                        {
                            user.FirstOrDefault().ACTIVE_FLG = XOXConstants.Locked;
                            DBContext.SaveChanges();
                        }

                        return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }
        }

        public void UserUnlock(long UserId)
        {
            using (var DBContext = new CRMDbContext())
            {
                var user = from d in DBContext.XOX_T_USER
                           where d.ROW_ID == UserId
                           select d;

                if (user.Count() > 0)
                {
                    user.FirstOrDefault().ACTIVE_FLG = XOXConstants.Active;
                    user.FirstOrDefault().LOGIN_FAILED_ATTEMPT = 0;
                    user.FirstOrDefault().LAST_UPD_BY = long.Parse(string.IsNullOrWhiteSpace(Thread.CurrentPrincipal.Identity.Name) == true ? "1" : Thread.CurrentPrincipal.Identity.Name);
                    user.FirstOrDefault().LAST_UPD = DateTime.Now;

                    DBContext.SaveChanges();
                }
            }
        }

        public void UserLock(long UserId)
        {
            using (var DBContext = new CRMDbContext())
            {
                var user = from d in DBContext.XOX_T_USER
                           where d.ROW_ID == UserId
                           select d;

                if (user.Count() > 0)
                {
                    user.FirstOrDefault().ACTIVE_FLG = XOXConstants.Locked;
                    user.FirstOrDefault().LAST_UPD_BY = long.Parse(string.IsNullOrWhiteSpace(Thread.CurrentPrincipal.Identity.Name) == true ? "1" : Thread.CurrentPrincipal.Identity.Name);
                    user.FirstOrDefault().LAST_UPD = DateTime.Now;

                    DBContext.SaveChanges();
                }
            }
        }

        public string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public List<UserGroupVO> UserGroupGetAll()
        {
            using (var DBContext = new CRMDbContext())
            {
                var result = from d in DBContext.XOX_T_GROUP
                             where d.ACTIVE_FLG == XOXConstants.Active
                             select d;

                var aList = result.ToList<XOX_T_GROUP>();

                return Mapper.UserGroup.Map(aList);
            }
        }

        public UserGroupVO UserGroupGet(int id)
        {
            using (var DBContext = new CRMDbContext())
            {
                var result = from d in DBContext.XOX_T_GROUP
                             where d.ROW_ID == id && d.ACTIVE_FLG == XOXConstants.Active
                             select d;

                return Mapper.UserGroup.Map(result.First());
            }
        }

        public bool UserGroupEdit(UserGroupVO vo)
        {
            long UserId = long.Parse(Thread.CurrentPrincipal.Identity.Name);
            using (var DBContext = new CRMDbContext())
            {
                var result = from d in DBContext.XOX_T_GROUP
                             where d.ROW_ID == vo.Id && d.ACTIVE_FLG == XOXConstants.Active
                             select d;

                if (result.Count() > 0)
                {
                    var v = result.First();

                    v.GROUP_CD = vo.GroupCode;
                    v.GROUP_DESC = vo.Description;
                    v.LAST_UPD = DateTime.Now;
                    v.LAST_UPD_BY = UserId;

                    DBContext.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public long UserGroupAdd(UserGroupVO vo)
        {
            long UserId = long.Parse(Thread.CurrentPrincipal.Identity.Name);
            using (var DBContext = new CRMDbContext())
            {
                XOX_T_GROUP v = new XOX_T_GROUP();
                
                v.GROUP_CD = vo.GroupCode;
                v.GROUP_DESC = vo.Description;
                v.CREATED = DateTime.Now;
                v.CREATED_BY = UserId;
                v.ACTIVE_FLG = XOXConstants.Active;

                DBContext.XOX_T_GROUP.Add(v);
                DBContext.SaveChanges();

                return v.ROW_ID;
            }
        }

        public List<ModuleVO> ModuleGetAll()
        {
            using (var DBContext = new CRMDbContext())
            {
                var result = from d in DBContext.XOX_T_MODULE
                             select d;

                var aList = result.ToList();

                return Mapper.Module.Map(aList);
            }
        }

        public ModuleVO ModuleGet(int id)
        {
            using (var DBContext = new CRMDbContext())
            {
                var result = from d in DBContext.XOX_T_MODULE
                             where d.ROW_ID == id
                             select d;

                return Mapper.Module.Map(result.First());
            }
        }

        public bool ModuleEdit(ModuleVO vo)
        {
            long UserId = long.Parse(Thread.CurrentPrincipal.Identity.Name);
            using (var DBContext = new CRMDbContext())
            {
                var result = from d in DBContext.XOX_T_MODULE
                             where d.ROW_ID == vo.ModuleId
                             select d;

                if (result.Count() > 0)
                {
                    var v = result.First();

                    v.MODULE_CD = vo.ModuleCode;
                    v.MODULE_NM = vo.ModuleName;
                    v.IS_ADDABLE = vo.IsAddable == true ? 1 : 0;
                    v.IS_APPROVABLE = vo.IsApprovable == true ? 1 : 0;
                    v.IS_DELETABLE = vo.IsDeleteable == true ? 1 : 0;
                    v.IS_EDITABLE = vo.IsEditable == true ? 1 : 0;
                    v.IS_REJECTABLE = vo.IsRejectable == true ? 1 : 0;
                    v.IS_VIEWABLE = vo.IsViewable == true ? 1 : 0;
                    v.LAST_UPD = DateTime.Now;
                    v.LAST_UPD_BY = UserId;

                    DBContext.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public long ModuleAdd(ModuleVO vo)
        {
            long UserId = long.Parse(Thread.CurrentPrincipal.Identity.Name);
            using (var DBContext = new CRMDbContext())
            {
                XOX_T_MODULE v = new XOX_T_MODULE();
                
                v.MODULE_CD = vo.ModuleCode;
                v.MODULE_NM = vo.ModuleName;
                v.IS_ADDABLE = vo.IsAddable == true ? 1 : 0;
                v.IS_APPROVABLE = vo.IsApprovable == true ? 1 : 0;
                v.IS_DELETABLE = vo.IsDeleteable == true ? 1 : 0;
                v.IS_EDITABLE = vo.IsEditable == true ? 1 : 0;
                v.IS_REJECTABLE = vo.IsRejectable == true ? 1 : 0;
                v.IS_VIEWABLE = vo.IsViewable == true ? 1 : 0;
                v.CREATED = DateTime.Now;
                v.CREATED_BY = UserId;

                DBContext.XOX_T_MODULE.Add(v);
                DBContext.SaveChanges();

                return v.ROW_ID;
            }
        }

        public List<ModuleVO> GroupAccessGet(int UserGroupId)
        {
            using (var DBContext = new CRMDbContext())
            {
                var result = from d in DBContext.XOX_T_GROUP_ACCESS
                             where d.GROUP_ID == UserGroupId
                             select d;

                var aList = result.ToList();

                return Mapper.GroupAccess.Map(aList);
            }
        }

        public bool GroupAccessEdit(long UserGroupId, List<ModuleVO> Modules)
        {
            long UserId = long.Parse(Thread.CurrentPrincipal.Identity.Name);
            using (var DBContext = new CRMDbContext())
            {
                foreach (var v in Modules)
                {
                    var result = from d in DBContext.XOX_T_GROUP_ACCESS
                                 where d.GROUP_ID == UserGroupId && d.MODULE_ID == v.ModuleId
                                 select d;

                    if (result.Count() > 0)
                    {
                        var n = result.First();
                        n.IS_ADDABLE = v.IsAddable == true ? 1 : 0;
                        n.IS_APPROVABLE = v.IsApprovable == true ? 1 : 0;
                        n.IS_DELETABLE = v.IsDeleteable == true ? 1 : 0;
                        n.IS_EDITABLE = v.IsEditable == true ? 1 : 0;
                        n.IS_REJECTABLE = v.IsRejectable == true ? 1 : 0;
                        n.IS_VIEWABLE = v.IsViewable == true ? 1 : 0;
                        n.LAST_UPD = DateTime.Now;
                        n.LAST_UPD_BY = UserId;

                        DBContext.SaveChanges();
                    }
                    else
                    {
                        XOX_T_GROUP_ACCESS n = new XOX_T_GROUP_ACCESS();
                        n.MODULE_ID = v.ModuleId;
                        n.GROUP_ID = UserGroupId;
                        n.IS_ADDABLE = v.IsAddable == true ? 1 : 0;
                        n.IS_APPROVABLE = v.IsApprovable == true ? 1 : 0;
                        n.IS_DELETABLE = v.IsDeleteable == true ? 1 : 0;
                        n.IS_EDITABLE = v.IsEditable == true ? 1 : 0;
                        n.IS_REJECTABLE = v.IsRejectable == true ? 1 : 0;
                        n.IS_VIEWABLE = v.IsViewable == true ? 1 : 0;
                        n.CREATED = DateTime.Now;
                        n.CREATED_BY = UserId;

                        DBContext.XOX_T_GROUP_ACCESS.Add(n);
                        DBContext.SaveChanges();
                    }

                }
                
                return true;                
            }
        }

        public List<UserVO> UserGetAll(long UserGroupId = 0)
        {
            using (var DBContext = new CRMDbContext())
            {
                var result = from d in DBContext.XOX_T_USER
                             //where d.ACTIVE_FLG == ActiveFlag
                             select d;

                var aList = new List<UserVO>();
                foreach (var v in result)
                {
                    var a = Mapper.User.Map(v);
                    var usergroup = from d in DBContext.XOX_T_USER_GROUP
                                    join e in DBContext.XOX_T_GROUP on d.GROUP_ID equals e.ROW_ID
                                    where d.USER_ID == v.ROW_ID
                                    select new { d.GROUP_ID, e.GROUP_CD };
                    if (usergroup.Count() > 0)
                    {
                        a.UserGroupId = (long)usergroup.First().GROUP_ID;
                        a.UserGroupCode = usergroup.First().GROUP_CD;
                    }
                    aList.Add(a);
                }

                if (UserGroupId > 0)
                {
                    aList = aList.Where(m => m.UserGroupId == UserGroupId).ToList();
                }

                return aList;
            }
        }

        public UserVO UserGet(int id)
        {
            using (var DBContext = new CRMDbContext())
            {
                var result = from d in DBContext.XOX_T_USER
                             where d.ROW_ID == id && (d.ACTIVE_FLG == XOXConstants.Active || d.ACTIVE_FLG == XOXConstants.Locked)
                             select d;

                var a = Mapper.User.Map(result.First());

                var usergroup = from d in DBContext.XOX_T_USER_GROUP
                                join e in DBContext.XOX_T_GROUP on d.GROUP_ID equals e.ROW_ID
                                where d.USER_ID == result.FirstOrDefault().ROW_ID
                                select new { d.GROUP_ID, e.GROUP_CD };
                if (usergroup.Count() > 0)
                {
                    a.UserGroupId = (long)usergroup.First().GROUP_ID;
                    a.UserGroupCode = usergroup.First().GROUP_CD;
                }

                return a;
            }
        }

        public bool UserEdit(UserVO vo)
        {
            long UserId = long.Parse(Thread.CurrentPrincipal.Identity.Name);
            using (var DBContext = new CRMDbContext())
            {
                var result = from d in DBContext.XOX_T_USER
                             where d.ROW_ID == vo.Id && d.ACTIVE_FLG == XOXConstants.Active
                             select d;

                if (result.Count() > 0)
                {
                    var v = result.First();

                    v.USERNAME = vo.Username;
                    var encrypted = CalculateMD5Hash(vo.Password);
                    v.PASSWORD = encrypted;
                    v.STAFF_NAME = vo.StaffName;
                    v.LAST_UPD = DateTime.Now;
                    v.LAST_UPD_BY = UserId;
                    DBContext.SaveChanges();

                    var usergroup = from d in DBContext.XOX_T_USER_GROUP
                                    where d.USER_ID == vo.Id
                                    select d;

                    if (usergroup.Count() > 0)
                    {
                        usergroup.First().GROUP_ID = (long)vo.UserGroupId;
                        v.LAST_UPD = DateTime.Now;
                        v.LAST_UPD_BY = UserId;
                        DBContext.SaveChanges();
                    }
                    else
                    {
                        XOX_T_USER_GROUP y = new XOX_T_USER_GROUP();
                        y.GROUP_ID = vo.UserGroupId;
                        y.USER_ID = v.ROW_ID;
                        y.CREATED = DateTime.Now;
                        y.CREATED_BY = UserId;
                        DBContext.XOX_T_USER_GROUP.Add(y);
                        DBContext.SaveChanges();
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public long UserAdd(UserVO vo)
        {
            long UserId = long.Parse(Thread.CurrentPrincipal.Identity.Name);
            using (var DBContext = new CRMDbContext())
            {
                XOX_T_USER v = new XOX_T_USER();
                v.USERNAME = vo.Username;
                var encrypted = CalculateMD5Hash(vo.Password);
                v.PASSWORD = encrypted;
                v.STAFF_NAME = vo.StaffName;
                v.CREATED = DateTime.Now;
                v.CREATED_BY = UserId;
                v.ACTIVE_FLG = XOXConstants.Active;
                DBContext.XOX_T_USER.Add(v);
                DBContext.SaveChanges();

                XOX_T_USER_GROUP y = new XOX_T_USER_GROUP();
                y.GROUP_ID = vo.UserGroupId;
                y.USER_ID = v.ROW_ID;
                y.CREATED = DateTime.Now;
                y.CREATED_BY = UserId;
                DBContext.XOX_T_USER_GROUP.Add(y);
                DBContext.SaveChanges();

                return v.ROW_ID;
            }
        }

    }
}
