using DotJson.Common;
using DotJson.Core;
using DotJson.Parser.Core;
using DotJson.Parser.Exceptions;
using DotJson.Parser.Policy;
using DotJson.Parser.Policy.Base;
using DotJson.Type.Factory;
using DotJson.Type.Factory.Impl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DotJson.Parser.Impl
{
    // Recursive descent parser implementation using C# types.
    public abstract class AbstractBareJsonParser : AbstractJsonParser, BareJsonParser
    {
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
        // --> Make them arguments of _parse() methods ???
        //    (Reusing jsonTokenizer is good for performance, but can make it potentially not thread-safe....)
        //    (Same for tokenTailBuffer and nodeTailBuffer....)

        // This is lazy initialized, and reused across multiple parse() sessions.
        private JsonTokenizer mJsonTokenizer = null;

        // For debugging/tracing...
        private JsonTokenBuffer mTokenTailBuffer = null;
        private ObjectTailBuffer mNodeTailBuffer = null;


        public AbstractBareJsonParser() : this(null)
        {
        }
        public AbstractBareJsonParser(JsonTypeFactory jsonTypeFactory) 
            : this(jsonTypeFactory, null)
        {
        }
        public AbstractBareJsonParser(JsonTypeFactory jsonTypeFactory, ParserPolicy parserPolicy) 
            : this(jsonTypeFactory, parserPolicy, false) // true or false ??
        {
        }
        public AbstractBareJsonParser(JsonTypeFactory jsonTypeFactory, ParserPolicy parserPolicy, bool threadSafe)
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

            // For subclasses
            Init();
        }

        // Having multiple ctor's is a bit inconvenient.
        // Put all init routines here.
        // To be overridden in subclasses.
        protected internal virtual void Init()
        {
            // Enabling look-ahead-parsing can cause parse failure because it cannot handle long strings...
            EnableLookAheadParsing();
            // disableLookAheadParsing();
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
            object topNode = _parse(reader);
            // TBD:
            // Convert it to map/list...
            // ...
            return topNode;
        }

        private object _parse(TextReader reader)
        {
            if (reader == null) {
                return null;
            }

            // TBD:
            // Does this make it thread safe???
            // ...

            JsonTokenizer jsonTokenizer = null;
            if (threadSafe) {
                // ???
                //jsonTokenizer = new AbstractJsonTokenizer(reader, parserPolicy) {};
                jsonTokenizer = new SimpleJsonTokenizer(reader, parserPolicy);
                SetLookAheadParsing(jsonTokenizer);
                SetTokenizerTracing(jsonTokenizer);
            } else {
                if (mJsonTokenizer != null && mJsonTokenizer is AbstractJsonTokenizer) {
                    ((AbstractJsonTokenizer)mJsonTokenizer).Reset(reader);
                } else {
                    // mJsonTokenizer = new AbstractJsonTokenizer(reader, parserPolicy) {};
                    mJsonTokenizer = new SimpleJsonTokenizer(reader, parserPolicy);
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

            return _parse(jsonTokenizer, tokenTailBuffer, nodeTailBuffer);
        }
        private object _parse(JsonTokenizer tokenizer, JsonTokenBuffer tokenTailBuffer, ObjectTailBuffer nodeTailBuffer)
        {
            if (tokenizer == null) {
                return null;
            }

            object topNode = null;
            var type = PeekAndGetType(tokenizer, tokenTailBuffer, nodeTailBuffer);
            if (type == TokenType.EOF || type == TokenType.LCURLY || type == TokenType.LSQUARE) {
                if (type == TokenType.EOF) {
                    topNode = ProduceJsonNull(tokenizer, tokenTailBuffer, nodeTailBuffer);
                } else if (type == TokenType.LCURLY) {
                    topNode = ProduceJsonObject(tokenizer, tokenTailBuffer, nodeTailBuffer);
                } else if (type == TokenType.LSQUARE) {
                    topNode = ProduceJsonArray(tokenizer, tokenTailBuffer, nodeTailBuffer);
                }
            } else {
                // TBD:
                // Process it here if parserPolicy.AllowLeadingJsonMarker() == true,
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
                    // For now, if parserPolicy.AllowLeadingJsonMarker() == true is interpreted as allowLeadingNonObjectNonArrayChars....
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
                            topNode = ProduceJsonObject(tokenizer, tokenTailBuffer, nodeTailBuffer);
                        } else if (type == TokenType.LSQUARE) {
                            topNode = ProduceJsonArray(tokenizer, tokenTailBuffer, nodeTailBuffer);
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

            //if (log.isLoggable(Level.FINE)) {
            //    log.fine("topnNode = " + topNode);
            //}
            return topNode;
        }

        private IDictionary<string, object> ProduceJsonObject(JsonTokenizer tokenizer, JsonTokenBuffer tokenTailBuffer, ObjectTailBuffer nodeTailBuffer)
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
                IDictionary<string, object> members = ProduceJsonObjectMembers(tokenizer, tokenTailBuffer, nodeTailBuffer);
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
                // ???
                nodeTailBuffer.Push(jObject);
            }

            //if (log.isLoggable(Level.FINE)) {
            //    log.fine("jObject = " + jObject);
            //}
            return jObject;
        }

        private IDictionary<string, object> ProduceJsonObjectMembers(JsonTokenizer tokenizer, JsonTokenBuffer tokenTailBuffer, ObjectTailBuffer nodeTailBuffer)
        {
            IDictionary<string, object> members = new Dictionary<string, object>();

            var type = PeekAndGetType(tokenizer, tokenTailBuffer, nodeTailBuffer);
            while (type != TokenType.RCURLY) {
                // KeyValuePair<string, object> member = ProduceJsonObjectMember(tokenizer, tokenTailBuffer, nodeTailBuffer);
                var member = ProduceJsonObjectMember(tokenizer, tokenTailBuffer, nodeTailBuffer);
                if (member != null) {
                    members[member.Key] = member.Value;
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

            //if (log.isLoggable(Level.FINER)) {
            //    log.finer("members = " + members);
            //}
            return members;
        }
        private MapEntry<string, object> ProduceJsonObjectMember(JsonTokenizer tokenizer, JsonTokenBuffer tokenTailBuffer, ObjectTailBuffer nodeTailBuffer)
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
                    value = ProduceJsonObject(tokenizer, tokenTailBuffer, nodeTailBuffer);
                    break;
                case TokenType.LSQUARE:
                    value = ProduceJsonArray(tokenizer, tokenTailBuffer, nodeTailBuffer);
                    break;
                default:
                    // ???
                    throw new InvalidJsonTokenException("Json array element not recognized: token = " + tokenizer.Peek() + "; " + tokenTailBuffer.ToTraceString(), GetTailCharStream(tokenizer), PeekCharStream(tokenizer));
            }

            // TBD: Use type factory ???
            // KeyValuePair<string, object> member = new MapEntry<string, object>(key, value);
            var member = new MapEntry<string, object>(key, value);

            //if (log.isLoggable(Level.FINER)) {
            //    log.finer("member = " + member);
            //}
            return member;
        }

        private IList<object> ProduceJsonArray(JsonTokenizer tokenizer, JsonTokenBuffer tokenTailBuffer, ObjectTailBuffer nodeTailBuffer)
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
                IList<object> elements = ProduceJsonArrayElements(tokenizer, tokenTailBuffer, nodeTailBuffer);

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
                // ???
                nodeTailBuffer.Push(jArray);
            }

            //if (log.isLoggable(Level.FINE)) {
            //    log.fine("jArray = " + jArray);
            //}
            return jArray;
        }

        private IList<object> ProduceJsonArrayElements(JsonTokenizer tokenizer, JsonTokenBuffer tokenTailBuffer, ObjectTailBuffer nodeTailBuffer)
        {
            IList<object> elements = new List<object>();

            var type = PeekAndGetType(tokenizer, tokenTailBuffer, nodeTailBuffer);
            while (type != TokenType.RSQUARE) {
                object element = ProduceJsonArrayElement(tokenizer, tokenTailBuffer, nodeTailBuffer);
                if (element != null) {
                    elements.Add(element);
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

            //if (log.isLoggable(Level.FINER)) {
            //    log.finer("elements = " + elements);
            //}
            return elements;
        }
        private object ProduceJsonArrayElement(JsonTokenizer tokenizer, JsonTokenBuffer tokenTailBuffer, ObjectTailBuffer nodeTailBuffer)
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
                    element = ProduceJsonObject(tokenizer, tokenTailBuffer, nodeTailBuffer);
                    break;
                case TokenType.LSQUARE:
                    element = ProduceJsonArray(tokenizer, tokenTailBuffer, nodeTailBuffer);
                    break;
                default:
                    // ???
                    throw new InvalidJsonTokenException("Json array element not recognized: token = " + tokenizer.Peek() + "; " + tokenTailBuffer.ToTraceString(), GetTailCharStream(tokenizer), PeekCharStream(tokenizer));
            }

            //if (log.isLoggable(Level.FINER)) {
            //    log.finer("element = " + element);
            //}
            return element;
        }

        private JsonToken PeekAndGetToken(JsonTokenizer tokenizer, JsonTokenBuffer tokenTailBuffer, ObjectTailBuffer nodeTailBuffer)
        {
            JsonToken s = tokenizer.Peek();
            if (s.IsInvalid) {
                throw new UnknownParserException("Failed to get the Next json token. " + tokenTailBuffer.ToTraceString(), GetTailCharStream(tokenizer), PeekCharStream(tokenizer));
            }
            return s;
        }
        private TokenType PeekAndGetType(JsonTokenizer tokenizer, JsonTokenBuffer tokenTailBuffer, ObjectTailBuffer nodeTailBuffer)
        {
            JsonToken s = tokenizer.Peek();
            if (s.IsInvalid) {
                throw new UnknownParserException("Failed to get the Next json token. " + tokenTailBuffer.ToTraceString(), GetTailCharStream(tokenizer), PeekCharStream(tokenizer));
            }
            var type = s.Type;
            return type;
        }
        private JsonToken NextAndGetToken(JsonTokenizer tokenizer, JsonTokenBuffer tokenTailBuffer, ObjectTailBuffer nodeTailBuffer)
        {
            JsonToken s = tokenizer.Next();
            if (s.IsInvalid) {
                throw new UnknownParserException("Failed to get the Next json token. " + tokenTailBuffer.ToTraceString(), GetTailCharStream(tokenizer), PeekCharStream(tokenizer));
            }
            return s;
        }
        private TokenType NextAndGetType(JsonTokenizer tokenizer, JsonTokenBuffer tokenTailBuffer, ObjectTailBuffer nodeTailBuffer)
        {
            JsonToken s = tokenizer.Next();
            if (s.IsInvalid) {
                throw new UnknownParserException("Failed to get the Next json token. " + tokenTailBuffer.ToTraceString(), GetTailCharStream(tokenizer), PeekCharStream(tokenizer));
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
                throw new UnknownParserException("Failed to create a String node. " + tokenTailBuffer.ToTraceString(), e, GetTailCharStream(tokenizer), PeekCharStream(tokenizer));
            }
            if (TracingEnabled) {
                // ???
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
                jNumber = (Number?)jsonTypeFactory.CreateNumber((Number?) value);
            } catch (Exception e) {
                throw new UnknownParserException("Failed to create a Number node. " + tokenTailBuffer.ToTraceString(), e, GetTailCharStream(tokenizer), PeekCharStream(tokenizer));
            }
            if (TracingEnabled) {
                // ????
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
                jBoolean = (bool?)jsonTypeFactory.CreateBoolean((bool?) value);
            } catch (Exception e) {
                throw new UnknownParserException("Failed to create a Boolean node. " + tokenTailBuffer.ToTraceString(), e, GetTailCharStream(tokenizer), PeekCharStream(tokenizer));
            }
            if (TracingEnabled) {
                //???
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
                throw new UnknownParserException("Failed to create a Null node. " + tokenTailBuffer.ToTraceString(), e, GetTailCharStream(tokenizer), PeekCharStream(tokenizer));
            }
            if (TracingEnabled) {
                // ???
                nodeTailBuffer.Push(jNull);
            }
            return jNull;
        }


    }

}