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
using System.Threading.Tasks;

namespace HoloJson.Builder.Partial.Impl
{
    /// <summary>
    /// Default MixedJsonBuilder implementation.
    /// Any string at the depth level (Or, any string at or below the depth level  ????)
    ///      is interpreted as JSON string (representing a sub-tree)
    ///      rather than a string.
    /// </summary>
    public abstract class AbstractMixedJsonBuilder : MixedJsonBuilder
    {
        // Default value.
        // Max value: equivalent to -1.
        private static readonly int DEFAULT_MAX_DRILL_DOWN_DEPTH = (int)sbyte.MaxValue; // Arbitrary.
        private static readonly int MAXIMUM_MIN_DRILL_DOWN_DEPTH = DEFAULT_MAX_DRILL_DOWN_DEPTH + 1; // Note + 1.
        private const int DEFAULT_MIN_DRILL_DOWN_DEPTH = 0; // 0 or 1??

        // "strategy" for building json structure.
        // No setters for builderPolicy (except through a ctor).
        private readonly BuilderPolicy builderPolicy;

        // Not being used.
        // TBD: Not sure if we can ensure thread safety.
        private readonly bool threadSafe;

        // TBD:
        // private final JsonParser jsonParser;


        // Note:
        // It's important not to keep any "state" as class variables for this class
        //   so that a single instance can be used for multiple/concurrent build operations.
        // (Often, the recursive implementation may involve multiple objects (each as a node in an object tree),
        //   which may use the same/single instance of this builder class.)
        // ...


        public AbstractMixedJsonBuilder() : this(null)
        {
        }
        public AbstractMixedJsonBuilder(BuilderPolicy builderPolicy) : this(builderPolicy, false) // true or false ????
        {
        }
        public AbstractMixedJsonBuilder(BuilderPolicy builderPolicy, bool threadSafe)
        {
            if (builderPolicy == null) {
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


        public async Task<string> BuildAsync(object jsonObj)
        {
            int maxDepth = builderPolicy.DrillDownDepth;
            return await BuildMixedAsync(jsonObj, MAXIMUM_MIN_DRILL_DOWN_DEPTH, maxDepth);
        }
        public async Task BuildAsync(TextWriter writer, object jsonObj)
        {
            int maxDepth = builderPolicy.DrillDownDepth;
            await BuildMixedAsync(writer, jsonObj, MAXIMUM_MIN_DRILL_DOWN_DEPTH, maxDepth);
        }

        public async Task<string> BuildMixedAsync(object jsonObj)
        {
            return await BuildMixedAsync(jsonObj, DEFAULT_MIN_DRILL_DOWN_DEPTH, DEFAULT_MAX_DRILL_DOWN_DEPTH);
        }
        public async Task BuildMixedAsync(TextWriter writer, object jsonObj)
        {
            await BuildMixedAsync(writer, jsonObj, DEFAULT_MIN_DRILL_DOWN_DEPTH, DEFAULT_MAX_DRILL_DOWN_DEPTH);
        }

        public virtual async Task<string> BuildMixedAsync(object jsonObj, int minDepth)
        {
            return await BuildMixedAsync(jsonObj, minDepth, DEFAULT_MAX_DRILL_DOWN_DEPTH);
        }
        public virtual async Task BuildMixedAsync(TextWriter writer, object jsonObj, int minDepth)
        {
            await BuildMixedAsync(writer, jsonObj, minDepth, DEFAULT_MAX_DRILL_DOWN_DEPTH);
        }

        public async Task<string> BuildMixedAsync(object jsonObj, int minDepth, int maxDepth)
        {
            return await BuildMixedAsync(jsonObj, minDepth, maxDepth, 0);
        }
        public async Task BuildMixedAsync(TextWriter writer, object jsonObj, int minDepth, int maxDepth)
        {
            await BuildMixedAsync(writer, jsonObj, minDepth, maxDepth, 0);
        }


        public async Task<string> BuildMixedAsync(object jsonObj, int minDepth, int maxDepth, int indent)
        {
            string jsonStr = null;
            StringWriter writer = new StringWriter();
            try {
                await BuildMixedAsync(writer, jsonObj, minDepth, maxDepth, indent);
                jsonStr = writer.ToString();
                //if (log.isLoggable(Level.FINE)) {
                //    log.fine("jsonStr = " + jsonStr);
                //}
            } catch (IOException e) {
                // log.log(Level.WARNING, "Failed to write jsonObj as JSON.", e);
            }
            return jsonStr;
        }

        public async Task BuildMixedAsync(TextWriter writer, object jsonObj, int minDepth, int maxDepth, int indent)
        {
            if (minDepth < 0) {
                minDepth = MAXIMUM_MIN_DRILL_DOWN_DEPTH;
            }
            if (maxDepth < 0) {
                maxDepth = DEFAULT_MAX_DRILL_DOWN_DEPTH;
            }
            // Note that minDepth does not have to be smaller than maxDepth.
            // 0 <= minDepth <= maxDpeth+1.
            int cutoff = maxDepth - minDepth;

            bool useBeanIntrospection = this.builderPolicy.UseBeanIntrospection;

            IndentInfoStruct indentInfo = new IndentInfoStruct(indent);
            bool includeWS = indentInfo.IsIncludingWhiteSpaces;
            bool includeLB = indentInfo.IsIncludingLineBreaks;
            bool lbAfterComma = indentInfo.IsLineBreakingAfterComma;
            int indentSize = indentInfo.IndentSize;
            int indentLevel = -1;

            await _buildAsync(writer, jsonObj, cutoff, maxDepth, useBeanIntrospection, includeWS, includeLB, lbAfterComma, indentSize, indentLevel);
            // tbd:  ????
            // writer.flush(); // ???
            // ????

            //        String jsonStr = writer.toString();
            //        if(log.isLoggable(Level.FINE)) log.fine("jsonStr = " + jsonStr);

        }


        private async Task _buildAsync(TextWriter writer, object obj, int cutoff, int depth, bool useBeanIntrospection, bool includeWS, bool includeLB, bool lbAfterComma, int indentSize, int indentLevel)
        {
            if (depth < 0) {
                return;
            }

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
                // IND = string.Format("%1$" + (indentSize * indentLevel) + "s", "");
                IND = string.Format("{0," + (indentSize * indentLevel) + "}", "");
            }
            if (indentSize > 0 && indentLevel >= 0) {
                // INDX = string.Format("%1$" + (indentSize * (indentLevel + 1)) + "s", "");
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
                            jSerial = await ((IndentedJsonSerializable)obj).ToJsonStringAsync(indentSize);
                        } else {
                            jSerial = await ((JsonSerializable)obj).ToJsonStringAsync();
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

                            // For "MixedJsonParser",
                            // when the tree traversing reaches the depth == 0 (or, maxDepth down from the beginning)
                            // we treat string values (and, only strings) differently.
                            // We treat the string as a JSON string (e.g., corresponding to an object) not as a regular string...
                            // ...

                            if (depth <= cutoff) {
                                // (a) because we treat it as a JSON string...
                                // TBD:
                                if (primStr.StartsWith(Symbols.LCURLY_STR, StringComparison.Ordinal) || primStr.StartsWith(Symbols.LSQUARE_STR, StringComparison.Ordinal)) { // No leading spaces allowed.
                                    writer.Write(primStr);
                                } else {
                                    // All the rest is just considered a string, including numbers, etc.
                                    writer.Write("\"");
                                    _appendEscapedString(writer, primStr);
                                    writer.Write("\"");
                                }
                                // ...
                            } else {
                                // ???
                                // (b) If it was a regular string...
                                writer.Write("\"");
                                _appendEscapedString(writer, primStr);
                                writer.Write("\"");
                                // ???
                            }

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

                            if (depth <= cutoff) {
                                // ????
                                string objJsonStr = await BuildAsync(obj);
                                if (objJsonStr.StartsWith(Symbols.LCURLY_STR, StringComparison.Ordinal) || objJsonStr.StartsWith(Symbols.LSQUARE_STR, StringComparison.Ordinal)) { // No leading spaces allowed.
                                    writer.Write(objJsonStr);
                                } else {
                                    // All the rest is just considered a string, including numbers, etc.
                                    writer.Write("\"");
                                    _appendEscapedString(writer, objJsonStr);
                                    writer.Write("\"");
                                }
                            } else {
                                writer.Write("\"");
                                _appendEscapedString(writer, primStr);
                                writer.Write("\"");
                            }

                        }
                    }
                } else {
                    // if(obj instanceof java.util.Map<?,?>)
                    if (obj is IDictionary<string, object>) {
                        IDictionary<string, object> map = null;
                        try {
                            // map = (java.util.Map<String,Object>)((java.util.Map<?,?>) obj);
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
                                if(isFirst) {
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
                                await _buildAsync(writer, val, cutoff, depth - 1, useBeanIntrospection, includeWS, includeLB, lbAfterComma, indentSize, indentLevel);
                            }
                            writer.Write(LB);
                            //                        for(String key : map.keySet()) {
                            //                            Object val = map.get(key);
                            //                            String str = _buildAsync(val, includeWS, includeLB, indentSize, indentLevel);
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
                    // else if(obj instanceof java.util.List<?>)
                    else if (obj is IList<object>) {
                        IList<object> list = null;
                        try {
                            // list = (java.util.List<Object>)((java.util.List<?>) obj);
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
                                await _buildAsync(writer, o, cutoff, depth - 1, useBeanIntrospection, includeWS, includeLB, lbAfterComma, indentSize, indentLevel);
                            }
                            writer.Write(LB);
                            //                        for(Object o : list) {
                            //                            String str = _buildAsync(o, includeWS, includeLB, indentSize, indentLevel);
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
                                                        //                    _buildAsync(writer, Arrays.asList(array), depth, useBeanIntrospection, includeWS, includeLB, lbAfterComma, indentSize, indentLevel);

                        object[] array = (object[]) obj;
                        writer.Write("["); writer.Write(LB);
                        if (array != null && array.Length > 0) {
                            int arrLen = array.Length;
                            writer.Write(INDX);
                            for (int i = 0; i < arrLen; i++) {
                                object o = array[i];
                                await _buildAsync(writer, o, cutoff, depth - 1, useBeanIntrospection, includeWS, includeLB, lbAfterComma, indentSize, indentLevel);
                                if (i < arrLen - 1) {
                                    writer.Write(","); writer.Write(WS);
                                }
                            }
                        }
                        writer.Write(LB); writer.Write(IND); writer.Write("]");
                    }
                      // else if(obj instanceof java.util.Collection<?>)
                      else if (obj is ICollection<object>) {
                        // ???????
                        ICollection<object> coll = null;
                        try {
                            // coll = (java.util.Collection<Object>)((java.util.Collection<?>) obj);
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
                                await _buildAsync(writer, o, cutoff, depth - 1, useBeanIntrospection, includeWS, includeLB, lbAfterComma, indentSize, indentLevel);
                            }
                            writer.Write(LB);
                        }
                        writer.Write(IND); writer.Write("]");
                    } else {
                        // ???
                        // TBD: indentLevel, etc. ??????
                        // ...
                        // This actually causes infinite recursion.
                        //    because a JsonSerializable object may use JsonBuilder for its implementation of JsonSerializable.ToJsonStringAsync()
                        // ???? How to fix this??? Is this fixable ???
                        //                    if(obj instanceof JsonSerializable) { 
                        //                        String jSerial = null;
                        //                        if(obj instanceof IndentedJsonSerializable) {
                        //                            jSerial = ((IndentedJsonSerializable) obj).ToJsonStringAsync(indentSize);
                        //                        } else {
                        //                            jSerial = ((JsonSerializable) obj).ToJsonStringAsync();
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
                                // jObj = ((JsonCompatible) obj).ToJsonStructureAsync();        // Use "default" depth of the object???
                                jObj = await ((JsonCompatible)obj).ToJsonStructureAsync(depth - 1); // ???
                                                                                         // Use this.ToJsonStructureAsync(jObj) ???
                                                                                         // ...
                            } catch (JsonBuilderException e) {
                                // Ignore
                                // log.log(Level.WARNING, "Failed to create JSON struct for a JsonCompatible object.", e);
                            }
                            if (jObj != null) {
                                await _buildAsync(writer, jObj, cutoff, depth, useBeanIntrospection, includeWS, includeLB, lbAfterComma, indentSize, indentLevel);
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
                            // ...
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


                                // For "MixedJsonParser",
                                // when the tree traversing reaches the depth == 0 (or, maxDepth down from the beginning)
                                // we treat string values (and, only strings) differently.
                                // We treat the string as a JSON string (e.g., corresponding to an object) not as a regular string...
                                // ...

                                if (depth <= cutoff) {
                                    // (a) because we treat it as a JSON string...
                                    // TBD:
                                    if (primStr.StartsWith(Symbols.LCURLY_STR, StringComparison.Ordinal) || primStr.StartsWith(Symbols.LSQUARE_STR, StringComparison.Ordinal)) { // No leading spaces allowed.
                                        writer.Write(primStr);
                                    } else {
                                        // All the rest is just considered a string, including numbers, etc.
                                        writer.Write("\"");
                                        _appendEscapedString(writer, primStr);
                                        writer.Write("\"");
                                    }
                                    // ...
                                } else {
                                    // ???
                                    // (b) If it was a regular string...
                                    writer.Write("\"");
                                    _appendEscapedString(writer, primStr);
                                    writer.Write("\"");
                                    // ???
                                }

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
                                            await _buildAsync(writer, mapEquivalent, cutoff, depth, useBeanIntrospection, includeWS, includeLB, lbAfterComma, indentSize, indentLevel); // Note: We do not change the depth.
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
                                //                            writer.Write("\"");
                                //                            _appendEscapedString(writer, primStr);
                                //                            writer.Write("\"");

                                if (depth <= cutoff) {
                                    // ????
                                    string objJsonStr = await BuildAsync(obj);
                                    if (objJsonStr.StartsWith(Symbols.LCURLY_STR, StringComparison.Ordinal) || objJsonStr.StartsWith(Symbols.LSQUARE_STR, StringComparison.Ordinal)) { // No leading spaces allowed.
                                        writer.Write(objJsonStr);
                                    } else {
                                        // All the rest is just considered a string, including numbers, etc.
                                        writer.Write("\"");
                                        _appendEscapedString(writer, objJsonStr);
                                        writer.Write("\"");
                                    }
                                } else {
                                    writer.Write("\"");
                                    _appendEscapedString(writer, primStr);
                                    writer.Write("\"");
                                }

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