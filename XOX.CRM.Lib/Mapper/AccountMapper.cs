using CRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOX.CRM.Lib.DBContext;

namespace XOX.CRM.Lib.Mapper
{
    public static class Account
    {
        public static List<AccountVO> Map(List<XOX_T_ACCNT> aList)
        {
            var result = new List<AccountVO>();
            foreach (var v in aList)
            {
                var a = Map(v);
                result.Add(a);
            }

            return result;
        }

        public static AccountVO Map(XOX_T_ACCNT a)
        {
            return new AccountVO()
            {
                AccountId = a.ROW_ID,
                PersonalInfo = new PersonalInfoVO(){
                    FullName = a.NAME,
                    BirthDate = a.BIRTH_DT == null ? DateTime.MinValue : (DateTime)a.BIRTH_DT,
                    ContactNumber = a.MOBILE_NO,
                    CreatedDate = a.CREATED,
                    CreditLimit = a.CREDIT_LIMIT == null ? 0 : (decimal)a.CREDIT_LIMIT,
                    CustomerAccountNumber = a.CUSTOMER_NUM,
                    CustomerStatus = EnumUtil.ParseEnumInt<AccountStatus>(a.ACCNT_STATUS),
                    Email = a.EMAIL_ADDR,
                    Gender = a.GENDER == null ? 0 : (int)a.GENDER,
                    IdentityNo = a.ID_NUM,
                    IdentityType = EnumUtil.ParseEnumInt<IdentityType>(a.ID_TYPE),
                    MotherMaidenName = a.MOTHER_MAIDEN_NAME,
                    MSISDNNumber = a.MSISDN,
                    Nationality = a.NATIONALITY != null ? int.Parse(a.NATIONALITY) : 0,
                    PreferredLanguage = EnumUtil.ParseEnumInt<Language>(a.PREFERRED_LANG),
                    Race = a.RACE != null ? int.Parse(a.RACE) : 0,
                    Salutation = EnumUtil.ParseEnumInt<Salutation>(a.SALUTATION),
                    SponsorPersonnel = a.SPONSOR_PERSONNEL,
                },
                BankingInfo = new BankingInfoVO(){
                    BankAccountNumber = a.BANK_ACC_NUM,
                    BankId = 1, ///
                    BankName = a.BANK_NAME,
                    CardExpiryMonth = a.BANK_EXPIRY_MONTH == null ? 0 : (int)a.BANK_EXPIRY_MONTH,
                    CardExpiryYear = a.BANK_EXPIRY_YEAR == null ? 0 : (int)a.BANK_EXPIRY_YEAR,
                    CardHolderName = a.BILL_ACCNT_NAME,
                    CardIssuerBank = a.BANK_ISSUER,
                    CardType = EnumUtil.ParseEnumInt<CardType>(a.BILL_CARD_TYPE),
                    CreditCardNo = a.BILL_ACCNT_NUM,
                    ThirdPartyFlag = a.BANK_THIRD_PARTY == null ? false : (bool)a.BANK_THIRD_PARTY,
                    BankAccountName = a.BANK_ACCNT_NAME,
                    PrintedBillingFlg = a.BILL_LANG == null ? false : bool.Parse(a.BILL_LANG),
                },
                AddressInfo = new AddressInfoVO()
                {
                    AddressId = a.ADDR_ID == null ? 0 : (long)a.ADDR_ID
                },
                AccountType = int.Parse(a.ACCNT_TYPE_CD),
                TerminationDate = a.TERMINATION_DT,
                RegistrationDate = a.CUST_SINCE,
                Grade = a.GRADE_SCORE == null ? 0 : (int)a.GRADE_SCORE,
                DealerCode = a.DEALER_CODE,
                SIMSerialNumber = a.SIM_SERIAL_NUMBER      ,
                ParentAccountId = a.PAR_ACCNT_ID
            };
        }

        public static AddressInfoVO Map(XOX_T_ADDR a)
        {
            return new AddressInfoVO()
            {
                AddressId = a.ROW_ID,
                AddressLine1 = a.ADDR_1,
                AddressLine2 = a.ADDR_2,
                City = a.CITY,
                Country = a.COUNTRY,
                Postcode = a.POSTAL_CD,
                State = a.STATE,                
                //Status = a.STATUS_CD,
                //AddressType = a.ADDR_TYPE
            };
        }

        public static List<FileVO> Map(List<XOX_T_ACCNT_ATT> aList)
        {
            var result = new List<FileVO>();
            foreach (var v in aList)
            {
                var a = Map(v);
                result.Add(a);
            }

            return result;
        }

        public static FileVO Map(XOX_T_ACCNT_ATT a)
        {
            return new FileVO()
            {
                AccountId = (long)a.ACCNT_ID,
                FileId = a.ROW_ID,
                Path = a.FILE_PATH_NAME
            };
        }

        public static List<AccountActivityVO> Map(List<XOX_T_ACCNT_ACT> dbRecords)
        {
            var result = new List<AccountActivityVO>();
            foreach (var v in dbRecords)
            {
                var a = Map(v);
                result.Add(a);
            }

            return result;
        }
        public static AccountActivityVO Map(XOX_T_ACCNT_ACT db)
        {
            return new AccountActivityVO
            {
                ROW_ID = db.ROW_ID,
                CREATED = db.CREATED,
                CREATED_BY = db.CREATED_BY,
                ACT_DESC = db.ACT_DESC,
                REASON = db.REASON,
                ASSIGNEE = db.ASSIGNEE,
                STATUS = db.STATUS,
                LAST_UPD = db.LAST_UPD,
                LAST_UPD_BY = db.LAST_UPD_BY,
                ACCNT_ID = db.ACCNT_ID ?? 0
            };
        }

    }
}
