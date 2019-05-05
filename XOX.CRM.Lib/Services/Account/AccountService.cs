using CRM;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XOX.CRM.Lib;
using XOX.CRM.Lib.Common.Constants;
using XOX.CRM.Lib.Common.VO.Audit;
using XOX.CRM.Lib.DBContext;
using XOX.CRM.Lib.Services;
using XOX.CRM.Lib.WebServices;

namespace XOX.CRM.Lib
{
    public class AccountService : IAccountService
    {
        public IAccountAttachmentService AccountAttachmentService = new AccountAttachmentService();
        public IAddressService AddressService = new AddressService();
        public IProductService ProductService = new ProductService();
        public ICommonService CommonService = new CommonService();

        public List<AccountVO> GetAll()
        {
            using (var DBContext = new CRMDbContext())
            {
                var result = from d in DBContext.XOX_T_ACCNT
                             select d;

                var aList = Mapper.Account.Map(result.ToList<XOX_T_ACCNT>());

                foreach (var v in aList)
                {
                    v.AddressInfo = AddressService.Get(v.AddressInfo.AddressId);
                }

                return aList;
            }
        }

        public List<AccountVO> GetAll(int startIdx, int length, ref int TotalCount, string orderBy = "", string orderDirection = "", string filterBy = "", string filterQuery = "")
        {
            using (var DBContext = new CRMDbContext())
            {
                var result = from d in DBContext.XOX_T_ACCNT
                             where d.PAR_ACCNT_ID == null
                             select d;

                //filtering
                if (filterBy != "" && filterQuery != "")
                {
                    if (filterBy == "FullName")
                        result = result.Where(m => m.NAME.ToLower().Contains(filterQuery.ToLower()));
                    else if (filterBy == "Email")
                        result = result.Where(m => m.EMAIL_ADDR.ToLower().Contains(filterQuery.ToLower()));
                    else if (filterBy == "IdentityNumber")
                        result = result.Where(m => m.ID_NUM.ToLower().Contains(filterQuery.ToLower()));
                }

                TotalCount = result.Count();

                //ordering && paging
                if (orderDirection == "asc")
                {
                    if (orderBy == "Salutation")
                        result = result.OrderBy(m => m.SALUTATION);
                    else if (orderBy == "FullName")
                        result = result.OrderBy(m => m.NAME);
                    else if (orderBy == "Email")
                        result = result.OrderBy(m => m.EMAIL_ADDR);
                    else if (orderBy == "IdentityType")
                        result = result.OrderBy(m => m.ID_TYPE);
                    else if (orderBy == "IdentityNumber")
                        result = result.OrderBy(m => m.ID_NUM);
                    else
                        result = result.OrderBy(m => m.ROW_ID);
                }
                else
                {
                    if (orderBy == "Salutation")
                        result = result.OrderByDescending(m => m.SALUTATION);
                    else if (orderBy == "FullName")
                        result = result.OrderByDescending(m => m.NAME);
                    else if (orderBy == "Email")
                        result = result.OrderByDescending(m => m.EMAIL_ADDR);
                    else if (orderBy == "IdentityType")
                        result = result.OrderByDescending(m => m.ID_TYPE);
                    else if (orderBy == "IdentityNumber")
                        result = result.OrderByDescending(m => m.ID_NUM);
                    else
                        result = result.OrderByDescending(m => m.ROW_ID);
                }

                result = result.Skip(startIdx).Take(length);

                var aList = Mapper.Account.Map(result.ToList<XOX_T_ACCNT>());

                return aList;
            }
        }

        public List<AccountVO> GetAllBySearch(int startIdx, int length, ref int TotalCount, string orderBy = "", string orderDirection = "", string Salutation = "0", string FullName = "", string Email = "", string IdentityType = "0", string IdentityNo = "", string MSISDN = "", string Status = "", string AccType = "1")
        {
            List<AccountVO> List = new List<AccountVO>();
            using (var dbContext = new CRMDbContext())
            {
                if (AccType != "1" && AccType != "3")
                {
                    AccType = "1";
                }
                
                var result = from d in dbContext.XOX_T_ACCNT
                             select d;

                if (AccType == "1")
                {
                    result = from d in dbContext.XOX_T_ACCNT
                                 where d.PAR_ACCNT_ID == null && d.ACCNT_TYPE_CD == AccType
                                 select d;
                }
                else
                {
                    result = from d in dbContext.XOX_T_ACCNT
                                 where d.PAR_ACCNT_ID != null && d.ACCNT_TYPE_CD == AccType
                                 select d;
                }

                //filtering string Salutation, string FullName, string Email, string IdentityType, string IdentityNo
                if (Salutation != "0")
                {
                    result = result.Where(m => m.SALUTATION.ToLower() == (Salutation.ToLower()));
                }
                if (FullName != "")
                {
                    result = result.Where(m => m.NAME.ToLower().Contains(FullName.ToLower()));
                }
                if (Email != "")
                {
                    result = result.Where(m => m.EMAIL_ADDR.ToLower().Contains(Email.ToLower()));
                }
                if (IdentityType != "0")
                {
                    result = result.Where(m => m.ID_TYPE.ToLower().Contains(IdentityType.ToLower()));
                }
                if (IdentityNo != "")
                {
                    result = result.Where(m => m.ID_NUM.ToLower().Contains(IdentityNo.ToLower()));
                }
                if (MSISDN != "")
                {
                    result = result.Where(m => m.MSISDN.ToLower().Contains(MSISDN.ToLower()));
                }
                if (Status != "0")
                {
                    result = result.Where(m => m.ACCNT_STATUS.ToLower().Contains(Status.ToLower()));
                }

                TotalCount = result.Count();

                //ordering && paging
                if (orderDirection == "asc")
                {
                    if (orderBy == "Salutation")
                        result = result.OrderBy(m => m.SALUTATION);
                    else if (orderBy == "FullName")
                        result = result.OrderBy(m => m.NAME);
                    else if (orderBy == "Email")
                        result = result.OrderBy(m => m.EMAIL_ADDR);
                    else if (orderBy == "IdentityType")
                        result = result.OrderBy(m => m.ID_TYPE);
                    else if (orderBy == "IdentityNo")
                        result = result.OrderBy(m => m.ID_NUM);
                    else if (orderBy == "MSISDN")
                        result = result.OrderBy(m => m.MSISDN);
                    else if (orderBy == "Status")
                        result = result.OrderBy(m => m.ACCNT_STATUS);
                    else
                        result = result.OrderBy(m => m.ROW_ID);
                }
                else
                {
                    if (orderBy == "Salutation")
                        result = result.OrderByDescending(m => m.SALUTATION);
                    else if (orderBy == "FullName")
                        result = result.OrderByDescending(m => m.NAME);
                    else if (orderBy == "Email")
                        result = result.OrderByDescending(m => m.EMAIL_ADDR);
                    else if (orderBy == "IdentityType")
                        result = result.OrderByDescending(m => m.ID_TYPE);
                    else if (orderBy == "IdentityNo")
                        result = result.OrderByDescending(m => m.ID_NUM);
                    else if (orderBy == "MSISDN")
                        result = result.OrderByDescending(m => m.MSISDN);
                    else if (orderBy == "Status")
                        result = result.OrderByDescending(m => m.ACCNT_STATUS);
                    else
                        result = result.OrderByDescending(m => m.ROW_ID);
                }

                if (length >= 0)
                    result = result.Skip(startIdx).Take(length);

                List = Mapper.Account.Map(result.ToList<XOX_T_ACCNT>());
            }

            return List;
        }

        public AccountVO Get(long id)
        {
            using (var DBContext = new CRMDbContext())
            {
                var result = from d in DBContext.XOX_T_ACCNT
                             where d.ROW_ID == id
                             select d;

                var v = Mapper.Account.Map(result.First());

                /*
                var order = from d in DBContext.XOX_T_ORDER
                            join e in DBContext.XOX_T_ORDER_ITEM on d.ROW_ID equals e.ORDER_ID
                            join f in DBContext.XOX_T_ACCNT on e.CUST_ID equals f.ROW_ID
                            select d.ORDER_SUBMIT_DT;

                v.RegistrationDate = order.First().Value;
                */

                v.AddressInfo = AddressService.Get(v.AddressInfo.AddressId) ?? new AddressInfoVO();

                var BillingLineType = ((int)AccountType.BillingLine).ToString();
                var result2 = from d in DBContext.XOX_T_ACCNT
                              where d.PAR_ACCNT_ID == id && d.ACCNT_TYPE_CD == BillingLineType
                              select d;

                if (result2.Count() > 0)
                {
                    var b = result2.First();
                    v.BillingAddressInfo = AddressService.Get((long)b.ADDR_ID);
                }
                else
                {
                    v.BillingAddressInfo = new AddressInfoVO();

                    if (result.First().ACCNT_TYPE_CD == "3" && result.First().PAR_ACCNT_ID != null && result.First().PAR_ACCNT_ID != 0)
                    {
                        //if this account is Supp Line. then get Parent's Billing Address
                        var PAR_ACCNT_ID = result.First().PAR_ACCNT_ID;
                        var ParentBillingAccount = from d in DBContext.XOX_T_ACCNT
                                                   where d.PAR_ACCNT_ID == PAR_ACCNT_ID && d.ACCNT_TYPE_CD == BillingLineType
                                                   select d;
                        if (ParentBillingAccount.Count() > 0)
                        {
                            v.BillingAddressInfo = AddressService.Get((long)ParentBillingAccount.First().ADDR_ID);
                        }
                    }
                }

                v.Files = AccountAttachmentService.GetFiles(id);

                return v;
            }
        }

        public long GetParentAccountId(long SupplineAccountId)
        {
            using (var DBContext = new CRMDbContext())
            {
                string SupplementaryLineType = ((int)AccountType.SupplementaryLine).ToString();
                var Suppline = (from d in DBContext.XOX_T_ACCNT
                                where d.ROW_ID == SupplineAccountId
                                && d.ACCNT_TYPE_CD == SupplementaryLineType
                                select d).First();

                return Suppline.PAR_ACCNT_ID.Value;
            }
        }

        public bool Edit(AccountVO vo, long UserId = 0, bool flg2EAI = true)
        {
            if (UserId == 0)
            {
                UserId = long.Parse(Thread.CurrentPrincipal.Identity.Name);
            }

            using (var DBContext = new CRMDbContext())
            {
                string PrincipalLineType = ((int)AccountType.PrincipalLine).ToString();
                string SupplementaryLineType = ((int)AccountType.SupplementaryLine).ToString();
                var result = from d in DBContext.XOX_T_ACCNT
                             where d.ROW_ID == vo.AccountId
                             && (d.ACCNT_TYPE_CD == PrincipalLineType || d.ACCNT_TYPE_CD == SupplementaryLineType)
                             select d;

                if (result.Count() > 0)
                {
                    var v = result.First();

                    v.NAME = vo.PersonalInfo.FullName;
                    v.BIRTH_DT = vo.PersonalInfo.BirthDate;
                    v.EMAIL_ADDR = vo.PersonalInfo.Email;
                    v.ID_NUM = vo.PersonalInfo.IdentityNo;
                    //v.CREDIT_LIMIT = vo.PersonalInfo.CreditLimit;
                    v.MOTHER_MAIDEN_NAME = vo.PersonalInfo.MotherMaidenName;
                    v.CUSTOMER_NUM = vo.PersonalInfo.CustomerAccountNumber;
                    v.SALUTATION = ((Salutation)vo.PersonalInfo.Salutation).ToString();
                    v.ID_TYPE = ((IdentityType)vo.PersonalInfo.IdentityType).ToString();
                    v.MOBILE_NO = vo.PersonalInfo.ContactNumber;
                    if (flg2EAI == false)
                    {
                        v.MSISDN = vo.PersonalInfo.MSISDNNumber;
                    }
                    v.GENDER = vo.PersonalInfo.Gender;
                    v.PREFERRED_LANG = ((Language)vo.PersonalInfo.PreferredLanguage).ToString();
                    v.SPONSOR_PERSONNEL = vo.PersonalInfo.SponsorPersonnel;
                    v.NATIONALITY = vo.PersonalInfo.Nationality.ToString();
                    v.RACE = vo.PersonalInfo.Race.ToString();
                    v.GRADE_SCORE = vo.Grade;
                    v.SIM_SERIAL_NUMBER = vo.SIMSerialNumber;

                    if (v.ACCNT_TYPE_CD == PrincipalLineType || v.ACCNT_TYPE_CD == SupplementaryLineType)
                    {
                        v.BANK_NAME = vo.BankingInfo.BankName;
                        v.BILL_ACCNT_NAME = vo.BankingInfo.CardHolderName;
                        v.BILL_ACCNT_NUM = vo.BankingInfo.CreditCardNo;
                        v.BANK_ISSUER = vo.BankingInfo.CardIssuerBank;
                        v.BANK_THIRD_PARTY = vo.BankingInfo.ThirdPartyFlag;
                        v.BANK_EXPIRY_MONTH = vo.BankingInfo.CardExpiryMonth;
                        v.BANK_EXPIRY_YEAR = vo.BankingInfo.CardExpiryYear;
                        v.BILL_CARD_TYPE = ((CardType)vo.BankingInfo.CardType).ToString();
                        v.BANK_ACC_NUM = vo.BankingInfo.BankAccountNumber;
                        v.BANK_ACCNT_NAME = vo.BankingInfo.BankAccountName;
                    }

                    v.LAST_UPD = DateTime.Now;
                    v.LAST_UPD_BY = UserId;

                    DBContext.ChangeTracker.DetectChanges();
                    var objectState = ((IObjectContextAdapter)DBContext).ObjectContext.ObjectStateManager.GetObjectStateEntry(v);
                    AuditService auditDal = new AuditService(UserId, XOXConstants.AUDIT_MODULE_ACCOUNT, XOXConstants.AUDIT_ACTION_UPDATE);
                    auditDal.Check(objectState, v.ROW_ID);
                    DBContext.SaveChanges();
                    
                    var AddressId = AddressService.Edit(vo.AddressInfo, UserId);

                    if (AddressId != 0)
                    {
                        v.ADDR_ID = AddressId;
                        DBContext.SaveChanges();
                    }

                    if (vo.BillingAddressInfo != null && v.ACCNT_TYPE_CD == PrincipalLineType)
                    {
                        string BillingLine = ((int)AccountType.BillingLine).ToString();
                        var BillingAccount = from d in DBContext.XOX_T_ACCNT
                                             where d.ACCNT_TYPE_CD == BillingLine && d.PAR_ACCNT_ID == v.ROW_ID
                                             select d.ADDR_ID;
                        vo.BillingAddressInfo.AddressId = (long)BillingAccount.First();

                        var BillingAddressId = AddressService.Edit(vo.BillingAddressInfo, UserId);
                    }

                    if (flg2EAI == true)
                    {
                        //CRP Syncing
                        if (v.INTEGRATION_ID != null && v.INTEGRATION_ID != "")
                        {
                            var isSupplementary = false;
                            if (v.ACCNT_TYPE_CD == SupplementaryLineType)
                            {
                                isSupplementary = true;
                            }
                            var EAIResult = "";
                            try
                            {
                                EAIResult = EAIService.EditAccount(new AccountEdit()
                                {
                                    Data = new AccountInfo()
                                    {
                                        PersonalInfo = new PersonalInfoEAI()
                                        {
                                            BirthDate = vo.PersonalInfo.BirthDate.ToString("dd-MM-yyyy"),
                                            ContactNumber = vo.PersonalInfo.ContactNumber,
                                            CreatedDate = vo.PersonalInfo.CreatedDate,
                                            CreditLimit = vo.PersonalInfo.CreditLimit,
                                            CustomerAccountNumber = vo.PersonalInfo.CustomerAccountNumber,
                                            CustomerStatus = vo.PersonalInfo.CustomerStatus,
                                            Email = vo.PersonalInfo.Email,
                                            FullName = vo.PersonalInfo.FullName,
                                            Gender = vo.PersonalInfo.Gender,
                                            IdentityNo = vo.PersonalInfo.IdentityNo,
                                            IdentityType = vo.PersonalInfo.IdentityType,
                                            MotherMaidenName = vo.PersonalInfo.MotherMaidenName,
                                            MSISDNNumber = v.MSISDN,
                                            Nationality = vo.PersonalInfo.Nationality,
                                            PreferredLanguage = vo.PersonalInfo.PreferredLanguage,
                                            Race = vo.PersonalInfo.Race,
                                            Salutation = v.SALUTATION,
                                            SponsorPersonnel = vo.PersonalInfo.SponsorPersonnel,
                                            SimSerialNumber = vo.SIMSerialNumber
                                        },
                                        BankingInfo = vo.BankingInfo,
                                        AddressInfo = vo.AddressInfo,
                                        BillingAddressInfo = vo.BillingAddressInfo,
                                        IntegrationId = long.Parse(v.INTEGRATION_ID),
                                        SIMSerialNumber = vo.SIMSerialNumber,
                                        FlgReceivedItemised = 0,
                                        User = new User()
                                        {
                                            Source = "CRM",
                                            UserId = int.Parse(v.INTEGRATION_ID)
                                        }
                                    },
                                    isSupplementary = isSupplementary
                                });
                            }
                            catch
                            {
                                throw new Exception("Successfully edited. But failed to syncing to CRP.");
                            }

                            try
                            {
                                bool.Parse(EAIResult);
                            }
                            catch
                            {
                                throw new Exception(EAIResult);
                            }
                        }
                        else
                        {
                            throw new Exception("Successfully edited. But failed to syncing to CRP due to unavailable INTEGRATION_ID.");
                        }
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public long Add(AccountVO vo, long UserId = 0, long IntegrationId = 0, bool? Supplementary = false)
        {
            if (UserId == 0)
            {
                UserId = long.Parse(Thread.CurrentPrincipal.Identity.Name);
            }
                
            using (var DBContext = new CRMDbContext())
            {
                var AddressId = AddressService.Add(vo.AddressInfo, UserId);

                XOX_T_ACCNT v = new XOX_T_ACCNT();

                v.NAME = vo.PersonalInfo.FullName == null ? "" : vo.PersonalInfo.FullName;
                if (vo.PersonalInfo.BirthDate == DateTime.MinValue)
                {
                    v.BIRTH_DT = null;
                }
                else
                {
                    v.BIRTH_DT = vo.PersonalInfo.BirthDate;
                }
                v.EMAIL_ADDR = vo.PersonalInfo.Email;
                v.ID_NUM = vo.PersonalInfo.IdentityNo;
                v.ACCNT_TYPE_CD = ((int)AccountType.PrincipalLine).ToString();
                v.CREDIT_LIMIT = vo.PersonalInfo.CreditLimit;
                v.MOTHER_MAIDEN_NAME = vo.PersonalInfo.MotherMaidenName;
                v.CUSTOMER_NUM = vo.PersonalInfo.CustomerAccountNumber;
                v.SALUTATION = ((Salutation)vo.PersonalInfo.Salutation).ToString();
                v.ID_TYPE = ((IdentityType)vo.PersonalInfo.IdentityType).ToString();
                v.MOBILE_NO = vo.PersonalInfo.ContactNumber;
                v.MSISDN = vo.PersonalInfo.MSISDNNumber;
                v.GENDER = vo.PersonalInfo.Gender;
                v.PREFERRED_LANG = ((Language)vo.PersonalInfo.PreferredLanguage).ToString();
                v.SPONSOR_PERSONNEL = vo.PersonalInfo.SponsorPersonnel;
                v.NATIONALITY = vo.PersonalInfo.Nationality.ToString();
                v.RACE = vo.PersonalInfo.Race.ToString();
                v.ACCNT_STATUS = AccountStatus.Prospect.ToString();
                v.INTEGRATION_ID = IntegrationId.ToString();
                v.SIM_SERIAL_NUMBER = vo.SIMSerialNumber;
                v.CUST_SINCE = vo.RegistrationDate;
                
                v.BANK_NAME = vo.BankingInfo.BankName;
                v.BILL_ACCNT_NAME = vo.BankingInfo.CardHolderName;
                v.BILL_ACCNT_NUM = vo.BankingInfo.CreditCardNo;
                v.BANK_ISSUER = vo.BankingInfo.CardIssuerBank;
                v.BANK_THIRD_PARTY = vo.BankingInfo.ThirdPartyFlag;
                v.BANK_EXPIRY_MONTH = vo.BankingInfo.CardExpiryMonth;
                v.BANK_EXPIRY_YEAR = vo.BankingInfo.CardExpiryYear;
                if (vo.BankingInfo.CardType != 0)
                {
                    v.BILL_CARD_TYPE = ((CardType)vo.BankingInfo.CardType).ToString();
                }
                v.BANK_ACC_NUM = vo.BankingInfo.BankAccountNumber;
                v.BANK_ACCNT_NAME = vo.BankingInfo.BankAccountName;

                v.ADDR_ID = AddressId;

                if (Supplementary == true)
                {
                    v.MASTER_ACCNT_ID = vo.AccountId;
                    v.PAR_ACCNT_ID = vo.AccountId;
                    v.ACCNT_TYPE_CD = ((int)AccountType.SupplementaryLine).ToString();
                }

                v.CREATED = DateTime.Now;
                v.CREATED_BY = UserId;

                DBContext.XOX_T_ACCNT.Add(v);
                DBContext.SaveChanges();

                return v.ROW_ID;
            }
        }

        public long BillAccAdd(AccountVO vo, long ParAccountId, long UserId = 0)
        {
            if (UserId == 0)
            {
                UserId = long.Parse(Thread.CurrentPrincipal.Identity.Name);
            }

            using (var DBContext = new CRMDbContext())
            {
                var AddressId = AddressService.Add(vo.BillingAddressInfo, UserId);

                XOX_T_ACCNT v = new XOX_T_ACCNT();

                v.NAME = vo.PersonalInfo.FullName == null ? "" : vo.PersonalInfo.FullName;
                if (vo.PersonalInfo.BirthDate == DateTime.MinValue)
                {
                    v.BIRTH_DT = null;
                }
                else
                {
                    v.BIRTH_DT = vo.PersonalInfo.BirthDate;
                }
                v.EMAIL_ADDR = vo.PersonalInfo.Email;
                v.ID_NUM = vo.PersonalInfo.IdentityNo;
                v.ACCNT_TYPE_CD = ((int)AccountType.BillingLine).ToString();
                v.CREDIT_LIMIT = vo.PersonalInfo.CreditLimit;
                v.MOTHER_MAIDEN_NAME = vo.PersonalInfo.MotherMaidenName;
                v.CUSTOMER_NUM = vo.PersonalInfo.CustomerAccountNumber;
                v.SALUTATION = ((Salutation)vo.PersonalInfo.Salutation).ToString();
                v.ID_TYPE = ((IdentityType)vo.PersonalInfo.IdentityType).ToString();
                v.MOBILE_NO = vo.PersonalInfo.ContactNumber;
                v.MSISDN = vo.PersonalInfo.MSISDNNumber;
                v.GENDER = vo.PersonalInfo.Gender;
                v.PREFERRED_LANG = ((Language)vo.PersonalInfo.PreferredLanguage).ToString();
                v.SPONSOR_PERSONNEL = vo.PersonalInfo.SponsorPersonnel;
                v.NATIONALITY = vo.PersonalInfo.Nationality.ToString();
                v.RACE = vo.PersonalInfo.Race.ToString();
                v.ACCNT_STATUS = AccountStatus.Prospect.ToString();
                v.SIM_SERIAL_NUMBER = vo.SIMSerialNumber;
                v.CUST_SINCE = vo.RegistrationDate;

                v.BANK_NAME = vo.BankingInfo.BankName;
                v.BILL_ACCNT_NAME = vo.BankingInfo.CardHolderName;
                v.BILL_ACCNT_NUM = vo.BankingInfo.CreditCardNo;
                v.BANK_ISSUER = vo.BankingInfo.CardIssuerBank;
                v.BANK_THIRD_PARTY = vo.BankingInfo.ThirdPartyFlag;
                v.BANK_EXPIRY_MONTH = vo.BankingInfo.CardExpiryMonth;
                v.BANK_EXPIRY_YEAR = vo.BankingInfo.CardExpiryYear;
                v.BILL_CARD_TYPE = ((CardType)vo.BankingInfo.CardType).ToString();
                v.BANK_ACC_NUM = vo.BankingInfo.BankAccountNumber;
                v.BANK_ACCNT_NAME = vo.BankingInfo.BankAccountName;

                v.ADDR_ID = AddressId;

                v.MASTER_ACCNT_ID = ParAccountId;
                v.PAR_ACCNT_ID = ParAccountId;

                v.CREATED = DateTime.Now;
                v.CREATED_BY = UserId;

                DBContext.XOX_T_ACCNT.Add(v);
                DBContext.SaveChanges();

                return v.ROW_ID;
            }
        }

        public bool UpdateStatus(AccountActivityVO Activity, string Status, string SubscriberJoinDate = "", DateTime? DeleteDate = null)
        {
            using (var DBContext = new CRMDbContext())
            {
                var result = from d in DBContext.XOX_T_ACCNT
                             where d.ROW_ID == Activity.ACCNT_ID
                             select d;

                if (result.Count() > 0)
                {
                    Activity.ACT_DESC = "Changing Account Status from " + result.First().ACCNT_STATUS + " to " + Status + ".";
                    var ActivityId = AddActivity(Activity);

                    result.First().ACCNT_STATUS = Status;
                    if (result.First().ACCNT_STATUS == AccountStatus.Terminated.ToString())
                    {
                        result.First().TERMINATION_DT = DeleteDate == null ? DateTime.Now : DeleteDate.Value;
                    }
                    result.First().LAST_UPD = DateTime.Now;
                    if (Activity.LAST_UPD_BY == null || Activity.LAST_UPD_BY == 0)
                    {
                        result.First().LAST_UPD_BY = int.Parse(Thread.CurrentPrincipal.Identity.Name);
                    }

                    DBContext.SaveChanges();

                    //CRP Syncing
                    if (result.First().INTEGRATION_ID != null && result.First().INTEGRATION_ID != "")
                    {
                        if (Status == AccountStatus.Active.ToString() || Status == AccountStatus.Barred.ToString() || Status == AccountStatus.Blocked.ToString() || Status == AccountStatus.Terminated.ToString())
                        {
                            var parameter = new AccountUpdateStatus()
                            {
                                AccountStatus = Status,
                                IntegrationId = long.Parse(result.First().INTEGRATION_ID),
                                User = new User()
                                {
                                    Source = "CRM",
                                },
                                SubscriberJoinDate = SubscriberJoinDate,
                                DeleteDate = result.First().TERMINATION_DT == null ? "" : result.First().TERMINATION_DT.Value.ToString("yyyy-MM-dd-HH-mm-ss"),
                                isSuppLine = result.First().ACCNT_TYPE_CD == "3" ? true : false
                            };
                            if (Activity.CREATED_BY == null || Activity.CREATED_BY == 0)
                            {
                                parameter.User.UserId = int.Parse(Thread.CurrentPrincipal.Identity.Name);
                            }
                            else
                            {
                                parameter.User.UserId = (int)Activity.CREATED_BY;
                            }
                            EAIService.AccountUpdateStatus(parameter);
                        }
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        
        #region Account Activities
        public long AddActivity(AccountActivityVO VO)
        {
            if (VO.CREATED_BY == 0 || VO.CREATED_BY == null)
            {
                VO.CREATED_BY = long.Parse(Thread.CurrentPrincipal.Identity.Name);
            }

            long newId = 0;
            using (var DBContext = new CRMDbContext())
            {
                XOX_T_ACCNT_ACT entry = new XOX_T_ACCNT_ACT
                {
                    CREATED = DateTime.Now,
                    CREATED_BY = VO.CREATED_BY,
                    ACCNT_ID = VO.ACCNT_ID,
                    ACT_DESC = VO.ACT_DESC,
                    ASSIGNEE = VO.ASSIGNEE,
                    REASON = VO.REASON,
                    STATUS = XOXConstants.Active
                };
                DBContext.XOX_T_ACCNT_ACT.Add(entry);
                DBContext.SaveChanges();

                newId = entry.ROW_ID;
            }
            return newId;
        }

        public List<AccountActivityVO> GetAccountActivities(long accntId)
        {
            List<AccountActivityVO> accntActivities = new List<AccountActivityVO>();
            using (var DBContext = new CRMDbContext())
            {
                var activityList = from x in DBContext.XOX_T_ACCNT_ACT
                                   where x.ACCNT_ID == accntId
                                   select x;
                if (activityList.Count() > 0)
                    accntActivities = Mapper.Account.Map(activityList.ToList());
            }
            return accntActivities;
        }
        #endregion
        
        public long GetIDByMSISDN(string MSISDN, string AccountName)
        {
            long AccountId = 0;
            using (var DBContext = new CRMDbContext())
            {
                var result = from d in DBContext.XOX_T_ACCNT
                             where d.MSISDN == MSISDN && d.NAME == AccountName
                             select d.ROW_ID;

                if (result.Count() > 0)
                {
                    AccountId = result.First();
                }
                else
                {
                    throw new Exception("MSISDN not found.");
                }
            }
            return AccountId;
        }

        public List<AccountVO> GetAllSupplementaryLine(long AccountId)
        {
            List<AccountVO> List = new List<AccountVO>();
            using (var DBContext = new CRMDbContext())
            {
                var SupplementaryLineType = ((int)AccountType.SupplementaryLine).ToString();
                var result = from d in DBContext.XOX_T_ACCNT
                             where d.PAR_ACCNT_ID == AccountId && d.ACCNT_TYPE_CD == SupplementaryLineType
                             select d;

                foreach(var v in result)
                {
                    List = Mapper.Account.Map(result.ToList<XOX_T_ACCNT>());
                }
            }

            return List;
        }

        public long GetAccountIdByOrderId(long OrderId)
        {
            using (var context = new CRMDbContext())
            {
                string PrincipalLineType = ((int)AccountType.PrincipalLine).ToString();
                var result = from d in context.XOX_T_ORDER_ITEM
                             join e in context.XOX_T_ACCNT on d.CUST_ID equals e.ROW_ID
                             where d.ORDER_ID == OrderId
                             //&& e.ACCNT_TYPE_CD == PrincipalLineType
                             select d.CUST_ID;

                if (result.Count() > 0)
                {
                    return result.First().Value;
                }
                else
                {
                    return 0;
                }
            }
        }

        public long GetAccountIdByIntegrationId(long IntegrationId)
        {
            using (var context = new CRMDbContext())
            {
                string PrincipalLineType = ((int)AccountType.PrincipalLine).ToString();
                string IntegrationIdS = IntegrationId.ToString();
                var result = from d in context.XOX_T_ACCNT
                             where d.INTEGRATION_ID == IntegrationIdS
                             && d.ACCNT_TYPE_CD == PrincipalLineType
                             select d.ROW_ID;

                if (result.Count() > 0)
                {
                    return result.First();
                }
                else
                {
                    return 0;
                }
            }
        }

        public List<long> GetAccountIdByMSISDN(string MSISDN, string Name = "", bool ActiveOnly = false)
        {
            using (var context = new CRMDbContext())
            {
                string BillingLine = ((int)AccountType.BillingLine).ToString();
                var Accounts = from d in context.XOX_T_ACCNT
                               where d.ACCNT_TYPE_CD != BillingLine
                               orderby d.ROW_ID descending
                               select d;

                if(ActiveOnly == true)
                {
                    var AccountStatus_Active = AccountStatus.Active.ToString();
                    Accounts = Accounts.Where(m => m.ACCNT_STATUS == AccountStatus_Active).OrderBy(m => m.ROW_ID);
                }

                var result = Accounts.Where(m => m.MSISDN == MSISDN);
                if (Name != "")
                {
                    result = result.Where(m => m.NAME == Name);
                }

                if (result.Count() > 0)
                {
                    List<long> AccountId = new List<long>();
                    foreach (var v in result)
                    {
                        AccountId.Add(result.First().ROW_ID);
                    }
                    return AccountId;
                }
                else
                {
                    var MSISDN2 = "";
                    if (MSISDN[0] == '6')
                    {
                        MSISDN2 = MSISDN.Substring(1);
                    }
                    else
                    {
                        MSISDN2 = '6' + MSISDN;
                    }

                    var result2 = Accounts.Where(m => m.MSISDN == MSISDN2);
                    if (Name != "")
                    {
                        result2 = result2.Where(m => m.NAME == Name);
                    }

                    if (result2.Count() > 0)
                    {
                        List<long> AccountId = new List<long>();
                        foreach (var v in result)
                        {
                            AccountId.Add(result2.First().ROW_ID);
                        }
                        return AccountId;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public List<long> Search(string query)
        {
            List<long> List = new List<long>();

            return List;
        }

        public string MakePayment(long AccountId, Payment payment, bool Manual = false, long UserId = 0)
        {
            if (UserId == 0)
            {
                UserId = long.Parse(Thread.CurrentPrincipal.Identity.Name);
            }

            string KeyInReference = payment.Reference;

            payment.User = new User()
            {
                UserId = (int)UserId
            };
            payment.MSISDN = payment.MSISDN[0] == '6' ? payment.MSISDN : '6' + payment.MSISDN;
            if (Manual == true && payment.PaymentMethod == XOXConstants.PAYMENT_METHOD_CREDIT_CARD)
            {
                payment.Reference = payment.CardIssuerBank + "-" + CommonService.MaskCreditCardDigit(payment.CardNumber);
            }

            if (payment.PaymentMethod == null)
            {
                throw new Exception("Payment Method parameter is null or empty before calling \"CreatePayment\" to MPP");
            }
            var result = EAIService.CreatePayment(payment);
            var r = JObject.Parse(result);

            payment.Reference = KeyInReference; //give the reference value back
            try
            {
                string response = "";
                try
                {
                    response = r["Result"]["status"].ToString();
                }
                catch { }

                if (response.ToLower() == "true")
                {
                    if (Manual == true)
                    {
                        this.CreatePayment(AccountId, payment, UserId);

                        //send sms
                        if (payment.PaymentType == XOXConstants.PAYMENT_TYPE_BILLING)
                        {
                            try
                            {
                                EAIService.SendSMSWithShortCode(new SendSMSWithShortCode()
                                {
                                    User = new User()
                                    {
                                        Source = "CRM",
                                        UserId = 1
                                    },
                                    MSISDN = payment.MSISDN,
                                    Message = "Thank you for your payment of RM" + payment.Amount + " for " + payment.MSISDN + ". If your line is currently barred and full payment was made, it will be reinstated within 2 hrs."
                                });
                            }
                            catch { }
                        }
                    }

                    return "true";
                }
                else
                {
                    try
                    {
                        response = r["Result"].ToString();
                        return response;
                    }
                    catch
                    {
                        return result;
                    }
                }
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }

        public void CreatePayment(long AccountId, Payment payment, long UserId = 0)
        {
            if (UserId == 0)
            {
                UserId = long.Parse(Thread.CurrentPrincipal.Identity.Name);
            }

            using (var context = new CRMDbContext())
            {
                XOX_T_ACCNT_PAYMENT v = new XOX_T_ACCNT_PAYMENT();
                v.ACCNT_ID = AccountId;
                v.AMOUNT = decimal.Parse(payment.Amount);
                v.CREATED = DateTime.Now;
                v.CREATED_BY = UserId;
                v.PAYMENT_TYPE = payment.PaymentType; //XOXConstants.PAYMENT_TYPE_BILLING;
                v.REFERENCE = payment.Reference == null ? "" : payment.Reference;
                v.REFERENCE = v.PAYMENT_TYPE + " - " + v.REFERENCE;
                v.PAYMENT_METHOD = payment.PaymentMethod;
                if (payment.PaymentMethod == XOXConstants.PAYMENT_METHOD_CREDIT_CARD)
                {
                    //addition info
                    v.BANK_ISSUER = payment.CardIssuerBank;
                    v.CARD_NUMBER = payment.CardNumber;
                    v.CARD_TYPE = payment.CardType;
                    if (payment.CardExpiryMonth != null && payment.CardExpiryYear != null)
                    {
                        v.CARD_EXPIRY_DATE = payment.CardExpiryMonth + "/" + payment.CardExpiryYear;
                    }
                }
                context.XOX_T_ACCNT_PAYMENT.Add(v);
                context.SaveChanges();
            }
        }

        public List<PaymentVO> GetAllPayment(long AccountId)
        {
            List<PaymentVO> List = new List<PaymentVO>();
            using (var context = new CRMDbContext())
            {
                var result = from d in context.XOX_T_ACCNT_PAYMENT
                             where d.ACCNT_ID == AccountId
                             select d;

                foreach (var v in result)
                {
                    List.Add(new PaymentVO()
                    {
                        AccountId = AccountId,
                        Amount = (v.AMOUNT == null ? 0 : (decimal)v.AMOUNT),
                        Reference = v.REFERENCE,
                        Created = (DateTime)v.CREATED,
                        PaymentType = v.PAYMENT_TYPE
                    });
                }
            }

            return List;
        }

        public string GetMSISDNByAccountId(long AccountId)
        {
            string MSISDN;
            using (var context = new CRMDbContext())
            {
                var result = from d in context.XOX_T_ACCNT
                             where d.ROW_ID == AccountId
                             select d;

                MSISDN = result.First().MSISDN;
            }
            return MSISDN;
        }

        public List<PaymentResponseList> GetPayment(long AccountId, long UserId = 0)
        {
            if (UserId == 0)
            {
                UserId = long.Parse(Thread.CurrentPrincipal.Identity.Name);
            }

            string MSISDN;
            using (var context = new CRMDbContext())
            {
                var result = from d in context.XOX_T_ACCNT
                             where d.ROW_ID == AccountId
                             select d;

                MSISDN = result.First().MSISDN[0] == '6' ? result.First().MSISDN : '6' + result.First().MSISDN;     
            }

            var SixMonthAgo = DateTime.Now.AddMonths(-6);

            List<PaymentResponseList> ResponseList = new List<PaymentResponseList>();
            for (int i = 1; i <= 6; i++)
            {
                DateTime dt = SixMonthAgo.AddMonths(i);
                GetPayment param = new GetPayment();
                param.User = new User()
                {
                    UserId = (int)UserId
                };
                param.Month = dt.Month;
                param.Year = dt.Year;
                param.Size = 100000;///
                param.MSISDN = MSISDN;


                var result = EAIService.GetPayment(param);
                var r = JObject.Parse(result);

                List<GetPaymentResponse> response = new List<GetPaymentResponse>();
                try
                {
                    JArray list = (JArray)r["Result"];
                    response = list.ToObject<List<GetPaymentResponse>>();

                    ResponseList.Add(new PaymentResponseList()
                    {
                        Month = dt.Month,
                        Year = dt.Year,
                        PaymentList = response
                    });
                }
                catch
                {
                    string Msg = r["Result"].ToString();

                    ResponseList.Add(new PaymentResponseList()
                    {
                        Month = dt.Month,
                        Year = dt.Year,
                        Message = Msg
                    });
                }
            }

            return ResponseList;
        }

        public GetSubscriberProfileResponse GetSubscriberProfile(long AccountId, long UserId = 0)
        {
            if (UserId == 0)
            {
                UserId = long.Parse(Thread.CurrentPrincipal.Identity.Name);
            }

            string MSISDN = GetMSISDNByAccountId(AccountId);
            if (MSISDN[0] != '6')
            {
                MSISDN = "6" + MSISDN;
            }

            var result = EAIService.GetSubscriberProfile(new GetSubscriberProfile()
            {
                MSISDN = MSISDN,
                User = new User()
                {
                    UserId = (int)UserId
                }
            });
            var r = JObject.Parse(result);

            try
            {
                var response = r["Result"].ToObject<GetSubscriberProfileResponse>();
                return response;
            }
            catch
            {
                throw new Exception(r["Result"].ToString());
            }
        }

        public string AddSubcriberProfile(long AccountId, long OrderId, int UserId = 0)
        {
            if (UserId == 0)
            {
                UserId = int.Parse(Thread.CurrentPrincipal.Identity.Name);
            }

            using (var DBContext = new CRMDbContext())
            {
                var result = from d in DBContext.XOX_T_ACCNT
                             where d.ROW_ID == AccountId
                             select d;

                var account = result.First();

                bool foreigner = account.ID_TYPE == "PassportNo" ? true : false;

                var result2 = from d in DBContext.XOX_T_ORDER
                              where d.ROW_ID == OrderId
                              select d;

                var order = result2.First();

                var address = AddressService.Get((long)account.ADDR_ID) ?? new AddressInfoVO();

                var dataPack = ProductService.GetDataPack(order.PLAN);

                var MSISDN = account.MSISDN[0] == '6' ? account.MSISDN : ('6' + account.MSISDN);

                var printedBill = "No";
                printedBill = this.GetPrintedBillingByOrder(OrderId) == true ? "Yes" : "No";
                /*try{
                    printedBill = bool.Parse(account.BILL_LANG) == true ? "Yes" : "No";
                }
                catch{}*/

                var r = ExternalService.CreateNewSubscriber(new AddSubscriberProfile()
                {
                    User = new User()
                    {
                        Source = "CRM",
                        UserId = UserId
                    },
                    MSISDN = MSISDN,
                    Customer = new CustomerRequest()
                    {
                        accountNumber = account.CUSTOMER_NUM,
                        armyId = "",
                        city = address.City,
                        emailAddress = account.EMAIL_ADDR,
                        ic = account.ID_NUM,
                        language = account.PREFERRED_LANG,
                        name = account.NAME,
                        organizationName = "",
                        passport = "",
                        planInfo = order.PLAN,
                        postalAddress = address.AddressLine1,
                        postalAddressL2 = address.AddressLine2,
                        postalCode = address.Postcode,
                        referrerCode = "",
                        state = address.State
                    },
                    Subscription = new Subscription()
                    {
                        autoDebit = "Yes",
                        contractPeriod = 24,
                        iccid = account.SIM_SERIAL_NUMBER,
                        motherMaidenName = account.MOTHER_MAIDEN_NAME,
                        printedBill = printedBill,
                        signUpChannel = "CRP"
                    },
                    Counter = new Counter()
                    {
                        creditLimit = (int)ProductService.GetCreditLimit(order.PLAN),
                        dataPack = dataPack,
                        deposit = (int)ProductService.GetDeposit(order.ROW_ID),// + (foreigner == true ? XOXConstants.FOREIGNER_DEPOSIT : 0), ////
                        initFreeData = (int)ProductService.GetinitFreeData(order.PLAN),
                        initFreeOnNetCalls = (int)ProductService.GetinitFreeOnNetCalls(order.PLAN),
                        initFreeOffNetCalls = (int)ProductService.GetinitFreeOffNetCalls(order.PLAN),
                        initFreeOnNetSms = (int)ProductService.GetinitFreeOnNetSms(order.PLAN),
                        initFreeOffNetSms = (int)ProductService.GetinitFreeOffNetSms(order.PLAN),
                        prime = (int)ProductService.GetPrime(order.PLAN)
                    },
                    fnfCounter = new FnfCounter()
                    {
                        initFnFData = (int)ProductService.GetinitFnFData(order.PLAN),
                        initFnFOnNetCalls = (int)ProductService.GetinitFnFOnNetCalls(order.PLAN),
                        initFnFOffNetCalls = (int)ProductService.GetinitFnFOffNetCalls(order.PLAN),
                        initFnFOnNetSms = (int)ProductService.GetinitFnFOnNetSms(order.PLAN),
                        initFnFOffNetSms = (int)ProductService.GetinitFnFOffNetSms(order.PLAN),
                    }
                });
                
                
                //add suppline subscriber profile
                var SupplementaryLine = ((int)AccountType.SupplementaryLine).ToString();
                var suppline = from d in DBContext.XOX_T_ACCNT
                               where d.PAR_ACCNT_ID == AccountId
                               && d.ACCNT_TYPE_CD == SupplementaryLine
                               select d;

                foreach (var v in suppline)
                {
                    try
                    {
                        var supplin_address = AddressService.Get((long)v.ADDR_ID) ?? new AddressInfoVO();

                        var SL_MSISDN = v.MSISDN[0] == '6' ? v.MSISDN : ('6' + v.MSISDN);

                        var sl_dataPack = "SUPP18"; /////////////

                        var sl_order_plan = "Supp 18"; //////////////////

                        var deposit = 0;
                        try{
                            deposit = (int)ProductService.GetDepositByAccountId(v.ROW_ID);
                        }
                        catch{}
                        deposit = deposit == 0 ? 18 : deposit;

                        var sl = ExternalService.CreateNewSubscriber(new AddSubscriberProfile()
                        {
                            User = new User()
                            {
                                Source = "CRM",
                                UserId = UserId
                            },
                            MSISDN = SL_MSISDN,
                            Customer = new CustomerRequest()
                            {
                                accountNumber = v.CUSTOMER_NUM,
                                armyId = "",
                                city = supplin_address.City,
                                emailAddress = v.EMAIL_ADDR,
                                ic = v.ID_NUM,
                                language = v.PREFERRED_LANG,
                                name = v.NAME,
                                organizationName = "",
                                passport = "",
                                planInfo = sl_order_plan,
                                postalAddress = supplin_address.AddressLine1,
                                postalAddressL2 = supplin_address.AddressLine2,
                                postalCode = supplin_address.Postcode,
                                referrerCode = "",
                                state = supplin_address.State
                            },
                            Subscription = new Subscription()
                            {
                                autoDebit = "Yes",
                                contractPeriod = 24,
                                iccid = v.SIM_SERIAL_NUMBER,
                                motherMaidenName = v.MOTHER_MAIDEN_NAME,
                                printedBill = printedBill,
                                signUpChannel = "CRP"
                            },
                            Counter = new Counter()
                            {
                                creditLimit = (int)ProductService.GetCreditLimit(sl_order_plan),
                                dataPack = sl_dataPack,
                                deposit = deposit,
                                initFreeData = (int)ProductService.GetinitFreeData(sl_order_plan),
                                initFreeOnNetCalls = (int)ProductService.GetinitFreeOnNetCalls(sl_order_plan),
                                initFreeOffNetCalls = (int)ProductService.GetinitFreeOffNetCalls(sl_order_plan),
                                initFreeOnNetSms = (int)ProductService.GetinitFreeOnNetSms(sl_order_plan),
                                initFreeOffNetSms = (int)ProductService.GetinitFreeOffNetSms(sl_order_plan),
                                prime = (int)ProductService.GetPrime(sl_order_plan)
                            },
                            fnfCounter = new FnfCounter()
                            {
                                initFnFData = (int)ProductService.GetinitFnFData(sl_order_plan),
                                initFnFOnNetCalls = (int)ProductService.GetinitFnFOnNetCalls(sl_order_plan),
                                initFnFOffNetCalls = (int)ProductService.GetinitFnFOffNetCalls(sl_order_plan),
                                initFnFOnNetSms = (int)ProductService.GetinitFnFOnNetSms(sl_order_plan),
                                initFnFOffNetSms = (int)ProductService.GetinitFnFOffNetSms(sl_order_plan),
                            }
                        });
                    }
                    catch
                    {
                        //////////
                    }
                }
                

                return r;
            }
        }

        public string migrateToPostpaid(long AccountId, long OrderId, int UserId = 0)
        {
            if (UserId == 0)
            {
                UserId = int.Parse(Thread.CurrentPrincipal.Identity.Name);
            }

            using (var DBContext = new CRMDbContext())
            {
                var result = from d in DBContext.XOX_T_ACCNT
                             where d.ROW_ID == AccountId
                             select d;

                var account = result.First();

                bool foreigner = account.ID_TYPE == "PassportNo" ? true : false;

                var result2 = from d in DBContext.XOX_T_ORDER
                              where d.ROW_ID == OrderId
                              select d;

                var order = result2.First();

                var address = AddressService.Get((long)account.ADDR_ID);

                var dataPack = ProductService.GetDataPack(order.PLAN);

                var MSISDN = account.MSISDN[0] == '6' ? account.MSISDN : ('6' + account.MSISDN);

                var printedBill = "No";
                printedBill = this.GetPrintedBillingByOrder(OrderId) == true ? "Yes" : "No";
                /*try{
                    printedBill = bool.Parse(account.BILL_LANG) == true ? "Yes" : "No";
                }
                catch{}*/

                var r = ExternalService.CreateCOBPSubsciber(new AddSubscriberProfile()
                {
                    User = new User()
                    {
                        Source = "CRM",
                        UserId = UserId
                    },
                    MSISDN = MSISDN,
                    Customer = new CustomerRequest()
                    {
                        accountNumber = account.CUSTOMER_NUM,
                        armyId = "",
                        city = address.City,
                        emailAddress = account.EMAIL_ADDR,
                        ic = account.ID_NUM,
                        language = account.PREFERRED_LANG,
                        name = account.NAME,
                        organizationName = "",
                        passport = "",
                        planInfo = order.PLAN,
                        postalAddress = address.AddressLine1,
                        postalAddressL2 = address.AddressLine2,
                        postalCode = address.Postcode,
                        referrerCode = "",
                        state = address.State
                    },
                    Subscription = new Subscription()
                    {
                        autoDebit = "Yes",
                        contractPeriod = 24,
                        iccid = account.SIM_SERIAL_NUMBER,
                        motherMaidenName = account.MOTHER_MAIDEN_NAME,
                        printedBill = printedBill,
                        signUpChannel = "CRP"
                    },
                    Counter = new Counter()
                    {
                        creditLimit = (int)ProductService.GetCreditLimit(order.PLAN),
                        dataPack = dataPack,
                        deposit = (int)ProductService.GetDeposit(order.ROW_ID),// + (foreigner == true ? XOXConstants.FOREIGNER_DEPOSIT : 0), ////
                        initFreeData = (int)ProductService.GetinitFreeData(order.PLAN),
                        initFreeOnNetCalls = (int)ProductService.GetinitFreeOnNetCalls(order.PLAN),
                        initFreeOffNetCalls = (int)ProductService.GetinitFreeOffNetCalls(order.PLAN),
                        initFreeOnNetSms = (int)ProductService.GetinitFreeOnNetSms(order.PLAN),
                        initFreeOffNetSms = (int)ProductService.GetinitFreeOffNetSms(order.PLAN),
                        prime = (int)ProductService.GetPrime(order.PLAN)
                    },
                    fnfCounter = new FnfCounter()
                    {
                        initFnFData = (int)ProductService.GetinitFnFData(order.PLAN),
                        initFnFOnNetCalls = (int)ProductService.GetinitFnFOnNetCalls(order.PLAN),
                        initFnFOffNetCalls = (int)ProductService.GetinitFnFOffNetCalls(order.PLAN),
                        initFnFOnNetSms = (int)ProductService.GetinitFnFOnNetSms(order.PLAN),
                        initFnFOffNetSms = (int)ProductService.GetinitFnFOffNetSms(order.PLAN),
                    }
                });


                return r;
            }
        }

        public long CheckIntegrationId(long IntegrationId, string AccountTypeCode = "1")
        {
            using (var DBContext = new CRMDbContext())
            {
                string IntegrationIdString = IntegrationId.ToString();
                var Account = from d in DBContext.XOX_T_ACCNT
                              where d.INTEGRATION_ID == IntegrationIdString && d.ACCNT_TYPE_CD == AccountTypeCode
                              select d;

                if (Account.Count() > 0)
                {
                    return Account.First().ROW_ID;
                }
            }

            return 0;
        }

        public bool SavePaymentRecord(PaymentRecordVO PaymentRecord, long UserId = 0)
        {
            if (UserId == 0)
            {
                UserId = long.Parse(Thread.CurrentPrincipal.Identity.Name);
            }

            using (var context = new CRMDbContext())
            {
                var ett = from d in context.XOX_T_ACCNT_PAYMENT
                          where d.ACCNT_ID == PaymentRecord.AccountId
                          select d;

                //Deposit
                var deposit = ett.Where(d => d.PAYMENT_TYPE == XOXConstants.PAYMENT_TYPE_DEPOSIT);

                if (deposit.Count() == 0)
                {
                    XOX_T_ACCNT_PAYMENT v = new XOX_T_ACCNT_PAYMENT();
                    v.ACCNT_ID = PaymentRecord.AccountId;
                    v.AMOUNT = PaymentRecord.Deposit;
                    v.CREATED = DateTime.Now;
                    v.CREATED_BY = UserId;
                    v.PAYMENT_TYPE = XOXConstants.PAYMENT_TYPE_DEPOSIT;
                    v.REFERENCE = PaymentRecord.Reference == null ? "" : PaymentRecord.Reference;
                    v.REFERENCE = v.PAYMENT_TYPE + " - " + v.REFERENCE;
                    v.PAYMENT_METHOD = "Credit Card";

                    context.XOX_T_ACCNT_PAYMENT.Add(v);
                    context.SaveChanges();
                }
                else
                {
                    deposit.First().ACCNT_ID = PaymentRecord.AccountId;
                    if (PaymentRecord.Deposit > 0)
                    {
                        deposit.First().AMOUNT = PaymentRecord.Deposit;
                    }
                    deposit.First().LAST_UPD = DateTime.Now;
                    deposit.First().LAST_UPD_BY = UserId;
                    deposit.First().REFERENCE = PaymentRecord.Reference == null ? "" : PaymentRecord.Reference;
                    deposit.First().REFERENCE = deposit.First().PAYMENT_TYPE + " - " + deposit.First().REFERENCE;
                    context.SaveChanges();
                }

                //Advance Payment
                var advancepayment = ett.Where(d => d.PAYMENT_TYPE == XOXConstants.PAYMENT_TYPE_ADVANCE_PAYMENT);

                if (advancepayment.Count() == 0)
                {
                    XOX_T_ACCNT_PAYMENT v = new XOX_T_ACCNT_PAYMENT();
                    v.ACCNT_ID = PaymentRecord.AccountId;
                    v.AMOUNT = PaymentRecord.AdvancePayment;
                    v.CREATED = DateTime.Now;
                    v.CREATED_BY = UserId;
                    v.PAYMENT_TYPE = XOXConstants.PAYMENT_TYPE_ADVANCE_PAYMENT;
                    v.REFERENCE = PaymentRecord.Reference == null ? "" : PaymentRecord.Reference;
                    v.REFERENCE = v.PAYMENT_TYPE + " - " + v.REFERENCE;
                    v.PAYMENT_METHOD = "Credit Card";

                    context.XOX_T_ACCNT_PAYMENT.Add(v);
                    context.SaveChanges();
                }
                else
                {
                    advancepayment.First().ACCNT_ID = PaymentRecord.AccountId;
                    if (PaymentRecord.AdvancePayment > 0)
                    {
                        advancepayment.First().AMOUNT = PaymentRecord.AdvancePayment;
                    }
                    advancepayment.First().LAST_UPD = DateTime.Now;
                    advancepayment.First().LAST_UPD_BY = UserId;
                    advancepayment.First().REFERENCE = PaymentRecord.Reference == null ? "" : PaymentRecord.Reference;
                    advancepayment.First().REFERENCE = advancepayment.First().PAYMENT_TYPE + " - " + advancepayment.First().REFERENCE;
                    context.SaveChanges();
                }
                
                //Foreign Deposit
                if (PaymentRecord.ForeignDeposit != null && PaymentRecord.ForeignDeposit != 0)
                {
                    var foreigndeposit = ett.Where(d => d.PAYMENT_TYPE == XOXConstants.PAYMENT_TYPE_FOREIGN_DEPOSIT);

                    if (foreigndeposit.Count() == 0)
                    {
                        XOX_T_ACCNT_PAYMENT v = new XOX_T_ACCNT_PAYMENT();
                        v.ACCNT_ID = PaymentRecord.AccountId;
                        v.AMOUNT = PaymentRecord.ForeignDeposit;
                        v.CREATED = DateTime.Now;
                        v.CREATED_BY = UserId;
                        v.PAYMENT_TYPE = XOXConstants.PAYMENT_TYPE_FOREIGN_DEPOSIT;
                        v.REFERENCE = PaymentRecord.Reference == null ? "" : PaymentRecord.Reference;
                        v.REFERENCE = v.PAYMENT_TYPE + " - " + v.REFERENCE;
                        v.PAYMENT_METHOD = "Credit Card";

                        context.XOX_T_ACCNT_PAYMENT.Add(v);
                        context.SaveChanges();
                    }
                    else
                    {
                        foreigndeposit.First().ACCNT_ID = PaymentRecord.AccountId;
                        if (PaymentRecord.ForeignDeposit > 0)
                        {
                            foreigndeposit.First().AMOUNT = PaymentRecord.ForeignDeposit;
                        }
                        foreigndeposit.First().LAST_UPD = DateTime.Now;
                        foreigndeposit.First().LAST_UPD_BY = UserId;
                        foreigndeposit.First().REFERENCE = PaymentRecord.Reference == null ? "" : PaymentRecord.Reference;
                        foreigndeposit.First().REFERENCE = foreigndeposit.First().PAYMENT_TYPE + " - " + foreigndeposit.First().REFERENCE;
                        context.SaveChanges();
                    }
                }

            }

            return true;
        }

        public bool CheckPaymentSufficient(long AccountId, long OrderId)
        {
            using (var context = new CRMDbContext())
            {
                var ACCNT_PAYMENT = from d in context.XOX_T_ACCNT_PAYMENT
                                    where d.ACCNT_ID == AccountId
                                    select d;

                if (ACCNT_PAYMENT.Count() > 0)
                {
                    var ORDER = from d in context.XOX_T_ORDER
                                where d.ROW_ID == OrderId
                                select d;

                    if (ORDER.Count() > 0)
                    {
                        decimal totalPayment = 0;
                        foreach (var v in ACCNT_PAYMENT)
                        {
                            if (v.PAYMENT_TYPE != XOXConstants.PAYMENT_TYPE_BILLING && v.PAYMENT_TYPE != XOXConstants.PAYMENT_TYPE_FOREIGN_DEPOSIT) /*TEMP*/
                            {
                                totalPayment += (v.AMOUNT == null ? 0 : (decimal)v.AMOUNT);
                            }
                        }

                        //suppline payment
                        foreach (var v in this.GetAllSupplementaryLine(AccountId))
                        {
                            var SUPPLINE_PAYMENT = from d in context.XOX_T_ACCNT_PAYMENT
                                                   where d.ACCNT_ID == v.AccountId
                                                   select d;

                            foreach (var y in SUPPLINE_PAYMENT)
                            {
                                if (y.PAYMENT_TYPE != XOXConstants.PAYMENT_TYPE_BILLING && y.PAYMENT_TYPE != XOXConstants.PAYMENT_TYPE_FOREIGN_DEPOSIT) /*TEMP*/
                                {
                                    totalPayment += (y.AMOUNT == null ? 0 : (decimal)y.AMOUNT);
                                }
                            }
                        }

                        var requiredPayment = ProductService.GetRequiredAdvancePayment(ORDER.First().ROW_ID) + ProductService.GetRequiredDeposit(ORDER.First().ROW_ID);

                        if (totalPayment >= requiredPayment)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
        
        public void ClearPaymentRecord(long OrderId)
        {
            using (var context = new CRMDbContext())
            {
                var AccountId = GetAccountIdByOrderId(OrderId);
                var ett = from d in context.XOX_T_ACCNT_PAYMENT
                          where d.ACCNT_ID == AccountId
                          && d.PAYMENT_TYPE != XOXConstants.PAYMENT_TYPE_BILLING
                          select d;

                foreach (var v in ett)
                {
                    v.AMOUNT = 0;
                }
                context.SaveChanges();                
            }
        }

        public void AddCUG(long AccountId, string CUGNo, long UserId = 0)
        {
            UserId = UserId == 0 ? long.Parse(Thread.CurrentPrincipal.Identity.Name) : UserId;

            using (var context = new CRMDbContext())
            {
                var ett = from d in context.XOX_T_ACCNT
                          where d.ROW_ID == AccountId
                          select d;

                if (ett.Count() > 0)
                {
                    var Account = ett.First();

                    var MSISDN = Account.MSISDN[0] == '6' ? Account.MSISDN : ('6' + Account.MSISDN);
                    var result = EAIService.AddCUG(new AddCUG()
                    {
                        CUGNo = CUGNo,
                        MSISDN = MSISDN,
                        User = new User()
                        {
                            Source = "CRM",
                            UserId = (int)UserId
                        }
                    });

                    var r = JObject.Parse(result);

                    string response = "";
                    try
                    {
                        response = r["Result"]["status"].ToString();
                    }
                    catch { }

                    if (response.ToLower() == "true") ///
                    {
                        var User = (from d in context.XOX_T_USER
                                   where d.ROW_ID == UserId
                                   select d).First();

                        //add activity
                        AccountActivityVO activity = new AccountActivityVO();
                        activity.ACCNT_ID = AccountId;
                        activity.ACT_DESC = CUGNo + " was added to Closed User Group";
                        activity.ASSIGNEE = User.USERNAME;
                        activity.REASON = "";
                        activity.CREATED_BY = UserId;
                        AddActivity(activity);
                    }
                    else
                    {
                        throw new Exception("Failed adding CUG.");
                    }
                }
                else
                {
                    throw new Exception("Account not found.");
                }
            }
        }

        public void RemoveCUG(long AccountId, string CUGNo, long UserId = 0)
        {
            UserId = UserId == 0 ? long.Parse(Thread.CurrentPrincipal.Identity.Name) : UserId;

            using (var context = new CRMDbContext())
            {
                var ett = from d in context.XOX_T_ACCNT
                          where d.ROW_ID == AccountId
                          select d;

                if (ett.Count() > 0)
                {
                    var Account = ett.First();

                    var MSISDN = Account.MSISDN[0] == '6' ? Account.MSISDN : ('6' + Account.MSISDN);
                    var result = EAIService.RemoveCUG(new RemoveCUG()
                    {
                        CUGNo = CUGNo,
                        MSISDN = MSISDN,
                        User = new User()
                        {
                            Source = "CRM",
                            UserId = (int)UserId
                        }
                    });

                    var r = JObject.Parse(result);

                    string response = "";
                    try
                    {
                        response = r["Result"]["status"].ToString();
                    }
                    catch { }

                    if (response.ToLower() == "true") ///
                    {
                        var User = (from d in context.XOX_T_USER
                                    where d.ROW_ID == UserId
                                    select d).First();

                        //add activity
                        AccountActivityVO activity = new AccountActivityVO();
                        activity.ACCNT_ID = AccountId;
                        activity.ACT_DESC = CUGNo + " was removed from Closed User Group";
                        activity.ASSIGNEE = User.USERNAME;
                        activity.REASON = "";
                        activity.CREATED_BY = UserId;
                        AddActivity(activity);
                    }
                    else
                    {
                        throw new Exception("Failed removing CUG.");
                    }
                }
                else
                {
                    throw new Exception("Account not found.");
                }
            }
        }

        public List<string> GetCUG(long AccountId, long UserId = 0)
        {
            UserId = UserId == 0 ? long.Parse(Thread.CurrentPrincipal.Identity.Name) : UserId;

            using (var context = new CRMDbContext())
            {
                var ett = from d in context.XOX_T_ACCNT
                          where d.ROW_ID == AccountId
                          select d;

                if (ett.Count() > 0)
                {
                    var Account = ett.First();

                    var MSISDN = Account.MSISDN[0] == '6' ? Account.MSISDN : ('6' + Account.MSISDN);
                    var result = EAIService.GetCUG(new GetCUG()
                    {
                        MSISDN = MSISDN,
                        User = new User()
                        {
                            Source = "CRM",
                            UserId = (int)UserId
                        }
                    });

                    var r = JObject.Parse(result);

                    try
                    {
                        var response = r["Result"].ToObject<List<cugNumberData>>();

                        List<string> ListCUG = new List<string>();
                        foreach (var v in response)
                        {
                            ListCUG.Add(v.cugNumber);
                        }

                        return ListCUG;
                    }
                    catch
                    {
                        try
                        {
                            string response = r["Result"].ToString();
                            throw new Exception(response);
                        }
                        catch
                        {
                            throw new Exception("Failed getting data from MPP.");
                        }
                    }
                }
                else
                {
                    throw new Exception("Account not found.");
                }
            }
        }

        public List<PaymentVO> GetAllPaymentBySearch(int startIdx, int length, ref int TotalCount, string orderBy = "", string orderDirection = "", PaymentVO Parameter = null)
        {
            List<PaymentVO> List = new List<PaymentVO>();

            if (Parameter != null)
            {
                using (var dbContext = new CRMDbContext())
                {
                    var ett = from d in dbContext.XOX_T_ACCNT_PAYMENT
                              join e in dbContext.XOX_T_ACCNT on d.ACCNT_ID equals e.ROW_ID
                              select new
                              {
                                  PAYMENT_TYPE = d.PAYMENT_TYPE,
                                  PAYMENT_METHOD = d.PAYMENT_METHOD,
                                  CREATED = d.CREATED,
                                  REFERENCE = d.REFERENCE,
                                  AMOUNT = d.AMOUNT,
                                  ROW_ID = d.ROW_ID,
                                  ACCNT_ID = d.ACCNT_ID,
                                  NAME = e.NAME,
                                  MSISDN = e.MSISDN,
                                  CREATED_BY = d.CREATED_BY,
                              };

                    //filtering
                    if (Parameter.PaymentType != "")
                    {
                        ett = ett.Where(m => m.PAYMENT_TYPE.ToLower().Contains(Parameter.PaymentType.ToLower()));
                    }
                    if (Parameter.PaymentMethod != "")
                    {
                        ett = ett.Where(m => m.PAYMENT_METHOD.ToLower().Contains(Parameter.PaymentMethod.ToLower()));
                    }
                    if (Parameter.From != "" && Parameter.To != "")
                    {
                        DateTime DateFrom = DateTime.Parse(Parameter.From);
                        DateTime DateTo = DateTime.Parse(Parameter.To).AddDays(1);
                        ett = ett.Where(m => m.CREATED.Value >= DateFrom && m.CREATED.Value < DateTo); //use LAST_UPD?
                    }

                    TotalCount = ett.Count();

                    //ordering && paging
                    if (orderDirection == "asc")
                    {
                        if (orderBy == "PaymentType")
                            ett = ett.OrderBy(m => m.PAYMENT_TYPE);
                        else if (orderBy == "PaymentMethod")
                            ett = ett.OrderBy(m => m.PAYMENT_METHOD);
                        else if (orderBy == "Date")
                            ett = ett.OrderBy(m => m.CREATED); //use LAST_UPD?
                        else if (orderBy == "Name")
                            ett = ett.OrderBy(m => m.NAME);
                        else if (orderBy == "MSISDN")
                            ett = ett.OrderBy(m => m.MSISDN);
                        else if (orderBy == "Amount")
                            ett = ett.OrderBy(m => m.AMOUNT);
                        else if (orderBy == "Reference")
                            ett = ett.OrderBy(m => m.REFERENCE);
                        else
                            ett = ett.OrderBy(m => m.ROW_ID);
                    }
                    else
                    {
                        if (orderBy == "PaymentType")
                            ett = ett.OrderByDescending(m => m.PAYMENT_TYPE);
                        else if (orderBy == "PaymentMethod")
                            ett = ett.OrderByDescending(m => m.PAYMENT_METHOD);
                        else if (orderBy == "Date")
                            ett = ett.OrderByDescending(m => m.CREATED); //use LAST_UPD?
                        else if (orderBy == "Name")
                            ett = ett.OrderByDescending(m => m.NAME);
                        else if (orderBy == "MSISDN")
                            ett = ett.OrderByDescending(m => m.MSISDN);
                        else if (orderBy == "Amount")
                            ett = ett.OrderByDescending(m => m.AMOUNT);
                        else if (orderBy == "Reference")
                            ett = ett.OrderByDescending(m => m.REFERENCE);
                        else
                            ett = ett.OrderByDescending(m => m.ROW_ID);
                    }

                    if (length >= 0)
                        ett = ett.Skip(startIdx).Take(length);

                    foreach (var v in ett)
                    {
                        try
                        {
                            PaymentVO e = new PaymentVO();
                            e.PaymentId = v.ROW_ID;
                            e.AccountId = v.ACCNT_ID;
                            e.Amount = (decimal)v.AMOUNT;
                            e.Created = v.CREATED;
                            e.PaymentMethod = v.PAYMENT_METHOD;
                            e.PaymentType = v.PAYMENT_TYPE;
                            e.Reference = v.REFERENCE;
                            e.Name = v.NAME;
                            e.MSISDN = v.MSISDN;
                            e.CreatedBy = UserService.GetName(v.CREATED_BY == null ? 0 : v.CREATED_BY.Value);
                            List.Add(e);
                        }
                        catch { }
                    }
                }
            }

            return List;
        }

        public void EditPrintedBillingFlg(long AccountId, bool PrintedBillingFlg = false)
        {
            using (var dbContext = new CRMDbContext())
            {
                var Account = (from d in dbContext.XOX_T_ACCNT
                           where d.ROW_ID == AccountId
                           select d).FirstOrDefault();

                if (Account != null)
                {
                    Account.BILL_LANG = PrintedBillingFlg.ToString();
                    dbContext.SaveChanges();
                }
            }

        }

        public bool GetPrintedBillingByOrder(long OrderId)
        {
            using (var dbContext = new CRMDbContext())
            {
                var OrderItem = from d in dbContext.XOX_T_ORDER_ITEM
                                join e in dbContext.XOX_T_PROD_ITEM on d.PROD_ID equals e.ROW_ID
                                join f in dbContext.XOX_T_PROD on e.PROD_ID equals f.ROW_ID
                                where d.ORDER_ID == OrderId
                                && f.EXT_PROD_NAME == "Printed Billing"
                                select d;

                if (OrderItem.Count() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public void ChangeAdvancePaymentFlagResponse(long AccountId, bool Response)
        {
            using (var DBContext = new CRMDbContext())
            {
                var ett = from d in DBContext.XOX_T_ACCNT_PAYMENT
                          where d.ACCNT_ID == AccountId
                          && d.PAYMENT_TYPE == XOXConstants.PAYMENT_TYPE_ADVANCE_PAYMENT
                          select d;

                if (ett.Count() > 0)
                {
                    var v = ett.First();
                    v.FLAG_RESPONSE = true;

                    DBContext.SaveChanges();
                }
            }
        }

        public bool? GetAdvancePaymentFlagResponse(long AccountId)
        {
            using (var DBContext = new CRMDbContext())
            {
                var ett = from d in DBContext.XOX_T_ACCNT_PAYMENT
                          where d.ACCNT_ID == AccountId
                          && d.PAYMENT_TYPE == XOXConstants.PAYMENT_TYPE_ADVANCE_PAYMENT
                          select d;

                if (ett.Count() > 0)
                {
                    var v = ett.First();

                    return v.FLAG_RESPONSE;
                }
                else
                {
                    return null;
                }
            }
        }

        public void MakeAdvancePayment(long AccountId)
        {
            using (var dbContext = new CRMDbContext())
            {
                var Account = (from d in dbContext.XOX_T_ACCNT
                               where d.ROW_ID == AccountId
                               select d).First();

                OrderService OrderService = new OrderService();
                Payment payment = OrderService.GetAdvancePaymentRecord(Account.ROW_ID);

                if (payment == null) //if payment got no record.
                {
                    throw new Exception("Advance Payment must be recorded in CRM side.");
                }
                if (Account.ACCNT_TYPE_CD == ((int)AccountType.PrincipalLine).ToString())
                {
                    payment.Reference = Account.BANK_ISSUER + "-" + CommonService.MaskCreditCardDigit(Account.BILL_ACCNT_NUM);
                }
                else
                {
                    var ParAccount = (from d in dbContext.XOX_T_ACCNT
                                      where d.ROW_ID == Account.PAR_ACCNT_ID
                                      select d).First();
                    payment.Reference = ParAccount.BANK_ISSUER + "-" + CommonService.MaskCreditCardDigit(ParAccount.BILL_ACCNT_NUM);
                }
                payment.MSISDN = Account.MSISDN[0] == '6' ? Account.MSISDN : '6' + Account.MSISDN;
                payment.PaymentMethod = XOXConstants.PAYMENT_METHOD_CREDIT_CARD;
                payment.User = new User()
                {
                    Source = "CRM",
                    UserId = 1
                };

                var MakePaymentResult = "true";//this.MakePayment(Account.ROW_ID, payment, false, 1);

                if (MakePaymentResult != "true")
                {
                    throw new Exception(MakePaymentResult);
                }
                else
                {
                    this.ChangeAdvancePaymentFlagResponse(Account.ROW_ID, true);
                }
            }
        }

        public List<AccountPaymentCardDetail> GetAllPaymentCardDetails(string CardType = "", int WithinMonths = 0)
        {
            List<AccountPaymentCardDetail> list = new List<AccountPaymentCardDetail>();
            using (var DBContext = new CRMDbContext())
            {
                string PrincipalLineType = ((int)AccountType.PrincipalLine).ToString();
                string SupplementaryLineType = ((int)AccountType.SupplementaryLine).ToString();
                var result = from d in DBContext.XOX_T_ACCNT
                             where d.ACCNT_TYPE_CD == PrincipalLineType || d.ACCNT_TYPE_CD == SupplementaryLineType
                             select d;

                if (CardType != null && CardType != "")
                {
                    result = result.Where(m => m.BILL_CARD_TYPE == CardType);
                }
                if (WithinMonths != null && WithinMonths != 0)
                {
                    int MonthsNow = (DateTime.Now.Year * 12) + DateTime.Now.Month;
                    result = result.Where(m => (((((m.BANK_EXPIRY_YEAR ?? 0) * 12) + (m.BANK_EXPIRY_MONTH ?? 0)) - MonthsNow) <= WithinMonths) && (((((m.BANK_EXPIRY_YEAR ?? 0) * 12) + (m.BANK_EXPIRY_MONTH ?? 0)) - MonthsNow) >= 0));
                }

                foreach (var v in result)
                {
                    try
                    {
                        list.Add(new AccountPaymentCardDetail()
                        {
                            AccountStatus = v.ACCNT_STATUS,
                            CardHolderName = v.BILL_ACCNT_NAME,
                            CardNumber = v.BILL_ACCNT_NUM,
                            CardType = v.BILL_CARD_TYPE,
                            CardExpiryMonth = v.BANK_EXPIRY_MONTH ?? 0,
                            CardExpiryYear = v.BANK_EXPIRY_YEAR ?? 0,
                            CardIssuerBank = v.BANK_ISSUER,
                            MSISDN = v.MSISDN,
                            Name = v.NAME
                        });
                    }
                    catch {}
                }
            }

            return list;
        }

        public int GetTerminatedAccountCount(DateTime? From = null, DateTime? To = null)
        {
            using (var DBContext = new CRMDbContext())
            {
                var TerminatedAccount = from d in DBContext.XOX_T_ACCNT
                                        select d;

                if (From != null && To != null)
                {
                    TerminatedAccount = TerminatedAccount.Where(m => m.TERMINATION_DT >= From && m.TERMINATION_DT < To);

                    return TerminatedAccount.Count();
                }
                else
                {
                    return -1;
                }
            }
        }
        
    }
}
