package org.minijson.builder.util;

import java.beans.BeanInfo;
import java.beans.IntrospectionException;
import java.beans.Introspector;
import java.beans.PropertyDescriptor;
import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;
import java.util.HashMap;
import java.util.Map;


// TBD: Use BuilderPolicy, etc. ???
public final class BeanIntrospectionUtil
{
    private BeanIntrospectionUtil() {}
    
    // Note:
    // Because of the way we implement builder.toJsonStructure(),
    // we do not "drill down" in the object.
    // An object is always converted to map, and the map is processed (to the specified depth)
    // hence we only need a method with drillDownDepth == 1.
    // ...

    // temporary
    public static Map<String, Object> introspect(Object obj) throws IntrospectionException, IllegalAccessException, IllegalArgumentException, InvocationTargetException
    {
        return introspect(obj, 1);
    }
    // drillDownDepth >= 1.
    public static Map<String, Object> introspect(Object obj, int drillDownDepth) throws IntrospectionException, IllegalAccessException, IllegalArgumentException, InvocationTargetException
    {
        Map<String, Object> result = new HashMap<String, Object>();
        BeanInfo info = Introspector.getBeanInfo(obj.getClass());
        for (PropertyDescriptor pd : info.getPropertyDescriptors()) {
            Method reader = pd.getReadMethod();
            if (reader != null) {
                String attr = pd.getName();
                if(attr != null && ! attr.equals("class")) {           // Remove "getClass()" method.
                    if(! reader.isAccessible()) {
                        reader.setAccessible(true);
                    }
                    // if(reader.isAccessible()) {
                        Object val =  reader.invoke(obj);
                        // Object val =  reader.invoke(obj, (Object[]) null);  // ???
                        
                        // TBD:
                        // is val another bean?
                        // Use drillDownDepth.
                        // --> See the comment above.
                        // ....
                        
                        result.put(attr, val);
                    // } else {
                    //     // ???
                    // }
                }
            }
        }
        return result;
    }
    
}
