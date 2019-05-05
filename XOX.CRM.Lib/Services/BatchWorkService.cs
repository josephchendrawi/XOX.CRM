using CRM;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XOX.CRM.Lib.Common.Constants;
using XOX.CRM.Lib.DBContext;
using XOX.CRM.Lib.WebServices;

namespace XOX.CRM.Lib.Services
{
    public class BatchWorkService 
    {
        public AssetService AssetService = new AssetService();
        public AccountService AccountService = new AccountService();
        public OrderService OrderService = new OrderService();
        public BillPaymentService BillPaymentService = new BillPaymentService();

        public void Add(BatchWorkVO vo, long UserId = 0)
        {
            if (UserId == 0)
            {
                UserId = int.Parse(Thread.CurrentPrincipal.Identity.Name);
            }

            using (var DBContext = new CRMDbContext())
            {
                XOX_T_BATCHWORK ett = new XOX_T_BATCHWORK();
                ett.CREATED = DateTime.Now;
                ett.CREATED_BY = UserId;
                ett.DESCRIPTION = vo.Description;
                ett.JOB_NAME = vo.JobName;
                ett.JOB_TYPE = vo.JobType;
                ett.REMARKS = vo.Remarks;
                ett.START_AT = vo.StartAt;
                ett.STATUS = (int)BatchWorkStatus.Stopped;
                ett.RUN_SEQUENCE = 0;
                ett.FREQUENCY = vo.Frequency;
                ett.FLG_SEND_EMAIL_NOTIF = vo.SendEmailNotifFlag;

                DBContext.XOX_T_BATCHWORK.Add(ett);
                DBContext.SaveChanges();
            }
        }

        public List<BatchWorkVO> GetAll(int startIdx, int length, ref int TotalCount, string orderBy = "", string orderDirection = "")
        {
            List<BatchWorkVO> list = new List<BatchWorkVO>();
            using (var DBContext = new CRMDbContext())
            {
                var result = from d in DBContext.XOX_T_BATCHWORK
                             select d;


                //filtering
                //

                TotalCount = result.Count();

                //ordering && paging
                if (orderDirection == "asc")
                {
                    if (orderBy == "JobName")
                        result = result.OrderBy(m => m.JOB_NAME);
                    else if (orderBy == "JobType")
                        result = result.OrderBy(m => m.JOB_TYPE);
                    else if (orderBy == "Created")
                        result = result.OrderBy(m => m.CREATED);
                    else if (orderBy == "Description")
                        result = result.OrderBy(m => m.DESCRIPTION);
                    else if (orderBy == "Frequency")
                        result = result.OrderBy(m => m.FREQUENCY);
                    else if (orderBy == "StartAt")
                        result = result.OrderBy(m => m.START_AT);
                    else if (orderBy == "RunSequence")
                        result = result.OrderBy(m => m.RUN_SEQUENCE);
                    else if (orderBy == "Status")
                        result = result.OrderBy(m => m.STATUS);
                    else if (orderBy == "SendEmailNotifFlag")
                        result = result.OrderBy(m => m.FLG_SEND_EMAIL_NOTIF);
                    else
                        result = result.OrderBy(m => m.ROW_ID);
                }
                else
                {
                    if (orderBy == "JobName")
                        result = result.OrderByDescending(m => m.JOB_NAME);
                    else if (orderBy == "JobType")
                        result = result.OrderByDescending(m => m.JOB_TYPE);
                    else if (orderBy == "Created")
                        result = result.OrderByDescending(m => m.CREATED);
                    else if (orderBy == "Description")
                        result = result.OrderByDescending(m => m.DESCRIPTION);
                    else if (orderBy == "Frequency")
                        result = result.OrderByDescending(m => m.FREQUENCY);
                    else if (orderBy == "StartAt")
                        result = result.OrderByDescending(m => m.START_AT);
                    else if (orderBy == "RunSequence")
                        result = result.OrderByDescending(m => m.RUN_SEQUENCE);
                    else if (orderBy == "Status")
                        result = result.OrderByDescending(m => m.STATUS);
                    else if (orderBy == "SendEmailNotifFlag")
                        result = result.OrderByDescending(m => m.FLG_SEND_EMAIL_NOTIF);
                    else
                        result = result.OrderByDescending(m => m.ROW_ID);
                }

                if (length >= 0)
                    result = result.Skip(startIdx).Take(length);

                foreach (var v in result)
                {
                    list.Add(new BatchWorkVO()
                    {
                        BatchWorkId = v.ROW_ID,
                        Description = v.DESCRIPTION,
                        EndAt = v.END_AT,
                        JobName = v.JOB_NAME,
                        JobType = v.JOB_TYPE,
                        Remarks = v.REMARKS,
                        StartAt = v.START_AT,
                        Status = v.STATUS.Value,
                        Created = v.CREATED,
                        RunSequence = v.RUN_SEQUENCE.Value,
                        Frequency = v.FREQUENCY.Value,
                        SendEmailNotifFlag = v.FLG_SEND_EMAIL_NOTIF ?? false
                    });
                }
            }

            return list;
        }

        public List<string> GetAllJobType()
        {
            List<string> AllJobType = new List<string>();
            using (var DBContext = new CRMDbContext())
            {
                var result = from d in DBContext.XOX_T_BATCHWORK
                             select d;

                foreach (var v in result)
                {
                    AllJobType.Add(v.JOB_TYPE);
                }
            }

            return AllJobType;
        }

        public List<string> GetAllLogFilename()
        {
            List<string> AllLogFilename = new List<string>();
            using (var DBContext = new CRMDbContext())
            {
                var result = (from d in DBContext.XOX_T_BATCHWORK_LOG
                             select d.FILE_NAME).Distinct();

                foreach (var v in result)
                {
                    AllLogFilename.Add(v);
                }
            }

            return AllLogFilename;
        }

        public BatchWorkVO Get(long BatchWorkId)
        {
            using (var DBContext = new CRMDbContext())
            {
                var result = from d in DBContext.XOX_T_BATCHWORK
                             where d.ROW_ID == BatchWorkId
                             select d;

                if (result.Count() > 0)
                {
                    var v = result.First();
                    return new BatchWorkVO()
                    {
                        BatchWorkId = v.ROW_ID,
                        Description = v.DESCRIPTION,
                        EndAt = v.END_AT,
                        JobName = v.JOB_NAME,
                        JobType = v.JOB_TYPE,
                        Remarks = v.REMARKS,
                        StartAt = v.START_AT,
                        Status = v.STATUS.Value,
                        Created = v.CREATED,
                        RunSequence = v.RUN_SEQUENCE.Value,
                        Frequency = v.FREQUENCY.Value,
                        SendEmailNotifFlag = v.FLG_SEND_EMAIL_NOTIF ?? false
                    };
                }
            }

            return null;
        }

        public bool Edit(BatchWorkVO vo)
        {
            using (var DBContext = new CRMDbContext())
            {
                var result = from d in DBContext.XOX_T_BATCHWORK
                             where d.ROW_ID == vo.BatchWorkId
                             select d;

                if (result.Count() > 0)
                {
                    var ett = result.First();

                    ett.DESCRIPTION = vo.Description;
                    ett.JOB_NAME = vo.JobName;
                    ett.JOB_TYPE = vo.JobType;
                    ett.REMARKS = vo.Remarks;
                    ett.START_AT = vo.StartAt;
                    //ett.STATUS = vo.Status;
                    ett.FREQUENCY = vo.Frequency;
                    ett.FLG_SEND_EMAIL_NOTIF = vo.SendEmailNotifFlag;

                    DBContext.SaveChanges();

                    return true;
                }
            }

            return false;
        }

        public void ChangeStatus(BatchWorkVO BatchWork)
        {
            using (var DBContext = new CRMDbContext())
            {
                var result = from d in DBContext.XOX_T_BATCHWORK
                             where d.ROW_ID == BatchWork.BatchWorkId
                             select d;

                if (result.Count() > 0)
                {
                    var v = result.First();
                    v.STATUS = BatchWork.Status;

                    DBContext.SaveChanges();
                }
            }
        }

        public void AddBatchLog(XOX_T_BATCHWORK_LOG log)
        {
            using (var DBContext = new CRMDbContext())
            {
                //add batch work log
                log.CREATED = DateTime.Now;

                DBContext.XOX_T_BATCHWORK_LOG.Add(log);
                DBContext.SaveChanges();
            }
        }

        public List<BatchWorkLogVO> GetAllLog(int startIdx, int length, ref int TotalCount, string orderBy = "", string orderDirection = "", BatchWorkLogVO filterby = null)
        {
            List<BatchWorkLogVO> list = new List<BatchWorkLogVO>();
            using (var DBContext = new CRMDbContext())
            {
                var result = from d in DBContext.XOX_T_BATCHWORK_LOG
                             join e in DBContext.XOX_T_BATCHWORK on d.BATCHWORK_ID equals e.ROW_ID
                             select new
                             {
                                 d.BATCHWORK_ID,
                                 d.CREATED,
                                 d.CREATED_BY,
                                 d.DESCRIPTION,
                                 d.JOB_STATUS,
                                 d.ROW_ID,
                                 d.RUN_SEQUENCE,
                                 e.JOB_NAME,
                                 e.JOB_TYPE,
                                 d.FILE_NAME,
                             };
                
                //filtering
                if (filterby.BatchWorkId != 0)
                {
                    result = result.Where(m => m.BATCHWORK_ID == filterby.BatchWorkId);
                }
                if (filterby.From != "" && filterby.To != "")
                {
                    DateTime DateFrom = DateTime.Parse(filterby.From);
                    DateTime DateTo = DateTime.Parse(filterby.To).AddDays(1);
                    result = result.Where(m => m.CREATED.Value >= DateFrom && m.CREATED.Value < DateTo);
                }
                if (filterby.Description != "")
                {
                    result = result.Where(m => m.DESCRIPTION.ToLower().Contains(filterby.Description));
                }
                if (filterby.RunSequence != 0)
                {
                    result = result.Where(m => m.RUN_SEQUENCE == filterby.RunSequence);
                }
                if (filterby.JobStatus != "")
                {
                    result = result.Where(m => m.JOB_STATUS.ToLower().Contains(filterby.JobStatus));
                }
                if (filterby.JobType != "")
                {
                    result = result.Where(m => m.JOB_TYPE == filterby.JobType);
                }
                if (filterby.FileName != "")
                {
                    result = result.Where(m => m.FILE_NAME == filterby.FileName);
                }

                TotalCount = result.Count();

                //ordering && paging
                if (orderDirection == "asc")
                {
                    if (orderBy == "BatchWorkId")
                        result = result.OrderBy(m => m.BATCHWORK_ID);
                    else if (orderBy == "JobName")
                        result = result.OrderBy(m => m.JOB_NAME);
                    else if (orderBy == "JobType")
                        result = result.OrderBy(m => m.JOB_TYPE);
                    else if (orderBy == "Created")
                        result = result.OrderBy(m => m.CREATED);
                    else if (orderBy == "Description")
                        result = result.OrderBy(m => m.DESCRIPTION);
                    else if (orderBy == "RunSequence")
                        result = result.OrderBy(m => m.RUN_SEQUENCE);
                    else if (orderBy == "JobStatus")
                        result = result.OrderBy(m => m.JOB_STATUS);
                    else if (orderBy == "FileName")
                        result = result.OrderBy(m => m.FILE_NAME);
                    else
                        result = result.OrderBy(m => m.ROW_ID);
                }
                else
                {
                    if (orderBy == "BatchWorkId")
                        result = result.OrderByDescending(m => m.BATCHWORK_ID);
                    else if (orderBy == "JobName")
                        result = result.OrderByDescending(m => m.JOB_NAME);
                    else if (orderBy == "JobType")
                        result = result.OrderByDescending(m => m.JOB_TYPE);
                    else if (orderBy == "Created")
                        result = result.OrderByDescending(m => m.CREATED);
                    else if (orderBy == "Description")
                        result = result.OrderByDescending(m => m.DESCRIPTION);
                    else if (orderBy == "RunSequence")
                        result = result.OrderByDescending(m => m.RUN_SEQUENCE);
                    else if (orderBy == "JobStatus")
                        result = result.OrderByDescending(m => m.JOB_STATUS);
                    else if (orderBy == "FileName")
                        result = result.OrderByDescending(m => m.FILE_NAME);
                    else
                        result = result.OrderByDescending(m => m.ROW_ID);
                }

                if (length >= 0)
                    result = result.Skip(startIdx).Take(length);

                foreach (var v in result)
                {
                    list.Add(new BatchWorkLogVO()
                    {
                        BatchWorkId = v.BATCHWORK_ID.Value,
                        Description = v.DESCRIPTION,
                        Created = v.CREATED,
                        RunSequence = v.RUN_SEQUENCE.Value,
                        BatchWork = Get(v.BATCHWORK_ID.Value),
                        JobStatus = v.JOB_STATUS,
                        FileName = v.FILE_NAME
                    });
                }
            }

            return list;
        }

        public List<BatchWorkSchedule> BatchWorkSchedules = new List<BatchWorkSchedule>();

        public void CheckAndDoBatchWork()
        {
            using (var DBContext = new CRMDbContext())
            {
                //Ignore Batch Work with status 'Stopped'
                var result = from d in DBContext.XOX_T_BATCHWORK
                             where d.STATUS != (int)BatchWorkStatus.Stopped
                             select d;

                List<XOX_T_BATCHWORK> ToDoBatchWorkList = new List<XOX_T_BATCHWORK>();

                foreach (var v in result)
                {
                    if (v.STATUS == (int)BatchWorkStatus.Waiting)
                    {
                        if (DateTime.Now >= v.START_AT)
                        {
                            /*v.TEMP_FREQUENCY = 0; //reset temp_frequency*/

                            //change Status to Running
                            v.STATUS = (int)BatchWorkStatus.Running;

                            //increment RUN_SEQUENCE
                            v.RUN_SEQUENCE += 1;

                            //Add to ToDoBatchWorkList
                            ToDoBatchWorkList.Add(v);

                            //store Next Schedule DateTime
                            //add new to BatchWorkSchedule
                            BatchWorkSchedules.Add(new BatchWorkSchedule()
                            {
                                BatchWorkId = v.ROW_ID,
                                NextSchedule = CalculateNextSchedule(v.START_AT.Value, v.FREQUENCY.Value)
                            });
                        }
                    }
                    else if (v.STATUS == (int)BatchWorkStatus.Running)
                    {
                        /*v.TEMP_FREQUENCY += 1; //increment by 1 minute

                        if (v.TEMP_FREQUENCY >= v.FREQUENCY)
                        {
                            v.TEMP_FREQUENCY = 0; //reset temp_frequency

                            //increment RUN_SEQUENCE
                            v.RUN_SEQUENCE += 1;

                            ToDoBatchWorkList.Add(v);
                        }*/

                        var BatchWorkSchedule = BatchWorkSchedules.Where(m => m.BatchWorkId == v.ROW_ID);
                        //check if got NextSchedule
                        if (BatchWorkSchedule.Count() > 0)
                        {
                            if (DateTime.Now >= BatchWorkSchedule.First().NextSchedule)
                            {
                                //increment RUN_SEQUENCE
                                v.RUN_SEQUENCE += 1;

                                //Add to ToDoBatchWorkList
                                ToDoBatchWorkList.Add(v);

                                //store Next Schedule DateTime
                                BatchWorkSchedule.First().NextSchedule = CalculateNextSchedule(v.START_AT.Value, v.FREQUENCY.Value);
                            }
                        }
                        else
                        {
                            //if NextSchedule not found
                            //add new to BatchWorkSchedule
                            BatchWorkSchedules.Add(new BatchWorkSchedule()
                            {
                                BatchWorkId = v.ROW_ID,
                                NextSchedule = CalculateNextSchedule(v.START_AT.Value, v.FREQUENCY.Value)
                            });
                        }
                    }
                }
                DBContext.SaveChanges();

                foreach (var v in ToDoBatchWorkList)
                {
                    DoBatchWork(v);
                }

            }
        }
        
        public DateTime CalculateNextSchedule(DateTime StartTime, long IntervalInMinute)
        {
            DateTime DateTimeNow = DateTime.Now;
            while (StartTime <= DateTimeNow)
            {
                StartTime = StartTime.AddMinutes(IntervalInMinute);
            }

            /*//Temporary
            var writepath = @"D:\Scheduler\next-scheduler.txt";
            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(writepath, true))
            {
                file.WriteLine(IntervalInMinute + " - " + DateTime.Now + " - " + StartTime);
            }
            //*/

            return StartTime;
        }

        public void DoBatchWork(XOX_T_BATCHWORK BatchWork)
        {
            using (var DBContext = new CRMDbContext())
            {
                //do work based on job type                
                if (BatchWork.JOB_TYPE == "Check Subscriber Profile")
                {
                    CheckAllActiveAccounts(BatchWork);
                }
                else if (BatchWork.JOB_TYPE == "Check Terminated Account")
                {
                    string FilePath = ConfigurationManager.AppSettings["TERMINATION_FILEPATH"] + DateTime.Now.ToString("yyyyMM") + @"\PPTerminations_" + DateTime.Now.AddDays(-1).ToString("yyyyMMdd") + ".csv";
                    ReadCSVFile(BatchWork, FilePath);

                    Termination(BatchWork);
                }
                else if (BatchWork.JOB_TYPE == "Change Plan")
                {
                    ChangePlan(BatchWork);
                }
                else if (BatchWork.JOB_TYPE == "Generate Bill Payment")
                {
                    foreach (var FilePath in FilesPath2Process(ConfigurationManager.AppSettings["GENERATE_BILL_FILEPATH"] + DateTime.Now.ToString("yyyyMM") + @"\", "BillPayment_" + DateTime.Now.ToString("yyyyMMdd"), "csv"))
                    {
                        ReadCSVFile(BatchWork, FilePath);
                    }

                    //single file
                    //string FilePath = ConfigurationManager.AppSettings["GENERATE_BILL_FILEPATH"] + DateTime.Now.ToString("yyyyMM") + @"\BillPayment_" + DateTime.Now.ToString("yyyyMMdd") + ".csv";
                    //ReadCSVFile(BatchWork, FilePath);

                    GenerateBillPayment(BatchWork);
                }
                else if (BatchWork.JOB_TYPE == "Send Daily Payment To CRP")
                {
                    ReadDailyPaymentFolder(BatchWork);

                    SendDailyPaymentToCRP(BatchWork);
                }
                else if (BatchWork.JOB_TYPE == "Write Daily Payment To CRM")
                {
                    ReadDailyPaymentFolder_ForRecording(BatchWork);
                    WriteDailyPaymentToCRM(BatchWork);
                }
            }
        }
        
        public List<string> FilesPath2Process(string DirectoryPath, string Contains, string FileFormat)
        {
            return new DirectoryInfo(DirectoryPath).GetFiles("*." + FileFormat).Where(m => m.Name.Contains(Contains)).Select(m => m.FullName).ToList();
        }

        public void BatchWorkLogChangeStatus(long BatchWorkLogId, string Status)
        {
            using (var DBContext = new CRMDbContext())
            {
                var BatchWorkLog = (from d in DBContext.XOX_T_BATCHWORK_LOG
                                    where d.ROW_ID == BatchWorkLogId
                                    select d).First();
                BatchWorkLog.JOB_STATUS = Status;
                DBContext.SaveChanges();
            }
        }

        public void CheckSubscriberProfile(AccountVO Account, long BatchWorkLogId)
        {
            var MSISDN = Account.PersonalInfo.MSISDNNumber[0] == '6' ? Account.PersonalInfo.MSISDNNumber : ('6' + Account.PersonalInfo.MSISDNNumber);

            EAIService eService = new EAIService();
            using (var DBContext = new CRMDbContext())
            {
                var result = EAIService.GetSubscriberProfile(new GetSubscriberProfile()
                {
                    MSISDN = MSISDN,
                    User = new User()
                    {
                        UserId = 0
                    }
                });
                var r = JObject.Parse(result);

                GetSubscriberProfileResponse SubscriberProfile = null;
                try
                {
                    SubscriberProfile = r["Result"].ToObject<GetSubscriberProfileResponse>();
                }
                catch (Exception e)
                {
                    BatchWorkLogChangeStatus(BatchWorkLogId, "Error : Subscriber Profile not found");

                    return;
                }

                if (SubscriberProfile != null)
                {
                    try
                    {
                        //AssetService.ConvertAssetFromPlan(Account, SubscriberProfile);
                        AssetService.ConvertAssetFromPlan(Account, SubscriberProfile);
                        
                        BatchWorkLogChangeStatus(BatchWorkLogId, "Success");
                    }
                    catch (Exception e)
                    {
                        BatchWorkLogChangeStatus(BatchWorkLogId, "Error : " + e.Message);
                    }
                }
            }
        }

        public void CheckAllActiveAccounts(XOX_T_BATCHWORK BatchWork)
        {
            using (var DBContext = new CRMDbContext())
            {
                var ActiveAccounts = from d in DBContext.XOX_T_ACCNT
                                     where d.ACCNT_STATUS == "Active"
                                     && d.ACCNT_TYPE_CD != "2"
                                     select d;

                foreach (var ActiveAccount in ActiveAccounts)
                {
                    try
                    {
                        AddBatchLog(new XOX_T_BATCHWORK_LOG()
                        {
                            BATCHWORK_ID = BatchWork.ROW_ID,
                            DESCRIPTION = ActiveAccount.ROW_ID + "-" + ActiveAccount.MSISDN,
                            JOB_STATUS = "",
                            RUN_SEQUENCE = BatchWork.RUN_SEQUENCE,
                        });

                    }
                    catch { }
                }

                var BatchWorkLogs = from d in DBContext.XOX_T_BATCHWORK_LOG
                                    where d.BATCHWORK_ID == BatchWork.ROW_ID
                                    && d.RUN_SEQUENCE == BatchWork.RUN_SEQUENCE
                                    //&& d.JOB_STATUS != "Success"
                                    select d;

                foreach (var BatchWorkLog in BatchWorkLogs)
                {
                    var AccountId = long.Parse(BatchWorkLog.DESCRIPTION.Split('-')[0]);

                    var Account = AccountService.Get(AccountId);

                    CheckSubscriberProfile(Account, BatchWorkLog.ROW_ID);
                }

            }
        }

        public void ReadCSVFile(XOX_T_BATCHWORK BatchWork, string FilePath)
        {
            var reader = new StreamReader(File.OpenRead(FilePath));
            var FileName = Path.GetFileName(FilePath);

            reader.ReadLine(); //ignore header line
            while (!reader.EndOfStream)
            {
                try
                {
                    string Result = "";
                    var line = reader.ReadLine();
                    line = line.Replace("\'\'", "\'"); //remove two quotes into one quotes
                    Result = line.Replace(',', '|');

                    AddBatchLog(new XOX_T_BATCHWORK_LOG()
                    {
                        BATCHWORK_ID = BatchWork.ROW_ID,
                        DESCRIPTION = Result,
                        JOB_STATUS = "",
                        RUN_SEQUENCE = BatchWork.RUN_SEQUENCE,
                        FILE_NAME = FileName
                    });
                }
                catch { }
            }

            reader.Close();
        }

        public void Termination(XOX_T_BATCHWORK BatchWork)
        {
            using (var DBContext = new CRMDbContext())
            {
                var BatchWorkLogs = from d in DBContext.XOX_T_BATCHWORK_LOG
                                    where d.BATCHWORK_ID == BatchWork.ROW_ID
                                    && d.RUN_SEQUENCE == BatchWork.RUN_SEQUENCE
                                    //&& d.JOB_STATUS != "Success"
                                    select d;

                foreach (var BatchWorkLog in BatchWorkLogs)
                {
                    try
                    {
                        var Description = BatchWorkLog.DESCRIPTION.Split('|');

                        var MSISDN = Description[0];
                        var Name = Description[1];
                        var TerminationDate = Description[2];

                        //check account existence to XOX_T_ACCNT (success if found)
                        var AccountIdS = AccountService.GetAccountIdByMSISDN(MSISDN, Name, true);
                        if (AccountIdS.Count() > 1)
                        {
                            throw new Exception("Got more than one Account");
                        }
                        else if (AccountIdS.Count() == 0 || AccountIdS == null)
                        {
                            throw new Exception("MSISDN not found");
                        }
                        var AccountId = AccountIdS.First();                        

                        if (AccountId != 0)
                        {
                            //check if Account status in CRM DB is terminated or not
                            var Account = AccountService.Get(AccountId);
                            if (Account.PersonalInfo.CustomerStatus == (int)AccountStatus.Terminated)
                            {
                                throw new Exception("Already Terminated");
                            }

                            //check subscriber profile existence to MPP (success if not found)
                            GetSubscriberProfileResponse SubscriberProfile = null;
                            try
                            {
                                SubscriberProfile = AccountService.GetSubscriberProfile(AccountId, 1);

                                //if Subscription found but OfferCode = Consumer Postpaid Offer, then error 
                                if (SubscriberProfile.Customer.offerCode == XOXConstants.OFFER_CODE_POSTPAID)
                                {
                                    var counter = SubscriberProfile.counter.ToDictionary(v => v.name, v => v.value);

                                    //if Subscription found, but Account status in MPP is not terminated, then error
                                    if (counter["State"] != ((int)MPPSubcriberStatus.Terminated).ToString())
                                    {
                                        throw new Exception("Profile Exists");
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                //if error != Subscription not found, then error
                                if (e.Message.Contains("Subscription Not Found") == false)
                                {
                                    throw e;
                                }
                            }

                            //check asset existence to XOX_T_ASSET  (success if found)
                            var ASSET = from d in DBContext.XOX_T_ASSET
                                        where d.CUST_ID == AccountId
                                        select d;

                            if (ASSET.Count() <= 0)
                            {
                                //throw new Exception("No Asset Found");

                                //change : add new asset
                                var Order = new OrderVO();
                                try
                                {
                                    Order = OrderService.GetOrderVO(OrderService.GetOrderIdByAccountId(AccountId));
                                }
                                catch { throw new Exception("No Order Found"); }

                                AssetService.ConvertAssetFromPlan(Account, Order);
                            }

                            //when success
                            OrderService.TerminateOrder(AccountId, TerminationDate);

                            BatchWorkLogChangeStatus(BatchWorkLog.ROW_ID, "Success");
                        }
                        else
                        {
                            BatchWorkLogChangeStatus(BatchWorkLog.ROW_ID, "Error : MSISDN not found.");
                        }
                    }
                    catch (Exception e)
                    {
                        BatchWorkLogChangeStatus(BatchWorkLog.ROW_ID, "Error : " + e.Message);
                    }
                }

            }
        }

        public void ChangePlan(XOX_T_BATCHWORK BatchWork)
        {
            using (var DBContext = new CRMDbContext())
            {
                var ChangePlanTypeOrder = from d in DBContext.XOX_T_ORDER
                                          where d.ORDER_TYPE == "Change Plan"
                                          && d.ORDER_STATUS == "Incomplete"
                                          && d.PREF_INSTALL_DT > DateTime.Now
                                          select d;

                ////check if submit date is today
                //ChangePlanTypeOrder = ChangePlanTypeOrder.Where(d => d.PREF_INSTALL_DT.Value.Day == DateTime.Now.Day && d.PREF_INSTALL_DT.Value.Month == DateTime.Now.Month && d.PREF_INSTALL_DT.Value.Year == DateTime.Now.Year);

                foreach (var ToChangePlan in ChangePlanTypeOrder)
                {
                    try
                    {
                        OrderService.ChangePlan(ToChangePlan.ROW_ID, 1);

                        AddBatchLog(new XOX_T_BATCHWORK_LOG()
                        {
                            BATCHWORK_ID = BatchWork.ROW_ID,
                            DESCRIPTION = "Successfully Changed Plan on Order with ID : " + ToChangePlan.ROW_ID,
                            JOB_STATUS = "Success",
                            RUN_SEQUENCE = BatchWork.RUN_SEQUENCE,
                        });
                    }
                    catch (Exception e)
                    {
                        AddBatchLog(new XOX_T_BATCHWORK_LOG()
                        {
                            BATCHWORK_ID = BatchWork.ROW_ID,
                            DESCRIPTION = e.Message,
                            JOB_STATUS = "Error",
                            RUN_SEQUENCE = BatchWork.RUN_SEQUENCE,
                        });
                    }
                }

            }
        }

        public void GenerateBillPayment(XOX_T_BATCHWORK BatchWork)
        {
            using (var DBContext = new CRMDbContext())
            {
                var BatchWorkLogs = from d in DBContext.XOX_T_BATCHWORK_LOG
                                    where d.BATCHWORK_ID == BatchWork.ROW_ID
                                    && d.RUN_SEQUENCE == BatchWork.RUN_SEQUENCE
                                    //&& d.JOB_STATUS != "Success"
                                    select d;

                foreach (var BatchWorkLog in BatchWorkLogs)
                {
                    try
                    {
                        var Result = BillPaymentService.PopualteBillPayment(BatchWorkLog.DESCRIPTION);

                        BatchWorkLogChangeStatus(BatchWorkLog.ROW_ID, "Success : Bill Payment Id - " + Result);
                    }
                    catch (Exception e)
                    {
                        BatchWorkLogChangeStatus(BatchWorkLog.ROW_ID, "Error : " + e.Message);
                    }
                }

            }
        }
        
        public void ReadDailyPaymentFolder(XOX_T_BATCHWORK BatchWork)
        {
            var SourceFolder = ConfigurationManager.AppSettings["DAILY_PAYMENT_FILEPATH"];
            var DestinationFolder = SourceFolder + @"Done\" + DateTime.Now.ToString("yyyyMM") + @"\";
            if (!Directory.Exists(DestinationFolder))
            {
                Directory.CreateDirectory(DestinationFolder);
            }

            if (Directory.Exists(SourceFolder))
            {
                DirectoryInfo di = new DirectoryInfo(SourceFolder);
                FileInfo[] Files = di.GetFiles();

                foreach (var v in Files)
                {
                    ReadDailyPaymentFile(BatchWork, v.FullName);

                    //move file
                    File.Move(v.FullName, Helper.GetUniqueFilename(DestinationFolder + v.Name));
                }
            }
        }

        public void ReadDailyPaymentFile(XOX_T_BATCHWORK BatchWork, string FilePath)
        {
            var reader = new StreamReader(File.OpenRead(FilePath));
            var FileName = Path.GetFileName(FilePath);

            while (!reader.EndOfStream)
            {
                try
                {
                    string Result = "";
                    var line = reader.ReadLine();
                    Result = line.Replace('\t', '|');

                    AddBatchLog(new XOX_T_BATCHWORK_LOG()
                    {
                        BATCHWORK_ID = BatchWork.ROW_ID,
                        DESCRIPTION = Result,
                        JOB_STATUS = "",
                        RUN_SEQUENCE = BatchWork.RUN_SEQUENCE,
                        FILE_NAME = FileName
                    });
                }
                catch { }
            }

            reader.Close();
        }

        public void SendDailyPaymentToCRP(XOX_T_BATCHWORK BatchWork)
        {
            using (var DBContext = new CRMDbContext())
            {
                var BatchWorkLogs = from d in DBContext.XOX_T_BATCHWORK_LOG
                                    where d.BATCHWORK_ID == BatchWork.ROW_ID
                                    && d.RUN_SEQUENCE == BatchWork.RUN_SEQUENCE
                                    select d;

                foreach (var BatchWorkLog in BatchWorkLogs)
                {
                    try
                    {
                        var Values = BatchWorkLog.DESCRIPTION.Split('|');

                        var Result = EAIService.CRPAddPayment(new CRPAddPayment()
                        {
                            User = new User()
                            {
                                Source = "CRM",
                                UserId = 1
                            },
                            PaymentId = long.Parse(Values[0]),
                            MSISDN = Values[1],
                            Amount = decimal.Parse(Values[2]),
                            ChargeType = int.Parse(Values[3]),
                            Desc = Values[4],
                            PaymentDate = Values[5],
                            Method = Values[6],
                        });
                        
                        if (Result.ToLower() == "true")
                        {
                            DBContext.SaveChanges();
                        }
                        else
                        {
                            throw new Exception(Result);
                        }

                        BatchWorkLogChangeStatus(BatchWorkLog.ROW_ID, "Success");
                    }
                    catch (Exception e)
                    {
                        BatchWorkLogChangeStatus(BatchWorkLog.ROW_ID, "Error : " + e.Message);
                    }
                }

            }
        }

        public void ReadDailyPaymentFolder_ForRecording(XOX_T_BATCHWORK BatchWork)
        {
            var SourceFolder = ConfigurationManager.AppSettings["DAILY_PAYMENT_FILEPATH"] + DateTime.Now.ToString("yyyyMM") + @"\";
            var DestinationFolder = ConfigurationManager.AppSettings["DAILY_PAYMENT_FILEPATH"] + @"Done\" + DateTime.Now.ToString("yyyyMM") + @"\";
            if (!Directory.Exists(DestinationFolder))
            {
                Directory.CreateDirectory(DestinationFolder);
            }

            if (Directory.Exists(SourceFolder))
            {
                DirectoryInfo di = new DirectoryInfo(SourceFolder);
                FileInfo[] Files = di.GetFiles();

                foreach (var v in Files)
                {
                    ReadDailyPaymentFolder_ForRecording(BatchWork, v.FullName);

                    //move file
                    File.Move(v.FullName, Helper.GetUniqueFilename(DestinationFolder + v.Name));
                }
            }
        }

        public void ReadDailyPaymentFolder_ForRecording(XOX_T_BATCHWORK BatchWork, string FilePath)
        {
            var reader = new StreamReader(File.OpenRead(FilePath));
            var FileName = Path.GetFileName(FilePath);

            while (!reader.EndOfStream)
            {
                try
                {
                    string Result = "";
                    var line = reader.ReadLine();
                    Result = line.Replace("\t", "|*|");

                    AddBatchLog(new XOX_T_BATCHWORK_LOG()
                    {
                        BATCHWORK_ID = BatchWork.ROW_ID,
                        DESCRIPTION = Result,
                        JOB_STATUS = "",
                        RUN_SEQUENCE = BatchWork.RUN_SEQUENCE,
                        FILE_NAME = FileName
                    });
                }
                catch { }
            }

            reader.Close();
        }

        public void WriteDailyPaymentToCRM(XOX_T_BATCHWORK BatchWork)
        {
            using (var DBContext = new CRMDbContext())
            {
                var BatchWorkLogs = from d in DBContext.XOX_T_BATCHWORK_LOG
                                    where d.BATCHWORK_ID == BatchWork.ROW_ID
                                    && d.RUN_SEQUENCE == BatchWork.RUN_SEQUENCE
                                    select d;

                foreach (var BatchWorkLog in BatchWorkLogs)
                {
                    try
                    {
                        var Values = BatchWorkLog.DESCRIPTION.Split(new string[] { "|*|" }, StringSplitOptions.None);

                        var PaymentDate = DateTime.ParseExact(Values[5], "yyyy-MM-dd HH:mm:ss", null);

                        var NewId = new PaymentCollectedService().Add(new PaymentCollectedVO()
                        {
                            PaymentId = long.Parse(Values[0]),
                            MSISDN = Values[1],
                            Amount = decimal.Parse(Values[2]),
                            ChargeType = int.Parse(Values[3]),
                            Description = Values[4],
                            PaymentDate = PaymentDate,
                            Method = Values[6],
                        });

                        BatchWorkLogChangeStatus(BatchWorkLog.ROW_ID, "Success");
                    }
                    catch (Exception e)
                    {
                        BatchWorkLogChangeStatus(BatchWorkLog.ROW_ID, "Error : " + e.Message);
                    }
                }

            }
        }

        public List<BatchWorkEmailVO> GetAllBatchWorkEmail(int startIdx, int length, ref int TotalCount, string orderBy = "", string orderDirection = "", long BatchWorkId = 0)
        {
            List<BatchWorkEmailVO> list = new List<BatchWorkEmailVO>();
            using (var DBContext = new CRMDbContext())
            {
                var result = from d in DBContext.XOX_T_BATCHWORK_EMAIL
                             select d;

                //filtering
                if (BatchWorkId != 0)
                {
                    result = result.Where(m => m.BATCHWORK_ID == BatchWorkId);
                }

                TotalCount = result.Count();

                //ordering && paging
                if (orderDirection == "asc")
                {
                    if (orderBy == "Email")
                        result = result.OrderBy(m => m.EMAIL);
                    else
                        result = result.OrderBy(m => m.ROW_ID);
                }
                else
                {
                    if (orderBy == "Email")
                        result = result.OrderByDescending(m => m.EMAIL);
                    else
                        result = result.OrderByDescending(m => m.ROW_ID);
                }

                if (length >= 0)
                    result = result.Skip(startIdx).Take(length);

                foreach (var v in result)
                {
                    list.Add(new BatchWorkEmailVO()
                    {
                        BatchWorkEmailId = v.ROW_ID,
                        Email = v.EMAIL,
                        BatchWorkId = v.BATCHWORK_ID ?? 0,
                    });
                }
            }

            return list;
        }

        public void AddBatchWorkEmail(BatchWorkEmailVO vo, long UserId = 0)
        {
            if (UserId == 0)
            {
                UserId = int.Parse(Thread.CurrentPrincipal.Identity.Name);
            }

            using (var DBContext = new CRMDbContext())
            {
                XOX_T_BATCHWORK_EMAIL ett = new XOX_T_BATCHWORK_EMAIL();
                ett.CREATED = DateTime.Now;
                ett.CREATED_BY = UserId;
                ett.BATCHWORK_ID = vo.BatchWorkId;
                ett.EMAIL = vo.Email;

                DBContext.XOX_T_BATCHWORK_EMAIL.Add(ett);
                DBContext.SaveChanges();
            }
        }

        public void DeleteBatchWorkEmail(long BatchWorkEmailId)
        {
            using (var DBContext = new CRMDbContext())
            {
                var XOX_T_BATCHWORK_EMAILs = from d in DBContext.XOX_T_BATCHWORK_EMAIL
                                             where d.ROW_ID == BatchWorkEmailId
                                             select d;

                if (XOX_T_BATCHWORK_EMAILs.Count() > 0)
                {
                    DBContext.XOX_T_BATCHWORK_EMAIL.Remove(XOX_T_BATCHWORK_EMAILs.First());
                    DBContext.SaveChanges();
                }
            }
        }

    }
}
