package org.minijson.builder.impl;

import java.beans.IntrospectionException;
import java.io.IOException;
import java.io.StringWriter;
import java.io.Writer;
import java.lang.reflect.Array;
import java.lang.reflect.InvocationTargetException;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collection;
import java.util.Date;
import java.util.Iterator;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;
import java.util.logging.Level;
import java.util.logging.Logger;

import org.minijson.IndentedJsonSerializable;
import org.minijson.JsonCompatible;
import org.minijson.JsonSerializable;
import org.minijson.builder.BareJsonBuilder;
import org.minijson.builder.JsonBuilderException;
import org.minijson.builder.core.IndentInfoStruct;
import org.minijson.builder.policy.BuilderPolicy;
import org.minijson.builder.policy.base.DefaultBuilderPolicy;
import org.minijson.builder.util.BeanIntrospectionUtil;
import org.minijson.common.JsonNull;
import org.minijson.common.Literals;
import org.minijson.common.Symbols;
import org.minijson.util.CharacterUtil;
import org.minijson.util.UnicodeUtil;


public abstract class AbstractBareJsonBuilder implements BareJsonBuilder
{
    private static final Logger log = Logger.getLogger(AbstractBareJsonBuilder.class.getName());

    // Default value.
    // private static final int DEF_DRILL_DOWN_DEPTH = 2;
    // Max value: equivalent to -1.
    private static final int MAX_DRILL_DOWN_DEPTH = (int) Byte.MAX_VALUE;  // Arbitrary.
    
    // "strategy" for building json structure.
    // No setters for builderPolicy (except through a ctor).
    private final BuilderPolicy builderPolicy;

    // Not being used.
    // TBD: Not sure if we can ensure thread safety.
    private final boolean threadSafe;


    // Note:
    // It's important not to keep any "state" as class variables for this class
    //   so that a single instance can be used for multiple/concurrent build operations.
    // (Often, the recursive implementation may involve multiple objects (each as a node in an object tree),
    //   which may use the same/single instance of this builder class.)
    // ...


    public AbstractBareJsonBuilder()
    {
        this(null);
    }
    public AbstractBareJsonBuilder(BuilderPolicy builderPolicy)
    {
        this(builderPolicy, false);     // true or false ????
    }
    public AbstractBareJsonBuilder(BuilderPolicy builderPolicy, boolean threadSafe)
    {
        if(builderPolicy == null) {
            this.builderPolicy = DefaultBuilderPolicy.MINIJSON;
        } else {
            this.builderPolicy = builderPolicy;
        }
        this.threadSafe = threadSafe;
    }

    public BuilderPolicy getBuilderPolicy()
    {
        return this.builderPolicy;
    }
//    public void setBuilderPolicy(BuilderPolicy builderPolicy)
//    {
//        this.builderPolicy = builderPolicy;
//    }


    @Override
    public String build(Object jsonObj) throws JsonBuilderException
    {
        return build(jsonObj, 0);
    }

    @Override
    public String build(Object jsonObj, int indent) throws JsonBuilderException
    {
//        IndentInfoStruct indentInfo = new IndentInfoStruct(indent);
//        boolean includeWS = indentInfo.isIncludingWhiteSpaces();
//        boolean includeLB = indentInfo.isIncludingLineBreaks();
//        boolean lbAfterComma = indentInfo.isLineBreakingAfterComma();
//        int indentSize = indentInfo.getIndentSize();
//        int indentLevel = -1;
//
//        // TBD:
//        int depth = 1;
//        boolean useBeanIntrospection = false;
        
        // Which is better?
        
        // [1] Using StringBuilder.
        // StringBuilder sb = new StringBuilder();
        // return _build(sb, jsonObj, includeWS, includeLB, lbAfterComma, indentSize, indentLevel);
    
        // Or, [2] Using StringWriter.
        String jsonStr = null;
        StringWriter writer = new StringWriter();
        try {
            // _build(writer, jsonObj, depth, useBeanIntrospection, includeWS, includeLB, lbAfterComma, indentSize, indentLevel);
            // writer.flush();   // ???
            build(writer, jsonObj, indent);
            jsonStr = writer.toString();
            // log.warning(">>>>>>>>>>>>>>>>>>>> jsonStr = " + jsonStr);
            // String jsonStr2 = writer.getBuffer().toString();
            // log.warning(">>>>>>>>>>>>>>>>>>>> jsonStr2 = " + jsonStr2);

            if(log.isLoggable(Level.FINE)) log.fine("jsonStr = " + jsonStr);
        } catch (IOException e) {
            log.log(Level.WARNING, "Failed to write jsonObj as JSON.", e);
        }
        return jsonStr;
    }

    @Override
    public void build(Writer writer, Object jsonObj) throws IOException, JsonBuilderException
    {
        build(writer, jsonObj, 0);
    }

    @Override
    public void build(Writer writer, Object jsonObj, int indent) throws IOException, JsonBuilderException
    {
        int maxDepth = builderPolicy.drillDownDepth();
        if(maxDepth < 0) {
            maxDepth = MAX_DRILL_DOWN_DEPTH;
        }
        boolean useBeanIntrospection = this.builderPolicy.useBeanIntrospection();

        IndentInfoStruct indentInfo = new IndentInfoStruct(indent);
        boolean includeWS = indentInfo.isIncludingWhiteSpaces();
        boolean includeLB = indentInfo.isIncludingLineBreaks();
        boolean lbAfterComma = indentInfo.isLineBreakingAfterComma();
        int indentSize = indentInfo.getIndentSize();
        int indentLevel = -1;
        
        _build(writer, jsonObj, maxDepth, useBeanIntrospection, includeWS, includeLB, lbAfterComma, indentSize, indentLevel);
        writer.flush();   // ???

//        String jsonStr = writer.toString();
//        if(log.isLoggable(Level.FINE)) log.fine("jsonStr = " + jsonStr);

    }



    ///////////////////////////////////////////////////////
    // TBD: JsonBuilder (~~ JsonCompatible) interface..
    // ....

    @Override
    public Object buildJsonStructure(Object obj) throws JsonBuilderException
    {
        int maxDepth = builderPolicy.drillDownDepth();
        return buildJsonStructure(obj, maxDepth);
        // return toJsonStructure(obj, DEF_DRILL_DOWN_DEPTH);
    }

    @Override
    public Object buildJsonStructure(Object obj, int depth) throws JsonBuilderException
    {
        if(depth < 0) {
            depth = MAX_DRILL_DOWN_DEPTH;
//            if(depth == -1) {   // Special value.
//                depth = MAX_DRILL_DOWN_DEPTH;
//            } else {
//                // ???
//                log.warning("Invalid depth = " + depth);
//                return null;
//                // throw new JsonBuilderException("Invalid depth has been specified: " + depth);
//            }
        }
        int maxDepth = builderPolicy.drillDownDepth();
        if(depth > maxDepth) {
            if(log.isLoggable(Level.INFO)) log.info("Input depth, " + depth + ", is greater than the policy drillDownDepth, " + maxDepth + ". Using the drillDownDepth.");
            depth = maxDepth;
        }
        boolean useBeanIntrospection = this.builderPolicy.useBeanIntrospection();
        return _buildJsonStruct(obj, depth, useBeanIntrospection);
    }


    // TBD:
    // The problem with this algo is we have no way to consistently represent null node.
    // For map value and list element, we can just use Java null.
    // But, in general, it may not be possible.....  ????    Is this true????
    // Seems to be working so far (based on the limited unit test cases...)
    @SuppressWarnings("unchecked")
    private Object _buildJsonStruct(Object obj, int depth, boolean useBeanIntrospection) throws JsonBuilderException
    {
//        if(depth < 0) {
//            return null;
//        }
        Object jsonStruct = null;
        if(obj == null || obj instanceof JsonNull) {    // ????
            // return null;
        } else {
            // log.warning(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>> depth = " + depth);
            // if(depth == 0) {
            if(depth <= 0) {
                if(obj instanceof Boolean) {
                    jsonStruct = (Boolean) obj;
                } else if(obj instanceof Character) {
                    jsonStruct = (Character) obj;
                } else if(obj instanceof Number) {
                    jsonStruct = (Number) obj;
                } else if(obj instanceof String) {
                    // Note that this string is not a "json string" (e.g., forward slash escaped, etc.)
                    // e.g., if the string is "...\\/...", we will read it as "...\\/..." not as ".../...".
                    jsonStruct = (String) obj;
                } else {
                    // ????
                    jsonStruct = obj.toString();
                }
            } else {
                if(obj instanceof Map<?,?>) {
                    Map<String,Object> jsonMap = new LinkedHashMap<String,Object>();
    
                    Map<String,Object> map = null;
                    try {
                        map = (Map<String,Object>) ((Map<?,?>) obj);
                    } catch(Exception e) {
                        log.log(Level.WARNING, "Invalid map type.", e);
                        // What to do???
                        // Use map.toString???
                    }
                    if(map != null && !map.isEmpty()) {
                        for(String f : map.keySet()) {
                            Object val = map.get(f);
                            Object jsonVal = _buildJsonStruct(val, depth - 1, useBeanIntrospection);
                            if(jsonVal != null) {
                                jsonMap.put(f, jsonVal);
                            } else {
                                // ???
                                jsonMap.put(f, null);
                            }
                        }
                    }
                    
                    jsonStruct = jsonMap;
                } else if(obj instanceof List<?>) {
                    List<Object> jsonList = new ArrayList<Object>();
    
                    List<Object> list = null;
                    try {
                        list = (List<Object>) ((List<?>) obj);
                    } catch(Exception e) {
                        log.log(Level.WARNING, "Invalid list type.", e);
                        // What to do???
                        // Use list.toString???
                    }
                    if(list != null && !list.isEmpty()) {
                        for(Object v : list) {
                            Object jsonVal = _buildJsonStruct(v, depth - 1, useBeanIntrospection);
                            if(jsonVal != null) {
                                jsonList.add(jsonVal);
                            } else {
                                // ???
                                jsonList.add(null);
                            }
                        }
                    }
                    
                    jsonStruct = jsonList;
                } else if(obj.getClass().isArray()) {          // ???
                    List<Object> jsonList = new ArrayList<Object>();
                    
//                    Object[] array = null;
//                    try {
//                        array = (Object[]) obj;
//                    } catch(Exception e) {
//                        log.log(Level.WARNING, "Invalid array type.", e);
//                        // What to do???
//                        // Use list.toString???
//                    }
//                    if(array!= null && array.length > 0) {
//                        for(Object o : array) {
//                            Object jsonVal = _buildJsonStruct(o, depth - 1, useBeanIntrospection);
//                            if(jsonVal != null) {
//                                jsonList.add(jsonVal);
//                            } else {
//                                // ???
//                                jsonList.add(null);
//                            }
//                        }
//                    }
                    Object array = obj;
                    if(array!= null && Array.getLength(array) > 0) {
                        int arrLen = Array.getLength(array);
                        // log.warning(">>>>>>>>>>>>>>>>>>>>>>>>>>>>> arrLen = " + arrLen);
                        for(int i=0; i<arrLen; i++) {
                            Object o = Array.get(array, i);
                            // log.warning(">>>>>>>>>>>>>>>>>>>>>>>>>>>>> o = " + o + "; " + o.getClass());
                            Object jsonVal = _buildJsonStruct(o, depth - 1, useBeanIntrospection);
                            // log.warning(">>>>>>>>>>>>>>>>>>>>>>>>>>>>> jsonVal = " + jsonVal + "; " + o.getClass());
                            if(jsonVal != null) {
                                jsonList.add(jsonVal);
                            } else {
                                // ???
                                jsonList.add(null);
                            }
                        }
                    }
    
                    jsonStruct = jsonList;
                } else if(obj instanceof Collection<?>) {
                    List<Object> jsonList = new ArrayList<Object>();
                    // jsonList.addAll((Collection<Object>) ((Collection<?>) obj));

                    Iterator<Object> it = ((Collection<Object>) ((Collection<?>) obj)).iterator();
                    while(it.hasNext()) {
                        Object o = it.next();
                        Object jsonVal = _buildJsonStruct(o, depth - 1, useBeanIntrospection);
                        if(jsonVal != null) {
                            jsonList.add(jsonVal);
                        } else {
                            // ???
                            jsonList.add(null);
                        }
                    }
                    
                    jsonStruct = jsonList;
                } else {
                    // ???
                    // This can potentially cause infinite recursion.
                    // because maybe JsonCompatible object implements toJsonStructure() using JsonBuilder.buidJsonStructure()
                    // which calls the object's toJsonStructure(), which calls JsonBuilder.buidJsonStructure(), etc.
                    // ....
                    // if(obj instanceof JsonCompatible) {
                    //     jsonStruct = ((JsonCompatible) obj).toJsonStructure(depth);
                    // } else {
                        // primitive types... ???
                        if(obj instanceof Boolean) {
                            jsonStruct = (Boolean) obj;
                        } else if(obj instanceof Character) {
                            jsonStruct = (Character) obj;
                        } else if(obj instanceof Number) {
                            jsonStruct = (Number) obj;
                        } else if(obj instanceof String) {
                            jsonStruct = (String) obj;
                        } else {

                            // Use inspection....
                            // TBD:
                            // BuilderPolicy ???
                            // ...
                            
                            if(useBeanIntrospection) {
                                Map<String, Object> mapEquivalent = null;
                                try {
                                    // mapEquivalent = BeanIntrospectionUtil.introspect(obj, depth);   // depth? or depth - 1 ?
                                    // Because we are just converting a bean to a map,
                                    // the depth param is not used. (or, depth == 1).
                                    mapEquivalent = BeanIntrospectionUtil.introspect(obj);
                                // } catch (IllegalAccessException
                                //         | IllegalArgumentException
                                //         | InvocationTargetException
                                //         | IntrospectionException e) {
                                } catch (Exception e) {
                                    // Ignore.
                                    if(log.isLoggable(Level.INFO)) log.log(Level.INFO, "Faild to introspect a bean.", e);
                                }
                                if(mapEquivalent != null) {
                                    jsonStruct = _buildJsonStruct(mapEquivalent, depth, useBeanIntrospection);   // Note: We do not change the depth.
                                } else {
                                    
                                    // ????
                                    // jsonStruct = null; ???
                                    jsonStruct = obj.toString();
                                    // ...
                                }
                                // log.warning(">>>>>>>>>>>>>>>>>>>>>>>>>>>>> jsonStruct = " + jsonStruct);
                                
                            } else {
                                // ????
                                // jsonStruct = null; ???
                                jsonStruct = obj.toString();                                
                            }
                        }
                    // }
                }
            }
        }

        return jsonStruct;
    }

    

    // Not being used....
    // TBD: To be deleted???
    // This method used to be much different from the other veriosn of _build().
    // Now they are almost the same, with one method using StringBuilder, and the other using Writer (or, StringWriter).
    
    @SuppressWarnings("unchecked")
    private void _build(StringBuilder sb, Object obj, boolean includeWS, boolean includeLB, boolean lbAfterComma, int indentSize, int indentLevel)
    {
        // TBD:
        // Just use global vars ???
        ++indentLevel;
        String WS = "";
        if(includeWS) {
            WS = " ";
        }
        String LB = "";
        if(includeLB) {
            LB = "\n";
        }
        String IND = "";
        String INDX = "";
        if(indentSize > 0 && indentLevel > 0) {
            IND = String.format("%1$" + (indentSize * indentLevel) + "s", "");
        }
        if(indentSize > 0 && indentLevel >= 0) {
            INDX = String.format("%1$" + (indentSize * (indentLevel+1)) + "s", "");
        }
        
        // StringBuilder sb = new StringBuilder();
        if(obj == null) {
            sb.append(Literals.NULL);
        } else {
            if(obj instanceof Map<?,?>) {
                Map<String,Object> map = null;
                try {
                    map = (Map<String,Object>) ((Map<?,?>) obj);
                } catch(Exception e) {
                    log.log(Level.INFO, "Invalid map type.", e);
                }
                sb.append("{").append(LB);
                if(map != null && !map.isEmpty()) {
                    sb.append(INDX);
                    
                    Iterator<String> it = map.keySet().iterator();
                    while(it.hasNext()) {
                        String key = it.next();
                        Object val = map.get(key);
                        sb.append("\"").append(key).append("\":").append(WS);
                        _build(sb, val, includeWS, includeLB, lbAfterComma, indentSize, indentLevel);
                        // sb.append("\"").append(key).append("\":").append(WS).append(str);
                        if(it.hasNext()) {
                            sb.append(",");
                            if(lbAfterComma) {
                                sb.append(LB).append(INDX);
                            } else {
                                sb.append(WS);
                            }
                        } else {
                            sb.append(LB);
                        }
                    }
//                    for(String key : map.keySet()) {
//                        Object val = map.get(key);
//                        String str = _build(val, includeWS, includeLB, indentSize, indentLevel);
//                        sb.append("\"").append(key).append("\":").append(WS).append(str);
//                        sb.append(",").append(WS);
//                    }
//                    if(sb.charAt(sb.length() - 1) == ',' || sb.charAt(sb.length() - 1) == ' ') {
//                        sb.deleteCharAt(sb.length() - 1);
//                        if(sb.charAt(sb.length() - 1) == ',') {
//                            sb.deleteCharAt(sb.length() - 1);
//                        }
//                    }
                }
                sb.append(IND).append("}");
            } else if(obj instanceof List<?>) {
                List<Object> list = null;
                try {
                    list = (List<Object>) ((List<?>) obj);
                } catch(Exception e) {
                    log.log(Level.INFO, "Invalid list type.", e);
                }
                sb.append("[").append(LB);
                if(list != null && !list.isEmpty()) {
                    sb.append(INDX);
                    
                    Iterator<Object> it = list.iterator();
                    while(it.hasNext()) {
                        Object o = it.next();
                        _build(sb, o, includeWS, includeLB, lbAfterComma, indentSize, indentLevel);
                        // sb.append(str);
                        if(it.hasNext()) {
                            sb.append(",");
                            if(lbAfterComma) {
                                sb.append(LB).append(INDX);
                            } else {
                                sb.append(WS);
                            }
                        } else {
                            sb.append(LB);
                        }
                    }
//                    for(Object o : list) {
//                        String str = _build(o, includeWS, includeLB, indentSize, indentLevel);
//                        sb.append(str);
//                        sb.append(",").append(WS);
//                    }
//                    if(sb.charAt(sb.length() - 1) == ',' || sb.charAt(sb.length() - 1) == ' ') {
//                        sb.deleteCharAt(sb.length() - 1);
//                        if(sb.charAt(sb.length() - 1) == ',') {
//                            sb.deleteCharAt(sb.length() - 1);
//                        }
//                    }
                }
                sb.append(IND).append("]");
            } else if(obj.getClass().isArray()) {          // ???
                Object[] array = null;
                try {
                    array = (Object[]) obj;
                } catch(Exception e) {
                    log.log(Level.INFO, "Invalid array type.", e);
                }
                // ???
                _build(sb, Arrays.asList(array), includeWS, includeLB, lbAfterComma, indentSize, indentLevel);
                // sb.append(strArr);
//                sb.append("[").append(LB);
//                if(array!= null && array.length > 0) {
//                    sb.append(INDX);
//                    for(Object o : array) {
//                        String str = _build(o, includeWS, includeLB, indentSize, indentLevel);
//                        sb.append(str);
//                        sb.append(",").append(WS);
//                    }
//                    if(sb.charAt(sb.length() - 1) == ',' || sb.charAt(sb.length() - 1) == ' ') {
//                        sb.deleteCharAt(sb.length() - 1);
//                        if(sb.charAt(sb.length() - 1) == ',') {
//                            sb.deleteCharAt(sb.length() - 1);
//                        }
//                    }
//                }
//                sb.append(LB).append(IND).append("]");
            } else if(obj instanceof Collection<?>) {       // ???????
                Collection<Object> coll = null;
                try {
                    coll = (Collection<Object>) ((Collection<?>) obj);
                } catch(Exception e) {
                    log.log(Level.INFO, "Invalid collection type.", e);
                }
                sb.append("[").append(LB);
                if(coll != null && !coll.isEmpty()) {
                    sb.append(INDX);
                
                    Iterator<Object> it = coll.iterator();
                    while(it.hasNext()) {
                        Object o = it.next();
                        _build(sb, o, includeWS, includeLB, lbAfterComma, indentSize, indentLevel);
                        // sb.append(str);
                        if(it.hasNext()) {
                            sb.append(",");
                            if(lbAfterComma) {
                                sb.append(LB).append(INDX);
                            } else {
                                sb.append(WS);
                            }
                        } else {
                            sb.append(LB);
                        }
                    }
                }
                sb.append(IND).append("]");
            } else {
                // ???
                // TBD: indentSize, etc. ????
                // This actually causes infinite recursion.
                //    because a JsonSerializable object may use JsonBuilder for its implementation of JsonSerializable.toJsonString()
                // ???? How to fix this??? Is this fixable ???
//                if(obj instanceof JsonSerializable) {
//                    String jSerial = null;
//                    if(obj instanceof IndentedJsonSerializable) {
//                        jSerial = ((IndentedJsonSerializable) obj).toJsonString(indentSize);
//                    } else {
//                        jSerial = ((JsonSerializable) obj).toJsonString();
//                    }
//                    sb.append(jSerial);
//                // Note: this is better than JsonSerializable since it obeys the indentLevel rule, etc. 
//                // But, it seems more reasonable to use JsonSerializable first if the object implements both interfaces.
//                } else if(obj instanceof JsonCompatible) {
                if(obj instanceof JsonCompatible) {
                    // ????
                    Object jObj = null;
                    try {
                        jObj = ((JsonCompatible) obj).toJsonStructure();
                        // Use this.toJsonStructure(jObj) ???
                        // ...
                    } catch (JsonBuilderException e) {
                        // Ignore
                        log.log(Level.WARNING, "Failed to create JSON struct for a JsonCompatible object.", e);
                    }
                    if(jObj != null) {
                        _build(sb, jObj, includeWS, includeLB, lbAfterComma, indentSize, indentLevel);
                    } else {
                        // ???
                        String jcStr = obj.toString();
                        sb.append("\"").append(jcStr).append("\"");
                    }
                } else {
                    // primitive types... ???
                    // ...
                    String primStr = null;
                    if(obj instanceof Boolean) {
                        if(((Boolean) obj).equals(Boolean.TRUE)) {
                            primStr = Literals.TRUE;
                        } else {
                            primStr = Literals.FALSE;
                        }                        
                        sb.append(primStr);
                    } else if (obj instanceof Character) {
                        // ???
                        Character strChar = (Character) obj;
                        sb.append("\"").append(strChar).append("\"");
                    } else if (obj instanceof Number) {
//                        double d = ((Number) obj).doubleValue();
//                        jsonStr = Double.valueOf(d);
                        primStr = ((Number) obj).toString();
                        sb.append(primStr);
                    } else if(obj instanceof String) {
                        // ????
                        // Is there a better/faster way to do this?

                        // ???
                        primStr = (String) obj;
                        // sb.append("\"").append(primStr).append("\"");

                        // ???
                        int escapeForwardSlash = builderPolicy.escapeForwardSlash();
                        sb.append("\"");
                        if(primStr != null && !primStr.isEmpty()) {
                            char[] primChars = primStr.toCharArray();
                            char prevEc = 0;
                            for(char ec : primChars) {
                                if(Symbols.isEscapedChar(ec)) {
                                    if(prevEc == '<' && ec == '/') {
                                        if(escapeForwardSlash >= 0) {
                                            sb.append("\\/");
                                        } else {
                                            sb.append("/");
                                        }
                                    } else {
                                        String str = Symbols.getEscapedCharString(ec, escapeForwardSlash > 0 ? true : false);
                                        if(str != null) {
                                            sb.append(str);
                                        } else {
                                            // ???
                                            sb.append(ec);
                                        }
                                    }
                                } else if(CharacterUtil.isISOControl(ec)) {
                                    char[] uc = UnicodeUtil.getUnicodeHexCodeFromChar(ec);
                                    sb.append(uc);
                                } else { 
                                    sb.append(ec);
                                }
                                prevEc = ec;
                            }
                        }
                        sb.append("\"");
                        // ???
                        
                    } else {
                        
                        // TBD:
                        // java.util.Date ???
                        // and other JDK built-in class support???
                        // ..

                        if(obj instanceof Date) {
                            // TBD:
                            // Create a struct ???
                            primStr = ((Date) obj).toString();
                            // ...
                        } else {
                            
                            // TBD:
                            // POJO/Bean support???
                            // ...
                            
                            // ????
                            primStr = obj.toString();
                        }
                        sb.append("\"").append(primStr).append("\"");
                    }
                }
            }
        }
        
//        String jsonStr = sb.toString();
//        if(log.isLoggable(Level.FINE)) log.fine("jsonStr = " + jsonStr);
//        return jsonStr;
    }
    
    

    @SuppressWarnings("unchecked")
    private void _build(Writer writer, Object obj, int depth, boolean useBeanIntrospection, boolean includeWS, boolean includeLB, boolean lbAfterComma, int indentSize, int indentLevel) throws JsonBuilderException, IOException
    {
        if(depth < 0) {
            return;
        }

        // TBD:
        // Just use global vars ???
        ++indentLevel;
        String WS = "";
        if(includeWS) {
            WS = " ";
        }
        String LB = "";
        if(includeLB) {
            LB = "\n";
        }
        String IND = "";
        String INDX = "";
        if(indentSize > 0 && indentLevel > 0) {
            IND = String.format("%1$" + (indentSize * indentLevel) + "s", "");
        }
        if(indentSize > 0 && indentLevel >= 0) {
            INDX = String.format("%1$" + (indentSize * (indentLevel+1)) + "s", "");
        }

        if(obj == null || obj instanceof JsonNull) {
            writer.append(Literals.NULL);
        } else {
            // if(depth == 0) {
            if(depth <= 0) {
                if(obj instanceof JsonSerializable) { 
                    String jSerial = null;
                    if(obj instanceof IndentedJsonSerializable) {
                        jSerial = ((IndentedJsonSerializable) obj).toJsonString(indentSize);
                    } else {
                        jSerial = ((JsonSerializable) obj).toJsonString();
                    }
                    writer.append(jSerial);
                } else {
                    // TBD:
                    // This section of code is repeated when depth==0 and and when depth>0,
                    //     almost identically... (but not quite exactly the same)
                    // Need to be refactored.
                    // ....
                    String primStr = null;
                    if(obj instanceof Boolean) {
                        if(((Boolean) obj).equals(Boolean.TRUE)) {
                            primStr = Literals.TRUE;
                        } else {
                            primStr = Literals.FALSE;
                        }                        
                        writer.append(primStr);
                    } else if (obj instanceof Character) {
                        // ???
                        Character strChar = (Character) obj;
                        writer.append("\"").append(strChar).append("\"");
                    } else if (obj instanceof Number) {
//                        double d = ((Number) obj).doubleValue();
//                        jsonStr = Double.valueOf(d);
                        primStr = ((Number) obj).toString();
                        writer.append(primStr);
                    } else if(obj instanceof String) {
                        // ????
                        // Is there a better/faster way to do this?

                        // ???
                        primStr = (String) obj;
                        // writer.append("\"").append(primStr).append("\"");

                        // ???
                        writer.append("\"");
                        _appendEscapedString(writer, primStr);
                        writer.append("\"");
                        // ???
                        
                    } else {
                        
                        // TBD:
                        // java.util.Date ???
                        // and other JDK built-in class support???
                        // ..

                        if(obj instanceof Date) {
                            // TBD:
                            // Create a struct ???
                            primStr = ((Date) obj).toString();
                            // ...
                        } else {
                            
                            // TBD:
                            // POJO/Bean support???
                            // ...
                            
                            // ????
                            primStr = obj.toString();
                        }
                        // TBD: ?????
                        // writer.append("\"").append(primStr).append("\"");
                        writer.append("\"");
                        _appendEscapedString(writer, primStr);
                        writer.append("\"");
                    }
                }
            } else {
                if(obj instanceof Map<?,?>) {
                    Map<String,Object> map = null;
                    try {
                        map = (Map<String,Object>) ((Map<?,?>) obj);
                    } catch(Exception e) {
                        log.log(Level.INFO, "Invalid map type.", e);
                    }
                    writer.append("{").append(LB);
                    if(map != null && !map.isEmpty()) {
                        writer.append(INDX);
                        
                        Iterator<String> it = map.keySet().iterator();
                        while(it.hasNext()) {
                            String key = it.next();
                            Object val = map.get(key);
                            writer.append("\"").append(key).append("\":").append(WS);
                            _build(writer, val, depth - 1, useBeanIntrospection, includeWS, includeLB, lbAfterComma, indentSize, indentLevel);
                            if(it.hasNext()) {
                                writer.append(",");
                                if(lbAfterComma) {
                                    writer.append(LB).append(INDX);
                                } else {
                                    writer.append(WS);
                                }
                            } else {
                                writer.append(LB);
                            }
                        }
//                        for(String key : map.keySet()) {
//                            Object val = map.get(key);
//                            String str = _build(val, includeWS, includeLB, indentSize, indentLevel);
//                            sb.append("\"").append(key).append("\":").append(WS).append(str);
//                            sb.append(",").append(WS);
//                        }
//                        if(sb.charAt(sb.length() - 1) == ',' || sb.charAt(sb.length() - 1) == ' ') {
//                            sb.deleteCharAt(sb.length() - 1);
//                            if(sb.charAt(sb.length() - 1) == ',') {
//                                sb.deleteCharAt(sb.length() - 1);
//                            }
//                        }
                    }
                    writer.append(IND).append("}");
                } else if(obj instanceof List<?>) {
                    List<Object> list = null;
                    try {
                        list = (List<Object>) ((List<?>) obj);
                    } catch(Exception e) {
                        log.log(Level.INFO, "Invalid list type.", e);
                    }
                    writer.append("[").append(LB);
                    if(list != null && !list.isEmpty()) {
                        writer.append(INDX);
                        
                        Iterator<Object> it = list.iterator();
                        while(it.hasNext()) {
                            Object o = it.next();
                            _build(writer, o, depth - 1, useBeanIntrospection, includeWS, includeLB, lbAfterComma, indentSize, indentLevel);
                            if(it.hasNext()) {
                                writer.append(",");
                                if(lbAfterComma) {
                                    writer.append(LB).append(INDX);
                                } else {
                                    writer.append(WS);
                                }
                            } else {
                                writer.append(LB);
                            }
                        }
//                        for(Object o : list) {
//                            String str = _build(o, includeWS, includeLB, indentSize, indentLevel);
//                            sb.append(str);
//                            sb.append(",").append(WS);
//                        }
//                        if(sb.charAt(sb.length() - 1) == ',' || sb.charAt(sb.length() - 1) == ' ') {
//                            sb.deleteCharAt(sb.length() - 1);
//                            if(sb.charAt(sb.length() - 1) == ',') {
//                                sb.deleteCharAt(sb.length() - 1);
//                            }
//                        }
                    }
                    writer.append(IND).append("]");
                } else if(obj.getClass().isArray()) {          // ???
//                    // This causes class cast exception because some arrays are of a primitive type.
//                    //   (e.g., char[], etc.)
//                    Object[] array = null;
//                    try {
//                        array = (Object[]) obj;
//                    } catch(Exception e) {
//                        log.log(Level.INFO, "Invalid array type.", e);
//                    }
//                    // ???
//                    _build(writer, Arrays.asList(array), depth, useBeanIntrospection, includeWS, includeLB, lbAfterComma, indentSize, indentLevel);
                    
                    Object array = obj;
                    writer.append("[").append(LB);
                    if(array!= null && Array.getLength(array) > 0) {
                        int arrLen = Array.getLength(array);
                        writer.append(INDX);
                        for(int i=0; i<arrLen; i++) {
                            Object o = Array.get(array, i);
                            _build(writer, o, depth - 1, useBeanIntrospection, includeWS, includeLB, lbAfterComma, indentSize, indentLevel);
                            if(i < arrLen - 1) {
                                writer.append(",").append(WS);
                            }
                        }
                    }
                    writer.append(LB).append(IND).append("]");
                } else if(obj instanceof Collection<?>) {       // ???????
                    Collection<Object> coll = null;
                    try {
                        coll = (Collection<Object>) ((Collection<?>) obj);
                    } catch(Exception e) {
                        log.log(Level.INFO, "Invalid collection type.", e);
                    }
                    writer.append("[").append(LB);
                    if(coll != null && !coll.isEmpty()) {
                        writer.append(INDX);
                    
                        Iterator<Object> it = coll.iterator();
                        while(it.hasNext()) {
                            Object o = it.next();
                            _build(writer, o, depth - 1, useBeanIntrospection, includeWS, includeLB, lbAfterComma, indentSize, indentLevel);
                            if(it.hasNext()) {
                                writer.append(",");
                                if(lbAfterComma) {
                                    writer.append(LB).append(INDX);
                                } else {
                                    writer.append(WS);
                                }
                            } else {
                                writer.append(LB);
                            }
                        }
                    }
                    writer.append(IND).append("]");
                } else {
                    // ???
                    // TBD: indentLevel, etc. ??????
                    // ...
                    // This actually causes infinite recursion.
                    //    because a JsonSerializable object may use JsonBuilder for its implementation of JsonSerializable.toJsonString()
                    // ???? How to fix this??? Is this fixable ???
//                    if(obj instanceof JsonSerializable) { 
//                        String jSerial = null;
//                        if(obj instanceof IndentedJsonSerializable) {
//                            jSerial = ((IndentedJsonSerializable) obj).toJsonString(indentSize);
//                        } else {
//                            jSerial = ((JsonSerializable) obj).toJsonString();
//                        }
//                        writer.append(jSerial);   // Note that this is actually (partial) json string, not a string (that needs to be escaped).
//                    // Note: this is better than JsonSerializable since it obeys the indentLevel rule, etc. 
//                    // But, it seems more reasonable to use JsonSerializable first if the object implements both interfaces.
//                    } else if(obj instanceof JsonCompatible) {
                    if(obj instanceof JsonCompatible) {  
                        // ????
                        Object jObj = null;
                        try {
                            // which one to use???
                            // jObj = ((JsonCompatible) obj).toJsonStructure();        // Use "default" depth of the object???
                            jObj = ((JsonCompatible) obj).toJsonStructure(depth - 1);  // ???
                            // Use this.toJsonStructure(jObj) ???
                            // ...
                        } catch (JsonBuilderException e) {
                            // Ignore
                            log.log(Level.WARNING, "Failed to create JSON struct for a JsonCompatible object.", e);
                        }
                        if(jObj != null) {
                            _build(writer, jObj, depth, useBeanIntrospection, includeWS, includeLB, lbAfterComma, indentSize, indentLevel);
                        } else {
                            // ???
                            // TBD: String need to be escaped.
                            String jcStr = obj.toString();
                            // TBD: ?????
                            // writer.append("\"").append(jcStr).append("\"");
                            writer.append("\"");
                            _appendEscapedString(writer, jcStr);
                            writer.append("\"");
                        }
                    } else {
                        // primitive types... ???
                        // ...
                        String primStr = null;
                        if(obj instanceof Boolean) {
                            if(((Boolean) obj).equals(Boolean.TRUE)) {
                                primStr = Literals.TRUE;
                            } else {
                                primStr = Literals.FALSE;
                            }                        
                            writer.append(primStr);
                        } else if (obj instanceof Character) {
                            // ???
                            Character strChar = (Character) obj;
                            writer.append("\"").append(strChar).append("\"");
                        } else if (obj instanceof Number) {
//                            double d = ((Number) obj).doubleValue();
//                            jsonStr = Double.valueOf(d);
                            primStr = ((Number) obj).toString();
                            writer.append(primStr);
                        } else if(obj instanceof String) {
                            // ????
                            // Is there a better/faster way to do this?
    
                            // ???
                            primStr = (String) obj;
                            // writer.append("\"").append(primStr).append("\"");
    
                            // ???
                            writer.append("\"");
                            _appendEscapedString(writer, primStr);
                            writer.append("\"");
                            // ???
                            
                        } else {
                            
                            // TBD:
                            // java.util.Date ???
                            // and other JDK built-in class support???
                            // ..
    
                            if(obj instanceof Date) {
                                // TBD:
                                // Create a struct ???
                                primStr = ((Date) obj).toString();
                                // ...
                            } else {
                                

                                if(useBeanIntrospection) {
                                    Map<String, Object> mapEquivalent = null;
                                    try {
                                        // mapEquivalent = BeanIntrospectionUtil.introspect(obj, depth);   // depth? or depth - 1 ?
                                        // Because we are just converting a bean to a map,
                                        // the depth param is not used. (or, depth == 1).
                                        mapEquivalent = BeanIntrospectionUtil.introspect(obj);
                                    // } catch (IllegalAccessException
                                    //         | IllegalArgumentException
                                    //         | InvocationTargetException
                                    //         | IntrospectionException e) {
                                    } catch (Exception e) {
                                        // Ignore.
                                        if(log.isLoggable(Level.INFO)) log.log(Level.INFO, "Faild to introspect a bean.", e);
                                    }
                                    if(mapEquivalent != null) {
                                        _build(writer, mapEquivalent, depth, useBeanIntrospection, includeWS, includeLB, lbAfterComma, indentSize, indentLevel);  // Note: We do not change the depth.
                                    } else {
                                        
                                        // ????
                                        // primStr = null; ???
                                        primStr = obj.toString();
                                        // ...
                                    }

                                } else {
                                    
                                    // ????
                                    primStr = obj.toString();
                                    
                                }

                            }
                            // TBD: ?????
                            // writer.append("\"").append(primStr).append("\"");
                            writer.append("\"");
                            _appendEscapedString(writer, primStr);
                            writer.append("\"");
                        }
                    }
                }
            }
        }
    }

    
    private void _appendEscapedString(Writer writer, String primStr) throws IOException
    {
        int escapeForwardSlash = builderPolicy.escapeForwardSlash();
        if(primStr != null && !primStr.isEmpty()) {
            char[] primChars = primStr.toCharArray();
            char prevEc = 0;
            for(char ec : primChars) {
                if(Symbols.isEscapedChar(ec)) {
                    if(prevEc == '<' && ec == '/') {
                        if(escapeForwardSlash >= 0) {
                            writer.append("\\/");
                        } else {
                            writer.append("/");
                        }
                    // } else if(prevEc == '\\' && ec == '\\') {
                    //     // Already escaped... ????
                    //     // Skip ???
                    } else {
                        String str = Symbols.getEscapedCharString(ec, escapeForwardSlash > 0 ? true : false);
                        if(str != null) {
                            writer.append(str);
                        } else {
                            // ???
                            writer.append(ec);
                        }
                    }
                } else if(CharacterUtil.isISOControl(ec)) {
                    char[] uc = UnicodeUtil.getUnicodeHexCodeFromChar(ec);
                    writer.write(uc);
                } else { 
                    writer.append(ec);
                }
                prevEc = ec;
            }
        }

    }
    
    
}
