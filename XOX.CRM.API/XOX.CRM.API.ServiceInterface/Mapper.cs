using CRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOX.CRM.API.ServiceModel;
using XOX.CRM.API.ServiceModel.Types;
using XOX.CRM.Lib;

namespace XOX.CRM.API.ServiceInterface
{
    public static class Mapper
    {
        public static class Account
        {
            public static AccountVO Map(PersonalInfo PersonalInfo, BankingInfo BankingInfo, AddressInfo AddressInfo, AddressInfo BillingAddressInfo, string SIMSerialNumber, DateTime? RegistrationDate = null)
            {
                return new AccountVO()
                {
                    PersonalInfo = new PersonalInfoVO
                    {
                        FullName = PersonalInfo.FullName,
                        BirthDate = (DateTime)PersonalInfo.BirthDate,
                        ContactNumber = PersonalInfo.ContactNumber,
                        CreditLimit = PersonalInfo.CreditLimit,
                        CustomerAccountNumber = PersonalInfo.CustomerAccountNumber,
                        CustomerStatus = PersonalInfo.CustomerStatus,
                        Email = PersonalInfo.Email,
                        Gender = PersonalInfo.Gender,
                        IdentityNo = PersonalInfo.IdentityNo,
                        IdentityType = PersonalInfo.IdentityType,
                        MotherMaidenName = PersonalInfo.MotherMaidenName,
                        MSISDNNumber = PersonalInfo.MSISDNNumber,
                        Nationality = PersonalInfo.Nationality,
                        PreferredLanguage = PersonalInfo.PreferredLanguage,
                        Race = PersonalInfo.Race,
                        Salutation = EnumUtil.ParseEnumInt<Salutation>(PersonalInfo.Salutation),
                        SponsorPersonnel = PersonalInfo.SponsorPersonnel.ToString(), //
                    },
                    BankingInfo = new BankingInfoVO()
                    {
                        BankAccountNumber = BankingInfo.BankAccountNumber,
                        BankId = BankingInfo.BankId,
                        BankName = BankingInfo.BankName,
                        CardExpiryMonth = BankingInfo.CardExpiryMonth,
                        CardExpiryYear = BankingInfo.CardExpiryYear,
                        CardHolderName = BankingInfo.CardHolderName,
                        CardIssuerBank = BankingInfo.CardIssuerBank,
                        CardType = BankingInfo.CardType,
                        CreditCardNo = BankingInfo.CreditCardNo,
                        ThirdPartyFlag = BankingInfo.ThirdPartyFlag,
                        BankAccountName = BankingInfo.BankAccountName
                    },
                    AddressInfo = new AddressInfoVO()
                    {
                        AddressLine1 = AddressInfo.AddressLine1,
                        AddressLine2 = AddressInfo.AddressLine2,
                        City = AddressInfo.City,
                        Country = AddressInfo.Country,
                        Postcode = AddressInfo.Postcode,
                        State = AddressInfo.State,
                        Status = AddressInfo.Status,
                        AddressType = AddressInfo.AddressType
                    },
                    BillingAddressInfo = new AddressInfoVO()
                    {
                        AddressLine1 = BillingAddressInfo.AddressLine1,
                        AddressLine2 = BillingAddressInfo.AddressLine2,
                        City = BillingAddressInfo.City,
                        Country = BillingAddressInfo.Country,
                        Postcode = BillingAddressInfo.Postcode,
                        State = BillingAddressInfo.State,
                        Status = BillingAddressInfo.Status,
                        AddressType = BillingAddressInfo.AddressType
                    },
                    SIMSerialNumber = SIMSerialNumber,
                    RegistrationDate = RegistrationDate
                };
            }
            public static AccountVO Map(PersonalInfo PersonalInfo, BankingInfo BankingInfo, AddressInfo AddressInfo, long AccountId, string SIMSerialNumber)
            {
                return new AccountVO()
                {
                    AccountId = AccountId,
                    PersonalInfo = new PersonalInfoVO
                    {
                        FullName = PersonalInfo.FullName,
                        BirthDate = (DateTime)PersonalInfo.BirthDate,
                        ContactNumber = PersonalInfo.ContactNumber,
                        CreditLimit = PersonalInfo.CreditLimit,
                        CustomerAccountNumber = PersonalInfo.CustomerAccountNumber,
                        CustomerStatus = PersonalInfo.CustomerStatus,
                        Email = PersonalInfo.Email,
                        Gender = PersonalInfo.Gender,
                        IdentityNo = PersonalInfo.IdentityNo,
                        IdentityType = PersonalInfo.IdentityType,
                        MotherMaidenName = PersonalInfo.MotherMaidenName,
                        MSISDNNumber = PersonalInfo.MSISDNNumber,
                        Nationality = PersonalInfo.Nationality,
                        PreferredLanguage = PersonalInfo.PreferredLanguage,
                        Race = PersonalInfo.Race,
                        Salutation = EnumUtil.ParseEnumInt<Salutation>(PersonalInfo.Salutation),
                        SponsorPersonnel = PersonalInfo.SponsorPersonnel.ToString(), //
                    },
                    BankingInfo = new BankingInfoVO()
                    {
                        BankAccountNumber = BankingInfo.BankAccountNumber,
                        BankId = BankingInfo.BankId,
                        BankName = BankingInfo.BankName,
                        CardExpiryMonth = BankingInfo.CardExpiryMonth,
                        CardExpiryYear = BankingInfo.CardExpiryYear,
                        CardHolderName = BankingInfo.CardHolderName,
                        CardIssuerBank = BankingInfo.CardIssuerBank,
                        CardType = BankingInfo.CardType,
                        CreditCardNo = BankingInfo.CreditCardNo,
                        ThirdPartyFlag = BankingInfo.ThirdPartyFlag,
                        BankAccountName = BankingInfo.BankAccountName
                    },
                    AddressInfo = new AddressInfoVO()
                    {
                        AddressLine1 = AddressInfo.AddressLine1,
                        AddressLine2 = AddressInfo.AddressLine2,
                        City = AddressInfo.City,
                        Country = AddressInfo.Country,
                        Postcode = AddressInfo.Postcode,
                        State = AddressInfo.State,
                        Status = AddressInfo.Status,
                        AddressType = AddressInfo.AddressType
                    },
                    SIMSerialNumber = SIMSerialNumber
                };
            }
        }
        public static class Order
        {
            public static OrderVO Map(OrderCreate Order, long UserId)
            {
                return new OrderVO()
                {
                    ASSIGNEE = Order.Assignee,
                    CUST_REP_ID = Order.CustomerRepId,
                    CustomerAccountId = Order.AccountId,
                    ORDER_SOURCE = Order.OrderSource,
                    ORDER_TYPE = Order.OrderType,
                    CREATED_BY = UserId,
                    PLAN = Order.Plan,
                    CATEGORY = Order.Category,
                    REMARKS = Order.Remarks,
                    ORDER_SUBMIT_DT = DateTime.ParseExact(Order.SubmitDate, "dd-MM-yyyy-HH-mm-ss", null),
                    ORDER_STATUS = Order.OrderStatus.ToString()
                };
            }

        }

        public static class Product
        {
            public static List<ServiceModel.Types.Product> Map(List<ProductVO> dbRecords)
            {
                var result = new List<ServiceModel.Types.Product>();
                foreach (var v in dbRecords)
                {
                    var a = Map(v);
                    result.Add(a);
                }

                return result;
            }

            public static ServiceModel.Types.Product Map(ProductVO v)
            {
                return new ServiceModel.Types.Product
                {
                    Is_Package = v.Is_Package,
                    Parent_Package_ID = v.Parent_Package_ID,
                    ProductId = v.ROW_ID,
                    Category = v.PRD_CATEGORY,
                    Description = v.PRD_DESC,
                    Level = v.PRD_LVL,
                    Price = v.PRD_PRICE,
                    PriceType = v.PRD_PRICE_TYPE,
                    Quota = v.QUOTA,
                    Type = v.PRD_TYPE,
                    VasFlag = v.VAS_FLG,
                    ExtProductName = v.EXT_PROD_NAME,
                    GST_CD = v.GST_CD,
                    GST_PT = v.GST_PT,
                    DISPLAY_FLG = v.DISPLAY_FLG
                };
            }
        }

    }
}
