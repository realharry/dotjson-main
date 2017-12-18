using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;


namespace DotJson.Util
{
    // Reference:
    //     Reflection in the .NET Framework
    //     https://msdn.microsoft.com/en-us/library/f7ykdhsy%28v=vs.110%29.aspx
    //     IntrospectionExtensions Class
    //     https://msdn.microsoft.com/en-us/library/system.reflection.introspectionextensions%28v=vs.110%29.aspx
    // ....
    public static class IntrospectionUtil
    {
        // Note:
        // Because of the way we implement builder.toJsonStructure(),
        // we do not "drill down" in the object.
        // An object is always converted to Dictionary, and the Dictionary is processed (to the specified depth)
        // hence we only need a method with drillDownDepth == 1.
        // ...

        // temporary
        public static IDictionary<string, object> Introspect(object obj)
        {
            return Introspect(obj, 1);
        }
        // drillDownDepth >= 1.
        private static IDictionary<string, object> Introspect(object obj, int drillDownDepth)
        {
            IDictionary<string, object> result = new Dictionary<string, object>();

            // TBD:
            // Does this work???

            var type = obj.GetType();
            // var propertyInfos = type.GetTypeInfo().DeclaredProperties;
            var propertyInfos = type.GetRuntimeProperties();
            foreach (var pi in propertyInfos) {
                if (pi.CanRead) {   // How to filter public properties only????
                    var name = pi.Name;
                    var val = pi.GetValue(obj);
                    result.Add(name, val);
                }
            }

            // tbd:
            // Include all public fields as well???
            // Also, public GetXXX() methods ????
            // ....


                // Java version.
            //        BeanInfo info = Introspector.getBeanInfo(obj.getClass());
            //        for (PropertyDescriptor pd : info.getPropertyDescriptors()) {
            //            Method reader = pd.getReadMethod();
            //            if (reader != null) {
            //                string attr = pd.getName();
            //                if(attr != null && ! attr.equals("class")) {           // Remove "getClass()" method.
            //                    if(! reader.isAccessible()) {
            //                        reader.setAccessible(true);
            //                    }
            //                    // if(reader.isAccessible()) {
            //                        object val =  reader.invoke(obj);
            //                        // object val =  reader.invoke(obj, (object[]) null);  // ???
            //                        
            //                        // TBD:
            //                        // is val another bean?
            //                        // Use drillDownDepth.
            //                        // --> See the comment above.
            //                        // ....
            //                        
            //                        result.put(attr, val);
            //                    // } else {
            //                    //     // ???
            //                    // }
            //                }
            //            }
            //        }

            return result;
        }

    }
}
