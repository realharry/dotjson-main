using HoloJson.Builder.Core;
using HoloJson.Builder.Policy;
using HoloJson.Builder.Policy.Base;
using HoloJson.Builder.Util;
using HoloJson.Common;
using HoloJson.Core;
using HoloJson.Trait;
using HoloJson.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoloJson.Builder.Impl
{
    public abstract class AbstractBareJsonBuilder : BareJsonBuilder
    {
        // Default value.
        // private static final int DEF_DRILL_DOWN_DEPTH = 2;
        // Max value: equivalent to -1.
        private static readonly int MAX_DRILL_DOWN_DEPTH = (int)sbyte.MaxValue; // Arbitrary.
                                                                                // "strategy" for building json structure.
                                                                                // No setters for builderPolicy (except through a ctor).
        private readonly BuilderPolicy builderPolicy;
        // Not being used.
        // TBD: Not sure if we can ensure thread safety.
        private readonly bool threadSafe;
        // Note:
        // It's important not to keep any "state" as class variables for this class
        //   so that a single instance can be used for multiple/concurrent build operations.
        // (Often, the recursive implementation may involve multiple objects (each as a node in an object tree),
        //   which may use the same/single instance of this builder class.)
        // ...
        public AbstractBareJsonBuilder() : this(null)
        {
        }
        public AbstractBareJsonBuilder(BuilderPolicy builderPolicy) : this(builderPolicy, false) // true or false ????
        {
        }
        public AbstractBareJsonBuilder(BuilderPolicy builderPolicy, bool threadSafe)
        {
            if (builderPolicy == null) {
                this.builderPolicy = DefaultBuilderPolicy.MINIJSON;
            } else {
                this.builderPolicy = builderPolicy;
            }
            this.threadSafe = threadSafe;
        }

        public virtual BuilderPolicy BuilderPolicy
        {
            get
            {
                return this.builderPolicy;
            }
        }
        //    public void setBuilderPolicy(BuilderPolicy builderPolicy)
        //    {
        //        this.builderPolicy = builderPolicy;
        //    }
        public virtual async Task<string> BuildAsync(object jsonObj)
        {
            return await BuildAsync(jsonObj, 0);
        }
        public virtual async Task<string> BuildAsync(object jsonObj, int indent)
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
            string jsonStr = null;
            StringWriter writer = new StringWriter();
            try {
                // _build(writer, jsonObj, depth, useBeanIntrospection, includeWS, includeLB, lbAfterComma, indentSize, indentLevel);
                // writer.flush();   // ???
                await BuildAsync(writer, jsonObj, indent);
                jsonStr = writer.ToString();
                // log.warning(">>>>>>>>>>>>>>>>>>>> jsonStr = " + jsonStr);
                // String jsonStr2 = writer.getBuffer().toString();
                // log.warning(">>>>>>>>>>>>>>>>>>>> jsonStr2 = " + jsonStr2);
                //if (log.isLoggable(Level.FINE)) {
                //    log.fine("jsonStr = " + jsonStr);
                //}
            } catch (IOException e) {
                //log.log(Level.WARNING, "Failed to write jsonObj as JSON.", e);
            }
            return jsonStr;
        }
        public virtual async Task BuildAsync(TextWriter writer, object jsonObj)
        {
            await BuildAsync(writer, jsonObj, 0);
        }
        public virtual async Task BuildAsync(TextWriter writer, object jsonObj, int indent)
        {
            int maxDepth = builderPolicy.DrillDownDepth;
            if (maxDepth < 0) {
                maxDepth = MAX_DRILL_DOWN_DEPTH;
            }
            bool useBeanIntrospection = this.builderPolicy.UseBeanIntrospection;

            IndentInfoStruct indentInfo = new IndentInfoStruct(indent);
            bool includeWS = indentInfo.IsIncludingWhiteSpaces;
            bool includeLB = indentInfo.IsIncludingLineBreaks;
            bool lbAfterComma = indentInfo.IsLineBreakingAfterComma;
            int indentSize = indentInfo.IndentSize;
            int indentLevel = -1;

            await _buildAsync(writer, jsonObj, maxDepth, useBeanIntrospection, includeWS, includeLB, lbAfterComma, indentSize, indentLevel);
            // TBD:
            // writer.flush(); // ???
            // ???

            //        String jsonStr = writer.toString();
            //        if(log.isLoggable(Level.FINE)) log.fine("jsonStr = " + jsonStr);
        }

        ///////////////////////////////////////////////////////
        // TBD: JsonBuilder (~~ JsonCompatible) interface..
        // ....
        public virtual async Task<object> BuildJsonStructureAsync(object obj)
        {
            int maxDepth = builderPolicy.DrillDownDepth;
            return await BuildJsonStructureAsync(obj, maxDepth);
            // return ToJsonStructure(obj, DEF_DRILL_DOWN_DEPTH);
        }
        public virtual async Task<object> BuildJsonStructureAsync(object obj, int depth)
        {
            if (depth < 0) {
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
            int maxDepth = builderPolicy.DrillDownDepth;
            if (depth > maxDepth) {
                //if (log.isLoggable(Level.INFO)) {
                //    log.info("Input depth, " + depth + ", is greater than the policy drillDownDepth, " + maxDepth + ". Using the drillDownDepth.");
                //}
                depth = maxDepth;
            }
            bool useBeanIntrospection = this.builderPolicy.UseBeanIntrospection;
            return await _buildJsonStructAsync(obj, depth, useBeanIntrospection);
        }

        // TBD:
        // The problem with this algo is we have no way to consistently represent null node.
        // For map value and list element, we can just use Java null.
        // But, in general, it may not be possible.....  ????    Is this true????
        // Seems to be working so far (based on the limited unit test cases...)
        private async Task<object> _buildJsonStructAsync(object obj, int depth, bool useBeanIntrospection)
        {
            //        if(depth < 0) {
            //            return null;
            //        }
            object jsonStruct = null;
            if (obj == null || obj is JsonNull) { // ????
                                                  // return null;
            } else {
                // log.warning(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>> depth = " + depth);
                // if(depth == 0) {
                if (depth <= 0) {
                    if (obj is bool?) {
                        jsonStruct = (bool?)obj;
                    } else if (obj is char?) {
                        jsonStruct = (char?)obj;
                    } else if (obj is Number) {
                        jsonStruct = (Number)obj;
                    } else if (obj is string) {
                        // Note that this string is not a "json string" (e.g., forward slash escaped, etc.)
                        // e.g., if the string is "...\\/...", we will read it as "...\\/..." not as ".../...".
                        jsonStruct = (string)obj;
                    } else {
                        // ????
                        jsonStruct = obj.ToString();
                    }
                } else {
                    // if(obj instanceof Map<?,?>)
                    if (obj is IDictionary<string, object>) {
                        IDictionary<string, object> jsonMap = new Dictionary<string, object>();

                        IDictionary<string, object> map = null;
                        try {
                            // map = (Map<String,Object>)((Map<?,?>) obj);
                            map = (IDictionary<string, object>) obj;
                        } catch (Exception e) {
                            // log.log(Level.WARNING, "Invalid map type.", e);
                            // What to do???
                            // Use map.toString???
                        }
                        if (map != null && map.Count > 0) {
                            foreach (string f in map.Keys) {
                                object val = map[f];
                                object jsonVal = await _buildJsonStructAsync(val, depth - 1, useBeanIntrospection);
                                if (jsonVal != null) {
                                    jsonMap[f] = jsonVal;
                                } else {
                                    // ???
                                    jsonMap[f] = null;
                                }
                            }
                        }
                        jsonStruct = jsonMap;
                    }
                    // else if(obj instanceof List<?>)
                    else if (obj is IList<object>) {
                        IList<object> jsonList = new List<object>();

                        IList<object> list = null;
                        try {
                            // list = (List<Object>)((List<?>) obj);
                            list = (IList<object>)obj;
                        } catch (Exception e) {
                            //log.log(Level.WARNING, "Invalid list type.", e);
                            // What to do???
                            // Use list.toString???
                        }
                        if (list != null && list.Count > 0) {
                            foreach (object v in list) {
                                object jsonVal = await _buildJsonStructAsync(v, depth - 1, useBeanIntrospection);
                                if (jsonVal != null) {
                                    jsonList.Add(jsonVal);
                                } else {
                                    // ???
                                    jsonList.Add(null);
                                }
                            }
                        }
                        jsonStruct = jsonList;
                    } else if (obj.GetType().IsArray) { // ???
                        IList<object> jsonList = new List<object>();
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
                        // ?????
                        object[] array = obj as object[];
                        if (array != null && array.Length > 0) {
                            int arrLen = array.Length;
                            // log.warning(">>>>>>>>>>>>>>>>>>>>>>>>>>>>> arrLen = " + arrLen);
                            for (int i = 0; i < arrLen; i++) {
                                object o = array[i];
                                // log.warning(">>>>>>>>>>>>>>>>>>>>>>>>>>>>> o = " + o + "; " + o.getClass());
                                object jsonVal = await _buildJsonStructAsync(o, depth - 1, useBeanIntrospection);
                                // log.warning(">>>>>>>>>>>>>>>>>>>>>>>>>>>>> jsonVal = " + jsonVal + "; " + o.getClass());
                                if (jsonVal != null) {
                                    jsonList.Add(jsonVal);
                                } else {
                                    // ???
                                    jsonList.Add(null);
                                }
                            }
                        }
                        jsonStruct = jsonList;
                    }
                      // else if(obj instanceof Collection<?>)
                      else if (obj is ICollection<object>) {
                        IList<object> jsonList = new List<object>();
                        // jsonList.addAll((Collection<Object>) ((Collection<?>) obj));

                        // Iterator<Object> it = ((Collection<Object>)((Collection<?>) obj)).iterator();
                        IEnumerator<object> it = ((ICollection<object>)((ICollection<object>)obj)).GetEnumerator();
                        while (it.MoveNext()) {
                            object o = it.Current;
                            object jsonVal = await _buildJsonStructAsync(o, depth - 1, useBeanIntrospection);
                            if (jsonVal != null) {
                                jsonList.Add(jsonVal);
                            } else {
                                // ???
                                jsonList.Add(null);
                            }
                        }
                        jsonStruct = jsonList;
                    } else {
                        // ???
                        // This can potentially cause infinite recursion.
                        // because maybe JsonCompatible object implements ToJsonStructure() using JsonBuilder.buidJsonStructure()
                        // which calls the object's ToJsonStructure(), which calls JsonBuilder.buidJsonStructure(), etc.
                        // ....
                        // if(obj instanceof JsonCompatible) {
                        //     jsonStruct = ((JsonCompatible) obj).ToJsonStructure(depth);
                        // } else {
                        // primitive types... ???
                        if (obj is bool?) {
                            jsonStruct = (bool?)obj;
                        } else if (obj is char?) {
                            jsonStruct = (char?)obj;
                        } else if (obj is Number) {
                            jsonStruct = (Number)obj;
                        } else if (obj is string) {
                            jsonStruct = (string)obj;
                        } else {
                            // Use inspection....
                            // TBD:                            // BuilderPolicy ???
                            if (useBeanIntrospection) {
                                IDictionary<string, object> mapEquivalent = null;
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
                                    //if (log.isLoggable(Level.INFO)) {
                                    //    log.log(Level.INFO, "Faild to introspect a bean.", e);
                                    //}
                                }
                                if (mapEquivalent != null) {
                                    jsonStruct = await _buildJsonStructAsync(mapEquivalent, depth, useBeanIntrospection); // Note: We do not change the depth.
                                } else {

                                    // ????
                                    // jsonStruct = null; ???
                                    jsonStruct = obj.ToString();
                                    // ...
                                }
                                // log.warning(">>>>>>>>>>>>>>>>>>>>>>>>>>>>> jsonStruct = " + jsonStruct);
                            } else {
                                // ????
                                // jsonStruct = null; ???
                                jsonStruct = obj.ToString();
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
        private async Task _buildAsync(StringBuilder sb, object obj, bool includeWS, bool includeLB, bool lbAfterComma, int indentSize, int indentLevel)
        {
            // TBD:
            // Just use global vars ???
            ++indentLevel;
            string WS = "";
            if (includeWS) {
                WS = " ";
            }
            string LB = "";
            if (includeLB) {
                LB = "\n";
            }
            string IND = "";
            string INDX = "";
            if (indentSize > 0 && indentLevel > 0) {
                // IND = String.format("%1$" + (indentSize * indentLevel) + "s", "");
                IND = string.Format("{0," + (indentSize * indentLevel) + "}", "");
            }
            if (indentSize > 0 && indentLevel >= 0) {
                // INDX = String.format("%1$" + (indentSize * (indentLevel+1)) + "s", "");
                INDX = string.Format("{0," + (indentSize * (indentLevel + 1)) + "}", "");
            }

            // StringBuilder sb = new StringBuilder();
            if (obj == null) {
                sb.Append(Literals.NULL);
            } else {
                // if(obj instanceof Map<?,?>)
                if (obj is IDictionary<string, object>) {
                    IDictionary<string, object> map = null;
                    try {
                        // map = (Map<String,Object>)((Map<?,?>) obj);
                        map = (IDictionary<string, object>)obj;
                    } catch (Exception e) {
                        // log.log(Level.INFO, "Invalid map type.", e);
                    }
                    sb.Append("{").Append(LB);
                    if (map != null && map.Count > 0) {
                        sb.Append(INDX);

                        var isFirst = true;
                        IEnumerator<string> it = map.Keys.GetEnumerator();
                        while (it.MoveNext()) {
                            if(isFirst) {
                                isFirst = false;
                            } else {
                                sb.Append(",");
                                if (lbAfterComma) {
                                    sb.Append(LB).Append(INDX);
                                } else {
                                    sb.Append(WS);
                                }
                            }
                            string key = it.Current;
                            object val = map[key];
                            sb.Append("\"").Append(key).Append("\":").Append(WS);
                            await _buildAsync(sb, val, includeWS, includeLB, lbAfterComma, indentSize, indentLevel);
                            // sb.Write("\""); writer.Write(key); writer.Write("\":"); writer.Write(WS); writer.Write(str);
                        }
                        sb.Append(LB);
                        //                    for(String key : map.keySet()) {
                        //                        Object val = map.get(key);
                        //                        String str = _build(val, includeWS, includeLB, indentSize, indentLevel);
                        //                        sb.Write("\""); writer.Write(key); writer.Write("\":"); writer.Write(WS); writer.Write(str);
                        //                        sb.Write(","); writer.Write(WS);
                        //                    }
                        //                    if(sb.charAt(sb.length() - 1) == ',' || sb.charAt(sb.length() - 1) == ' ') {
                        //                        sb.deleteCharAt(sb.length() - 1);
                        //                        if(sb.charAt(sb.length() - 1) == ',') {
                        //                            sb.deleteCharAt(sb.length() - 1);
                        //                        }
                        //                    }
                    }
                    sb.Append(IND).Append("}");
                }
                // else if(obj instanceof List<?>)
                else if (obj is IList<object>) {
                    IList<object> list = null;
                    try {
                        // list = (List<Object>)((List<?>) obj);
                        list = (IList<object>)obj;
                    } catch (Exception e) {
                        // log.log(Level.INFO, "Invalid list type.", e);
                    }
                    sb.Append("[").Append(LB);
                    if (list != null && list.Count > 0) {
                        sb.Append(INDX);

                        var isFirst = true;
                        IEnumerator<object> it = list.GetEnumerator();
                        while (it.MoveNext()) {
                            if (isFirst) {
                                isFirst = false;
                            } else {
                                sb.Append(",");
                                if (lbAfterComma) {
                                    sb.Append(LB).Append(INDX);
                                } else {
                                    sb.Append(WS);
                                }
                            }
                            object o = it.Current;
                            await _buildAsync(sb, o, includeWS, includeLB, lbAfterComma, indentSize, indentLevel);
                            // sb.Write(str);
                        }
                        sb.Append(LB);
                        //                    for(Object o : list) {
                        //                        String str = _build(o, includeWS, includeLB, indentSize, indentLevel);
                        //                        sb.Write(str);
                        //                        sb.Write(","); writer.Write(WS);
                        //                    }
                        //                    if(sb.charAt(sb.length() - 1) == ',' || sb.charAt(sb.length() - 1) == ' ') {
                        //                        sb.deleteCharAt(sb.length() - 1);
                        //                        if(sb.charAt(sb.length() - 1) == ',') {
                        //                            sb.deleteCharAt(sb.length() - 1);
                        //                        }
                        //                    }
                    }
                    sb.Append(IND).Append("]");
                } else if (obj.GetType().IsArray) { // ???
                    object[] array = null;
                    try {
                        array = (object[])obj;
                    } catch (Exception e) {
                        // log.log(Level.INFO, "Invalid array type.", e);
                    }
                    // ???
                    await _buildAsync(sb, array, includeWS, includeLB, lbAfterComma, indentSize, indentLevel);
                    // sb.Write(strArr);
                    //                sb.Write("["); writer.Write(LB);
                    //                if(array!= null && array.length > 0) {
                    //                    sb.Write(INDX);
                    //                    for(Object o : array) {
                    //                        String str = _build(o, includeWS, includeLB, indentSize, indentLevel);
                    //                        sb.Write(str);
                    //                        sb.Write(","); writer.Write(WS);
                    //                    }
                    //                    if(sb.charAt(sb.length() - 1) == ',' || sb.charAt(sb.length() - 1) == ' ') {
                    //                        sb.deleteCharAt(sb.length() - 1);
                    //                        if(sb.charAt(sb.length() - 1) == ',') {
                    //                            sb.deleteCharAt(sb.length() - 1);
                    //                        }
                    //                    }
                    //                }
                    //                sb.Write(LB); writer.Write(IND); writer.Write("]");
                }
                  // else if(obj instanceof Collection<?>)
                  else if (obj is ICollection<object>) { // ???????
                    ICollection<object> coll = null;
                    try {
                        // coll = (Collection<Object>)((Collection<?>) obj);
                        coll = (ICollection<object>)((ICollection<object>)obj);
                    } catch (Exception e) {
                        // log.log(Level.INFO, "Invalid collection type.", e);
                    }
                    sb.Append("[").Append(LB);
                    if (coll != null && coll.Count > 0) {
                        sb.Append(INDX);

                        var isFirst = true;
                        IEnumerator<object> it = coll.GetEnumerator();
                        while (it.MoveNext()) {
                            if (isFirst) {
                                isFirst = false;
                            } else {
                                sb.Append(",");
                                if (lbAfterComma) {
                                    sb.Append(LB).Append(INDX);
                                } else {
                                    sb.Append(WS);
                                }
                            }
                            object o = it.Current;
                            await _buildAsync(sb, o, includeWS, includeLB, lbAfterComma, indentSize, indentLevel);
                            // sb.Write(str);
                        }
                        sb.Append(LB);
                    }
                    sb.Append(IND).Append("]");
                } else {
                    // ???
                    // TBD: indentSize, etc. ????
                    // This actually causes infinite recursion.
                    //    because a JsonSerializable object may use JsonBuilder for its implementation of JsonSerializable.ToJsonString()
                    // ???? How to fix this??? Is this fixable ???
                    //                if(obj instanceof JsonSerializable) {
                    //                    String jSerial = null;
                    //                    if(obj instanceof IndentedJsonSerializable) {
                    //                        jSerial = ((IndentedJsonSerializable) obj).ToJsonString(indentSize);
                    //                    } else {
                    //                        jSerial = ((JsonSerializable) obj).ToJsonString();
                    //                    }
                    //                    sb.Write(jSerial);
                    //                // Note: this is better than JsonSerializable since it obeys the indentLevel rule, etc. 
                    //                // But, it seems more reasonable to use JsonSerializable first if the object implements both interfaces.
                    //                } else if(obj instanceof JsonCompatible) {
                    if (obj is JsonCompatible) {
                        // ????
                        object jObj = null;
                        try {
                            jObj = ((JsonCompatible)obj).ToJsonStructure();
                            // Use this.ToJsonStructure(jObj) ???
                            // ...
                        } catch (JsonBuilderException e) {
                            // Ignore
                            // log.log(Level.WARNING, "Failed to create JSON struct for a JsonCompatible object.", e);
                        }
                        if (jObj != null) {
                            await _buildAsync(sb, jObj, includeWS, includeLB, lbAfterComma, indentSize, indentLevel);
                        } else {
                            // ???
                            string jcStr = obj.ToString();
                            sb.Append("\"").Append(jcStr).Append("\"");
                        }
                    } else {
                        // primitive types... ???
                        // ...
                        string primStr = null;
                        if (obj is bool?) {
                            if (((bool?)obj).Equals(true)) {
                                primStr = Literals.TRUE;
                            } else {
                                primStr = Literals.FALSE;
                            }
                            sb.Append(primStr);
                        } else if (obj is char?) {
                            // ???
                            char? strChar = (char?)obj;
                            sb.Append("\"").Append(strChar).Append("\"");
                        } else if (obj is Number) {
                            //                        double d = ((Number) obj).doubleValue();
                            //                        jsonStr = Double.valueOf(d);
                            primStr = ((Number)obj).ToString();
                            sb.Append(primStr);
                        } else if (obj is string) {
                            // ????
                            // Is there a better/faster way to do this?
                            // ???
                            primStr = (string)obj;
                            // sb.Write("\""); writer.Write(primStr); writer.Write("\"");
                            // ???
                            int escapeForwardSlash = builderPolicy.EscapeForwardSlash;
                            sb.Append("\"");
                            if (!string.ReferenceEquals(primStr, null) && primStr.Length > 0) {
                                char[] primChars = primStr.ToCharArray();
                                char prevEc = (char)0;
                                foreach (char ec in primChars) {
                                    if (Symbols.IsEscapedChar(ec)) {
                                        if (prevEc == '<' && ec == '/') {
                                            if (escapeForwardSlash >= 0) {
                                                sb.Append("\\/");
                                            } else {
                                                sb.Append("/");
                                            }
                                        } else {
                                            string str = Symbols.GetEscapedCharString(ec, escapeForwardSlash > 0 ? true : false);
                                            if (!string.ReferenceEquals(str, null)) {
                                                sb.Append(str);
                                            } else {
                                                // ???
                                                sb.Append(ec);
                                            }
                                        }
                                    } else if (CharUtil.IsISOControl(ec)) {
                                        char[] uc = UnicodeUtil.GetUnicodeHexCodeFromChar(ec);
                                        sb.Append(uc);
                                    } else {
                                        sb.Append(ec);
                                    }
                                    prevEc = ec;
                                }
                            }
                            sb.Append("\"");
                            // ???
                        } else {
                            // TBD:
                            // java.util.Date ???                         // and other JDK built-in class support???
                            if (obj is DateTime) {
                                // TBD:                             // Create a struct ???
                                primStr = ((DateTime)obj).ToString();
                                // ...
                            } else {
                                // TBD:                            // POJO/Bean support???
                                // ...
                                // ????
                                primStr = obj.ToString();
                            }
                            sb.Append("\"").Append(primStr).Append("\"");
                        }
                    }
                }
            }
            //        String jsonStr = sb.toString();
            //        if(log.isLoggable(Level.FINE)) log.fine("jsonStr = " + jsonStr);
            //        return jsonStr;
        }

        private async Task _buildAsync(TextWriter writer, object obj, int depth, bool useBeanIntrospection, bool includeWS, bool includeLB, bool lbAfterComma, int indentSize, int indentLevel)
        {
            if (depth < 0) {
                return;
            }
            // TBD:        // Just use global vars ???
            ++indentLevel;
            string WS = "";
            if (includeWS) {
                WS = " ";
            }
            string LB = "";
            if (includeLB) {
                LB = "\n";
            }
            string IND = "";
            string INDX = "";
            if (indentSize > 0 && indentLevel > 0) {
                // IND = String.format("%1$" + (indentSize * indentLevel) + "s", "");
                IND = string.Format("{0," + (indentSize * indentLevel) + "}", "");
            }
            if (indentSize > 0 && indentLevel >= 0) {
                // INDX = String.format("%1$" + (indentSize * (indentLevel+1)) + "s", "");
                INDX = string.Format("{0," + (indentSize * (indentLevel + 1)) + "}", "");
            }
            if (obj == null || obj is JsonNull) {
                writer.Write(Literals.NULL);
            } else {
                // if(depth == 0) {
                if (depth <= 0) {
                    if (obj is JsonSerializable) {
                        string jSerial = null;
                        if (obj is IndentedJsonSerializable) {
                            jSerial = ((IndentedJsonSerializable)obj).ToJsonString(indentSize);
                        } else {
                            jSerial = ((JsonSerializable)obj).ToJsonString();
                        }
                        writer.Write(jSerial);
                    } else {
                        // TBD:
                        // This section of code is repeated when depth==0 and and when depth>0,
                        //     almost identically... (but not quite exactly the same)
                        // Need to be refactored.
                        // ....
                        string primStr = null;
                        if (obj is bool?) {
                            if (((bool?)obj).Equals(true)) {
                                primStr = Literals.TRUE;
                            } else {
                                primStr = Literals.FALSE;
                            }
                            writer.Write(primStr);
                        } else if (obj is char?) {
                            // ???
                            char? strChar = (char?)obj;
                            writer.Write("\""); writer.Write(strChar); writer.Write("\"");
                        } else if (obj is Number) {
                            //                        double d = ((Number) obj).doubleValue();
                            //                        jsonStr = Double.valueOf(d);
                            primStr = ((Number)obj).ToString();
                            writer.Write(primStr);
                        } else if (obj is string) {
                            // ????
                            // Is there a better/faster way to do this?

                            // ???
                            primStr = (string)obj;
                            // writer.Write("\""); writer.Write(primStr); writer.Write("\"");

                            // ???
                            writer.Write("\"");
                            _appendEscapedString(writer, primStr);
                            writer.Write("\"");
                            // ???

                        } else {

                            // TBD:
                            // java.util.Date ???
                            // and other JDK built-in class support???
                            // ..

                            if (obj is DateTime) {
                                // TBD:
                                // Create a struct ???
                                primStr = ((DateTime)obj).ToString();
                                // ...
                            } else {

                                // TBD:
                                // POJO/Bean support???
                                // ...

                                // ????
                                primStr = obj.ToString();
                            }
                            // TBD: ?????
                            // writer.Write("\""); writer.Write(primStr); writer.Write("\"");
                            writer.Write("\"");
                            _appendEscapedString(writer, primStr);
                            writer.Write("\"");
                        }
                    }
                } else {
                    // if(obj instanceof Map<?,?>)
                    if (obj is IDictionary<string, object>) {
                        IDictionary<string, object> map = null;
                        try {
                            // map = (Map<String,Object>)((Map<?,?>) obj);
                            map = (IDictionary<string, object>)obj;
                        } catch (Exception e) {
                            // log.log(Level.INFO, "Invalid map type.", e);
                        }
                        writer.Write("{"); writer.Write(LB);
                        if (map != null && map.Count > 0) {
                            writer.Write(INDX);

                            var isFirst = true;
                            IEnumerator<string> it = map.Keys.GetEnumerator();
                            while (it.MoveNext()) {
                                if (isFirst) {
                                    isFirst = false;
                                } else {
                                    writer.Write(",");
                                    if (lbAfterComma) {
                                        writer.Write(LB); writer.Write(INDX);
                                    } else {
                                        writer.Write(WS);
                                    }
                                }
                                string key = it.Current;
                                object val = map[key];
                                writer.Write("\""); writer.Write(key); writer.Write("\":"); writer.Write(WS);
                                await _buildAsync(writer, val, depth - 1, useBeanIntrospection, includeWS, includeLB, lbAfterComma, indentSize, indentLevel);
                            }
                            writer.Write(LB);
                            //                        for(String key : map.keySet()) {
                            //                            Object val = map.get(key);
                            //                            String str = _build(val, includeWS, includeLB, indentSize, indentLevel);
                            //                            sb.Write("\""); writer.Write(key); writer.Write("\":"); writer.Write(WS); writer.Write(str);
                            //                            sb.Write(","); writer.Write(WS);
                            //                        }
                            //                        if(sb.charAt(sb.length() - 1) == ',' || sb.charAt(sb.length() - 1) == ' ') {
                            //                            sb.deleteCharAt(sb.length() - 1);
                            //                            if(sb.charAt(sb.length() - 1) == ',') {
                            //                                sb.deleteCharAt(sb.length() - 1);
                            //                            }
                            //                        }
                        }
                        writer.Write(IND); writer.Write("}");
                    }
                    // else if(obj instanceof List<?>)
                    else if (obj is IList<object>) {
                        IList<object> list = null;
                        try {
                            // list = (List<Object>)((List<?>) obj);
                            list = (IList<object>)obj;
                        } catch (Exception e) {
                            // log.log(Level.INFO, "Invalid list type.", e);
                        }
                        writer.Write("["); writer.Write(LB);
                        if (list != null && list.Count > 0) {
                            writer.Write(INDX);

                            var isFirst = true;
                            IEnumerator<object> it = list.GetEnumerator();
                            while (it.MoveNext()) {
                                if (isFirst) {
                                    isFirst = false;
                                } else {
                                    writer.Write(",");
                                    if (lbAfterComma) {
                                        writer.Write(LB); writer.Write(INDX);
                                    } else {
                                        writer.Write(WS);
                                    }
                                }
                                object o = it.Current;
                                await _buildAsync(writer, o, depth - 1, useBeanIntrospection, includeWS, includeLB, lbAfterComma, indentSize, indentLevel);
                            }
                            writer.Write(LB);
                            //                        for(Object o : list) {
                            //                            String str = _build(o, includeWS, includeLB, indentSize, indentLevel);
                            //                            sb.Write(str);
                            //                            sb.Write(","); writer.Write(WS);
                            //                        }
                            //                        if(sb.charAt(sb.length() - 1) == ',' || sb.charAt(sb.length() - 1) == ' ') {
                            //                            sb.deleteCharAt(sb.length() - 1);
                            //                            if(sb.charAt(sb.length() - 1) == ',') {
                            //                                sb.deleteCharAt(sb.length() - 1);
                            //                            }
                            //                        }
                        }
                        writer.Write(IND); writer.Write("]");
                    } else if (obj.GetType().IsArray) { // ???
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

                        // ?????
                        object[] array = obj as object[];
                        writer.Write("["); writer.Write(LB);
                        if (array != null && array.Length > 0) {
                            int arrLen = array.Length;
                            writer.Write(INDX);
                            for (int i = 0; i < arrLen; i++) {
                                object o = array[i];
                                await _buildAsync(writer, o, depth - 1, useBeanIntrospection, includeWS, includeLB, lbAfterComma, indentSize, indentLevel);
                                if (i < arrLen - 1) {
                                    writer.Write(","); writer.Write(WS);
                                }
                            }
                        }
                        writer.Write(LB); writer.Write(IND); writer.Write("]");
                    }
                      // else if(obj instanceof Collection<?>)
                      else if (obj is ICollection<object>) { // ???????
                        ICollection<object> coll = null;
                        try {
                            // coll = (Collection<Object>)((Collection<?>) obj);
                            coll = (ICollection<object>)((ICollection<object>)obj);
                        } catch (Exception e) {
                            // log.log(Level.INFO, "Invalid collection type.", e);
                        }
                        writer.Write("["); writer.Write(LB);
                        if (coll != null && coll.Count > 0) {
                            writer.Write(INDX);

                            var isFirst = true;
                            IEnumerator<object> it = coll.GetEnumerator();
                            while (it.MoveNext()) {
                                if (isFirst) {
                                    isFirst = false;
                                } else {
                                    writer.Write(",");
                                    if (lbAfterComma) {
                                        writer.Write(LB); writer.Write(INDX);
                                    } else {
                                        writer.Write(WS);
                                    }
                                }
                                object o = it.Current;
                                await _buildAsync(writer, o, depth - 1, useBeanIntrospection, includeWS, includeLB, lbAfterComma, indentSize, indentLevel);
                            }
                            writer.Write(LB);
                        }
                        writer.Write(IND); writer.Write("]");
                    } else {
                        // ???
                        // TBD: indentLevel, etc. ??????
                        // ...
                        // This actually causes infinite recursion.
                        //    because a JsonSerializable object may use JsonBuilder for its implementation of JsonSerializable.ToJsonString()
                        // ???? How to fix this??? Is this fixable ???
                        //                    if(obj instanceof JsonSerializable) { 
                        //                        String jSerial = null;
                        //                        if(obj instanceof IndentedJsonSerializable) {
                        //                            jSerial = ((IndentedJsonSerializable) obj).ToJsonString(indentSize);
                        //                        } else {
                        //                            jSerial = ((JsonSerializable) obj).ToJsonString();
                        //                        }
                        //                        writer.Write(jSerial);   // Note that this is actually (partial) json string, not a string (that needs to be escaped).
                        //                    // Note: this is better than JsonSerializable since it obeys the indentLevel rule, etc. 
                        //                    // But, it seems more reasonable to use JsonSerializable first if the object implements both interfaces.
                        //                    } else if(obj instanceof JsonCompatible) {
                        if (obj is JsonCompatible) {
                            // ????
                            object jObj = null;
                            try {
                                // which one to use???
                                // jObj = ((JsonCompatible) obj).ToJsonStructure();        // Use "default" depth of the object???
                                jObj = ((JsonCompatible)obj).ToJsonStructure(depth - 1); // ???
                                                                                         // Use this.ToJsonStructure(jObj) ???
                                                                                         // ...
                            } catch (JsonBuilderException e) {
                                // Ignore
                                // log.log(Level.WARNING, "Failed to create JSON struct for a JsonCompatible object.", e);
                            }
                            if (jObj != null) {
                                await _buildAsync(writer, jObj, depth, useBeanIntrospection, includeWS, includeLB, lbAfterComma, indentSize, indentLevel);
                            } else {
                                // ???
                                // TBD: String need to be escaped.
                                string jcStr = obj.ToString();
                                // TBD: ?????
                                // writer.Write("\""); writer.Write(jcStr); writer.Write("\"");
                                writer.Write("\"");
                                _appendEscapedString(writer, jcStr);
                                writer.Write("\"");
                            }
                        } else {
                            // primitive types... ???
                            string primStr = null;
                            if (obj is bool?) {
                                if (((bool?)obj).Equals(true)) {
                                    primStr = Literals.TRUE;
                                } else {
                                    primStr = Literals.FALSE;
                                }
                                writer.Write(primStr);
                            } else if (obj is char?) {
                                // ???
                                char? strChar = (char?)obj;
                                writer.Write("\""); writer.Write(strChar); writer.Write("\"");
                            } else if (obj is Number) {
                                //                            double d = ((Number) obj).doubleValue();
                                //                            jsonStr = Double.valueOf(d);
                                primStr = ((Number)obj).ToString();
                                writer.Write(primStr);
                            } else if (obj is string) {
                                // ????
                                // Is there a better/faster way to do this?
                                // ???
                                primStr = (string)obj;
                                // writer.Write("\""); writer.Write(primStr); writer.Write("\"");
                                // ???
                                writer.Write("\"");
                                _appendEscapedString(writer, primStr);
                                writer.Write("\"");
                                // ???
                            } else {
                                // TBD:                            // java.util.Date ???                            // and other JDK built-in class support???
                                if (obj is DateTime) {
                                    // TBD:                                // Create a struct ???
                                    primStr = ((DateTime)obj).ToString();
                                    // ...
                                } else {
                                    if (useBeanIntrospection) {
                                        IDictionary<string, object> mapEquivalent = null;
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
                                            //if (log.isLoggable(Level.INFO)) {
                                            //    log.log(Level.INFO, "Faild to introspect a bean.", e);
                                            //}
                                        }
                                        if (mapEquivalent != null) {
                                            await _buildAsync(writer, mapEquivalent, depth, useBeanIntrospection, includeWS, includeLB, lbAfterComma, indentSize, indentLevel); // Note: We do not change the depth.
                                        } else {
                                            // ????
                                            // primStr = null; ???
                                            primStr = obj.ToString();
                                            // ...
                                        }
                                    } else {
                                        // ????
                                        primStr = obj.ToString();
                                    }
                                }
                                // TBD: ?????
                                // writer.Write("\""); writer.Write(primStr); writer.Write("\"");
                                writer.Write("\"");
                                _appendEscapedString(writer, primStr);
                                writer.Write("\"");
                            }
                        }
                    }
                }
            }
        }

        private void _appendEscapedString(TextWriter writer, string primStr)
        {
            int escapeForwardSlash = builderPolicy.EscapeForwardSlash;
            if (!string.ReferenceEquals(primStr, null) && primStr.Length > 0) {
                char[] primChars = primStr.ToCharArray();
                char prevEc = (char)0;
                foreach (char ec in primChars) {
                    if (Symbols.IsEscapedChar(ec)) {
                        if (prevEc == '<' && ec == '/') {
                            if (escapeForwardSlash >= 0) {
                                writer.Write("\\/");
                            } else {
                                writer.Write("/");
                            }
                            // } else if(prevEc == '\\' && ec == '\\') {
                            //     // Already escaped... ????
                            //     // Skip ???
                        } else {
                            string str = Symbols.GetEscapedCharString(ec, escapeForwardSlash > 0 ? true : false);
                            if (!string.ReferenceEquals(str, null)) {
                                writer.Write(str);
                            } else {
                                // ???
                                writer.Write(ec);
                            }
                        }
                    } else if (CharUtil.IsISOControl(ec)) {
                        char[] uc = UnicodeUtil.GetUnicodeHexCodeFromChar(ec);
                        writer.Write(uc);
                    } else {
                        writer.Write(ec);
                    }
                    prevEc = ec;
                }
            }
        }
    }

}