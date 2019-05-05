using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOX.CRM.Lib
{
    public class LookupVO
    {
        public long CreatedBy { get; set; }
        public string LookupKey { get; set; }
        public string LookupValueParent { get; set; }
        public List<KeyValuePair<string, string>> KeyValues { get; set; }
        public List<KeyValuePair<string, string>> NewKeyValues { get; set; }
    }
}
