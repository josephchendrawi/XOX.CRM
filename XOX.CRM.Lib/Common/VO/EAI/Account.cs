using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOX.CRM.Lib
{
    public class Payment : Request
    {
        public string MSISDN { get; set; }
        public string Amount { get; set; }
        public string Reference { get; set; }
        public string PaymentMethod { get; set; }

        public string CardIssuerBank { get; set; }
        public string CardType { get; set; }
        public string CardNumber { get; set; }
        public int? CardExpiryMonth { get; set; }
        public int? CardExpiryYear { get; set; }

        public string PaymentType { get; set; }
    }

    public class GetPayment : Request
    {
        public string MSISDN { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Size { get; set; }
    }

    public class GetSubscriberProfile : Request
    {
        public string MSISDN { get; set; }
    }

    public class CreateSubscriberProfile : Request
    {
        public string MSISDN { get; set; }
        public string Name { get; set; }
        public string PostalAddress { get; set; }
        public string PostalAddress2 { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string RegType { get; set; }
        public string SubscriberID { get; set; }
        public string Language { get; set; }
        public string SimCardNumber { get; set; }
        public string CreditLimit { get; set; }
        public int ContractPeriod { get; set; }
        public string PlanInfo { get; set; }
    }

    public class AddSubscriberProfile : Request
    {
        public string MSISDN { get; set; }
        public CustomerRequest Customer { get; set; }
        public Subscription Subscription { get; set; }
        public Counter Counter { get; set; }
        public FnfCounter fnfCounter { get; set; }
    }

    public class CustomerRequest
    {
        public string name { get; set; }
        public string emailAddress { get; set; }
        public string postalAddress { get; set; }
        public string postalAddressL2 { get; set; }
        public string postalCode { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string organizationName { get; set; }
        public string language { get; set; }
        public string ic { get; set; }
        public string accountNumber { get; set; }
        public string planInfo { get; set; }
        public string passport { get; set; }
        public string armyId { get; set; }
        public string referrerCode { get; set; }
    }

    public class Subscription
    {
        public string iccid { get; set; }
        public string motherMaidenName { get; set; }
        public string signUpChannel { get; set; }
        public int contractPeriod { get; set; }
        public string printedBill { get; set; }
        public string autoDebit { get; set; }
    }

    public class Counter
    {
        public int creditLimit { get; set; }
        public int prime { get; set; }
        public int initFreeOnNetCalls { get; set; }
        public int initFreeOffNetCalls { get; set; }
        public int initFreeOnNetSms { get; set; }
        public int initFreeOffNetSms { get; set; }
        public int initFreeData { get; set; }
        public int deposit { get; set; }
        public string dataPack { get; set; }
    }

    public class FnfCounter
    {
        public int initFnFOnNetCalls { get; set; }
        public int initFnFOffNetCalls { get; set; }
        public int initFnFOnNetSms { get; set; }
        public int initFnFOffNetSms { get; set; }
        public int initFnFData { get; set; }
    }
    
    public class AddSubscriberProfileMNP : Request
    {
        public SubscriberData SubscriberData { get; set; }
        public NumbersToPort NumbersToPort { get; set; }
    }

    public class AddSubscriberProfileMNPSubs : Request
    {
        public SubscriberData SubscriberData { get; set; }
        public string NumbersToPort { get; set; }
    }

    public class NumberLists
    {
        public List<NumbersToPort> NumberList { get; set; }
    }

    public class SubscriberData
    {
        public string customerName { get; set; }
        public string regType { get; set; }
        public string subsId { get; set; }
        public string motherMaidenName { get; set; }
        public int dateOfBirth { get; set; }
        public string race { get; set; }
        public string gender { get; set; }
        public int age { get; set; }
        public int nationality { get; set; }
        public string contactPhone1 { get; set; }
        public string portReqFormId { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string address3 { get; set; }
        public string city { get; set; }
        public int stateCode { get; set; }
        public string postCode { get; set; }
        public string donorId { get; set; }
        public string emailAddress { get; set; }
    }

    public class NumbersToPort
    {
        public string Msisdn { get; set; }
        public string SimCardNumber { get; set; }
        public string PlanName { get; set; }
        public string AccountType { get; set; }
        public string planInfo { get; set; }
        public int CreditLimit { get; set; }
        public string SignUpChannel { get; set; }
        public int Prime { get; set; }
        public int InitFreeOnNetCalls { get; set; }
        public int InitFreeOffNetCalls { get; set; }
        public int InitFreeOnNetSms { get; set; }
        public int InitFreeOffNetSms { get; set; }
        public string DataPack { get; set; }
        public int InitFnFOnNetCalls { get; set; }
        public int InitFnFOffNetCalls { get; set; }
        public int InitFnFOnNetSms { get; set; }
        public int InitFnFOffNetSms { get; set; }
        public int InitFnFData { get; set; }
        public int InitFreeData { get; set; }
        public int ContractPeriod { get; set; }
        public int Deposit { get; set; }
        public string PrintedBill { get; set; }
        public string AutoDebit { get; set; }
    }

    public class CreditLimitUpdate : Request
    {
        public string MSISDN { get; set; }
        public string CreditLimit { get; set; }
    }
    public class SimNumberUpdate : Request
    {
        public string MSISDN { get; set; }
        public string SimNumber { get; set; }
    }

    public class ItemisedBillingUpdate : Request
    {
        public string MSISDN { get; set; }
        public bool ItemisedBilling { get; set; }
    }

    public class PlanUpdateRequest : Request
    {
        public string MSISDN { get; set; }
        public decimal InitFreeOnNetCalls { get; set; }
        public decimal InitFreeOffNetCalls { get; set; }
        public decimal InitFreeOnNetSMS { get; set; }
        public decimal InitFreeOffNetSMS { get; set; }
        public decimal InitFnFOffNetCalls { get; set; }
        public decimal Deposit { get; set; }
        public string DataPack { get; set; }
    }

    public class PlanUpdate : Request
    {
        public string MSISDN { get; set; }
        public string Plan { get; set; }
        public decimal Prime { get; set; }
        public string CreditLimit { get; set; }
    }
    public class DepositUpdate : Request
    {
        public string MSISDN { get; set; }
        public decimal Deposit { get; set; }
    }

    public class CheckOneXDealer : Request
    {
        public string CustomerNRIC { get; set; }
    }

    public class AddCUG : Request
    {
        public string MSISDN { get; set; }
        public string CUGNo { get; set; }
    }

    public class RemoveCUG : Request
    {
        public string MSISDN { get; set; }
        public string CUGNo { get; set; }
    }

    public class GetCUG : Request
    {
        public string MSISDN { get; set; }
    }

    public class SendSMS : Request
    {
        public string MSISDN { get; set; }
        public string Message { get; set; }
    }

    public class SendSMSWithShortCode : Request
    {
        public string MSISDN { get; set; }
        public string Message { get; set; }
    }

    public class UpdateSubscriberProfile : Request
    {
        public string MSISDN { get; set; }
        public SubscriberProfile SubscriberProfile { get; set; }
    }
    public class SubscriberProfile
    {
        public string salutation { get; set; }
        public string name { get; set; }
        public DateTime? birthDate { get; set; }
        public string ic { get; set; }
        public string preferredLanguage { get; set; }
        public string postalAddressL2 { get; set; }
        public string postalAddress { get; set; }
        public string city { get; set; }
        public string postalCode { get; set; }
        public string state { get; set; }
        public string emailAddress { get; set; }

        //public string passport { get; set; }

        //public string iMSI { get; set; }
        //public string monthlyStatement { get; set; }
        //public string organizationName { get; set; }
        //public string businessRegistrationNo { get; set; }
    }

    /// Response

    public class GetPaymentResponse
    {
        public string amount { get; set; }
        public string buyerID { get; set; }
        public string description { get; set; }
        public string operationID { get; set; }
        public string paymentDate { get; set; }
        public string paymentID { get; set; }
        public string paymentInstrumentID { get; set; }
        public string paymentMethod { get; set; }
        public string payType { get; set; }
        public string status { get; set; }
        public string unspecifiedMethod { get; set; }
        public string vendorID { get; set; }
    }

    public class GetSubscriberProfileResponse
    {
        public Customer Customer { get; set; }
        public detail[] additionalInfo { get; set; }
        public detail[] additionalInformation { get; set; }
        public detail[] parameter { get; set; }
        public detail[] counter { get; set; }
    }

    public class Customer
    {
        public string customerID { get; set; }
        public string customerType { get; set; }
        public string name { get; set; }
        public string postalCode { get; set; }
        public string countryCode { get; set; }
        public string postalAddressL2 { get; set; }
        public string organizationName { get; set; }
        public string postalAddress { get; set; }
        public string city { get; set; }
        public string offerCode { get; set; }
        public string effective { get; set; }
    }

    public class detail
    {
        public string name { get; set; }
        public string value { get; set; }
    }

    public class cugNumberData
    {
        public string cugNumber { get; set; }
    }

}
