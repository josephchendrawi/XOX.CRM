using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOX.CRM.Lib.Interfaces;
using XOX.CRM.Lib.Common.Constants;
using XOX.CRM.Lib.DBContext;
using XOX.CRM.Lib.DAL;
using CRM;

namespace XOX.CRM.Lib
{
    public class BillPaymentService : IBillPaymentService
    {
        private AccountDAL accountDataAdapter;
        private List<string> ReadBillPayment(string billCharge)
        {
            string[] billingData = billCharge.Split('|');
            List<string> billingBlock = new List<string>();
            string msisdn = billingData.First().Substring(1, billingData.First().Length - 1);
            billingBlock.Add(msisdn); //MSISDN
            billingBlock.Add(billingData[1]); //NAME
            billingBlock.Add(billingData[2]); //Plan Name
            billingBlock.Add(billingData[3]); //Amount
            return billingBlock;
        }

        public long PopualteBillPayment(string billItem)
        {
            List<string> billingData = this.ReadBillPayment(billItem);
            StringBuilder sb = new StringBuilder();
            accountDataAdapter = new AccountDAL();
            XOX_T_ACCNT custAccount = accountDataAdapter.GetAccountIDByMsisdnByName(billingData[0], billingData[1]);

            XOX_T_BILL_PAYMENT billPayment = new XOX_T_BILL_PAYMENT();
            billPayment.ACCNT_ID = custAccount.ROW_ID;
            billPayment.CREATED = DateTime.Now;
            billPayment.CREATED_BY = 1;
            billPayment.BILL_CYCLE = "";
            billPayment.SUBMISSION_DATE = DateTime.Now.ToString("ddMMyyyy");
            billPayment.MSISDN = billingData[0];
            billPayment.POSTPAID_PLAN = billingData[2];
            billPayment.AMOUNT_DUE = Decimal.Parse(billingData[3]);
            billPayment.CREDIT_CARD = custAccount.BILL_ACCNT_NUM;
            billPayment.SUBSCRIBER_NAME = custAccount.NAME;
            billPayment.CARD_EXPIRY_MONTH = custAccount.BANK_EXPIRY_MONTH;
            billPayment.CARD_EXPIRY_YEAR = custAccount.BANK_EXPIRY_YEAR;
            billPayment.COMPANY_NAME = XOXConstants.XOX_BILL_COMPANY_NAME;
            billPayment.BANK_ISSUER = custAccount.BANK_ISSUER;

            // Validate data and include into remarks
            bool compareAccntStatus = custAccount.ACCNT_STATUS.Equals(XOXConstants.STATUS_PROSPECT_CD, StringComparison.OrdinalIgnoreCase);
            if (compareAccntStatus)
                sb.Append("Account status is not Active/Terminated.");

            if (billPayment.CREDIT_CARD != null)
            {
                if (billPayment.CREDIT_CARD.Length < 16)
                    sb.Append("Invalid credit card length.");

                if (billPayment.CARD_EXPIRY_MONTH <= 0 || billPayment.CARD_EXPIRY_MONTH > 12)
                    sb.Append("Invalid credit card expiry month.");

                if (billPayment.CARD_EXPIRY_YEAR < DateTime.Now.Year)
                    sb.Append("Invalid credit card expiry year.");
            }
            else
            {
                sb.Append("Credit card number not found.");
            }

            billPayment.REMARKS = sb.ToString();

            long recrodId = accountDataAdapter.AddBillPayment(billPayment);
            return recrodId;
        }
        
        public List<BillPaymentVO> GetAll(int startIdx, int length, ref int TotalCount, string orderBy = "", string orderDirection = "", BillPaymentVO qFilter = null)
        {
            List<BillPaymentVO> List = new List<BillPaymentVO>();
            using (var dbContext = new CRMDbContext())
            {
                var PrincipalLine = ((int)AccountType.PrincipalLine).ToString();
                var result = from d in dbContext.XOX_T_BILL_PAYMENT
                             join e in dbContext.XOX_T_ACCNT on d.ACCNT_ID equals e.ROW_ID
                             where e.ACCNT_TYPE_CD == PrincipalLine
                             select new {
                                AMOUNT_DUE = d.AMOUNT_DUE,
                                CARD_EXPIRY_MONTH = d.CARD_EXPIRY_MONTH,
                                CARD_EXPIRY_YEAR = d.CARD_EXPIRY_YEAR,
                                COMPANY_NAME = d.COMPANY_NAME,
                                CVV = d.CVV,
                                CREDIT_CARD = d.CREDIT_CARD,
                                MSISDN = d.MSISDN,
                                SUBSCRIBER_NAME = d.SUBSCRIBER_NAME,
                                CREATED = d.CREATED,
                                SUBMISSION_DATE = d.SUBMISSION_DATE,
                                ROW_ID = d.ROW_ID,
                                BANK_ISSUER = e.BANK_ISSUER,
                             };

                //filtering
                if (qFilter.AmountDue != null && qFilter.AmountDue != 0)
                {
                    result = result.Where(m => m.AMOUNT_DUE == qFilter.AmountDue);
                }
                if (qFilter.CCExpiryMonth != null && qFilter.CCExpiryMonth != 0)
                {
                    result = result.Where(m => m.CARD_EXPIRY_MONTH == qFilter.CCExpiryMonth);
                }
                if (qFilter.CCExpiryYear != null && qFilter.CCExpiryYear != 0)
                {
                    result = result.Where(m => m.CARD_EXPIRY_YEAR == qFilter.CCExpiryYear);
                }
                if (qFilter.CompanyName != null && qFilter.CompanyName != "")
                {
                    result = result.Where(m => m.COMPANY_NAME.ToLower().Contains(qFilter.CompanyName.ToLower()));
                }
                if (qFilter.CreditCardCVV != null && qFilter.CreditCardCVV != "")
                {
                    result = result.Where(m => m.CVV.ToLower().Contains(qFilter.CreditCardCVV.ToLower()));
                }
                if (qFilter.CreditCardNo != null && qFilter.CreditCardNo != "")
                {
                    result = result.Where(m => m.CREDIT_CARD.ToLower().Contains(qFilter.CreditCardNo.ToLower()));
                }
                if (qFilter.MSISDN != null && qFilter.MSISDN != "")
                {
                    result = result.Where(m => m.MSISDN.ToLower().Contains(qFilter.MSISDN.ToLower()));
                }
                if (qFilter.SubscriberName != null && qFilter.SubscriberName != "")
                {
                    result = result.Where(m => m.SUBSCRIBER_NAME.ToLower().Contains(qFilter.SubscriberName.ToLower()));
                }
                if (qFilter.SubmissionDate != null && qFilter.SubmissionDate != "")
                {
                    result = result.Where(m => m.SUBMISSION_DATE.ToLower().Contains(qFilter.SubmissionDate.ToLower()));
                }
                if (qFilter.FromDate != null && qFilter.ToDate != null)
                {
                    var tempToDate = qFilter.ToDate.Value.AddDays(1);
                    result = result.Where(m => qFilter.FromDate.Value <= m.CREATED.Value && m.CREATED.Value < tempToDate);
                }
                if (qFilter.CardIssuerBank != null && qFilter.CardIssuerBank != "")
                {
                    result = result.Where(m => m.BANK_ISSUER.ToLower().Contains(qFilter.CardIssuerBank.ToLower()));
                }

                TotalCount = result.Count();

                //ordering && paging
                if (orderDirection == "asc")
                {
                    if (orderBy == "AmountDue")
                        result = result.OrderBy(m => m.AMOUNT_DUE);
                    else if (orderBy == "CCExpiry")
                        result = result.OrderBy(m => (m.CARD_EXPIRY_YEAR + "" + (m.CARD_EXPIRY_MONTH < 10 ? "0" + m.CARD_EXPIRY_MONTH : "" + m.CARD_EXPIRY_MONTH)));
                    else if (orderBy == "CompanyName")
                        result = result.OrderBy(m => m.COMPANY_NAME);
                    else if (orderBy == "CreditCardCVV")
                        result = result.OrderBy(m => m.CVV);
                    else if (orderBy == "CreditCardNo")
                        result = result.OrderBy(m => m.CREDIT_CARD);
                    else if (orderBy == "MSISDN")
                        result = result.OrderBy(m => m.MSISDN);
                    else if (orderBy == "SubscriberName")
                        result = result.OrderBy(m => m.SUBSCRIBER_NAME);
                    else if (orderBy == "SubmissionDate")
                        result = result.OrderBy(m => m.SUBMISSION_DATE);
                    else if (orderBy == "CardIssuerBank")
                        result = result.OrderBy(m => m.BANK_ISSUER);
                    else
                        result = result.OrderBy(m => m.ROW_ID);
                }
                else
                {
                    if (orderBy == "AmountDue")
                        result = result.OrderByDescending(m => m.AMOUNT_DUE);
                    else if (orderBy == "CCExpiry")
                        result = result.OrderByDescending(m => (m.CARD_EXPIRY_YEAR + "" + (m.CARD_EXPIRY_MONTH < 10 ? "0" + m.CARD_EXPIRY_MONTH : "" + m.CARD_EXPIRY_MONTH)));
                    else if (orderBy == "CompanyName")
                        result = result.OrderByDescending(m => m.COMPANY_NAME);
                    else if (orderBy == "CreditCardCVV")
                        result = result.OrderByDescending(m => m.CVV);
                    else if (orderBy == "CreditCardNo")
                        result = result.OrderByDescending(m => m.CREDIT_CARD);
                    else if (orderBy == "MSISDN")
                        result = result.OrderByDescending(m => m.MSISDN);
                    else if (orderBy == "SubscriberName")
                        result = result.OrderByDescending(m => m.SUBSCRIBER_NAME);
                    else if (orderBy == "SubmissionDate")
                        result = result.OrderByDescending(m => m.SUBMISSION_DATE);
                    else if (orderBy == "CardIssuerBank")
                        result = result.OrderByDescending(m => m.BANK_ISSUER);
                    else
                        result = result.OrderByDescending(m => m.ROW_ID);
                }

                if (length >= 0)
                    result = result.Skip(startIdx).Take(length);

                foreach (var v in result)
                {
                    List.Add(new BillPaymentVO()
                    {
                        AmountDue = v.AMOUNT_DUE,
                        BillPaymentId = v.ROW_ID,
                        SubmissionDate = v.SUBMISSION_DATE,
                        SubscriberName = v.SUBSCRIBER_NAME,
                        MSISDN = v.MSISDN,
                        CreditCardNo = v.CREDIT_CARD,
                        CreditCardCVV = v.CVV,
                        CompanyName = v.COMPANY_NAME,
                        CCExpiryYear = v.CARD_EXPIRY_YEAR,
                        CCExpiryMonth = v.CARD_EXPIRY_MONTH,
                        CardIssuerBank = v.BANK_ISSUER
                    });
                }
            }

            return List;
        }

        public List<DateTime> GetAllCreatedDate()
        {
            List<DateTime> List = new List<DateTime>();
            using (var dbContext = new CRMDbContext())
            {
                var BILL_PAYMENT = from d in dbContext.XOX_T_BILL_PAYMENT
                                   where d.CREATED != null
                                   group d by new { Day = d.CREATED.Value.Day, Month = d.CREATED.Value.Month, Year = d.CREATED.Value.Year } into g
                                   orderby new { g.Key.Year, g.Key.Month, g.Key.Day }
                                   select g;

                foreach (var v in BILL_PAYMENT)
                {
                    List.Add(new DateTime(v.Key.Year, v.Key.Month, v.Key.Day));
                }
            }

            return List;
        }

        public BatchWorkLogStatistic GetErrorAndProcessedCountInBatchWorkLog(DateTime BatchDate)
        {
            using (var DBContext = new CRMDbContext())
            {
                var result = from d in DBContext.XOX_T_BATCHWORK_LOG
                             join e in DBContext.XOX_T_BATCHWORK on d.BATCHWORK_ID equals e.ROW_ID
                             where e.JOB_TYPE == "Generate Bill Payment"
                             && d.CREATED != null
                             && d.CREATED.Value.Day == BatchDate.Day && d.CREATED.Value.Month == BatchDate.Month && d.CREATED.Value.Year == BatchDate.Year
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

                return new BatchWorkLogStatistic()
                {
                    SuccessCount = result.Where(m => m.JOB_STATUS.Contains("Success")).Count(),
                    ErrorCount = result.Where(m => m.JOB_STATUS.Contains("Error")).Count(),
                    TotalCount = result.Count()
                };
            }
        }

    }
}
