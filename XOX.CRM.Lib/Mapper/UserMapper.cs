using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOX.CRM.Lib.DBContext;

namespace XOX.CRM.Lib.Mapper
{
    public static class User
    {
        public static UserVO Map(XOX_T_USER a)
        {
            return new UserVO()
            {
                Id = a.ROW_ID,
                Username = a.USERNAME,
                Password = a.PASSWORD,
                StaffName = a.STAFF_NAME,
                ActiveFlag = a.ACTIVE_FLG,
            };
        }
    }

    public static class UserGroup
    {
        public static List<UserGroupVO> Map(List<XOX_T_GROUP> aList)
        {
            var result = new List<UserGroupVO>();
            foreach (var v in aList)
            {
                var a = Map(v);
                result.Add(a);
            }

            return result;
        }

        public static UserGroupVO Map(XOX_T_GROUP a)
        {
            return new UserGroupVO()
            {
                Id = a.ROW_ID,
                GroupCode = a.GROUP_CD,
                Description = a.GROUP_DESC
            };
        }
    }

    public static class Module
    {
        public static List<ModuleVO> Map(List<XOX_T_MODULE> aList)
        {
            var result = new List<ModuleVO>();
            foreach (var v in aList)
            {
                var a = Map(v);
                result.Add(a);
            }

            return result;
        }

        public static ModuleVO Map(XOX_T_MODULE a)
        {
            return new ModuleVO()
            {
                ModuleId = a.ROW_ID,
                ModuleCode = a.MODULE_CD,
                ModuleName = a.MODULE_NM,
                IsAddable = a.IS_ADDABLE == 1 ? true : false,
                IsApprovable = a.IS_APPROVABLE == 1 ? true : false,
                IsDeleteable = a.IS_DELETABLE == 1 ? true : false,
                IsEditable = a.IS_EDITABLE == 1 ? true : false,
                IsRejectable = a.IS_REJECTABLE == 1 ? true : false,
                IsViewable = a.IS_VIEWABLE == 1 ? true : false
            };
        }
    }

    public static class GroupAccess
    {
        public static List<ModuleVO> Map(List<XOX_T_GROUP_ACCESS> aList)
        {
            var result = new List<ModuleVO>();
            foreach (var v in aList)
            {
                var a = Map(v);
                result.Add(a);
            }

            return result;
        }

        public static ModuleVO Map(XOX_T_GROUP_ACCESS a)
        {
            return new ModuleVO()
            {
                ModuleId = (long)a.MODULE_ID,
                IsAddable = a.IS_ADDABLE == 1 ? true : false,
                IsApprovable = a.IS_APPROVABLE == 1 ? true : false,
                IsDeleteable = a.IS_DELETABLE == 1 ? true : false,
                IsEditable = a.IS_EDITABLE == 1 ? true : false,
                IsRejectable = a.IS_REJECTABLE == 1 ? true : false,
                IsViewable = a.IS_VIEWABLE == 1 ? true : false
            };
        }
    }

}
