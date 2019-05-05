using CRM;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using XOX.CRM.Lib.Common.Constants;
using XOX.CRM.Lib.DBContext;
using XOX.CRM.Lib.WebServices;

namespace XOX.CRM.Lib
{
    public class ServiceRequestService : IServiceRequestService
    {
        public IAccountService AccountService = new AccountService();
        public IProductService ProductService = new ProductService();
        public IAssetService AssetService = new AssetService();

        public List<ServiceRequestVO> GetAllServiceRequests(int startIdx, int length, ref int TotalCount, string orderBy = "", string orderDirection = "", ServiceRequestVO qFilter = null, long AccountId = 0)
        {
            List<ServiceRequestVO> List = new List<ServiceRequestVO>();
            using (var dbContext = new CRMDbContext())
            {
                var result = from d in dbContext.XOX_T_SRV_TIC
                             join y in dbContext.XOX_T_ACCNT on d.CUST_ID equals y.ROW_ID
                             select new { SRV_TIC = d, ACCNT = y };

                if (AccountId != 0 && AccountId != null)
                {
                    result = result.Where(m => m.ACCNT.ROW_ID == AccountId);
                }

                //filtering
                if (qFilter != null)
                {
                    if (qFilter.Category != null && qFilter.Category != "")
                        result = result.Where(m => m.SRV_TIC.CATEGORY.ToLower().Contains(qFilter.Category.ToLower()));
                    else if (qFilter.Created != null)
                    {
                        DateTime date = qFilter.Created.Value;
                        result = result.Where(m => m.SRV_TIC.CREATED != null);
                        result = result.Where(m => m.SRV_TIC.CREATED.Value.Year == date.Year && m.SRV_TIC.CREATED.Value.Month == date.Month && m.SRV_TIC.CREATED.Value.Day == date.Day);
                    }
                    else if (qFilter.Status != null && qFilter.Status != 0)
                        result = result.Where(m => m.SRV_TIC.STATUS_CD.ToLower().Contains(((ServiceRequestStatus)qFilter.Status).ToString().ToLower()));
                    else if (qFilter.Priority != null && qFilter.Priority != 0)
                        result = result.Where(m => m.SRV_TIC.PRIORITY.ToLower().Contains(((Priority)qFilter.Priority).ToString().ToLower()));
                    else if (qFilter.Assignee != null && qFilter.Assignee != "")
                        result = result.Where(m => m.SRV_TIC.ASSIGNEE.ToLower().Contains(qFilter.Assignee.ToLower()));
                    else if (qFilter.AccountName != null && qFilter.AccountName != "")
                        result = result.Where(m => m.ACCNT.NAME.ToLower().Contains(qFilter.AccountName.ToLower()));
                    else if (qFilter.MSISDN != null && qFilter.MSISDN != "")
                        result = result.Where(m => m.ACCNT.MSISDN.ToLower().Contains(qFilter.MSISDN.ToLower()));
                }

                TotalCount = result.Count();

                //ordering && paging
                if (orderDirection == "asc")
                {
                    if (orderBy == "Category")
                        result = result.OrderBy(m => m.SRV_TIC.CATEGORY);
                    else if (orderBy == "CreatedDate")
                        result = result.OrderBy(m => m.SRV_TIC.CREATED);
                    else if (orderBy == "Status")
                        result = result.OrderBy(m => m.SRV_TIC.STATUS_CD);
                    else if (orderBy == "Priority")
                        result = result.OrderBy(m => m.SRV_TIC.PRIORITY);
                    else if (orderBy == "Assignee")
                        result = result.OrderBy(m => m.SRV_TIC.ASSIGNEE);
                }
                else
                {
                    if (orderBy == "Category")
                        result = result.OrderByDescending(m => m.SRV_TIC.CATEGORY);
                    else if (orderBy == "CreatedDate")
                        result = result.OrderByDescending(m => m.SRV_TIC.CREATED);
                    else if (orderBy == "Status")
                        result = result.OrderByDescending(m => m.SRV_TIC.STATUS_CD);
                    else if (orderBy == "Priority")
                        result = result.OrderByDescending(m => m.SRV_TIC.PRIORITY);
                    else if (orderBy == "Assignee")
                        result = result.OrderByDescending(m => m.SRV_TIC.ASSIGNEE);
                }

                if (length >= 0)
                    result = result.Skip(startIdx).Take(length);

                foreach (var p in result)
                {
                    ServiceRequestVO e = new ServiceRequestVO();
                    e.Assignee = p.SRV_TIC.ASSIGNEE;
                    e.Category = p.SRV_TIC.CATEGORY;
                    e.Description = p.SRV_TIC.CASE_DESC;
                    e.DueDate = (DateTime)p.SRV_TIC.CASE_DUE_DT;
                    e.Priority = EnumUtil.ParseEnumInt<Priority>(p.SRV_TIC.PRIORITY);
                    e.Resolution = EnumUtil.ParseEnumInt<ServiceRequestResolution>(p.SRV_TIC.RESOLUTION);
                    e.ServiceRequestId = p.SRV_TIC.ROW_ID;
                    e.Status = EnumUtil.ParseEnumInt<ServiceRequestStatus>(p.SRV_TIC.STATUS_CD);

                    e.MSISDN = p.ACCNT.MSISDN;
                    e.OldLimit = p.SRV_TIC.OLD_CREDIT_LIMIT;
                    e.NewLimit = p.SRV_TIC.NEW_CREDIT_LIMIT;
                    
                    e.SimMSISDN = p.SRV_TIC.NEW_CON_MSISDN;
                    e.NewSIMNumber = p.SRV_TIC.NEW_CON_NUM;
                    e.OldSIMNumber = p.SRV_TIC.OLD_CON_NUM;

                    e.AccountId = p.ACCNT.ROW_ID;
                    e.AccountName = p.ACCNT.NAME;

                    e.Created = p.SRV_TIC.CREATED;

                    List.Add(e);
                }
            }
            return List;
        }

        public ServiceRequestVO GetServiceRequest(long ServiceRequestId)
        {
            using (var dbContext = new CRMDbContext())
            {
                var result = from d in dbContext.XOX_T_SRV_TIC
                             join y in dbContext.XOX_T_ACCNT on d.CUST_ID equals y.ROW_ID
                             where d.ROW_ID == ServiceRequestId
                             select new { SRV_TIC = d, ACCNT = y };

                var resul = result.Count();

                if (result.Count() > 0)
                {
                    var p = result.First();

                    ServiceRequestVO e = new ServiceRequestVO();
                    e.Assignee = p.SRV_TIC.ASSIGNEE;
                    e.Category = p.SRV_TIC.CATEGORY;
                    e.Description = p.SRV_TIC.CASE_DESC;
                    e.DueDate = (DateTime)p.SRV_TIC.CASE_DUE_DT;
                    e.Priority = EnumUtil.ParseEnumInt<Priority>(p.SRV_TIC.PRIORITY);
                    e.Resolution = EnumUtil.ParseEnumInt<ServiceRequestResolution>(p.SRV_TIC.RESOLUTION);
                    e.ServiceRequestId = p.SRV_TIC.ROW_ID;
                    e.Status = EnumUtil.ParseEnumInt<ServiceRequestStatus>(p.SRV_TIC.STATUS_CD);

                    e.MSISDN = p.ACCNT.MSISDN;
                    e.OldLimit = p.SRV_TIC.OLD_CREDIT_LIMIT;
                    e.NewLimit = p.SRV_TIC.NEW_CREDIT_LIMIT;

                    e.SimMSISDN = p.SRV_TIC.NEW_CON_MSISDN;
                    e.NewSIMNumber = p.SRV_TIC.NEW_CON_NUM;
                    e.OldSIMNumber = p.SRV_TIC.OLD_CON_NUM;

                    e.ServiceRequestNumber = "PPSR" + p.SRV_TIC.SR_NUM.ToString().PadLeft(8, '0');

                    e.AccountId = p.SRV_TIC.CUST_ID;

                    e.OldProfile = p.SRV_TIC.OLD_ACCNT_PROFILE;
                    e.NewProfie = p.SRV_TIC.NEW_ACCNT_PROFILE;

                    e.NewItemisedBilling = p.SRV_TIC.NEW_BILL_TYPE == "Yes" ? true : false;

                    return e;
                }
            }

            return null;
        }

        public List<ServiceRequestAttachmentVO> GetAllServiceRequestAttachments(long ServiceRequestId)
        {
            List<ServiceRequestAttachmentVO> List = new List<ServiceRequestAttachmentVO>();
            using (var dbContext = new CRMDbContext())
            {
                var result = from d in dbContext.XOX_T_SRV_ATT
                             where d.SRV_ID == ServiceRequestId
                             && d.LAST_UPD_BY == null ///
                             select d;

                foreach (var p in result)
                {
                    ServiceRequestAttachmentVO e = new ServiceRequestAttachmentVO();
                    e.AttachmentId = p.ROW_ID;
                    e.Path = p.FILE_PATH_NAME;

                    List.Add(e);
                }
            }
            return List;
        }

        public List<ServiceRequestNoteVO> GetAllServiceRequestNote(long ServiceRequestId)
        {
            List<ServiceRequestNoteVO> List = new List<ServiceRequestNoteVO>();
            using (var dbContext = new CRMDbContext())
            {
                var result = from d in dbContext.XOX_T_SRV_NOTE
                             where d.SRV_ID == ServiceRequestId
                             && d.LAST_UPD_BY == null ///
                             select d;

                foreach (var p in result)
                {
                    ServiceRequestNoteVO e = new ServiceRequestNoteVO();
                    e.NoteId = p.ROW_ID;
                    e.Note = p.NOTE;

                    List.Add(e);
                }
            }
            return List;
        }

        public List<ServiceRequestActivityVO> GetAllServiceRequestActivity(long ServiceRequestId)
        {
            List<ServiceRequestActivityVO> List = new List<ServiceRequestActivityVO>();
            using (var dbContext = new CRMDbContext())
            {
                var result = from d in dbContext.XOX_T_SRV_ACT
                             where d.SRV_ID == ServiceRequestId
                             select d;

                foreach (var p in result)
                {
                    ServiceRequestActivityVO e = new ServiceRequestActivityVO();
                    e.ActivityId = p.ROW_ID;
                    e.FieldStaff = p.FIELD_STAFF;
                    e.Notes = p.NOTES;
                    e.Status = p.STATUS_CD;
                    e.VisitDateTime = (DateTime)p.VISIT_DT;

                    List.Add(e);
                }
            }
            return List;
        }

        public long Add(ServiceRequestVO vo, long AccountId)
        {
            long UserId = 0;
            try
            {
                UserId = long.Parse(Thread.CurrentPrincipal.Identity.Name);
            }
            catch { }

            using (var DBContext = new CRMDbContext())
            {
                var Account = (from d in DBContext.XOX_T_ACCNT
                              where d.ROW_ID == AccountId
                              select d).First();

                XOX_T_SRV_TIC v = new XOX_T_SRV_TIC();

                v.CUST_ID = AccountId;

                v.ASSIGNEE = vo.Assignee;
                v.CATEGORY = vo.Category;
                v.CASE_DESC = vo.Description;
                v.CASE_DUE_DT = vo.DueDate;
                v.PRIORITY = ((Priority)vo.Priority).ToString();
                v.RESOLUTION = ((ServiceRequestResolution)vo.Resolution).ToString();
                v.STATUS_CD = ((ServiceRequestStatus)vo.Status).ToString();

                v.NEW_CREDIT_LIMIT = 0;

                v.NEW_CON_MSISDN = "";
                v.NEW_CON_NUM = "";
                v.OLD_CON_NUM = Account.SIM_SERIAL_NUMBER;

                v.CREATED = DateTime.Now;
                v.CREATED_BY = UserId;

                v.SR_NUM = 0;

                DBContext.XOX_T_SRV_TIC.Add(v);
                DBContext.SaveChanges();
                
                v.SR_NUM = v.ROW_ID;
                DBContext.SaveChanges();
                
                //add activity
                XOX_T_SRV_ACT act = new XOX_T_SRV_ACT();
                act.CREATED = DateTime.Now;
                act.CREATED_BY = UserId;
                act.SRV_ID = v.ROW_ID;
                act.NOTES = "Service Request " + "PPSR" + v.SR_NUM.ToString().PadLeft(8, '0') + " created";
                act.FIELD_STAFF = UserService.GetName(UserId);
                act.STATUS_CD = "-";
                act.VISIT_DT = DateTime.Now;

                DBContext.XOX_T_SRV_ACT.Add(act);
                DBContext.SaveChanges();

                return v.ROW_ID;
            }
        }

        public void SubmitCreditLimitRequest(ServiceRequestVO vo, long AccountId)
        {
            var UserId = long.Parse(Thread.CurrentPrincipal.Identity.Name);

            using (var DBContext = new CRMDbContext())
            {
                //process to mpp
                var MSISDN = vo.MSISDN[0] == '6' ? vo.MSISDN : ('6' + vo.MSISDN);
                var result = EAIService.CreditLimitUpdate(new CreditLimitUpdate()
                {
                    CreditLimit = vo.NewLimit.ToString(),
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

                if (response.ToLower() == "true")
                {
                    decimal OldLimit = 0;
                    var Account = (from d in DBContext.XOX_T_ACCNT
                                  where d.ROW_ID == AccountId
                                  select d).First();

                    //if successfully done, update credit limit
                    OldLimit = Account.CREDIT_LIMIT ?? 0;
                    Account.CREDIT_LIMIT = vo.NewLimit;
                    DBContext.SaveChanges();

                    var ServiceRequest = (from d in DBContext.XOX_T_SRV_TIC
                                         where d.ROW_ID == vo.ServiceRequestId
                                         select d).First();

                    ServiceRequest.OLD_CREDIT_LIMIT = OldLimit;
                    ServiceRequest.NEW_CREDIT_LIMIT = vo.NewLimit;
                    DBContext.SaveChanges();

                    //add success activity
                    XOX_T_SRV_ACT act2 = new XOX_T_SRV_ACT();
                    act2.CREATED = DateTime.Now;
                    act2.CREATED_BY = UserId;
                    act2.SRV_ID = ServiceRequest.ROW_ID;
                    act2.NOTES = "Submitted to external service and returned response " + response;
                    act2.FIELD_STAFF = UserService.GetName(UserId);
                    act2.STATUS_CD = "-";
                    act2.VISIT_DT = DateTime.Now;
                    DBContext.XOX_T_SRV_ACT.Add(act2);
                    DBContext.SaveChanges();
                    

                    //change status and resolution
                    ServiceRequest.RESOLUTION = (ServiceRequestResolution.Closed).ToString();
                    ServiceRequest.STATUS_CD = (ServiceRequestStatus.Closed).ToString();
                    DBContext.SaveChanges();

                    //add status closed activity
                    XOX_T_SRV_ACT act3 = new XOX_T_SRV_ACT();
                    act3.CREATED = DateTime.Now;
                    act3.CREATED_BY = UserId;
                    act3.SRV_ID = ServiceRequest.ROW_ID;
                    act3.NOTES = "Service Request " + "PPSR" + ServiceRequest.SR_NUM.ToString().PadLeft(8, '0') + " closed";
                    act3.FIELD_STAFF = UserService.GetName(UserId);
                    act3.STATUS_CD = "-";
                    act3.VISIT_DT = DateTime.Now;
                    DBContext.XOX_T_SRV_ACT.Add(act3);
                    DBContext.SaveChanges();
                }
                else
                {
                    try
                    {
                        response = r["Result"].ToString();

                        //add status closed activity
                        var ServiceRequest = (from d in DBContext.XOX_T_SRV_TIC
                                              where d.ROW_ID == vo.ServiceRequestId
                                              select d).First();
                        XOX_T_SRV_ACT act3 = new XOX_T_SRV_ACT();
                        act3.CREATED = DateTime.Now;
                        act3.CREATED_BY = UserId;
                        act3.SRV_ID = ServiceRequest.ROW_ID;
                        act3.NOTES = "Failed : " + response;
                        act3.FIELD_STAFF = UserService.GetName(UserId);
                        act3.STATUS_CD = "-";
                        act3.VISIT_DT = DateTime.Now;
                        DBContext.XOX_T_SRV_ACT.Add(act3);
                        DBContext.SaveChanges();

                        throw new Exception(response);
                    }
                    catch
                    {
                        throw new Exception(result);
                    }
                }
            }
        }

        public void SubmitSIMNumberRequest(ServiceRequestVO vo, long AccountId)
        {
            var UserId = long.Parse(Thread.CurrentPrincipal.Identity.Name);

            using (var DBContext = new CRMDbContext())
            {
                /*
                //check sim number range
                var iNewSIMNumber = int.Parse(vo.NewSIMNumber);

                var FromRange = 123456789; ////
                var ToRange = 987654321; ////

                if (iNewSIMNumber < FromRange || iNewSIMNumber > ToRange)
                {
                    throw new Exception("The New SIM Number is out of range. Please check again.");
                }
                */

                //process to mpp
                var MSISDN = vo.MSISDN[0] == '6' ? vo.MSISDN : ('6' + vo.MSISDN);
                var result = EAIService.SimNumberUpdate(new SimNumberUpdate()
                {
                    MSISDN = MSISDN,
                    SimNumber = vo.NewSIMNumber,
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

                if (response.ToLower() == "true")
                {
                    var Account = (from d in DBContext.XOX_T_ACCNT
                                   where d.ROW_ID == AccountId
                                   select d).First();

                    //if successfully done, update sim number
                    Account.SIM_SERIAL_NUMBER = vo.NewSIMNumber;
                    DBContext.SaveChanges();

                    var ServiceRequest = (from d in DBContext.XOX_T_SRV_TIC
                                          where d.ROW_ID == vo.ServiceRequestId
                                          select d).First();

                    ServiceRequest.OLD_CON_NUM = vo.OldSIMNumber;
                    ServiceRequest.NEW_CON_NUM = vo.NewSIMNumber;
                    DBContext.SaveChanges();

                    //crp syncing
                    var ErrMsg = "";
                    try
                    {
                        if (Account.INTEGRATION_ID != null && Account.INTEGRATION_ID != "")
                        {
                            var isSupplementary = false;
                            if (Account.ACCNT_TYPE_CD == ((int)AccountType.SupplementaryLine).ToString())
                            {
                                isSupplementary = true;
                            }
                            var AccountVO = AccountService.Get(AccountId);
                            var EAIResult = EAIService.EditAccount(new AccountEdit()
                            {
                                Data = new AccountInfo()
                                {
                                    PersonalInfo = new PersonalInfoEAI()
                                    {
                                        BirthDate = AccountVO.PersonalInfo.BirthDate.ToString("dd-MM-yyyy"),
                                        ContactNumber = AccountVO.PersonalInfo.ContactNumber,
                                        CreatedDate = AccountVO.PersonalInfo.CreatedDate,
                                        CreditLimit = AccountVO.PersonalInfo.CreditLimit,
                                        CustomerAccountNumber = AccountVO.PersonalInfo.CustomerAccountNumber,
                                        CustomerStatus = AccountVO.PersonalInfo.CustomerStatus,
                                        Email = AccountVO.PersonalInfo.Email,
                                        FullName = AccountVO.PersonalInfo.FullName,
                                        Gender = AccountVO.PersonalInfo.Gender,
                                        IdentityNo = AccountVO.PersonalInfo.IdentityNo,
                                        IdentityType = AccountVO.PersonalInfo.IdentityType,
                                        MotherMaidenName = AccountVO.PersonalInfo.MotherMaidenName,
                                        MSISDNNumber = Account.MSISDN,
                                        Nationality = AccountVO.PersonalInfo.Nationality,
                                        PreferredLanguage = AccountVO.PersonalInfo.PreferredLanguage,
                                        Race = AccountVO.PersonalInfo.Race,
                                        Salutation = Account.SALUTATION,
                                        SponsorPersonnel = AccountVO.PersonalInfo.SponsorPersonnel,
                                        SimSerialNumber = AccountVO.SIMSerialNumber
                                    },
                                    BankingInfo = AccountVO.BankingInfo,
                                    AddressInfo = AccountVO.AddressInfo,
                                    BillingAddressInfo = AccountVO.BillingAddressInfo,
                                    IntegrationId = long.Parse(Account.INTEGRATION_ID),
                                    SIMSerialNumber = AccountVO.SIMSerialNumber,
                                    FlgReceivedItemised = 0,
                                    User = new User()
                                    {
                                        Source = "CRM",
                                        UserId = int.Parse(Account.INTEGRATION_ID)
                                    }
                                },
                                isSupplementary = isSupplementary
                            });

                            try
                            {
                                bool.Parse(EAIResult);
                                //if (bool.Parse(EAIResult) == true)
                                //{
                                //    ErrMsg = "; " + "Failed syncing to CRP.";
                                //}
                            }
                            catch
                            {
                                ErrMsg = "; " + EAIResult;
                            }
                        }
                        else
                        {
                            ErrMsg = "; " + "Successfully edited. But failed syncing to CRP due to missing INTEGRATION_ID.";
                        }
                    }
                    catch (Exception e)
                    {
                        ErrMsg = "; " + e.Message;
                    }

                    //add success activity
                    XOX_T_SRV_ACT act2 = new XOX_T_SRV_ACT();
                    act2.CREATED = DateTime.Now;
                    act2.CREATED_BY = UserId;
                    act2.SRV_ID = ServiceRequest.ROW_ID;
                    act2.NOTES = "Submitted to external service and returned response " + response + ErrMsg;
                    act2.FIELD_STAFF = UserService.GetName(UserId);
                    act2.STATUS_CD = "-";
                    act2.VISIT_DT = DateTime.Now;
                    DBContext.XOX_T_SRV_ACT.Add(act2);
                    DBContext.SaveChanges();


                    //change status and resolution
                    ServiceRequest.RESOLUTION = (ServiceRequestResolution.Closed).ToString();
                    ServiceRequest.STATUS_CD = (ServiceRequestStatus.Closed).ToString();
                    DBContext.SaveChanges();

                    //add status closed activity
                    XOX_T_SRV_ACT act3 = new XOX_T_SRV_ACT();
                    act3.CREATED = DateTime.Now;
                    act3.CREATED_BY = UserId;
                    act3.SRV_ID = ServiceRequest.ROW_ID;
                    act3.NOTES = "Service Request " + "PPSR" + ServiceRequest.SR_NUM.ToString().PadLeft(8, '0') + " closed";
                    act3.FIELD_STAFF = UserService.GetName(UserId);
                    act3.STATUS_CD = "-";
                    act3.VISIT_DT = DateTime.Now;
                    DBContext.XOX_T_SRV_ACT.Add(act3);
                    DBContext.SaveChanges();
                }
                else
                {
                    try
                    {
                        response = r["Result"].ToString();

                        //add status closed activity
                        var ServiceRequest = (from d in DBContext.XOX_T_SRV_TIC
                                              where d.ROW_ID == vo.ServiceRequestId
                                              select d).First();
                        XOX_T_SRV_ACT act3 = new XOX_T_SRV_ACT();
                        act3.CREATED = DateTime.Now;
                        act3.CREATED_BY = UserId;
                        act3.SRV_ID = ServiceRequest.ROW_ID;
                        act3.NOTES = "Failed : " + response;
                        act3.FIELD_STAFF = UserService.GetName(UserId);
                        act3.STATUS_CD = "-";
                        act3.VISIT_DT = DateTime.Now;
                        DBContext.XOX_T_SRV_ACT.Add(act3);
                        DBContext.SaveChanges();

                        throw new Exception(response);
                    }
                    catch
                    {
                        throw new Exception(result);
                    }
                }
            }
        }

        public void AddAttachment(String Path, long ServiceRequestId)
        {
            var UserId = long.Parse(Thread.CurrentPrincipal.Identity.Name);

            using (var DBContext = new CRMDbContext())
            {
                var result = from d in DBContext.XOX_T_SRV_TIC
                             where d.ROW_ID == ServiceRequestId
                             select d;

                if (result.Count() > 0)
                {
                    XOX_T_SRV_ATT n = new XOX_T_SRV_ATT();
                    n.SRV_ID = ServiceRequestId;
                    n.FILE_PATH_NAME = Path;
                    n.CREATED = DateTime.Now;
                    n.CREATED_BY = UserId;

                    DBContext.XOX_T_SRV_ATT.Add(n);
                    DBContext.SaveChanges();
                }
            }
        }

        public void AddNote(String Note, long ServiceRequestId)
        {
            var UserId = long.Parse(Thread.CurrentPrincipal.Identity.Name);

            using (var DBContext = new CRMDbContext())
            {
                var result = from d in DBContext.XOX_T_SRV_TIC
                             where d.ROW_ID == ServiceRequestId
                             select d;

                if (result.Count() > 0)
                {
                    XOX_T_SRV_NOTE n = new XOX_T_SRV_NOTE();
                    n.SRV_ID = ServiceRequestId;
                    n.NOTE = Note;
                    n.CREATED = DateTime.Now;
                    n.CREATED_BY = UserId;

                    DBContext.XOX_T_SRV_NOTE.Add(n);
                    DBContext.SaveChanges();
                }
            }
        }

        public void AddActivity(ServiceRequestActivityVO Act, long ServiceRequestId)
        {
            var UserId = long.Parse(Thread.CurrentPrincipal.Identity.Name);

            using (var DBContext = new CRMDbContext())
            {
                var result = from d in DBContext.XOX_T_SRV_TIC
                             where d.ROW_ID == ServiceRequestId
                             select d;

                if (result.Count() > 0)
                {
                    XOX_T_SRV_ACT n = new XOX_T_SRV_ACT();
                    n.SRV_ID = ServiceRequestId;
                    n.FIELD_STAFF = Act.FieldStaff;
                    n.NOTES = Act.Notes;
                    n.STATUS_CD = Act.Status;
                    n.VISIT_DT = Act.VisitDateTime;
                    n.CREATED = DateTime.Now;
                    n.CREATED_BY = UserId;

                    DBContext.XOX_T_SRV_ACT.Add(n);
                    DBContext.SaveChanges();
                }
            }
        }

        public bool Edit(ServiceRequestVO vo)
        {
            var UserId = long.Parse(Thread.CurrentPrincipal.Identity.Name);
            using (var DBContext = new CRMDbContext())
            {
                var result = from d in DBContext.XOX_T_SRV_TIC
                             where d.ROW_ID == vo.ServiceRequestId
                             select d;

                if (result.Count() > 0)
                {
                    var v = result.First();
                    
                    v.ASSIGNEE = vo.Assignee;
                    v.CATEGORY = vo.Category;
                    v.CASE_DESC = vo.Description;
                    v.CASE_DUE_DT = vo.DueDate;
                    v.PRIORITY = ((Priority)vo.Priority).ToString();
                    v.RESOLUTION = ((ServiceRequestResolution)vo.Resolution).ToString();
                    v.STATUS_CD = ((ServiceRequestStatus)vo.Status).ToString();

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

        public bool RemoveAttachment(long AttachmentId)
        {
            long UserId = long.Parse(Thread.CurrentPrincipal.Identity.Name);
            using (var DBContext = new CRMDbContext())
            {
                var att = from d in DBContext.XOX_T_SRV_ATT
                          where d.ROW_ID == AttachmentId
                          select d;

                if (att.Count() > 0)
                {
                    att.First().LAST_UPD = DateTime.Now;
                    att.First().LAST_UPD_BY = UserId;

                    DBContext.SaveChanges();
                }

                return true;
            }
        }

        public bool RemoveNote(long NoteId)
        {
            long UserId = long.Parse(Thread.CurrentPrincipal.Identity.Name);
            using (var DBContext = new CRMDbContext())
            {
                var att = from d in DBContext.XOX_T_SRV_NOTE
                          where d.ROW_ID == NoteId
                          select d;

                if (att.Count() > 0)
                {
                    att.First().LAST_UPD = DateTime.Now;
                    att.First().LAST_UPD_BY = UserId;

                    DBContext.SaveChanges();
                }

                return true;
            }
        }

        public void SubmitUpdateSubcriberProfile(ServiceRequestVO vo, long AccountId, int UserId = 0)
        {
            if (UserId == 0)
            {
                UserId = int.Parse(Thread.CurrentPrincipal.Identity.Name);
            }

            using (var DBContext = new CRMDbContext())
            {
                var Account = (from d in DBContext.XOX_T_ACCNT
                               where d.ROW_ID == AccountId
                               select d).FirstOrDefault();

                if (Account == null)
                {
                    throw new Exception("Account not found");
                }
                else if (Account.INTEGRATION_ID == null)
                {
                    throw new Exception("Integration ID not found");
                }                

                //process to mpp
                var MSISDN = Account.MSISDN;
                MSISDN = MSISDN[0] == '6' ? MSISDN : ('6' + MSISDN);
                var result = EAIService.UpdateSubscriberProfile(new UpdateSubscriberProfile()
                {
                    MSISDN = MSISDN,
                    SubscriberProfile = vo.SubscriberProfile,
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
                
                //if successfully done
                if (response.ToLower() == "true")
                {
                    //save before info
                    var OldSubscriberProfile = new SubscriberProfile();
                    OldSubscriberProfile.birthDate = Account.BIRTH_DT;
                    OldSubscriberProfile.emailAddress = Account.EMAIL_ADDR;
                    OldSubscriberProfile.ic = Account.ID_NUM;
                    OldSubscriberProfile.name = Account.NAME;
                    OldSubscriberProfile.preferredLanguage = Account.PREFERRED_LANG;
                    OldSubscriberProfile.salutation = Account.SALUTATION;

                    //update info
                    Account.BIRTH_DT = vo.SubscriberProfile.birthDate;
                    Account.EMAIL_ADDR = vo.SubscriberProfile.emailAddress;
                    Account.ID_NUM = vo.SubscriberProfile.ic;
                    Account.NAME = vo.SubscriberProfile.name;
                    Account.PREFERRED_LANG = vo.SubscriberProfile.preferredLanguage;
                    Account.SALUTATION = vo.SubscriberProfile.salutation;
                    //Account.BUSINESS_TYPE = vo.SubscriberProfile.businessRegistrationNo;
                    //Account.MSISDN = vo.SubscriberProfile.iMSI;
                    //Account.monthly = vo.SubscriberProfile.monthlyStatement;
                    //Account.organization = vo.SubscriberProfile.organizationName;
                    DBContext.SaveChanges();

                    var Address = (from d in DBContext.XOX_T_ADDR
                                   where d.ROW_ID == Account.ADDR_ID
                                   select d).FirstOrDefault();
                    if (Address != null)
                    {
                        OldSubscriberProfile.city = Address.CITY;
                        OldSubscriberProfile.postalAddress = Address.ADDR_1;
                        OldSubscriberProfile.postalAddressL2 = Address.ADDR_2;
                        OldSubscriberProfile.postalCode = Address.POSTAL_CD;
                        OldSubscriberProfile.state = Address.STATE;

                        Address.CITY = vo.SubscriberProfile.city;
                        Address.ADDR_1 = vo.SubscriberProfile.postalAddress;
                        Address.ADDR_2 = vo.SubscriberProfile.postalAddressL2;
                        Address.POSTAL_CD = vo.SubscriberProfile.postalCode;
                        Address.STATE = vo.SubscriberProfile.state;
                        DBContext.SaveChanges();
                    }

                    var ServiceRequest = (from d in DBContext.XOX_T_SRV_TIC
                                          where d.ROW_ID == vo.ServiceRequestId
                                          select d).First();

                    ServiceRequest.OLD_ACCNT_PROFILE = new JavaScriptSerializer().Serialize(OldSubscriberProfile);
                    ServiceRequest.NEW_ACCNT_PROFILE = new JavaScriptSerializer().Serialize(vo.SubscriberProfile);
                    DBContext.SaveChanges();

                    //crp syncing
                    var ErrMsg = "";
                    try
                    {
                        if (Account.INTEGRATION_ID != null && Account.INTEGRATION_ID != "")
                        {
                            var isSupplementary = false;
                            if (Account.ACCNT_TYPE_CD == ((int)AccountType.SupplementaryLine).ToString())
                            {
                                isSupplementary = true;
                            }
                            var AccountVO = AccountService.Get(AccountId);
                            var EAIResult = EAIService.EditAccount(new AccountEdit()
                            {
                                Data = new AccountInfo()
                                {
                                    PersonalInfo = new PersonalInfoEAI()
                                    {
                                        BirthDate = AccountVO.PersonalInfo.BirthDate.ToString("dd-MM-yyyy"),
                                        ContactNumber = AccountVO.PersonalInfo.ContactNumber,
                                        CreatedDate = AccountVO.PersonalInfo.CreatedDate,
                                        CreditLimit = AccountVO.PersonalInfo.CreditLimit,
                                        CustomerAccountNumber = AccountVO.PersonalInfo.CustomerAccountNumber,
                                        CustomerStatus = AccountVO.PersonalInfo.CustomerStatus,
                                        Email = AccountVO.PersonalInfo.Email,
                                        FullName = AccountVO.PersonalInfo.FullName,
                                        Gender = AccountVO.PersonalInfo.Gender,
                                        IdentityNo = AccountVO.PersonalInfo.IdentityNo,
                                        IdentityType = AccountVO.PersonalInfo.IdentityType,
                                        MotherMaidenName = AccountVO.PersonalInfo.MotherMaidenName,
                                        MSISDNNumber = Account.MSISDN,
                                        Nationality = AccountVO.PersonalInfo.Nationality,
                                        PreferredLanguage = AccountVO.PersonalInfo.PreferredLanguage,
                                        Race = AccountVO.PersonalInfo.Race,
                                        Salutation = Account.SALUTATION,
                                        SponsorPersonnel = AccountVO.PersonalInfo.SponsorPersonnel,
                                        SimSerialNumber = AccountVO.SIMSerialNumber
                                    },
                                    BankingInfo = AccountVO.BankingInfo,
                                    AddressInfo = AccountVO.AddressInfo,
                                    BillingAddressInfo = AccountVO.BillingAddressInfo,
                                    IntegrationId = long.Parse(Account.INTEGRATION_ID),
                                    SIMSerialNumber = AccountVO.SIMSerialNumber,
                                    FlgReceivedItemised = 0,
                                    User = new User()
                                    {
                                        Source = "CRM",
                                        UserId = int.Parse(Account.INTEGRATION_ID)
                                    }
                                },
                                isSupplementary = isSupplementary
                            });

                            try
                            {
                                bool.Parse(EAIResult);
                            }
                            catch
                            {
                                ErrMsg = "; " + EAIResult;
                            }
                        }
                        else
                        {
                            ErrMsg = "; " + "Successfully edited. But failed syncing to CRP due to missing INTEGRATION_ID.";
                        }
                    }
                    catch (Exception e)
                    {
                        ErrMsg = "; " + e.Message;
                    }

                    //add success activity
                    XOX_T_SRV_ACT act2 = new XOX_T_SRV_ACT();
                    act2.CREATED = DateTime.Now;
                    act2.CREATED_BY = UserId;
                    act2.SRV_ID = ServiceRequest.ROW_ID;
                    act2.NOTES = "Submitted to external service and returned response " + response + ErrMsg;
                    act2.FIELD_STAFF = UserService.GetName(UserId);
                    act2.STATUS_CD = "-";
                    act2.VISIT_DT = DateTime.Now;
                    DBContext.XOX_T_SRV_ACT.Add(act2);
                    DBContext.SaveChanges();


                    //change status and resolution
                    ServiceRequest.RESOLUTION = (ServiceRequestResolution.Closed).ToString();
                    ServiceRequest.STATUS_CD = (ServiceRequestStatus.Closed).ToString();
                    DBContext.SaveChanges();

                    //add status closed activity
                    XOX_T_SRV_ACT act3 = new XOX_T_SRV_ACT();
                    act3.CREATED = DateTime.Now;
                    act3.CREATED_BY = UserId;
                    act3.SRV_ID = ServiceRequest.ROW_ID;
                    act3.NOTES = "Service Request " + "PPSR" + ServiceRequest.SR_NUM.ToString().PadLeft(8, '0') + " closed";
                    act3.FIELD_STAFF = UserService.GetName(UserId);
                    act3.STATUS_CD = "-";
                    act3.VISIT_DT = DateTime.Now;
                    DBContext.XOX_T_SRV_ACT.Add(act3);
                    DBContext.SaveChanges();
                }
                else
                {
                    try
                    {
                        response = r["Result"].ToString();

                        //add status closed activity
                        var ServiceRequest = (from d in DBContext.XOX_T_SRV_TIC
                                              where d.ROW_ID == vo.ServiceRequestId
                                              select d).First();
                        XOX_T_SRV_ACT act3 = new XOX_T_SRV_ACT();
                        act3.CREATED = DateTime.Now;
                        act3.CREATED_BY = UserId;
                        act3.SRV_ID = ServiceRequest.ROW_ID;
                        act3.NOTES = "Failed : " + response;
                        act3.FIELD_STAFF = UserService.GetName(UserId);
                        act3.STATUS_CD = "-";
                        act3.VISIT_DT = DateTime.Now;
                        DBContext.XOX_T_SRV_ACT.Add(act3);
                        DBContext.SaveChanges();

                        throw new Exception(response);
                    }
                    catch
                    {
                        throw new Exception(result);
                    }
                }
            }
        }

        public void CancelRequest(ServiceRequestVO vo)
        {
            var UserId = long.Parse(Thread.CurrentPrincipal.Identity.Name);

            using (var DBContext = new CRMDbContext())
            {
                var ServiceRequest = (from d in DBContext.XOX_T_SRV_TIC
                                      where d.ROW_ID == vo.ServiceRequestId
                                      select d).First();

                ServiceRequest.STATUS_CD = ((ServiceRequestStatus)vo.Status).ToString();
                DBContext.SaveChanges();
            }
        }
        public void SubmitManageItemisedBillingRequest(ServiceRequestVO vo, long AccountId)
        {
            var UserId = long.Parse(Thread.CurrentPrincipal.Identity.Name);

            using (var DBContext = new CRMDbContext())
            {
                //process to mpp
                var MSISDN = vo.MSISDN[0] == '6' ? vo.MSISDN : ('6' + vo.MSISDN);
                var result = EAIService.ItemisedBillingUpdate(new ItemisedBillingUpdate()
                {
                    MSISDN = MSISDN,
                    ItemisedBilling = vo.NewItemisedBilling,
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

                if (response.ToLower() == "true")
                {
                    //change itemised billing of asset
                    var Assets = from g in DBContext.XOX_T_ASSET
                                 join h in DBContext.XOX_T_PROD_ITEM on g.PROD_ID equals h.ROW_ID
                                 where h.PROD_ID == 30 //30 = PrintedBilling PROD_ID
                                 && g.CUST_ID == AccountId
                                 select g;

                    if (Assets.Count() > 0)
                    {
                        //if got printed billing (itemised) asset, just edit the status
                        if (vo.NewItemisedBilling == true)
                        {
                            var Asset = Assets.First();
                            Asset.STATUS_CD = XOXConstants.STATUS_ACTIVE_CD;
                            DBContext.SaveChanges();
                        }
                        else
                        {
                            var Asset = Assets.First();
                            Asset.STATUS_CD = XOXConstants.STATUS_INACTIVE_CD;
                            DBContext.SaveChanges();
                        }
                    }
                    else
                    {
                        //if got no printed billing (itemised) asset, add new asset
                        var SubscriberProfile = AccountService.GetSubscriberProfile(AccountId);
                        var additionalInformation = SubscriberProfile.additionalInformation.ToDictionary(v => v.name, v => v.value);

                        var ProductItems = ProductService.GetProductItemByPlan(additionalInformation["planInfo"]);

                        var selectedProducts = ProductItems.Where(d => d.EXT_PROD_NAME == XOXConstants.PRODUCT_PRINTED_BILLING).First();
                        OrderItemVO orderItem = new OrderItemVO();
                        orderItem.CUST_ID = AccountId;
                        orderItem.QTY = 1;
                        if (vo.NewItemisedBilling == true)
                        {
                            orderItem.STATUS_CD = XOXConstants.STATUS_ACTIVE_CD;
                        }
                        else
                        {
                            orderItem.STATUS_CD = XOXConstants.STATUS_INACTIVE_CD;
                        }
                        orderItem.INSTALL_DT = DateTime.Parse(SubscriberProfile.Customer.effective);
                        orderItem.SERVICE_NUM = MSISDN;
                        orderItem.PROD_ID = selectedProducts.ROW_ID;

                        AssetService.AddAsset(orderItem);
                    }

                    //change itemised billing of order item
                    //


                    //update service request
                    var ServiceRequest = (from d in DBContext.XOX_T_SRV_TIC
                                          where d.ROW_ID == vo.ServiceRequestId
                                          select d).First();

                    ServiceRequest.NEW_BILL_TYPE = vo.NewItemisedBilling == true ? "Yes" : "No";
                    DBContext.SaveChanges();

                    //crp syncing
                    var Account = (from d in DBContext.XOX_T_ACCNT
                                   where d.ROW_ID == AccountId
                                   select d).First();

                    var ErrMsg = "";
                    try
                    {
                        if (Account.INTEGRATION_ID != null && Account.INTEGRATION_ID != "")
                        {
                            var isSupplementary = false;
                            if (Account.ACCNT_TYPE_CD == ((int)AccountType.SupplementaryLine).ToString())
                            {
                                isSupplementary = true;
                            }
                            var AccountVO = AccountService.Get(AccountId);
                            var EAIResult = EAIService.EditAccount(new AccountEdit()
                            {
                                Data = new AccountInfo()
                                {
                                    PersonalInfo = new PersonalInfoEAI()
                                    {
                                        BirthDate = AccountVO.PersonalInfo.BirthDate.ToString("dd-MM-yyyy"),
                                        ContactNumber = AccountVO.PersonalInfo.ContactNumber,
                                        CreatedDate = AccountVO.PersonalInfo.CreatedDate,
                                        CreditLimit = AccountVO.PersonalInfo.CreditLimit,
                                        CustomerAccountNumber = AccountVO.PersonalInfo.CustomerAccountNumber,
                                        CustomerStatus = AccountVO.PersonalInfo.CustomerStatus,
                                        Email = AccountVO.PersonalInfo.Email,
                                        FullName = AccountVO.PersonalInfo.FullName,
                                        Gender = AccountVO.PersonalInfo.Gender,
                                        IdentityNo = AccountVO.PersonalInfo.IdentityNo,
                                        IdentityType = AccountVO.PersonalInfo.IdentityType,
                                        MotherMaidenName = AccountVO.PersonalInfo.MotherMaidenName,
                                        MSISDNNumber = Account.MSISDN,
                                        Nationality = AccountVO.PersonalInfo.Nationality,
                                        PreferredLanguage = AccountVO.PersonalInfo.PreferredLanguage,
                                        Race = AccountVO.PersonalInfo.Race,
                                        Salutation = Account.SALUTATION,
                                        SponsorPersonnel = AccountVO.PersonalInfo.SponsorPersonnel,
                                        SimSerialNumber = AccountVO.SIMSerialNumber
                                    },
                                    BankingInfo = AccountVO.BankingInfo,
                                    AddressInfo = AccountVO.AddressInfo,
                                    BillingAddressInfo = AccountVO.BillingAddressInfo,
                                    IntegrationId = long.Parse(Account.INTEGRATION_ID),
                                    SIMSerialNumber = AccountVO.SIMSerialNumber,
                                    FlgReceivedItemised = vo.NewItemisedBilling == true ? 2 : 1,
                                    User = new User()
                                    {
                                        Source = "CRM",
                                        UserId = int.Parse(Account.INTEGRATION_ID)
                                    }
                                },
                                isSupplementary = isSupplementary
                            });

                            try
                            {
                                bool.Parse(EAIResult);
                                //if (bool.Parse(EAIResult) == true)
                                //{
                                //    ErrMsg = "; " + "Failed syncing to CRP.";
                                //}
                            }
                            catch
                            {
                                ErrMsg = "; " + EAIResult;
                            }
                        }
                        else
                        {
                            ErrMsg = "; " + "Successfully edited. But failed syncing to CRP due to missing INTEGRATION_ID.";
                        }
                    }
                    catch (Exception e)
                    {
                        ErrMsg = "; " + e.Message;
                    }

                    //add success activity
                    XOX_T_SRV_ACT act2 = new XOX_T_SRV_ACT();
                    act2.CREATED = DateTime.Now;
                    act2.CREATED_BY = UserId;
                    act2.SRV_ID = ServiceRequest.ROW_ID;
                    act2.NOTES = "Submitted to external service and returned response " + response + ErrMsg;
                    act2.FIELD_STAFF = UserService.GetName(UserId);
                    act2.STATUS_CD = "-";
                    act2.VISIT_DT = DateTime.Now;
                    DBContext.XOX_T_SRV_ACT.Add(act2);
                    DBContext.SaveChanges();


                    //change status and resolution
                    ServiceRequest.RESOLUTION = (ServiceRequestResolution.Closed).ToString();
                    ServiceRequest.STATUS_CD = (ServiceRequestStatus.Closed).ToString();
                    DBContext.SaveChanges();

                    //add status closed activity
                    XOX_T_SRV_ACT act3 = new XOX_T_SRV_ACT();
                    act3.CREATED = DateTime.Now;
                    act3.CREATED_BY = UserId;
                    act3.SRV_ID = ServiceRequest.ROW_ID;
                    act3.NOTES = "Service Request " + "PPSR" + ServiceRequest.SR_NUM.ToString().PadLeft(8, '0') + " closed";
                    act3.FIELD_STAFF = UserService.GetName(UserId);
                    act3.STATUS_CD = "-";
                    act3.VISIT_DT = DateTime.Now;
                    DBContext.XOX_T_SRV_ACT.Add(act3);
                    DBContext.SaveChanges();
                }
                else
                {
                    try
                    {
                        response = r["Result"].ToString();

                        //add status closed activity
                        var ServiceRequest = (from d in DBContext.XOX_T_SRV_TIC
                                              where d.ROW_ID == vo.ServiceRequestId
                                              select d).First();
                        XOX_T_SRV_ACT act3 = new XOX_T_SRV_ACT();
                        act3.CREATED = DateTime.Now;
                        act3.CREATED_BY = UserId;
                        act3.SRV_ID = ServiceRequest.ROW_ID;
                        act3.NOTES = "Failed : " + response;
                        act3.FIELD_STAFF = UserService.GetName(UserId);
                        act3.STATUS_CD = "-";
                        act3.VISIT_DT = DateTime.Now;
                        DBContext.XOX_T_SRV_ACT.Add(act3);
                        DBContext.SaveChanges();

                        throw new Exception(response);
                    }
                    catch
                    {
                        throw new Exception(result);
                    }
                }
            }
        }

        public void SubmitUpdateDeposit(ServiceRequestVO vo, long AccountId)
        {
            var UserId = long.Parse(Thread.CurrentPrincipal.Identity.Name);

            using (var DBContext = new CRMDbContext())
            {
                //process to mpp
                var MSISDN = vo.MSISDN[0] == '6' ? vo.MSISDN : ('6' + vo.MSISDN);
                var result = EAIService.DepositUpdate(new DepositUpdate()
                {
                    MSISDN = MSISDN,
                    Deposit = vo.NewDeposit.Value,
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

                if (response.ToLower() == "true")
                {
                    decimal OldDeposit = 0;
                    var Deposit = from d in DBContext.XOX_T_ACCNT_PAYMENT
                                  where d.ACCNT_ID == AccountId
                                  && d.PAYMENT_TYPE == XOXConstants.PAYMENT_TYPE_DEPOSIT
                                  select d;

                    if (Deposit.Count() > 0)
                    {
                        OldDeposit = Deposit.First().AMOUNT == null ? 0 : (decimal)Deposit.First().AMOUNT;
                        
                        //if successfully done, update deposit in DB
                        Deposit.First().AMOUNT = vo.NewDeposit;
                        DBContext.SaveChanges();
                    }

                    //update servicerequest
                    var ServiceRequest = (from d in DBContext.XOX_T_SRV_TIC
                                          where d.ROW_ID == vo.ServiceRequestId
                                          select d).First();

                    ServiceRequest.OLD_ACCNT_PROFILE = OldDeposit.ToString();
                    ServiceRequest.NEW_ACCNT_PROFILE = vo.NewDeposit.ToString();
                    DBContext.SaveChanges();

                    //add success activity
                    XOX_T_SRV_ACT act2 = new XOX_T_SRV_ACT();
                    act2.CREATED = DateTime.Now;
                    act2.CREATED_BY = UserId;
                    act2.SRV_ID = ServiceRequest.ROW_ID;
                    act2.NOTES = "Submitted to external service and returned response " + response;
                    act2.FIELD_STAFF = UserService.GetName(UserId);
                    act2.STATUS_CD = "-";
                    act2.VISIT_DT = DateTime.Now;
                    DBContext.XOX_T_SRV_ACT.Add(act2);
                    DBContext.SaveChanges();


                    //change status and resolution
                    ServiceRequest.RESOLUTION = (ServiceRequestResolution.Closed).ToString();
                    ServiceRequest.STATUS_CD = (ServiceRequestStatus.Closed).ToString();
                    DBContext.SaveChanges();

                    //add status closed activity
                    XOX_T_SRV_ACT act3 = new XOX_T_SRV_ACT();
                    act3.CREATED = DateTime.Now;
                    act3.CREATED_BY = UserId;
                    act3.SRV_ID = ServiceRequest.ROW_ID;
                    act3.NOTES = "Service Request " + "PPSR" + ServiceRequest.SR_NUM.ToString().PadLeft(8, '0') + " closed";
                    act3.FIELD_STAFF = UserService.GetName(UserId);
                    act3.STATUS_CD = "-";
                    act3.VISIT_DT = DateTime.Now;
                    DBContext.XOX_T_SRV_ACT.Add(act3);
                    DBContext.SaveChanges();
                }
                else
                {
                    try
                    {
                        response = r["Result"].ToString();

                        //add status closed activity
                        var ServiceRequest = (from d in DBContext.XOX_T_SRV_TIC
                                              where d.ROW_ID == vo.ServiceRequestId
                                              select d).First();
                        XOX_T_SRV_ACT act3 = new XOX_T_SRV_ACT();
                        act3.CREATED = DateTime.Now;
                        act3.CREATED_BY = UserId;
                        act3.SRV_ID = ServiceRequest.ROW_ID;
                        act3.NOTES = "Failed : " + response;
                        act3.FIELD_STAFF = UserService.GetName(UserId);
                        act3.STATUS_CD = "-";
                        act3.VISIT_DT = DateTime.Now;
                        DBContext.XOX_T_SRV_ACT.Add(act3);
                        DBContext.SaveChanges();

                        throw new Exception(response);
                    }
                    catch
                    {
                        throw new Exception(result);
                    }
                }
            }
        }

    }
}
