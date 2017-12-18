using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotJson.Builder.Util
{
    // WARNING: Not fully ported.

    // TBD: Use BuilderPolicy, etc. ???
    public static class BeanIntrospectionUtil
    {
        // Note:
        // Because of the way we implement builder.toJsonStructure(),
        // we do not "drill down" in the object.
        // An object is always converted to map, and the map is processed (to the specified depth)
        // hence we only need a method with drillDownDepth == 1.
        // ...

        // temporary
        public static IDictionary<string, object> introspect(object obj)
        {
            return introspect(obj, 1);
        }
        // drillDownDepth >= 1.
        public static IDictionary<string, object> introspect(object obj, int drillDownDepth)
        {
            IDictionary<string, object> result = new Dictionary<string, object>();

            // tbd:
            //BeanInfo info = Introspector.getBeanInfo(obj.GetType());
            //foreach (PropertyDescriptor pd in info.PropertyDescriptors) {
            //    Method reader = pd.ReadMethod;
            //    if (reader != null) {
            //        string attr = pd.Name;
            //        if (!string.ReferenceEquals(attr, null) && !attr.Equals("class")) { // Remove "getClass()" method.
            //            if (!reader.Accessible) {
            //                reader.Accessible = true;
            //            }
            //            // if(reader.isAccessible()) {
            //            object val = reader.invoke(obj);
            //            // Object val =  reader.invoke(obj, (Object[]) null);  // ???

            //            // TBD:
            //            // is val another bean?
            //            // Use drillDownDepth.
            //            // --> See the comment above.
            //            // ....

            //            result[attr] = val;
            //            // } else {
            //            //     // ???
            //            // }
            //        }
            //    }
            //}

            return result;
        }

    }

}