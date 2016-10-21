using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HoloJson.Util
{
    public static class CollectionUtil
    {
        // These ToDebugString() methods are
        // primarily for debugging purposes.
        // This really "works" in the context of JSON
        // (which essentially comprises JsonArray of objects/T
        //     and JsonObject of key-value pairs of string->object/K->T.)

        public static string ToDebugString<K, T>(this object obj)
        {
            if (obj == null) {
                return null;
            }
            string valStr = null;
            if (obj != null) {
                if (obj is IList<T>) {   // ??? T?
                    valStr = ((IList<T>) obj).ToDebugString<K, T>();
                } else if (obj is IDictionary<K, T>) {   // ??? K,T?
                    valStr = ((IDictionary<K, T>) obj).ToDebugString<K, T>();
                } else {
                    valStr = obj.ToString();
                }
            } else {
                valStr = "";   // ???
            }
            return valStr;
        }
        public static string ToDebugString<K,T>(this IList<T> list)
        {
            if (list == null) {
                return null;
            }
            if (list.Count == 0) {
                return "";
            }
            var sb = new StringBuilder();
            sb.Append("[");
            foreach (var elem in list) {
                string valStr = null;
                if (elem != null) {
                    if (elem is IList<T>) {   // ??? T?
                        valStr = ((IList<T>) elem).ToDebugString<K, T>();
                    } else if (elem is IDictionary<K, T>) {   // ??? K,T?
                        valStr = ((IDictionary<K, T>) elem).ToDebugString<K, T>();
                    } else {
                        valStr = elem.ToString();
                    }
                } else {
                    valStr = "";   // ???
                }
                sb.Append(valStr).Append(",");
            }
            sb.Append("]");
            return sb.ToString();
        }
        public static string ToDebugString<K, T>(this IDictionary<K, T> dictionary)
        {
            if (dictionary == null) {
                return null;
            }
            if (dictionary.Count == 0) {
                return "";
            }
            var sb = new StringBuilder();
            sb.Append("{");
            foreach (var k in dictionary.Keys) {
                string valStr = null;
                object valObj = dictionary[k];
                if (valObj != null) {
                    if (valObj is IList<T>) {   // ??? T?
                        valStr = ((IList<T>) valObj).ToDebugString<K,T>();
                    } else if (valObj is IDictionary<K, T>) {   // ??? K,T?
                        valStr = ((IDictionary<K, T>) valObj).ToDebugString<K,T>();
                    } else {
                        valStr = valObj.ToString();
                    }
                } else {
                    valStr = "";   // ???
                }
                sb.Append(k).Append("=").Append(valStr).Append(";");
            }
            sb.Append("}");
            return sb.ToString();
        }

    }
}
