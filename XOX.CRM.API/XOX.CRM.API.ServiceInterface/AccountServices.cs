using CRM;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOX.CRM.API.ServiceModel;
using XOX.CRM.API.ServiceModel.Types;
using XOX.CRM.Lib;
using XOX.CRM.Lib.Common.Constants;
using XOX.CRM.Lib.WebServices;

namespace XOX.CRM.API.ServiceInterface
{
    public class AccountServices : Service
    {
        private readonly IAccountService AccountService;
        private readonly IAccountAttachmentService AccountAttachmentService;

        public AccountServices(IAccountService AccountService, IAccountAttachmentService AccountAttachmentService)
        {
            this.AccountService = AccountService;
            this.AccountAttachmentService = AccountAttachmentService;
        }

        public object Post(AccountAdd request)
        {
            AuthUserSession session = (AuthUserSession)this.GetSession();
            long UserId = long.Parse(session.UserAuthId);
            string APIKey = session.RequestTokenSecret;
            string Key = session.UserAuthName;

            var AccountVO = Mapper.Account.Map(request.PersonalInfo, request.BankingInfo, request.AddressInfo, request.BillingAddressInfo, request.SIMSerialNumber, request.RegistrationDate);

            var result = AccountService.Add(AccountVO, UserId, request.IntegrationId);
            var result2 = AccountService.BillAccAdd(AccountVO, result, UserId);

            APIKey = APIService.GenerateAPIKey(UserId, APIKey);
            return new LongResponse { Result = result, Key = APIKey };
        }

        public object Post(AccountAddFiles request)
        {
            AuthUserSession session = (AuthUserSession)this.GetSession();
            long UserId = long.Parse(session.UserAuthId);
            string APIKey = session.RequestTokenSecret;
            string Key = session.UserAuthName;

            List<string> filespath = new List<string>();
            foreach (var v in request.Files)
            {
                if (String.IsNullOrEmpty(v.Base64)==false)
                {
                    var filepath = Helper.FileUpload(v.Base64, v.FileName);
                    if (filepath != null)
                        filespath.Add(filepath);
                }
                else if (String.IsNullOrEmpty(v.FileURL) == false)
                {
                    var filepath = Helper.FileUploadURL(v.FileURL, v.FileName);
                    if (filepath != null)
                        filespath.Add(filepath);
                }
            }

            var result = AccountAttachmentService.AddFiles(filespath, request.AccountId, UserId);

            APIKey = APIService.GenerateAPIKey(UserId, APIKey);
            return new BoolResponse { Result = result, Key = APIKey };
        }

        public object Post(AccountSupplementaryAdd request)
        {
            AuthUserSession session = (AuthUserSession)this.GetSession();
            long UserId = long.Parse(session.UserAuthId);
            string APIKey = session.RequestTokenSecret;
            string Key = session.UserAuthName;

            var AccountVO = Mapper.Account.Map(request.PersonalInfo, request.BankingInfo, request.AddressInfo, request.AccountId, request.SIMSerialNumber);

            long result = 0;
            if (AccountService.CheckIntegrationId(request.IntegrationId, "3") == 0)
            {
                result = AccountService.Add(AccountVO, UserId, request.IntegrationId, true);
            }
            else
            {
                //using existing Supp Line Account Id
                AccountVO.AccountId = AccountService.CheckIntegrationId(request.IntegrationId, "3");
                AccountService.Edit(AccountVO, UserId, false);
                result = -1;
            }

            APIKey = APIService.GenerateAPIKey(UserId, APIKey);
            return new LongResponse { Result = result, Key = APIKey };
        }

        public object Any(AccountIntegrationIdCheck request)
        {
            AuthUserSession session = (AuthUserSession)this.GetSession();
            long UserId = long.Parse(session.UserAuthId);
            string APIKey = session.RequestTokenSecret;
            string Key = session.UserAuthName;

            var result = AccountService.CheckIntegrationId(request.IntegrationId);
            
            APIKey = APIService.GenerateAPIKey(UserId, APIKey);
            return new LongResponse { Result = result, Key = APIKey };
        }

        public object Any(AccountCreatePayment request)
        {
            AuthUserSession session = (AuthUserSession)this.GetSession();
            long UserId = long.Parse(session.UserAuthId);
            string APIKey = session.RequestTokenSecret;
            string Key = session.UserAuthName;

            foreach (var v in request.Payment)
            {
                long AccountId = 0;
                if (v.SupplementaryLine == false)
                {
                    AccountId = AccountService.CheckIntegrationId(v.IntegrationId, ((int)AccountType.PrincipalLine).ToString());
                }
                else if (v.SupplementaryLine == true)
                {
                    AccountId = AccountService.CheckIntegrationId(v.IntegrationId, ((int)AccountType.SupplementaryLine).ToString());
                }
                if (AccountId == 0)
                {
                    throw new Exception(v.IntegrationId + " - Integration Id not found.");
                }
            }

            foreach (var v in request.Payment)
            {
                PaymentRecordVO vo = new PaymentRecordVO();
                vo.AdvancePayment = v.AdvancePayment;
                vo.Deposit = v.Deposit;
                vo.ForeignDeposit = v.ForeignDeposit;
                vo.Reference = v.Reference;
                if (v.SupplementaryLine == false)
                {
                    vo.AccountId = AccountService.CheckIntegrationId(v.IntegrationId, ((int)AccountType.PrincipalLine).ToString());
                }
                else if (v.SupplementaryLine == true)
                {
                    vo.AccountId = AccountService.CheckIntegrationId(v.IntegrationId, ((int)AccountType.SupplementaryLine).ToString());
                }

                if (vo.AccountId != 0)
                {
                    var result = AccountService.SavePaymentRecord(vo, 1);
                }
            }

            APIKey = APIService.GenerateAPIKey(UserId, APIKey);
            return new BoolResponse { Result = true, Key = APIKey };
        }

        public object Any(AccountRedemptionPayment request)
        {
            AuthUserSession session = (AuthUserSession)this.GetSession();
            long UserId = long.Parse(session.UserAuthId);
            string APIKey = session.RequestTokenSecret;
            string Key = session.UserAuthName;

            string result = "";

            var AccountId = AccountService.CheckIntegrationId(request.IntegrationId, request.isSupplementaryLine == true ? "3" : "1");
            if (AccountId == 0)
            {
                result = "Integration Id not found.";
            }
            else
            {
                var Account = AccountService.Get(AccountId);
                
                //check AuthenticationKey
                if (request.AuthenticationKey != Helper.Encrypt("X0xC12m", "XOX-" + Account.PersonalInfo.MSISDNNumber + "-" + Account.PersonalInfo.IdentityNo))
                {
                    result = "Unauthenticated";
                }
                else
                {
                    var MakePaymentResult = AccountService.MakePayment(AccountId, new Payment()
                    {
                        Amount = request.Amount.ToString(),
                        MSISDN = Account.PersonalInfo.MSISDNNumber,
                        Reference = XOXConstants.PAYMENT_TYPE_REDEMPTION + "|INTEGRATIONID:" + request.IntegrationId + "|" + (request.isSupplementaryLine == true ? "3" : "1"),
                        PaymentMethod = XOXConstants.PAYMENT_METHOD_XPOINT,
                        PaymentType = XOXConstants.PAYMENT_TYPE_REDEMPTION,
                        CardIssuerBank = "",
                        CardType = "",
                        CardNumber = "",
                        CardExpiryMonth = 0,
                        CardExpiryYear = 0
                    }, true, 1);

                    if (MakePaymentResult == "true")
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
                                MSISDN = Account.PersonalInfo.MSISDNNumber[0] == '6' ? Account.PersonalInfo.MSISDNNumber : ("6" + Account.PersonalInfo.MSISDNNumber),
                                Message = "RM0: Your request for Xpoint conversion of " + request.Amount.ToString() + " points to bill payment amounting of RM" + request.Amount.ToString("0.00#####") + " had been successful! Thank you for your continuous support."
                            });
                        }
                        catch { }

                        AccountActivityVO AccountActivity = new AccountActivityVO();
                        AccountActivity.ACCNT_ID = AccountId;
                        AccountActivity.REASON = "";
                        AccountActivity.ASSIGNEE = "abc@abc.com";
                        AccountActivity.CREATED_BY = 1;
                        AccountActivity.LAST_UPD_BY = 1;
                        AccountActivity.ACT_DESC = "Make Redemption Payment";
                        AccountService.AddActivity(AccountActivity);

                        result = "true";
                    }
                    else
                    {
                        result = "Unknown Error";
                    }
                }
            }

            APIKey = APIService.GenerateAPIKey(UserId, APIKey);
            return new ObjResponse { Result = result, Key = APIKey };
        }

        public object Any(AccountCRPMakePayment request)
        {
            AuthUserSession session = (AuthUserSession)this.GetSession();
            long UserId = long.Parse(session.UserAuthId);
            string APIKey = session.RequestTokenSecret;
            string Key = session.UserAuthName;

            string result = "";

            var AccountId = AccountService.CheckIntegrationId(request.IntegrationId, request.isSupplementaryLine == true ? "3" : "1");
            if (AccountId == 0)
            {
                result = "PayForIntegrationId Id not found.";
            }
            else
            {
                var Account = AccountService.Get(AccountId);

                //check AuthenticationKey
                if (request.AuthenticationKey != Helper.Encrypt("X0xC12m", "XOX-" + Account.PersonalInfo.MSISDNNumber + "-" + Account.PersonalInfo.IdentityNo))
                {
                    result = "Unauthenticated";
                }
                else
                {
                    var MakePaymentResult = AccountService.MakePayment(AccountId, new Payment()
                    {
                        Amount = request.Amount.ToString(),
                        MSISDN = Account.PersonalInfo.MSISDNNumber,
                        Reference = request.Reference,
                        PaymentMethod = XOXConstants.PAYMENT_METHOD_CASH,
                        PaymentType = XOXConstants.PAYMENT_TYPE_BILLING,
                        CardIssuerBank = "",
                        CardType = "",
                        CardNumber = "",
                        CardExpiryMonth = 0,
                        CardExpiryYear = 0
                    }, true, 1);

                    if (MakePaymentResult == "true")
                    {
                       
                        AccountActivityVO AccountActivity = new AccountActivityVO();
                        AccountActivity.ACCNT_ID = AccountId;
                        AccountActivity.REASON = "";
                        AccountActivity.ASSIGNEE = "abc@abc.com";
                        AccountActivity.CREATED_BY = 1;
                        AccountActivity.LAST_UPD_BY = 1;
                        AccountActivity.ACT_DESC = request.Reference;
                        AccountService.AddActivity(AccountActivity);

                        result = "true";
                    }
                    else
                    {
                        result = "Unknown Error";
                    }
                }
            }

            APIKey = APIService.GenerateAPIKey(UserId, APIKey);
            return new ObjResponse { Result = result, Key = APIKey };
        }

    }
}
