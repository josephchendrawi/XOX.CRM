using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOX.CRM.Lib
{
    public class UserVO
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string StaffName { get; set; }
        public long UserGroupId { get; set; }
        public string UserGroupCode { get; set; }
        public string ActiveFlag { get; set; }
    }

    public class UserGroupVO
    {
        public long Id { get; set; }
        public string GroupCode { get; set; }
        public string Description { get; set; }
    }

    public class ModuleVO
    {
        public long ModuleId { get; set; }
        public string ModuleCode { get; set; }
        public string ModuleName { get; set; }
        public bool IsViewable { get; set; }
        public bool IsAddable { get; set; }
        public bool IsEditable { get; set; }
        public bool IsDeleteable { get; set; }
        public bool IsApprovable { get; set; }
        public bool IsRejectable { get; set; }
    }

}
