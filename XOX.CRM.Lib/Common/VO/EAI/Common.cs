using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOX.CRM.Lib
{
    public class Request
    {
        public User User { get; set; }
    }
    public class ResultObj
    {
        public string Result { get; set; }
    }
    public class User
    {
        public int UserId { get; set; }
        public string Source { get; set; }
    }
}
