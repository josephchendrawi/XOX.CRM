using CRM.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using XOX.CRM.Lib;

namespace CRM
{
    public static class AccountMapper
    {
        public static List<AccountModel> Map(List<AccountVO> voList)
        {
            var result = new List<AccountModel>();
            foreach (var v in voList)
            {
                var a = Map(v);
                result.Add(a);
            }

            return result;
        }
        public static AccountModel Map(AccountVO vo)
        {
            var files = new List<File>();
            foreach (var v in vo.Files)
            {
                var type = v.Path.Substring(v.Path.LastIndexOf(".") + 1);
                if (type == "jpg" || type == "jpeg" || type == "png" || type == "gif")
                    type = "IMAGE";

                files.Add(new File() {
                    AccountId = v.AccountId,
                    FileId = v.FileId,
                    Path = v.Path,
                    FullPath = "~/Account/GetFile?filepath=" + v.Path,
                    Type = type
                });
            }

            return new AccountModel()
            {
                AccountId = vo.AccountId,
                PersonalInfo = new CRM.Models.PersonalInfo()
                {
                    FullName = vo.PersonalInfo.FullName,
                    BirthDate = (DateTime)vo.PersonalInfo.BirthDate,
                    ContactNumber = vo.PersonalInfo.ContactNumber,
                    CreatedDate = vo.PersonalInfo.CreatedDate,
                    CreditLimit = vo.PersonalInfo.CreditLimit,
                    CustomerAccountNumber = vo.PersonalInfo.CustomerAccountNumber,
                    CustomerStatus = ((AccountStatus)vo.PersonalInfo.CustomerStatus).ToString(),
                    Email = vo.PersonalInfo.Email,
                    Gender = vo.PersonalInfo.Gender,
                    IdentityNo = vo.PersonalInfo.IdentityNo,
                    IdentityType = vo.PersonalInfo.IdentityType,
                    MotherMaidenName = vo.PersonalInfo.MotherMaidenName,
                    MSISDNNumber = vo.PersonalInfo.MSISDNNumber,
                    Nationality = vo.PersonalInfo.Nationality.ToString(),
                    PreferredLanguage = vo.PersonalInfo.PreferredLanguage,
                    Race = vo.PersonalInfo.Race.ToString(),
                    Salutation = vo.PersonalInfo.Salutation,
                    SponsorPersonnel = vo.PersonalInfo.SponsorPersonnel,
                },
                BankingInfo = new CRM.Models.BankingInfo()
                {
                    BankAccountNumber = vo.BankingInfo.BankAccountNumber,
                    BankId = vo.BankingInfo.BankId,
                    BankName = vo.BankingInfo.BankName,
                    CardExpiryMonth = vo.BankingInfo.CardExpiryMonth,
                    CardExpiryYear = vo.BankingInfo.CardExpiryYear,
                    CardHolderName = vo.BankingInfo.CardHolderName,
                    CardIssuerBank = vo.BankingInfo.CardIssuerBank,
                    CardType = vo.BankingInfo.CardType,
                    CreditCardNo = vo.BankingInfo.CreditCardNo,
                    ThirdPartyFlag = vo.BankingInfo.ThirdPartyFlag ? 1 : 0,
                    BankAccountName = vo.BankingInfo.BankAccountName,
                    PrintedBillingFlg = vo.BankingInfo.PrintedBillingFlg ? 1 : 0,
                },
                AddressInfo = new Models.AddressInfo()
                {
                    AddressId = vo.AddressInfo.AddressId,
                    AddressLine1 = vo.AddressInfo.AddressLine1,
                    AddressLine2 = vo.AddressInfo.AddressLine2,
                    City = vo.AddressInfo.City,
                    Country = vo.AddressInfo.Country,
                    Postcode = vo.AddressInfo.Postcode,
                    State = vo.AddressInfo.State,
                    Status = vo.AddressInfo.Status,
                    AddressType = vo.AddressInfo.AddressType
                },
                BillingAddressInfo = new Models.AddressInfo()
                {
                    AddressId = vo.BillingAddressInfo.AddressId,
                    AddressLine1 = vo.BillingAddressInfo.AddressLine1,
                    AddressLine2 = vo.BillingAddressInfo.AddressLine2,
                    City = vo.BillingAddressInfo.City,
                    Country = vo.BillingAddressInfo.Country,
                    Postcode = vo.BillingAddressInfo.Postcode,
                    State = vo.BillingAddressInfo.State,
                    Status = vo.BillingAddressInfo.Status,
                    AddressType = vo.BillingAddressInfo.AddressType
                },
                Files = files,
                AccountType = vo.AccountType,
                RegistrationDate = vo.RegistrationDate,
                TerminationDate = vo.TerminationDate,
                Grade = vo.Grade,
                SIMSerialNumber = vo.SIMSerialNumber,
                ParentAccountId = vo.ParentAccountId
            };
        }
        public static AccountVO ReMap(AccountModel m)
        {
            AccountVO AccountVO = new AccountVO()
            {
                AccountId = m.AccountId,
                PersonalInfo = new PersonalInfoVO
                {
                    FullName = m.PersonalInfo.FullName,
                    BirthDate = (DateTime)m.PersonalInfo.BirthDate,
                    ContactNumber = m.PersonalInfo.ContactNumber,
                    CreatedDate = m.PersonalInfo.CreatedDate,
                    CreditLimit = m.PersonalInfo.CreditLimit,
                    CustomerAccountNumber = m.PersonalInfo.CustomerAccountNumber,
                    CustomerStatus = EnumUtil.ParseEnumInt<AccountStatus>(m.PersonalInfo.CustomerStatus),
                    Email = m.PersonalInfo.Email,
                    Gender = m.PersonalInfo.Gender,
                    IdentityNo = m.PersonalInfo.IdentityNo,
                    IdentityType = m.PersonalInfo.IdentityType,
                    MotherMaidenName = m.PersonalInfo.MotherMaidenName,
                    MSISDNNumber = m.PersonalInfo.MSISDNNumber,
                    Nationality = int.Parse(m.PersonalInfo.Nationality),
                    PreferredLanguage = m.PersonalInfo.PreferredLanguage,
                    Race = int.Parse(m.PersonalInfo.Race),
                    Salutation = m.PersonalInfo.Salutation,
                    SponsorPersonnel = m.PersonalInfo.SponsorPersonnel,
                },
                BankingInfo = new BankingInfoVO()
                {
                    BankAccountNumber = m.BankingInfo.BankAccountNumber,
                    BankId = m.BankingInfo.BankId,
                    BankName = m.BankingInfo.BankName,
                    CardExpiryMonth = m.BankingInfo.CardExpiryMonth,
                    CardExpiryYear = m.BankingInfo.CardExpiryYear,
                    CardHolderName = m.BankingInfo.CardHolderName,
                    CardIssuerBank = m.BankingInfo.CardIssuerBank,
                    CardType = m.BankingInfo.CardType,
                    CreditCardNo = m.BankingInfo.CreditCardNo,
                    ThirdPartyFlag = m.BankingInfo.ThirdPartyFlag == 1 ? true : false,
                    BankAccountName = m.BankingInfo.BankAccountName,
                    PrintedBillingFlg = m.BankingInfo.PrintedBillingFlg == 1 ? true : false,
                },
                AddressInfo = new AddressInfoVO()
                {
                    AddressId = m.AddressInfo.AddressId,
                    AddressLine1 = m.AddressInfo.AddressLine1,
                    AddressLine2 = m.AddressInfo.AddressLine2,
                    City = m.AddressInfo.City,
                    Country = m.AddressInfo.Country,
                    Postcode = m.AddressInfo.Postcode,
                    State = m.AddressInfo.State,
                    Status = m.AddressInfo.Status,
                    AddressType = m.AddressInfo.AddressType
                },
                Grade = m.Grade,
                SIMSerialNumber = m.SIMSerialNumber
            };

            if (m.BillingAddressInfo != null)
            {
                AccountVO.BillingAddressInfo = new AddressInfoVO()
                {
                    AddressId = m.BillingAddressInfo.AddressId,
                    AddressLine1 = m.BillingAddressInfo.AddressLine1,
                    AddressLine2 = m.BillingAddressInfo.AddressLine2,
                    City = m.BillingAddressInfo.City,
                    Country = m.BillingAddressInfo.Country,
                    Postcode = m.BillingAddressInfo.Postcode,
                    State = m.BillingAddressInfo.State,
                    Status = m.BillingAddressInfo.Status,
                    AddressType = m.BillingAddressInfo.AddressType
                };
            }

            return AccountVO;
        }
    }

    public static class UserGroupMapper
    {
        public static List<UserGroupModel> Map(List<UserGroupVO> voList)
        {
            var result = new List<UserGroupModel>();
            foreach (var v in voList)
            {
                var a = Map(v);
                result.Add(a);
            }

            return result;
        }
        public static UserGroupModel Map(UserGroupVO vo)
        {
            return new UserGroupModel()
            {
                Id = vo.Id,
                GroupCode = vo.GroupCode,
                Description = vo.Description
            };
        }
        public static UserGroupVO ReMap(UserGroupModel m)
        {
            return new UserGroupVO()
            {
                Id = m.Id,
                GroupCode = m.GroupCode,
                Description = m.Description
            };
        }
    }
    
    public static class ModuleMapper
    {
        public static List<ModuleModel> Map(List<ModuleVO> voList)
        {
            var result = new List<ModuleModel>();
            foreach (var v in voList)
            {
                var a = Map(v);
                result.Add(a);
            }

            return result;
        }
        public static ModuleModel Map(ModuleVO vo)
        {
            return new ModuleModel()
            {
                Id = vo.ModuleId,
                ModuleCode = vo.ModuleCode,
                ModuleName = vo.ModuleName,
                IsAddable = vo.IsAddable,
                IsApprovable = vo.IsApprovable,
                IsDeleteable = vo.IsDeleteable,
                IsEditable = vo.IsEditable,
                IsRejectable = vo.IsRejectable,
                IsViewable = vo.IsViewable
            };
        }
        public static ModuleVO ReMap(ModuleModel m)
        {
            return new ModuleVO()
            {
                ModuleId = m.Id,
                ModuleCode = m.ModuleCode,
                ModuleName = m.ModuleName,
                IsAddable = m.IsAddable,
                IsApprovable = m.IsApprovable,
                IsDeleteable = m.IsDeleteable,
                IsEditable = m.IsEditable,
                IsRejectable = m.IsRejectable,
                IsViewable = m.IsViewable
            };
        }
    }
    
    public static class GroupAccessMapper
    {
        public static List<ModuleModel> Map(List<ModuleVO> aList)
        {
            var result = new List<ModuleModel>();
            foreach (var v in aList)
            {
                var a = Map(v);
                result.Add(a);
            }

            return result;
        }

        public static ModuleModel Map(ModuleVO a)
        {
            return new ModuleModel()
            {
                Id = a.ModuleId,
                IsAddable = a.IsAddable,
                IsApprovable = a.IsApprovable,
                IsDeleteable = a.IsDeleteable,
                IsEditable = a.IsEditable,
                IsRejectable = a.IsRejectable,
                IsViewable = a.IsViewable
            };
        }
    }

    public static class UserMapper
    {
        public static List<UserModel> Map(List<UserVO> aList)
        {
            var result = new List<UserModel>();
            foreach (var v in aList)
            {
                var a = Map(v);
                result.Add(a);
            }

            return result;
        }

        public static UserModel Map(UserVO a)
        {
            return new UserModel()
            {
                Id = a.Id,
                Username = a.Username,
                Password = a.Password,
                UserGroupId = a.UserGroupId,
                UserGroupCode = a.UserGroupCode,
                StaffName = a.StaffName,
                ActiveFlag = a.ActiveFlag,
            };
        }
        public static UserVO ReMap(UserModel m)
        {
            return new UserVO()
            {
                Id = m.Id,
                Username = m.Username,
                Password = m.Password,
                UserGroupId = m.UserGroupId,
                StaffName = m.StaffName
            };
        }
    }
    
    public static class MobileNumMapper
    {
        public static List<MobileNumModel> Map(List<MobileNumVO> aList)
        {
            var result = new List<MobileNumModel>();
            foreach (var v in aList)
            {
                var a = Map(v);
                result.Add(a);
            }

            return result;
        }

        public static MobileNumModel Map(MobileNumVO a)
        {
            return new MobileNumModel()
            {
                BatchNum = a.BatchNum,
                CreatedDate = a.CreatedDate.ToString("dd MMM yyy - hh:mm:ss"),
                MobileNumId = a.MobileNumId,
                MSISDN = a.MSISDN,
                Price = a.Price
            };
        }
        public static MobileNumVO ReMap(MobileNumModel a)
        {
            return new MobileNumVO()
            {
                BatchNum = a.BatchNum,
                MobileNumId = a.MobileNumId,
                MSISDN = a.MSISDN,
                Price = a.Price
            };
        }
    }

    public static class OrderMapper
    {
        public static List<OrderModel> Map(List<OrderDetailVO> aList)
        {
            var result = new List<OrderModel>();
            foreach (var v in aList)
            {
                var a = Map(v);
                result.Add(a);
            }

            return result;
        }

        public static OrderModel Map(OrderDetailVO result)
        {
            var model = new OrderModel();

            model.OrderId = result.OrderId;
            model.MSISDN = result.MSISDN;
            model.SubscriptionPlan = result.SubscriptionPlan;
            model.SIMCard = result.SIMCard;
            model.Category = result.Category;
            model.OrderStatus = result.OrderStatus;
            model.RegistrationDate = result.Account.RegistrationDate;
            model.SubmissionDate = result.SubmissionDate;
            model.OrderNum = result.OrderNum;
            model.OrderType = result.OrderType;
            model.Remarks = result.Remarks;
            model.PaymentCollected = result.PaymentCollected;
            model.AccountId = result.Account.AccountId;
            model.ParentAccountId = result.Account.ParentAccountId;
            model.PortReqFormId = result.PortReqFormId;
            model.PersonalDetails = new PersonalDetails()
            {
                BankAccountNumber = result.Account.BankingInfo.BankAccountNumber,
                BankName = result.Account.BankingInfo.BankName,
                BirthDate = result.Account.PersonalInfo.BirthDate,
                Email = result.Account.PersonalInfo.Email,
                FullName = result.Account.PersonalInfo.FullName,
                Gender = result.Account.PersonalInfo.Gender,
                IdentityNo = result.Account.PersonalInfo.IdentityNo,
                IdentityType = result.Account.PersonalInfo.IdentityType,
                MobileNo = result.Account.PersonalInfo.ContactNumber,
                MotherMaidenName = result.Account.PersonalInfo.MotherMaidenName,
                Nationality = result.Account.PersonalInfo.Nationality.ToString(),
                PreferredLanguage = result.Account.PersonalInfo.PreferredLanguage,
                Race = result.Account.PersonalInfo.Race.ToString(),
                ReceivedItemisedBilling = result.Account.BankingInfo.PrintedBillingFlg,
                Salutation = result.Account.PersonalInfo.Salutation,
            };
            model.PermanentAddress = new AddressDetails()
            {
                AddressLine1 = result.Account.AddressInfo.AddressLine1,
                AddressLine2 = result.Account.AddressInfo.AddressLine2,
                City = result.Account.AddressInfo.City,
                Country = result.Account.AddressInfo.Country,
                Postcode = result.Account.AddressInfo.Postcode,
                State = result.Account.AddressInfo.State,
            };
            model.BillingAddress = new AddressDetails()
            {
                AddressLine1 = result.Account.BillingAddressInfo.AddressLine1,
                AddressLine2 = result.Account.BillingAddressInfo.AddressLine2,
                City = result.Account.BillingAddressInfo.City,
                Country = result.Account.BillingAddressInfo.Country,
                Postcode = result.Account.BillingAddressInfo.Postcode,
                State = result.Account.BillingAddressInfo.State,
            };
            model.BillingDetails = new BillingDetails()
            {
                CardExpiryMonth = result.Account.BankingInfo.CardExpiryMonth,
                CardExpiryYear = result.Account.BankingInfo.CardExpiryYear,
                CardHolderName = result.Account.BankingInfo.CardHolderName,
                CardIssuerBank = result.Account.BankingInfo.CardIssuerBank,
                CardType = result.Account.BankingInfo.CardType,
                CardNo = result.Account.BankingInfo.CreditCardNo,
                ThirdPartyFlag = result.Account.BankingInfo.ThirdPartyFlag,
                PrintedBillingFlg = result.Account.BankingInfo.PrintedBillingFlg,
            };

            if (result.OrderPayment != null && result.OrderSLPayment != null)
            {
                model.OrderPayment = new OrderPayment();
                model.OrderPayment.AccountId = result.OrderPayment.AccountId;
                model.OrderPayment.Deposit = result.OrderPayment.Deposit;
                model.OrderPayment.AdvancePayment = result.OrderPayment.AdvancePayment;
                model.OrderPayment.ForeignDeposit = result.OrderPayment.ForeignDeposit;
                model.OrderPayment.Reference = result.OrderPayment.Reference;

                model.OrderSLPayment = new List<OrderPayment>();
                foreach (var v in result.OrderSLPayment)
                {
                    model.OrderSLPayment.Add(new OrderPayment()
                    {
                        AccountId = v.AccountId,
                        Deposit = v.Deposit,
                        AdvancePayment = v.AdvancePayment
                    });
                }
            }

            return model;
        }
    }

}