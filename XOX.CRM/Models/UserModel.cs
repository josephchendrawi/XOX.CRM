using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CRM.Models
{
    public class LoginModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class UserModel
    {
        public long Id { get; set; }
        [Display(Name = "Email")]
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Staff Name")]
        public string StaffName { get; set; }
        [Required]
        [Display(Name = "User Group")]
        public long UserGroupId { get; set; }
        public string UserGroupCode { get; set; }

        public string ActiveFlag { get; set; }
    }
    public class UserListData : DataTableModel
    {
        public List<UserModel> aaData;
    }

    public class UserGroupModel
    {
        public long Id { get; set; }
        [Required]
        [Display(Name = "Group Code")]
        public string GroupCode { get; set; }
        [Required]
        public string Description { get; set; }
    }
    public class UserGroupListData : DataTableModel
    {
        public List<UserGroupModel> aaData;
    }

    public class ModuleModel
    {
        public long Id { get; set; }
        [Display(Name = "Module Code")]
        public string ModuleCode { get; set; }
        [Display(Name = "Module Name")]
        public string ModuleName { get; set; }
        [Display(Name = "Allow View")]
        public bool IsViewable { get; set; }
        [Display(Name = "Allow Add")]
        public bool IsAddable { get; set; }
        [Display(Name = "Allow Edit")]
        public bool IsEditable { get; set; }
        [Display(Name = "Allow Delete")]
        public bool IsDeleteable { get; set; }
        [Display(Name = "Allow Approve")]
        public bool IsApprovable { get; set; }
        [Display(Name = "Allow Reject")]
        public bool IsRejectable { get; set; }
    }
    public class ModuleListData : DataTableModel
    {
        public List<ModuleModel> aaData;
    }

    public class GroupAccessVM
    {
        public List<UserGroupModel> UserGroups;
        public List<ModuleModel> Modules;
    }

}