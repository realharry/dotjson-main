using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoloJson.Core
{
    public static class Arrays
    {
        public static string ToString<T>(T[] arr)
        {
            if (arr == null) {
                return null;
            }
            if (arr.Length == 0) {
                return "";
            }
            StringBuilder sb = new StringBuilder();
            string comma = "";
            foreach (T a in arr) {
                sb.Append(comma).Append(a.ToString());
                comma = ",";
            }

            return sb.ToString();
        }
    }
}
