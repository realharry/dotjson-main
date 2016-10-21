using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HoloJson.Util
{
    public static class NumberUtil
    {
        // TBD:
        // Use Number.IsNumber() ???
        [Obsolete]
        public static bool IsNumber(object obj)
        {
            var type = obj.GetType();
            if(type == typeof(Int16)
                || type == typeof(Int32)
                || type == typeof(Int64)
                || type == typeof(UInt16)
                || type == typeof(UInt32)
                || type == typeof(UInt64)
                || type == typeof(Single)
                || type == typeof(Double)
                || type == typeof(Decimal)
                ) {
                return true;
            } else {
                return false;
            }
        }
    }
}
