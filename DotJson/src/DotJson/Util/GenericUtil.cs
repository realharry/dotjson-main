using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;


namespace DotJson.Util
{
    public static class GenericUtil
    {
        public static bool IsList(object obj)
        {
            if (obj == null) {
                return false;
            }
            var type = obj.GetType();


            if (!type.GetTypeInfo().IsGenericType) {
                return false;
            }

            // ???
            // var paramTypes = type.GetTypeInfo().GenericTypeParameters;

            // ????
            // System.Diagnostics.Debug.WriteLine("type.Name = {0}", type.Name);
            if (type.Namespace != null && type.Namespace.Equals("System.Collections.Generic") && type.Name != null && type.Name.Contains("List")) {
                return true;
            }
            // ????

            return false;
        }


        public static bool IsDictionary(object obj)
        {
            if (obj == null) {
                return false;
            }
            var type = obj.GetType();


            if (!type.GetTypeInfo().IsGenericType) {
                return false;
            }

            // ???
            // var paramTypes = type.GetTypeInfo().GenericTypeParameters;

            // ????
            // System.Diagnostics.Debug.WriteLine("type.Name = {0}", type.Name);
            if (type.Namespace != null && type.Namespace.Equals("System.Collections.Generic") && type.Name != null && type.Name.Contains("Dictionary")) {
                return true;
            }
            // ????

            return false;
        }


    }
}
