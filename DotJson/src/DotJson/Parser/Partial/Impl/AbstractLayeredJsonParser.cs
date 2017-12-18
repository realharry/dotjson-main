using DotJson.Builder;
using DotJson.Builder.Impl;
using DotJson.Common;
using DotJson.Core;
using DotJson.Parser.Core;
using DotJson.Parser.Exceptions;
using DotJson.Parser.Impl;
using DotJson.Parser.Policy;
using DotJson.Parser.Policy.Base;
using DotJson.Type.Factory;
using DotJson.Type.Factory.Impl;
using DotJson.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotJson.Parser.Partial.Impl
{
    /// <summary>
    /// Recursive descent parser implementation.
    /// Note that although it's a "partial" parser,
    ///    there is no way to achieve "partial" parsing without scanning the whole json string.
    /// ParseAsync() returns a nested tree of Map/List down to the maxDepth.
    /// Any sub-tree below the maxDepth is just returned as "partial" json string representing that subtree.
    /// </summary>
    public abstract class AbstractLayeredJsonParser : AbstractJsonParser, LayeredJsonParser
    {
        // temporary
        private static readonly int MAX_PARSE_DEPTH = (int)sbyte.MaxValue; // arbitrary

        // temporary
        private const int DEF_TOKEN_TAIL_BUFFER_SIZE = 24;
        private const int DEF_NODE_TAIL_BUFFER_SIZE = 16;

        private readonly JsonTypeFactory jsonTypeFactory;
        private readonly ParserPolicy parserPolicy;

        // TBD: Not sure if threadSafe==true makes the code really thread safe.
        // It's just a flag for now (which may or may not ensure thread safety).
        private readonly bool threadSafe;

        // TBD:
        // The following class variables make this class not thread safe.
        //    (Note: jsonTypeFactory and parserPolicy have no setters).
        // --> Make them arguments of _parseAsync() methods ???
        //    (Reusing jsonTokenizer is good for performance, but can make it potentially not thread-safe....)
        //    (Same for tokenTailBuffer and nodeTailBuffer....)

        // This is lazy Initialized, and reused across multiple ParseAsync() sessions.
        private JsonTokenizer mJsonTokenizer = null;

        // For debugging/tracing...
        private JsonTokenBuffer mTokenTailBuffer = null;
        private ObjectTailBuffer mNodeTailBuffer = null;

        // temporary.
        // Is it safe to re-use this ????
        private readonly AbstractBareJsonBuilder jsonBuilder;
        // ....


        public AbstractLayeredJsonParser() : this(null)
        {
        }
        public AbstractLayeredJsonParser(JsonTypeFactory jsonTypeFactory) : this(jsonTypeFactory, null)
        {
        }
        public AbstractLayeredJsonParser(JsonTypeFactory jsonTypeFactory, ParserPolicy parserPolicy) : this(jsonTypeFactory, parserPolicy, false) // true or false ??
        {
        }
        public AbstractLayeredJsonParser(JsonTypeFactory jsonTypeFactory, ParserPolicy parserPolicy, bool threadSafe)
        {
            // temporary
            if (jsonTypeFactory == null) {
                this.jsonTypeFactory = BareJsonTypeFactory.Instance;
            } else {
                this.jsonTypeFactory = jsonTypeFactory;
            }
            if (parserPolicy == null) {
                this.parserPolicy = DefaultParserPolicy.MINIJSON;
            } else {
                this.parserPolicy = parserPolicy;
            }
            this.threadSafe = threadSafe;

            // ???
            // jsonBuilder = new AbstractBareJsonBuilder() {};
            jsonBuilder = new SimpleJsonBuilder();

            // For subclasses
            Init();
        }

        // Having multiple ctor's is a bit inconvenient.
        // Put all Init routines here.
        // To be overridden in subclasses.
        protected internal virtual void Init()
        {
            // Enabling look-ahead-parsing can cause parse failure because it cannot handle long strings...
            EnableLookAheadParsing();
            // DisableLookAheadParsing();
            DisableTracing();
        }


        public override async Task<object> ParseAsync(string jsonStr)
        {
            StringReader sr = new StringReader(jsonStr);
            object jsonObj = null;
            try {
                jsonObj = await ParseAsync(sr);
            } catch (IOException e) {
                // throw new JsonParserException("IO error during JSON parsing. " + tokenTailBuffer.ToTraceString(), e);
                throw new JsonParserException("IO error during JSON parsing.", e);
            }
            return jsonObj;
        }
        public override async Task<object> ParseAsync(TextReader reader)
        {
            object topNode = await _parseAsync(reader);
            // TBD:
            // Convert it to map/list... ??
            // ...
            return topNode;
        }

        public async Task<object> ParseAsync(string jsonStr, int depth)
        {
            StringReader sr = new StringReader(jsonStr);
            object jsonObj = null;
            try {
                jsonObj = await ParseAsync(sr, depth);
            } catch (IOException e) {
                // throw new JsonParserException("IO error during JSON parsing. " + tokenTailBuffer.ToTraceString(), e);
                throw new JsonParserException("IO error during JSON parsing.", e);
            }
            return jsonObj;
        }

        public async Task<object> ParseAsync(TextReader reader, int depth)
        {
            object topNode = await _parseAsync(reader, depth);
            return topNode;
        }


        private async Task<object> _parseAsync(TextReader reader)
        {
            return await _parseAsync(reader, MAX_PARSE_DEPTH);
        }
        private async Task<object> _parseAsync(TextReader reader, int maxDepth)
        {
            if (reader == null) {
                return null;
            }
            if (maxDepth < 0) {
                maxDepth = MAX_PARSE_DEPTH;
            }

            // ???
            if (maxDepth == 0) {
                // Returns the original JSON string ???
                string jStr = await _readFromReaderAsync(reader);
                return jStr;
            }

            // TBD:
            // Does this make it thread safe???
            // ...

            JsonTokenizer jsonTokenizer = null;
            if (threadSafe) {
                // ???
                jsonTokenizer = new AbstractJsonTokenizerAnonymousInnerClass(this, reader, parserPolicy);
                SetLookAheadParsing(jsonTokenizer);
                SetTokenizerTracing(jsonTokenizer);
            } else {
                if (mJsonTokenizer != null && mJsonTokenizer is AbstractJsonTokenizer) {
                    ((AbstractJsonTokenizer)mJsonTokenizer).Reset(reader);
                } else {
                    mJsonTokenizer = new AbstractJsonTokenizerAnonymousInnerClass2(this, reader, parserPolicy);
                    SetLookAheadParsing(mJsonTokenizer);
                    SetTokenizerTracing(mJsonTokenizer);
                }
                jsonTokenizer = mJsonTokenizer;
            }

            JsonTokenBuffer tokenTailBuffer = null;
            if (threadSafe) {
                tokenTailBuffer = new JsonTokenBuffer(DEF_TOKEN_TAIL_BUFFER_SIZE);
            } else {
                if (mTokenTailBuffer != null) {
                    mTokenTailBuffer.Reset();
                } else {
                    mTokenTailBuffer = new JsonTokenBuffer(DEF_TOKEN_TAIL_BUFFER_SIZE);
                }
                tokenTailBuffer = mTokenTailBuffer;
            }

            ObjectTailBuffer nodeTailBuffer = null;
            if (threadSafe) {
                nodeTailBuffer = new ObjectTailBuffer(DEF_NODE_TAIL_BUFFER_SIZE);
            } else {
                if (mNodeTailBuffer != null) {
                    mNodeTailBuffer.Reset();
                } else {
                    mNodeTailBuffer = new ObjectTailBuffer(DEF_NODE_TAIL_BUFFER_SIZE);
                }
                nodeTailBuffer = mNodeTailBuffer;
            }

            return await _parseAsync(jsonTokenizer, maxDepth, tokenTailBuffer, nodeTailBuffer);
        }

        private class AbstractJsonTokenizerAnonymousInnerClass : AbstractJsonTokenizer
        {
            private readonly AbstractLayeredJsonParser outerInstance;

            public AbstractJsonTokenizerAnonymousInnerClass(AbstractLayeredJsonParser outerInstance, TextReader reader, ParserPolicy parserPolicy) : base(reader, parserPolicy)
            {
                this.outerInstance = outerInstance;
            }

        }

        private class AbstractJsonTokenizerAnonymousInnerClass2 : AbstractJsonTokenizer
        {
            private readonly AbstractLayeredJsonParser outerInstance;

            public AbstractJsonTokenizerAnonymousInnerClass2(AbstractLayeredJsonParser outerInstance, TextReader reader, ParserPolicy parserPolicy) : base(reader, parserPolicy)
            {
                this.outerInstance = outerInstance;
            }

        }

        private async Task<object> _parseAsync(JsonTokenizer tokenizer, int depth, JsonTokenBuffer tokenTailBuffer, ObjectTailBuffer nodeTailBuffer)
        {
            object topNode = null;
            var type = PeekAndGetType(tokenizer, tokenTailBuffer, nodeTailBuffer);
            if (type == TokenType.EOF || type == TokenType.LCURLY || type == TokenType.LSQUARE) {
                if (type == TokenType.EOF) {
                    topNode = ProduceJsonNull(tokenizer, tokenTailBuffer, nodeTailBuffer);
                } else if (type == TokenType.LCURLY) {
                    topNode = await ProduceJsonObjectAsync(tokenizer, depth, tokenTailBuffer, nodeTailBuffer);
                } else if (type == TokenType.LSQUARE) {
                    topNode = await ProduceJsonArrayAsync(tokenizer, depth, tokenTailBuffer, nodeTailBuffer);
                }
            } else {
                // TBD:
                // Process it here if parserPolicy.AllowLeadingJsonMarker == true,
                // ???
                if (parserPolicy.AllowNonObjectOrNonArray) {
                    // This is actually error according to json.org JSON grammar.
                    // But, we allow partial JSON string.
                    switch (type) {
                        case TokenType.NULL:
                            topNode = ProduceJsonNull(tokenizer, tokenTailBuffer, nodeTailBuffer);
                            break;
                        case TokenType.BOOLEAN:
                            topNode = ProduceJsonBoolean(tokenizer, tokenTailBuffer, nodeTailBuffer);
                            break;
                        case TokenType.NUMBER:
                            topNode = ProduceJsonNumber(tokenizer, tokenTailBuffer, nodeTailBuffer);
                            break;
                        case TokenType.STRING:
                            // log.warning(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
                            topNode = ProduceJsonString(tokenizer, tokenTailBuffer, nodeTailBuffer);
                            // log.warning(">>>>>>>>>>>>>>>>>>>>>>>>>>>>> topNode = " + topNode);
                            break;
                        default:
                            // ???
                            throw new InvalidJsonTokenException("JsonToken not recognized: tokenType = " + TokenTypes.GetDisplayName(type) + "; " + tokenTailBuffer.ToTraceString(), GetTailCharStream(tokenizer), PeekCharStream(tokenizer));
                    }
                } else {
                    // TBD
                    // this is a bit too lenient probably...
                    // there was some special char sequence which some parsers allowed, which I cannot remember..
                    // For now, if parserPolicy.AllowLeadingJsonMarker == true is interpreted as allowLeadingNonObjectNonArrayChars....
                    //    --> we remove all leading chars until we reach { or [.
                    if (parserPolicy.AllowLeadingJsonMarker) {
                        while (type != TokenType.LCURLY && type != TokenType.LSQUARE) {
                            JsonToken t = tokenizer.Next(); // swallow one token.
                            if (TracingEnabled) {
                                tokenTailBuffer.Push(t);
                            }
                            type = PeekAndGetType(tokenizer, tokenTailBuffer, nodeTailBuffer);
                        }
                        if (type == TokenType.LCURLY) {
                            topNode = await ProduceJsonObjectAsync(tokenizer, depth, tokenTailBuffer, nodeTailBuffer);
                        } else if (type == TokenType.LSQUARE) {
                            topNode = await ProduceJsonArrayAsync(tokenizer, depth, tokenTailBuffer, nodeTailBuffer);
                        } else {
                            // ???
                            throw new InvalidJsonTokenException("Invalid input Json string. " + tokenTailBuffer.ToTraceString(), GetTailCharStream(tokenizer), PeekCharStream(tokenizer));
                        }
                    } else {
                        // ???
                        throw new InvalidJsonTokenException("Json string should be Object or Array. Input tokenType = " + TokenTypes.GetDisplayName(type) + "; " + tokenTailBuffer.ToTraceString(), GetTailCharStream(tokenizer), PeekCharStream(tokenizer));
                    }
                }
            }

            //if (log.IsLoggable(Level.FINE)) {
            //    log.fine("topnNode = " + topNode);
            //}
            return topNode;
        }


        private async Task<IDictionary<string, object>> ProduceJsonObjectAsync(JsonTokenizer tokenizer, int depth, JsonTokenBuffer tokenTailBuffer, ObjectTailBuffer nodeTailBuffer)
        {
            var lcurl = NextAndGetType(tokenizer, tokenTailBuffer, nodeTailBuffer); // pop the leading {.
            if (lcurl != TokenType.LCURLY) {
                // this cannot happen.
                throw new InvalidJsonTokenException("JSON object should start with {. " + tokenTailBuffer.ToTraceString(), GetTailCharStream(tokenizer), PeekCharStream(tokenizer));
            }

            IDictionary<string, object> map = new Dictionary<string, object>();
            var type = PeekAndGetType(tokenizer, tokenTailBuffer, nodeTailBuffer);
            if (type == TokenType.RCURLY) {
                // empty object
                JsonToken t = tokenizer.Next(); // discard the trailing }.
                if (TracingEnabled) {
                    tokenTailBuffer.Push(t);
                }
            } else {
                IDictionary<string, object> members = await ProduceJsonObjectMembersAsync(tokenizer, depth, tokenTailBuffer, nodeTailBuffer);
                TokenType rcurl;
                if (TracingEnabled) {
                    JsonToken t = NextAndGetToken(tokenizer, tokenTailBuffer, nodeTailBuffer); // discard the trailing }.
                    tokenTailBuffer.Push(t);
                    rcurl = t.Type;
                } else {
                    rcurl = NextAndGetType(tokenizer, tokenTailBuffer, nodeTailBuffer); // discard the trailing }.
                }
                if (rcurl == TokenType.RCURLY) {
                    // Done
                    // ???
                    // (map as Dictionary<string, object>)?.putAll(members);
                    // map = (IDictionary<string, JsonNode>) members; ????
                    foreach (var m in members) {
                        map[m.Key] = m.Value;
                    }
                    // ???
                } else {
                    // ???
                    throw new InvalidJsonTokenException("JSON object should end with }. " + tokenTailBuffer.ToTraceString(), GetTailCharStream(tokenizer), PeekCharStream(tokenizer));
                }
            }
            IDictionary<string, object> jObject = jsonTypeFactory.CreateObject(map);
            if (TracingEnabled) {
                nodeTailBuffer.Push(jObject);
            }

            //if (log.IsLoggable(Level.FINE)) {
            //    log.fine("jObject = " + jObject);
            //}
            return jObject;
        }


        private async Task<IDictionary<string, object>> ProduceJsonObjectMembersAsync(JsonTokenizer tokenizer, int depth, JsonTokenBuffer tokenTailBuffer, ObjectTailBuffer nodeTailBuffer)
        {
            IDictionary<string, object> members = new Dictionary<string, object>();
            var type = PeekAndGetType(tokenizer, tokenTailBuffer, nodeTailBuffer);
            while (type != TokenType.RCURLY) {
                MapEntry<string, object> member = await ProduceJsonObjectMemberAsync(tokenizer, depth, tokenTailBuffer, nodeTailBuffer); // No depth change...
                if (member != null) {
                    // This is a bit strange implementation, but it actually works...
                    // The parser traverses the object tree (depth first) down to the leaf,
                    //    and on the way up, if it reaches depth==1, it converts the sub-tree below depth==1 to Json String.
                    //    the json string is then used as the node value (instead of the object sub tree).
                    // Going above depth > 1, this transformation does not happen.
                    if (depth == 1) {
                        object mem = member.Value;
                        try {
                            // String jStr = _escapeString(jsonBuilder.BuildAsync(mem));
                            string jStr = await jsonBuilder.BuildAsync(mem);
                            // Object memObj = jsonBuilder.buildJsonStructure(mem);
                            //  String jStr = jsonBuilder.BuildAsync(memObj);
                            // String jStr = jsonBuilder.BuildAsync(mem, 1);
                            // log.warning(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> depth = " + depth + " }}}}}}}} jStr = " + jStr);
                            members[member.Key] = jStr;
                        } catch (JsonBuilderException e) {
                            throw new JsonParserException(e);
                        }
                    } else {
                        members[member.Key] = member.Value;
                    }
                }
                type = PeekAndGetType(tokenizer, tokenTailBuffer, nodeTailBuffer);

                // "consume" the comma.
                if (parserPolicy.AllowExtraCommas) {
                    while (type == TokenType.COMMA) {
                        JsonToken t = tokenizer.Next();
                        if (TracingEnabled) {
                            tokenTailBuffer.Push(t);
                        }
                        type = PeekAndGetType(tokenizer, tokenTailBuffer, nodeTailBuffer);
                    }
                } else {
                    if (type == TokenType.COMMA) {
                        JsonToken t = tokenizer.Next();
                        if (TracingEnabled) {
                            tokenTailBuffer.Push(t);
                        }
                        type = PeekAndGetType(tokenizer, tokenTailBuffer, nodeTailBuffer);

                        if (parserPolicy.AllowTrailingComma) {
                            // Continue.
                        } else {
                            // Invalid  char sequence: ",}" 
                            if (type == TokenType.RCURLY) {
                                throw new InvalidJsonTokenException("Syntax error: Object has a trailing comma. " + tokenTailBuffer.ToTraceString(), GetTailCharStream(tokenizer), PeekCharStream(tokenizer));
                            }
                        }
                    }
                }
            }

            //if (log.IsLoggable(Level.FINER)) {
            //    log.finer("members = " + members);
            //}
            return members;
        }
        private async Task<MapEntry<string, object>> ProduceJsonObjectMemberAsync(JsonTokenizer tokenizer, int depth, JsonTokenBuffer tokenTailBuffer, ObjectTailBuffer nodeTailBuffer)
        {
            JsonToken keyToken = NextAndGetToken(tokenizer, tokenTailBuffer, nodeTailBuffer);
            if (TracingEnabled) {
                tokenTailBuffer.Push(keyToken);
            }
            var keyType = keyToken.Type;
            if (keyType != TokenType.STRING) {
                throw new InvalidJsonTokenException("JSON Object member should start with a string key. keyType = " + keyType + "; " + tokenTailBuffer.ToTraceString(), GetTailCharStream(tokenizer), PeekCharStream(tokenizer));
            }
            string key = (string)keyToken.Value;

            JsonToken colonToken = NextAndGetToken(tokenizer, tokenTailBuffer, nodeTailBuffer); // "consume" :.
            if (TracingEnabled) {
                tokenTailBuffer.Push(colonToken);
            }
            var colonType = colonToken.Type;
            if (colonType != TokenType.COLON) {
                throw new InvalidJsonTokenException("JSON Object member should include a colon (:). " + tokenTailBuffer.ToTraceString(), GetTailCharStream(tokenizer), PeekCharStream(tokenizer));
            }

            object value = null;
            var type = PeekAndGetType(tokenizer, tokenTailBuffer, nodeTailBuffer);
            switch (type) {
                case TokenType.NULL:
                    value = ProduceJsonNull(tokenizer, tokenTailBuffer, nodeTailBuffer);
                    break;
                case TokenType.BOOLEAN:
                    value = ProduceJsonBoolean(tokenizer, tokenTailBuffer, nodeTailBuffer);
                    break;
                case TokenType.NUMBER:
                    value = ProduceJsonNumber(tokenizer, tokenTailBuffer, nodeTailBuffer);
                    break;
                case TokenType.STRING:
                    value = ProduceJsonString(tokenizer, tokenTailBuffer, nodeTailBuffer);
                    break;
                case TokenType.LCURLY:
                    value = await ProduceJsonObjectAsync(tokenizer, depth - 1, tokenTailBuffer, nodeTailBuffer);
                    break;
                case TokenType.LSQUARE:
                    value = await ProduceJsonArrayAsync(tokenizer, depth - 1, tokenTailBuffer, nodeTailBuffer);
                    break;
                default:
                    // ???
                    throw new InvalidJsonTokenException("Json array element not recognized: token = " + tokenizer.Peek() + "; " + tokenTailBuffer.ToTraceString(), GetTailCharStream(tokenizer), PeekCharStream(tokenizer));
            }

            // TBD: Use type factory ???
            MapEntry<string, object> member = new MapEntry<string, object>(key, value);

            //if (log.IsLoggable(Level.FINER)) {
            //    log.finer("member = " + member);
            //}
            return member;
        }


        private async Task<IList<object>> ProduceJsonArrayAsync(JsonTokenizer tokenizer, int depth, JsonTokenBuffer tokenTailBuffer, ObjectTailBuffer nodeTailBuffer)
        {
            TokenType lsq;
            if (TracingEnabled) {
                JsonToken t = NextAndGetToken(tokenizer, tokenTailBuffer, nodeTailBuffer); // pop the leading [.
                tokenTailBuffer.Push(t);
                lsq = t.Type;
            } else {
                lsq = NextAndGetType(tokenizer, tokenTailBuffer, nodeTailBuffer);
            }
            if (lsq != TokenType.LSQUARE) {
                // this cannot happen.
                throw new InvalidJsonTokenException("JSON array should start with [. " + tokenTailBuffer.ToTraceString(), GetTailCharStream(tokenizer), PeekCharStream(tokenizer));
            }

            IList<object> list = new List<object>();
            var type = PeekAndGetType(tokenizer, tokenTailBuffer, nodeTailBuffer);
            if (type == TokenType.RSQUARE) {
                // empty array
                JsonToken t = tokenizer.Next(); // discard the trailing ].
                if (TracingEnabled) {
                    tokenTailBuffer.Push(t);
                }
            } else {
                IList<object> elements = await ProduceJsonArrayElementsAsync(tokenizer, depth, tokenTailBuffer, nodeTailBuffer);

                var rsq = NextAndGetType(tokenizer, tokenTailBuffer, nodeTailBuffer); // discard the trailing ].
                if (rsq == TokenType.RSQUARE) {
                    // Done
                    ((List<object>)list).AddRange(elements);
                } else {
                    // ???
                    throw new InvalidJsonTokenException("JSON array should end with ]. " + tokenTailBuffer.ToTraceString(), GetTailCharStream(tokenizer), PeekCharStream(tokenizer));
                }
            }
            IList<object> jArray = jsonTypeFactory.CreateArray(list);
            if (TracingEnabled) {
                nodeTailBuffer.Push(jArray);
            }

            //if (log.IsLoggable(Level.FINE)) {
            //    log.fine("jArray = " + jArray);
            //}
            return jArray;
        }

        private async Task<IList<object>> ProduceJsonArrayElementsAsync(JsonTokenizer tokenizer, int depth, JsonTokenBuffer tokenTailBuffer, ObjectTailBuffer nodeTailBuffer)
        {
            IList<object> elements = new List<object>();
            var type = PeekAndGetType(tokenizer, tokenTailBuffer, nodeTailBuffer);
            while (type != TokenType.RSQUARE) {
                object element = await ProduceJsonArrayElementAsync(tokenizer, depth, tokenTailBuffer, nodeTailBuffer); // No depth change...
                if (element != null) {
                    // This is a bit strange implementation, but it actually works...
                    // The parser traverses the object tree (depth first) down to the leaf,
                    //    and on the way up, if it reaches depth==1, it converts the sub-tree below depth==1 to Json String.
                    //    the json string is then used as the node value (instead of the object sub tree).
                    // Going above depth > 1, this transformation does not happen.
                    if (depth == 1) {
                        try {
                            // String jStr = _escapeString(jsonBuilder.BuildAsync(element));
                            string jStr = await jsonBuilder.BuildAsync(element);
                            // Object elObj = jsonBuilder.buildJsonStructure(element);
                            // String jStr = jsonBuilder.BuildAsync(elObj);
                            // String jStr = jsonBuilder.BuildAsync(element, 1);
                            // log.warning(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> depth = " + depth + " ]]]]]]] jStr = " + jStr);
                            elements.Add(jStr);
                        } catch (JsonBuilderException e) {
                            throw new JsonParserException(e);
                        }
                    } else {
                        elements.Add(element);
                    }
                }
                type = PeekAndGetType(tokenizer, tokenTailBuffer, nodeTailBuffer);

                // "consume" the comma.
                if (parserPolicy.AllowExtraCommas) {
                    while (type == TokenType.COMMA) {
                        JsonToken t = tokenizer.Next();
                        if (TracingEnabled) {
                            tokenTailBuffer.Push(t);
                        }
                        type = PeekAndGetType(tokenizer, tokenTailBuffer, nodeTailBuffer);
                    }
                } else {
                    if (type == TokenType.COMMA) {
                        JsonToken t = tokenizer.Next();
                        if (TracingEnabled) {
                            tokenTailBuffer.Push(t);
                        }
                        type = PeekAndGetType(tokenizer, tokenTailBuffer, nodeTailBuffer);

                        if (parserPolicy.AllowTrailingComma) {
                            // Continue.
                        } else {
                            // Invalid  char sequence: ",]" 
                            if (type == TokenType.RSQUARE) {
                                throw new InvalidJsonTokenException("Syntax error: Array has a trailing comma. " + tokenTailBuffer.ToTraceString(), GetTailCharStream(tokenizer), PeekCharStream(tokenizer));
                            }
                        }
                    }
                }
            }

            //if (log.IsLoggable(Level.FINER)) {
            //    log.finer("elements = " + elements);
            //}
            return elements;
        }
        private async Task<object> ProduceJsonArrayElementAsync(JsonTokenizer tokenizer, int depth, JsonTokenBuffer tokenTailBuffer, ObjectTailBuffer nodeTailBuffer)
        {
            object element = null;
            var type = PeekAndGetType(tokenizer, tokenTailBuffer, nodeTailBuffer);
            switch (type) {
                case TokenType.NULL:
                    element = ProduceJsonNull(tokenizer, tokenTailBuffer, nodeTailBuffer);
                    break;
                case TokenType.BOOLEAN:
                    element = ProduceJsonBoolean(tokenizer, tokenTailBuffer, nodeTailBuffer);
                    break;
                case TokenType.NUMBER:
                    element = ProduceJsonNumber(tokenizer, tokenTailBuffer, nodeTailBuffer);
                    break;
                case TokenType.STRING:
                    element = ProduceJsonString(tokenizer, tokenTailBuffer, nodeTailBuffer);
                    break;
                case TokenType.LCURLY:
                    element = await ProduceJsonObjectAsync(tokenizer, depth - 1, tokenTailBuffer, nodeTailBuffer);
                    break;
                case TokenType.LSQUARE:
                    element = await ProduceJsonArrayAsync(tokenizer, depth - 1, tokenTailBuffer, nodeTailBuffer);
                    break;
                default:
                    // ???
                    throw new InvalidJsonTokenException("Json array element not recognized: token = " + tokenizer.Peek() + "; " + tokenTailBuffer.ToTraceString(), GetTailCharStream(tokenizer), PeekCharStream(tokenizer));
            }

            //if (log.IsLoggable(Level.FINER)) {
            //    log.finer("element = " + element);
            //}
            return element;
        }

        private JsonToken PeekAndGetToken(JsonTokenizer tokenizer, JsonTokenBuffer tokenTailBuffer, ObjectTailBuffer nodeTailBuffer)
        {
            JsonToken s = tokenizer.Peek();
            if (s == null) {
                throw new UnknownParserException("Failed to get the next json token. " + tokenTailBuffer.ToTraceString(), GetTailCharStream(tokenizer), PeekCharStream(tokenizer));
            }
            return s;
        }
        private TokenType PeekAndGetType(JsonTokenizer tokenizer, JsonTokenBuffer tokenTailBuffer, ObjectTailBuffer nodeTailBuffer)
        {
            JsonToken s = tokenizer.Peek();
            if (s == null) {
                throw new UnknownParserException("Failed to get the next json token. " + tokenTailBuffer.ToTraceString(), GetTailCharStream(tokenizer), PeekCharStream(tokenizer));
            }
            var type = s.Type;
            return type;
        }
        private JsonToken NextAndGetToken(JsonTokenizer tokenizer, JsonTokenBuffer tokenTailBuffer, ObjectTailBuffer nodeTailBuffer)
        {
            JsonToken s = tokenizer.Next();
            if (s == null) {
                throw new UnknownParserException("Failed to get the next json token. " + tokenTailBuffer.ToTraceString(), GetTailCharStream(tokenizer), PeekCharStream(tokenizer));
            }
            return s;
        }
        private TokenType NextAndGetType(JsonTokenizer tokenizer, JsonTokenBuffer tokenTailBuffer, ObjectTailBuffer nodeTailBuffer)
        {
            JsonToken s = tokenizer.Next();
            if (s == null) {
                throw new UnknownParserException("Failed to get the next json token. " + tokenTailBuffer.ToTraceString(), GetTailCharStream(tokenizer), PeekCharStream(tokenizer));
            }
            var type = s.Type;
            return type;
        }

        private string ProduceJsonString(JsonTokenizer tokenizer, JsonTokenBuffer tokenTailBuffer, ObjectTailBuffer nodeTailBuffer)
        {
            string jString = null;
            try {
                JsonToken t = tokenizer.Next();
                // log.warning(">>>>>>>>>>>>>>>>>>>>>>>>>>>>> t = " + t);
                if (TracingEnabled) {
                    tokenTailBuffer.Push(t);
                }
                string value = (string)t.Value;
                // log.warning(">>>>>>>>>>>>>>>>>>>>>>>>>>>>> value = " + value);
                jString = (string)jsonTypeFactory.CreateString(value);
                // log.warning(">>>>>>>>>>>>>>>>>>>>>>>>>>>>> jString = " + jString);
            } catch (Exception e) {
                throw new UnknownParserException("Failed to Create a String node. " + tokenTailBuffer.ToTraceString(), e, GetTailCharStream(tokenizer), PeekCharStream(tokenizer));
            }
            if (TracingEnabled) {
                nodeTailBuffer.Push(jString);
            }
            return jString;
        }
        private Number? ProduceJsonNumber(JsonTokenizer tokenizer, JsonTokenBuffer tokenTailBuffer, ObjectTailBuffer nodeTailBuffer)
        {
            Number? jNumber = null;
            try {
                JsonToken t = tokenizer.Next();
                if (TracingEnabled) {
                    tokenTailBuffer.Push(t);
                }
                var value = (Number)t.Value;
                jNumber = (Number?)jsonTypeFactory.CreateNumber((Number?)value);
            } catch (Exception e) {
                throw new UnknownParserException("Failed to Create a Number node. " + tokenTailBuffer.ToTraceString(), e, GetTailCharStream(tokenizer), PeekCharStream(tokenizer));
            }
            if (TracingEnabled) {
                nodeTailBuffer.Push(jNumber);
            }
            return jNumber;
        }
        private bool? ProduceJsonBoolean(JsonTokenizer tokenizer, JsonTokenBuffer tokenTailBuffer, ObjectTailBuffer nodeTailBuffer)
        {
            bool? jBoolean = null;
            try {
                JsonToken t = tokenizer.Next();
                if (TracingEnabled) {
                    tokenTailBuffer.Push(t);
                }
                var value = (bool)t.Value;
                jBoolean = (bool?)jsonTypeFactory.CreateBoolean((bool?)value);
            } catch (Exception e) {
                throw new UnknownParserException("Failed to Create a Boolean node. " + tokenTailBuffer.ToTraceString(), e, GetTailCharStream(tokenizer), PeekCharStream(tokenizer));
            }
            if (TracingEnabled) {
                nodeTailBuffer.Push(jBoolean);
            }
            return jBoolean;
        }

        private object ProduceJsonNull(JsonTokenizer tokenizer, JsonTokenBuffer tokenTailBuffer, ObjectTailBuffer nodeTailBuffer)
        {
            object jNull = null;
            try {
                JsonToken t = tokenizer.Next(); // Consume the "null" literal.
                if (TracingEnabled) {
                    tokenTailBuffer.Push(t);
                }
                jNull = jsonTypeFactory.CreateNull();
            } catch (Exception e) {
                throw new UnknownParserException("Failed to Create a Null node. " + tokenTailBuffer.ToTraceString(), e, GetTailCharStream(tokenizer), PeekCharStream(tokenizer));
            }
            if (TracingEnabled) {
                nodeTailBuffer.Push(jNull);
            }
            return jNull;
        }


        private string _escapeString(string primStr)
        {
            StringBuilder sb = new StringBuilder();
            if (!string.ReferenceEquals(primStr, null) && primStr.Length > 0) {
                char[] primChars = primStr.ToCharArray();
                char prevEc = (char)0;
                foreach (char ec in primChars) {
                    if (Symbols.IsEscapedChar(ec)) {
                        if (prevEc == '<' && ec == '/') {
                            sb.Append("\\/");
                        } else {
                            string str = Symbols.GetEscapedCharString(ec, false);
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
            return sb.ToString();
        }

        private async Task<string> _readFromReaderAsync(TextReader reader)
        {
            try {
                StringBuilder sb = new StringBuilder();
                string line = null;
                while ((line = await reader.ReadLineAsync()) != null) {
                    sb.Append(line);
                }
                return sb.ToString();
            } catch (Exception ex) {
                // log...
            }
            return null;
        }


    }

}