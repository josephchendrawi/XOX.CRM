using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.IO;
using System.ComponentModel;
using XOX.CRM.Lib;
using System.Web.Mvc;
using CRM.Models;
using System.Text;
using XOX.CRM.Lib.Common.Constants;

namespace CRM
{
    public static class Helper
    {
        public static string GetDescription(this Enum enumValue)
        {
            try
            {
                return enumValue.GetType()
                                .GetMember(enumValue.ToString())
                                .First()
                                .GetCustomAttribute<DescriptionAttribute>()
                                .Description;
            }
            catch
            {
                return "-";
            }
        }
        
        public static IEnumerable<int> GetYearList()
        {
            return Enumerable.Range(DateTime.Now.Year, 15);
        }
        public static string UploadingFile(HttpPostedFileBase FileModel)
        {
            string datetime = DateTime.Now.ToString("yyyyMMddhhmmss");
            string uploadedPath = "";
            if (FileModel.ContentLength > 0)
            {
                var fileName = Path.GetFileName(FileModel.FileName);
                fileName = datetime + "-" + fileName;
                fileName = fileName.Replace(" ", "-");
                //upload to local
                uploadedPath = fileName;
                var path = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/upload"), fileName);
                FileModel.SaveAs(path);
            }
            else
            {
                throw new Exception("Invalid File.");
            }

            return uploadedPath;
        }
        public static string UploadingFile2DriveD(HttpPostedFileBase FileModel, string SubFolderName)
        {
            string datetime = DateTime.Now.ToString("yyyyMMddhhmmss");
            string uploadedPath = "";
            if (FileModel.ContentLength > 0)
            {
                var fileName = Path.GetFileName(FileModel.FileName);
                fileName = datetime + "-" + fileName;
                fileName = fileName.Replace(" ", "-");
                //upload to local
                uploadedPath = SubFolderName + @"\" + fileName;
                var path = XOXConstants.DRIVE_D_ATT_PATH + uploadedPath;

                if (!Directory.Exists(Path.GetDirectoryName(path)))
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    
                FileModel.SaveAs(path);
            }
            else
            {
                throw new Exception("Invalid File.");
            }

            return uploadedPath;
        }
            
        public static List<KeyValuePair<string, string>> GetLookupList(string LookupKey)
        {
            ICommonService CommonService = new CommonService();

            var lookup = new LookupVO() { LookupKey = LookupKey };
            CommonService.GetLookupValues(lookup);
                
            return lookup.KeyValues;
        }

        public static List<KeyValuePair<string, string>> GetEnumList(Type enumType, bool NameAsValue = false)
        {
            var typeOfProperty = enumType;
            List<KeyValuePair<string, string>> enumValues = new List<KeyValuePair<string, string>>();
            foreach (var v in Enum.GetValues(typeOfProperty))
            {
                var text = v.GetType().GetMember(v.ToString()).First().GetCustomAttribute<DescriptionAttribute>();
                if (NameAsValue == false)
                {
                    enumValues.Add(new KeyValuePair<string, string>(text == null ? v.ToString() : text.Description, ((int)v).ToString()));
                }
                else
                {
                    enumValues.Add(new KeyValuePair<string, string>(text == null ? v.ToString() : text.Description, v.ToString()));
                }
            }

            return enumValues;
        }

        public static List<SelectListItem> GetAssigneeList()
        {
            var list = UserService.GetAll();

            List<SelectListItem> result = new List<SelectListItem>();
            foreach (var v in list)
            {
                result.Add(new SelectListItem()
                {
                    Text = v.Username,
                    Value = v.Id.ToString()
                });
            }

            return result;
        }
        
        public static string GetLookupNameByVal(string LookupVal, string LookupType)
        {
            ICommonService CommonService = new CommonService();
            var result = CommonService.GetLookupNameByValue(LookupVal, LookupType);
            return result == null ? "-" : result;
        }
        public static string GetLookupValByName(string LookupName, string LookupType)
        {
            ICommonService CommonService = new CommonService();
            var result = CommonService.GetLookupValueByName(LookupName, LookupType);
            return result == null ? "-" : result;
        }

        public static string GetNameByUsername(long UserId)
        {
            return UserService.GetName(UserId);
        }

        public static decimal GetDeposit(long OrderId)
        {
            IProductService ProductService = new ProductService();
            return ProductService.GetDeposit(OrderId);
        }

        public static decimal GetAdvancePayment(long OrderId)
        {
            IProductService ProductService = new ProductService();
            return ProductService.GetAdvancePayment(OrderId);
        }

        public static bool IsAuthorized(string Module, string Access)
        {
            bool authorize = UserService.IsAuthorized(Module, Access);

            return authorize;
        }

        public static List<AccountListVM> GetSuppLines(long AccountId)
        {
            List<AccountListVM> list = new List<AccountListVM>();
            IAccountService AccountService = new AccountService();
            var result = AccountService.GetAllSupplementaryLine(AccountId);

            foreach (var v in result)
            {
                list.Add(new AccountListVM()
                {
                    AccountId = v.AccountId,
                    MSISDN = v.PersonalInfo.MSISDNNumber
                });
            }

            return list;
        }

        public static string GetCSV<T>(this List<T> list)
        {
            StringBuilder sb = new StringBuilder();

            //Get the properties for type T for the headers
            PropertyInfo[] propInfos = typeof(T).GetProperties();
            for (int i = 0; i <= propInfos.Length - 1; i++)
            {
                sb.Append(propInfos[i].Name);

                if (i < propInfos.Length - 1)
                {
                    sb.Append(",");
                }
            }

            sb.AppendLine();

            //Loop through the collection, then the properties and add the values
            for (int i = 0; i <= list.Count - 1; i++)
            {
                T item = list[i];
                for (int j = 0; j <= propInfos.Length - 1; j++)
                {
                    object o = item.GetType().GetProperty(propInfos[j].Name).GetValue(item, null);
                    if (o != null)
                    {
                        string value = o.ToString();

                        //Check if the value contans a comma and place it in quotes if so
                        if (value.Contains(","))
                        {
                            value = string.Concat("\"", value, "\"");
                        }

                        //Replace any \r or \n special characters from a new line with a space
                        if (value.Contains("\r"))
                        {
                            value = value.Replace("\r", " ");
                        }
                        if (value.Contains("\n"))
                        {
                            value = value.Replace("\n", " ");
                        }

                        sb.Append(value);
                    }

                    if (j < propInfos.Length - 1)
                    {
                        sb.Append(",");
                    }
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

        public static GetSubscriberProfileResponse GetSubsProfile(long AccountId)
        {
            IAccountService AccountService = new AccountService();
            var subProfile = AccountService.GetSubscriberProfile(AccountId);

            return subProfile;
        }

        public static bool? GetAdvancePaymentFlagResponse(long AccountId)
        {
            IAccountService AccountService = new AccountService();
            var flgResponse = AccountService.GetAdvancePaymentFlagResponse(AccountId);

            return flgResponse;
        }

    }

    public enum GroupPosition
    {
        Right = 1,
        Left = 2
    }

}