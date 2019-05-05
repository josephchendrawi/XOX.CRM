using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOX.CRM.Lib
{
    public interface ICommonService
    {
        void GetLookupValuesChild(LookupVO lookupVO);
      
        void GetLookupValues(LookupVO lookupVO);

        void AddLookupValues(LookupVO lookupVO);

        void EditLookupValues(LookupVO LookupVO);

        void UpdateSequence(string Type, string Value, int Sequence);

        List<string> GetLookupTypes();
        string GetLookupNameByValue(string Val, string Type);
        string GetLookupValueByName(string Name, string Type);

        string MapDonorName(string DonorName);
        string MaskCreditCardDigit(string Digit);
    }
}
