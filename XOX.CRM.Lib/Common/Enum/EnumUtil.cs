using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOX.CRM.Lib
{
    public static class EnumUtil
    {
        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static int ParseEnumInt<T>(string value)
        {
            try
            {
                return (int)Enum.Parse(typeof(T), value, true);
            }
            catch
            {
                return 0;
            }
        }

    }
}
