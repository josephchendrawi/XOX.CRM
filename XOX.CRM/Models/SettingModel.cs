using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRM.Models
{
    public class LookupModel
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public int? Sequence { get; set; }
    }

    public class LookupListData : DataTableModel
    {
        public List<LookupModel> aaData;
    }

    public class LookupSequenceVM
    {
        public string Type { get; set; }
        public List<LookupModel> List { get; set; }
    }
}