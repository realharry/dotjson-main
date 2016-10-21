package org.minijson.builder.partial.impl;

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
import org.minijson.builder.JsonBuilderException;
import org.minijson.builder.core.IndentInfoStruct;
import org.minijson.builder.partial.MixedJsonBuilder;
import org.minijson.builder.policy.BuilderPolicy;
import org.minijson.builder.policy.base.DefaultBuilderPolicy;
import org.minijson.builder.util.BeanIntrospectionUtil;
import org.minijson.common.JsonNull;
import org.minijson.common.Literals;
import org.minijson.common.Symbols;
import org.minijson.parser.JsonParser;
import org.minijson.parser.impl.AbstractBareJsonParser;
import org.minijson.parser.impl.SimpleJsonParser;
import org.minijson.util.CharacterUtil;
import org.minijson.util.UnicodeUtil;


/**
 * Default MixedJsonBuilder implementation.
 * Any string at the depth level (Or, any string at or below the depth level  ????)
 *      is interpreted as JSON string (representing a sub-tree)
 *      rather than a string.
 */
public abstract class AbstractMixedJsonBuilder implements MixedJsonBuilder
{
    private static final Logger log = Logger.getLogger(AbstractMixedJsonBuilder.class.getName());

    // Default value.
    // Max value: equivalent to -1.
    private static final int DEFAULT_MAX_DRILL_DOWN_DEPTH = (int) Byte.MAX_VALUE;  // Arbitrary.
    private static final int MAXIMUM_MIN_DRILL_DOWN_DEPTH = DEFAULT_MAX_DRILL_DOWN_DEPTH + 1;   // Note + 1.
    private static final int DEFAULT_MIN_DRILL_DOWN_DEPTH = 0;  // 0 or 1??
    
    // "strategy" for building json structure.
    // No setters for builderPolicy (except through a ctor).
    private final BuilderPolicy builderPolicy;

    // Not being used.
    // TBD: Not sure if we can ensure thread safety.
    private final boolean threadSafe;
    
    // TBD:
    // private final JsonParser jsonParser;


    // Note:
    // It's important not to keep any "state" as class variables for this class
    //   so that a single instance can be used for multiple/concurrent build operations.
    // (Often, the recursive implementation may involve multiple objects (each as a node in an object tree),
    //   which may use the same/single instance of this builder class.)
    // ...


    public AbstractMixedJsonBuilder()
    {
        this(null);
    }
    public AbstractMixedJsonBuilder(BuilderPolicy builderPolicy)
    {
        this(builderPolicy, false);     // true or false ????
    }
    public AbstractMixedJsonBuilder(BuilderPolicy builderPolicy, boolean threadSafe)
    {
        if(builderPolicy == null) {
            this.builderPolicy = DefaultBuilderPolicy.MINIJSON;
        } else {
            this.builderPolicy = builderPolicy;
        }
        this.threadSafe = threadSafe;
        
//        // ???
//        // jsonParser = new AbstractBareJsonParser() {};
//        jsonParser = new SimpleJsonParser();
//        // ....
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
        int maxDepth = builderPolicy.drillDownDepth();
        return buildMixed(jsonObj, MAXIMUM_MIN_DRILL_DOWN_DEPTH, maxDepth);
    }
    @Override
    public void build(Writer writer, Object jsonObj) throws IOException, JsonBuilderException
    {
        int maxDepth = builderPolicy.drillDownDepth();
        buildMixed(writer, jsonObj, MAXIMUM_MIN_DRILL_DOWN_DEPTH, maxDepth);
    }

    @Override
    public String buildMixed(Object jsonObj) throws JsonBuilderException
    {
        return buildMixed(jsonObj, DEFAULT_MIN_DRILL_DOWN_DEPTH, DEFAULT_MAX_DRILL_DOWN_DEPTH);
    }
    @Override
    public void buildMixed(Writer writer, Object jsonObj) throws JsonBuilderException, IOException
    {
        buildMixed(writer, jsonObj, DEFAULT_MIN_DRILL_DOWN_DEPTH, DEFAULT_MAX_DRILL_DOWN_DEPTH);
    }

    // @Override
    public String buildMixed(Object jsonObj, int minDepth) throws JsonBuilderException
    {
        return buildMixed(jsonObj, minDepth, DEFAULT_MAX_DRILL_DOWN_DEPTH);
    }
    // @Override
    public void buildMixed(Writer writer, Object jsonObj, int minDepth) throws JsonBuilderException, IOException
    {
        buildMixed(writer, jsonObj, minDepth, DEFAULT_MAX_DRILL_DOWN_DEPTH);
    }

    @Override
    public String buildMixed(Object jsonObj, int minDepth, int maxDepth) throws JsonBuilderException
    {
        return buildMixed(jsonObj, minDepth, maxDepth, 0);
    }
    @Override
    public void buildMixed(Writer writer, Object jsonObj, int minDepth, int maxDepth) throws JsonBuilderException, IOException
    {
        buildMixed(writer, jsonObj, minDepth, maxDepth, 0);
    }


    @Override
    public String buildMixed(Object jsonObj, int minDepth, int maxDepth, int indent) throws JsonBuilderException
    {
        String jsonStr = null;
        StringWriter writer = new StringWriter();
        try {
            buildMixed(writer, jsonObj, minDepth, maxDepth, indent);
            jsonStr = writer.toString();
            if(log.isLoggable(Level.FINE)) log.fine("jsonStr = " + jsonStr);
        } catch (IOException e) {
            log.log(Level.WARNING, "Failed to write jsonObj as JSON.", e);
        }
        return jsonStr;
    }

    @Override
    public void buildMixed(Writer writer, Object jsonObj, int minDepth, int maxDepth, int indent) throws JsonBuilderException, IOException
    {
        if(minDepth < 0) {
            minDepth = MAXIMUM_MIN_DRILL_DOWN_DEPTH;
        }
        if(maxDepth < 0) {
            maxDepth = DEFAULT_MAX_DRILL_DOWN_DEPTH;
        }
        // Note that minDepth does not have to be smaller than maxDepth.
        // 0 <= minDepth <= maxDpeth+1.
        int cutoff = maxDepth - minDepth;

        boolean useBeanIntrospection = this.builderPolicy.useBeanIntrospection();

        IndentInfoStruct indentInfo = new IndentInfoStruct(indent);
        boolean includeWS = indentInfo.isIncludingWhiteSpaces();
        boolean includeLB = indentInfo.isIncludingLineBreaks();
        boolean lbAfterComma = indentInfo.isLineBreakingAfterComma();
        int indentSize = indentInfo.getIndentSize();
        int indentLevel = -1;
        
        _build(writer, jsonObj, cutoff, maxDepth, useBeanIntrospection, includeWS, includeLB, lbAfterComma, indentSize, indentLevel);
        writer.flush();   // ???

//        String jsonStr = writer.toString();
//        if(log.isLoggable(Level.FINE)) log.fine("jsonStr = " + jsonStr);

    }


    @SuppressWarnings("unchecked")
    private void _build(Writer writer, Object obj, int cutoff, int depth, boolean useBeanIntrospection, boolean includeWS, boolean includeLB, boolean lbAfterComma, int indentSize, int indentLevel) throws JsonBuilderException, IOException
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

                        // For "MixedJsonParser",
                        // when the tree traversing reaches the depth == 0 (or, maxDepth down from the beginning)
                        // we treat string values (and, only strings) differently.
                        // We treat the string as a JSON string (e.g., corresponding to an object) not as a regular string...
                        // ...

                        if(depth <= cutoff) {
                            // (a) because we treat it as a JSON string...
                            // TBD:
                            if(primStr.startsWith(Symbols.LCURLY_STR) || primStr.startsWith(Symbols.LSQUARE_STR) ) {   // No leading spaces allowed.
                                writer.append(primStr);
                            } else {
                                // All the rest is just considered a string, including numbers, etc.
                                writer.append("\"");
                                _appendEscapedString(writer, primStr);
                                writer.append("\"");
                            }
                            // ...
                        } else {
                            // ???
                            // (b) If it was a regular string...
                            writer.append("\"");
                            _appendEscapedString(writer, primStr);
                            writer.append("\"");
                            // ???
                        }

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

                        if(depth <= cutoff) {
                            // ????
                            String objJsonStr = build(obj);
                            if(objJsonStr.startsWith(Symbols.LCURLY_STR) || objJsonStr.startsWith(Symbols.LSQUARE_STR) ) {   // No leading spaces allowed.
                                writer.append(objJsonStr);
                            } else {
                                // All the rest is just considered a string, including numbers, etc.
                                writer.append("\"");
                                _appendEscapedString(writer, objJsonStr);
                                writer.append("\"");
                            }
                        } else {
                            writer.append("\"");
                            _appendEscapedString(writer, primStr);
                            writer.append("\"");
                        }
                    
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
                            _build(writer, val, cutoff, depth - 1, useBeanIntrospection, includeWS, includeLB, lbAfterComma, indentSize, indentLevel);
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
                            _build(writer, o, cutoff, depth - 1, useBeanIntrospection, includeWS, includeLB, lbAfterComma, indentSize, indentLevel);
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
                            _build(writer, o, cutoff, depth - 1, useBeanIntrospection, includeWS, includeLB, lbAfterComma, indentSize, indentLevel);
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
                            _build(writer, o, cutoff, depth - 1, useBeanIntrospection, includeWS, includeLB, lbAfterComma, indentSize, indentLevel);
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
                            _build(writer, jObj, cutoff, depth, useBeanIntrospection, includeWS, includeLB, lbAfterComma, indentSize, indentLevel);
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
    

                            // For "MixedJsonParser",
                            // when the tree traversing reaches the depth == 0 (or, maxDepth down from the beginning)
                            // we treat string values (and, only strings) differently.
                            // We treat the string as a JSON string (e.g., corresponding to an object) not as a regular string...
                            // ...

                            if(depth <= cutoff) {
                                // (a) because we treat it as a JSON string...
                                // TBD:
                                if(primStr.startsWith(Symbols.LCURLY_STR) || primStr.startsWith(Symbols.LSQUARE_STR) ) {   // No leading spaces allowed.
                                    writer.append(primStr);
                                } else {
                                    // All the rest is just considered a string, including numbers, etc.
                                    writer.append("\"");
                                    _appendEscapedString(writer, primStr);
                                    writer.append("\"");
                                }
                                // ...
                            } else {
                                // ???
                                // (b) If it was a regular string...
                                writer.append("\"");
                                _appendEscapedString(writer, primStr);
                                writer.append("\"");
                                // ???
                            }
                            
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
                                        _build(writer, mapEquivalent, cutoff, depth, useBeanIntrospection, includeWS, includeLB, lbAfterComma, indentSize, indentLevel);  // Note: We do not change the depth.
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
//                            writer.append("\"");
//                            _appendEscapedString(writer, primStr);
//                            writer.append("\"");

                            if(depth <= cutoff) {
                                // ????
                                String objJsonStr = build(obj);
                                if(objJsonStr.startsWith(Symbols.LCURLY_STR) || objJsonStr.startsWith(Symbols.LSQUARE_STR) ) {   // No leading spaces allowed.
                                    writer.append(objJsonStr);
                                } else {
                                    // All the rest is just considered a string, including numbers, etc.
                                    writer.append("\"");
                                    _appendEscapedString(writer, objJsonStr);
                                    writer.append("\"");
                                }
                            } else {
                                writer.append("\"");
                                _appendEscapedString(writer, primStr);
                                writer.append("\"");
                            }

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
