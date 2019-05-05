using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOX.CRM.API.ServiceModel.Types
{
    public class Response
    {
        public string Key { get; set; }
    }

    public class LongResponse : Response
    {
        public long Result { get; set; }
    }

    public class BoolResponse : Response
    {
        public bool Result { get; set; }
    }

    public class ObjResponse : Response
    {
        public object Result { get; set; }
    }

    public class ResultResponse : Response
    {
        public object Result { get; set; }
        public bool Success { get; set; }
    }
}
