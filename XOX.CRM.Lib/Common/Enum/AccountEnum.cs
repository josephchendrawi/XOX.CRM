using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM
{
    public enum AccountStatus
    {
        [Description("Prospect")]
        Prospect = 1,
        [Description("Active")]
        Active = 2,
        [Description("Barred")]
        Barred = 3,
        [Description("Blocked")]
        Blocked = 4,
        [Description("Terminated")]
        Terminated = 5,
        [Description("Withdraw")]
        Withdraw = 6
    }

    public enum Salutation
    {
        [Description("Mr.")]
        Mr = 1,
        [Description("Mrs.")]
        Mrs = 2,
        [Description("Ms.")]
        Miss = 3,
        [Description("Dato.")]
        Dato = 4,
        [Description("Tan Sri.")]
        TanSri = 5,
        [Description("Dr.")]
        Dr = 6,
    }

    public enum IdentityType
    {
        [Description("NRIC No.")]
        NRICNo = 1,
        [Description("Armed Force ID")]
        ArmedForceID = 2,
        [Description("Passport No.")]
        PassportNo = 3
    }

    public enum IdentityTypeMNP
    {
        NewNRIC = 1,
        ArmedForcesId = 2,
        PassportNum = 3
    }

    public enum Language
    {
        [Description("English")]
        English = 1,
        [Description("Malay")]
        Malay = 2,
        [Description("Chinese")]
        Chinese = 3
    }

    public enum Gender
    {
        [Description("Male")]
        M = 1,
        [Description("Female")]
        F = 2
    }

    public enum CardType
    {
        [Description("Credit Card (Visa / Master)")]
        CreditCard = 1,
        [Description("Debit Card (Visa / Master)")]
        DebitCard = 2
    }

    public enum AddressType
    {
        [Description("Permanent Address")]
        PermanentAddress = 1,
        [Description("Billing Address")]
        BillingAddress = 2
    }

    public enum Flag
    {
        Yes = 1,
        No = 0
    }

    public enum AccountType
    {
        [Description("Principal Line")]
        PrincipalLine = 1,
        [Description("Billing Line")]
        BillingLine = 2,
        [Description("Supplementary Line")]
        SupplementaryLine = 3
    }

    public enum AccountGrade
    {
        Red = 1,
        Yellow = 2,
        Green = 3
    }

    public enum OrderCategory
    {
        NEW = 1,
        MNP = 2,
        COBP = 3
    }

    public enum MPPSubcriberStatus
    {
        PreActive = 0,
        Active = 1,
        Barred = 2,
        Blocked = 3,
        Terminated = 4,
    }

}
