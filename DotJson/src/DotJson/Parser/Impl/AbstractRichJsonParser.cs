using DotJson.Common;
using DotJson.Core;
using DotJson.Parser.Exceptions;
using DotJson.Parser.Policy;
using DotJson.Parser.Policy.Base;
using DotJson.Type;
using DotJson.Type.Base;
using DotJson.Type.Factory;
using DotJson.Type.Factory.Impl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DotJson.Parser.Impl
{
    // Recursive descent parser implementation.
    public abstract class AbstractRichJsonParser : AbstractJsonParser, RichJsonParser, FlexibleJsonParser
    {
        private JsonTypeFactory jsonTypeFactory = null;
        private readonly ParserPolicy parserPolicy;

        public AbstractRichJsonParser() : this(null)
        {
        }
        public AbstractRichJsonParser(JsonTypeFactory jsonTypeFactory) 
            : this(jsonTypeFactory, null)
        {
        }
        public AbstractRichJsonParser(JsonTypeFactory jsonTypeFactory, ParserPolicy parserPolicy)
        {
            // temporary
            if (jsonTypeFactory == null) {
                this.jsonTypeFactory = AbstractJsonTypeFactory.Instance;
            } else {
                this.jsonTypeFactory = jsonTypeFactory;
            }
            if (parserPolicy == null) {
                this.parserPolicy = DefaultParserPolicy.MINIJSON;
            } else {
                this.parserPolicy = parserPolicy;
            }

            // For subclasses
            Init();
        }

        // Having multiple ctor's is a bit inconvenient.
        // Put all Init routines here.
        // To be overridden in subclasses.
        protected internal virtual void Init()
        {
            EnableLookAheadParsing();
            EnableTracing();
        }


        public ParserPolicy ParserPolicy
        {
            get
            {
                return parserPolicy;
            }
        }
        public override async Task<object> ParseAsync(string jsonStr)
        {
            StringReader sr = new StringReader(jsonStr);
            object jsonObj = null;
            try {
                jsonObj = await ParseAsync(sr);
            } catch (IOException e) {
                throw new JsonParserException("IO error during JSON parsing.", e);
            }
            return jsonObj;
        }
        public override async Task<object> ParseAsync(TextReader reader)
        {
            JsonNode topNode = await ParseJsonAsync(reader);
            // TBD:
            // Convert it to map/list...
            // ...
            return topNode;
        }

        public async Task<JsonNode> ParseJsonAsync(string jsonStr)
        {
            StringReader sr = new StringReader(jsonStr);
            JsonNode jsonObj = null;
            try {
                jsonObj = await ParseJsonAsync(sr);
            } catch (IOException e) {
                throw new JsonParserException("IO error during JSON parsing.", e);
            }
            return jsonObj;
        }
        public async Task<JsonNode> ParseJsonAsync(TextReader reader)
        {
            return await _ParseAsync(reader);
        }

        private async Task<JsonNode> _ParseAsync(TextReader reader)
        {
            if (reader == null) {
                return null;
            }
            JsonTokenizer tokenizer = new CustomJsonTokenizer(reader, parserPolicy);
            SetLookAheadParsing(tokenizer);
            SetTokenizerTracing(tokenizer);
            return await _ParseAsync(tokenizer);
        }
        private async Task<JsonNode> _ParseAsync(JsonTokenizer tokenizer)
        {
            if (tokenizer == null) {
                return null;
            }

            JsonNode topNode = null;
            var type = PeekAndGetType(tokenizer);
            if (type == TokenType.EOF) {
                topNode = ProduceJsonNull(tokenizer);
            } else if (type == TokenType.LCURLY) {
                topNode = ProduceJsonObject(tokenizer);
            } else if (type == TokenType.LSQUARE) {
                topNode = ProduceJsonArray(tokenizer);
            } else {
                // TBD:
                // Process it here if parserPolicy.AllowLeadingJsonMarker() == true,
                // ???
                if (parserPolicy.AllowNonObjectOrNonArray) {
                    // This is actually error according to json.org JSON grammar.
                    // But, we allow partial JSON string.
                    switch (type) {
                        case TokenType.NULL:
                            topNode = ProduceJsonNull(tokenizer);
                            break;
                        case TokenType.BOOLEAN:
                            topNode = ProduceJsonBoolean(tokenizer);
                            break;
                        case TokenType.NUMBER:
                            topNode = ProduceJsonNumber(tokenizer);
                            break;
                        case TokenType.STRING:
                            topNode = ProduceJsonString(tokenizer);
                            break;
                        default:
                            // ???
                            throw new InvalidJsonTokenException("JsonToken not recognized: tokenType = " + TokenTypes.GetDisplayName(type), GetTailCharStream(tokenizer), PeekCharStream(tokenizer));
                    }
                } else {
                    // TBD
                    // this is a bit too lenient probably...
                    // there was some special char sequence which some parsers allowed, which I cannot remember..
                    // For now, if parserPolicy.AllowLeadingJsonMarker() == true is interpreted as allowLeadingNonObjectNonArrayChars....
                    //    --> we remove all leading chars until we reach { or [.
                    if (parserPolicy.AllowLeadingJsonMarker) {
                        while (type != TokenType.LCURLY && type != TokenType.LSQUARE) {
                            tokenizer.Next(); // swallow oen char.
                            type = PeekAndGetType(tokenizer);
                        }
                        if (type == TokenType.LCURLY) {
                            topNode = ProduceJsonObject(tokenizer);
                        } else if (type == TokenType.LSQUARE) {
                            topNode = ProduceJsonArray(tokenizer);
                        } else {
                            // ???
                            throw new InvalidJsonTokenException("Invalid input Json string.", GetTailCharStream(tokenizer), PeekCharStream(tokenizer));
                        }
                    } else {
                        // ???
                        throw new InvalidJsonTokenException("Json string should be Object or Array. Input tokenType = " + TokenTypes.GetDisplayName(type), GetTailCharStream(tokenizer), PeekCharStream(tokenizer));
                    }
                }
            }

            //if (log.isLoggable(Level.FINE)) {
            //    log.fine("topnNode = " + topNode);
            //}
            return topNode;
        }

        private JsonObjectNode ProduceJsonObject(JsonTokenizer tokenizer)
        {
            var lcurl = NextAndGetType(tokenizer); // pop the leading {.
            if (lcurl != TokenType.LCURLY) {
                // this cannot happen.
                throw new InvalidJsonTokenException("JSON object should start with {.", GetTailCharStream(tokenizer), PeekCharStream(tokenizer));
            }

            IDictionary<string, object> map = new Dictionary<string, object>();
            var type = PeekAndGetType(tokenizer);
            if (type == TokenType.RCURLY) {
                // empty object
                tokenizer.Next(); // discard the trailing }.
            } else {
                IDictionary<string, JsonNode> members = ProduceJsonObjectMembers(tokenizer);
                var rcurl = NextAndGetType(tokenizer); // discard the trailing }.
                if (rcurl == TokenType.RCURLY) {
                    // Done
                    // ???
                    // (map as Dictionary<string, object>)?.putAll(members);
                    // map = (IDictionary<string, JsonNode>) members; ????
                    foreach(var m in members) {
                        map[m.Key] = m.Value;
                    }
                    // ???
                } else {
                    // ???
                    throw new InvalidJsonTokenException("JSON object should end with }.", GetTailCharStream(tokenizer), PeekCharStream(tokenizer));
                }
            }
            JsonObjectNode jObject = (JsonObjectNode)jsonTypeFactory.CreateObject(map);

            //if (log.isLoggable(Level.FINE)) {
            //    log.fine("jObject = " + jObject);
            //}
            return jObject;
        }

        private IDictionary<string, JsonNode> ProduceJsonObjectMembers(JsonTokenizer tokenizer)
        {
            IDictionary<string, JsonNode> members = new Dictionary<string, JsonNode>();

            var type = PeekAndGetType(tokenizer);
            while (type != TokenType.RCURLY) {
                JsonObjectMember member = ProduceJsonObjectMember(tokenizer);
                if (member != null) {
                    members[member.Key] = member.Value;
                }
                type = PeekAndGetType(tokenizer);

                // "consume" the comma.
                if (parserPolicy.AllowExtraCommas) {
                    while (type == TokenType.COMMA) {
                        tokenizer.Next();
                        type = PeekAndGetType(tokenizer);
                    }
                } else {
                    if (type == TokenType.COMMA) {
                        tokenizer.Next();
                        type = PeekAndGetType(tokenizer);

                        if (parserPolicy.AllowTrailingComma) {
                            // Continue.
                        } else {
                            // Invalid  char sequence: ",}" 
                            if (type == TokenType.RCURLY) {
                                throw new InvalidJsonTokenException("Syntax error: Object has a trailing comma.", GetTailCharStream(tokenizer), PeekCharStream(tokenizer));
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
        private JsonObjectMember ProduceJsonObjectMember(JsonTokenizer tokenizer)
        {
            JsonToken keyToken = NextAndGetToken(tokenizer);
            var keyType = keyToken.Type;
            if (keyType != TokenType.STRING) {
                throw new InvalidJsonTokenException("JSON Object member should start with a string key.", GetTailCharStream(tokenizer), PeekCharStream(tokenizer));
            }
            string key = (string)keyToken.Value;

            JsonToken colonToken = NextAndGetToken(tokenizer); // "consume" :.
            var colonType = colonToken.Type;
            if (colonType != TokenType.COLON) {
                throw new InvalidJsonTokenException("JSON Object member should include a colon (:).", GetTailCharStream(tokenizer), PeekCharStream(tokenizer));
            }

            JsonNode value = null;
            var type = PeekAndGetType(tokenizer);
            switch (type) {
                case TokenType.NULL:
                    value = ProduceJsonNull(tokenizer);
                    break;
                case TokenType.BOOLEAN:
                    value = ProduceJsonBoolean(tokenizer);
                    break;
                case TokenType.NUMBER:
                    value = ProduceJsonNumber(tokenizer);
                    break;
                case TokenType.STRING:
                    value = ProduceJsonString(tokenizer);
                    break;
                case TokenType.LCURLY:
                    value = ProduceJsonObject(tokenizer);
                    break;
                case TokenType.LSQUARE:
                    value = ProduceJsonArray(tokenizer);
                    break;
                default:
                    // ???
                    throw new InvalidJsonTokenException("Json array element not recognized: token = " + tokenizer.Peek(), GetTailCharStream(tokenizer), PeekCharStream(tokenizer));
            }

            // TBD: Use type factory ???
            JsonObjectMember member = new AbstractJsonObjectMember(key, value);

            //if (log.isLoggable(Level.FINER)) {
            //    log.finer("member = " + member);
            //}
            return member;
        }


        private JsonArrayNode ProduceJsonArray(JsonTokenizer tokenizer)
        {
            var lsq = NextAndGetType(tokenizer); // pop the leading [.
            if (lsq != TokenType.LSQUARE) {
                // this cannot happen.
                throw new InvalidJsonTokenException("JSON array should start with [.", GetTailCharStream(tokenizer), PeekCharStream(tokenizer));
            }

            IList<object> list = new List<object>();
            var type = PeekAndGetType(tokenizer);
            if (type == TokenType.RSQUARE) {
                // empty array
                tokenizer.Next(); // discard the trailing ].
            } else {
                IList<JsonNode> elements = ProduceJsonArrayElements(tokenizer);

                var rsq = NextAndGetType(tokenizer); // discard the trailing ].
                if (rsq == TokenType.RSQUARE) {
                    // Done
                    ((List<object>)list).AddRange(elements);
                } else {
                    // ???
                    throw new InvalidJsonTokenException("JSON array should end with ].", GetTailCharStream(tokenizer), PeekCharStream(tokenizer));
                }
            }
            JsonArrayNode jArray = (JsonArrayNode)jsonTypeFactory.CreateArray(list);

            //if (log.isLoggable(Level.FINE)) {
            //    log.fine("jArray = " + jArray);
            //}
            return jArray;
        }

        private IList<JsonNode> ProduceJsonArrayElements(JsonTokenizer tokenizer)
        {
            IList<JsonNode> elements = new List<JsonNode>();

            var type = PeekAndGetType(tokenizer);
            while (type != TokenType.RSQUARE) {
                JsonNode element = ProduceJsonArrayElement(tokenizer);
                if (element != null) {
                    elements.Add(element);
                }
                type = PeekAndGetType(tokenizer);

                // "consume" the comma.
                if (parserPolicy.AllowExtraCommas) {
                    while (type == TokenType.COMMA) {
                        tokenizer.Next();
                        type = PeekAndGetType(tokenizer);
                    }
                } else {
                    if (type == TokenType.COMMA) {
                        tokenizer.Next();
                        type = PeekAndGetType(tokenizer);

                        if (parserPolicy.AllowTrailingComma) {
                            // Continue.
                        } else {
                            // Invalid  char sequence: ",]" 
                            if (type == TokenType.RSQUARE) {
                                throw new InvalidJsonTokenException("Syntax error: Array has a trailing comma.", GetTailCharStream(tokenizer), PeekCharStream(tokenizer));
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
        private JsonNode ProduceJsonArrayElement(JsonTokenizer tokenizer)
        {
            JsonNode element = null;
            var type = PeekAndGetType(tokenizer);
            switch (type) {
                case TokenType.NULL:
                    element = ProduceJsonNull(tokenizer);
                    break;
                case TokenType.BOOLEAN:
                    element = ProduceJsonBoolean(tokenizer);
                    break;
                case TokenType.NUMBER:
                    element = ProduceJsonNumber(tokenizer);
                    break;
                case TokenType.STRING:
                    element = ProduceJsonString(tokenizer);
                    break;
                case TokenType.LCURLY:
                    element = ProduceJsonObject(tokenizer);
                    break;
                case TokenType.LSQUARE:
                    element = ProduceJsonArray(tokenizer);
                    break;
                default:
                    // ???
                    throw new InvalidJsonTokenException("Json array element not recognized: token = " + tokenizer.Peek(), GetTailCharStream(tokenizer), PeekCharStream(tokenizer));
            }

            //if (log.isLoggable(Level.FINER)) {
            //    log.finer("element = " + element);
            //}
            return element;
        }

        private JsonToken PeekAndGetToken(JsonTokenizer tokenizer)
        {
            JsonToken s = tokenizer.Peek();
            if (s.IsInvalid) {
                throw new UnknownParserException("Failed to get the Next json token.", GetTailCharStream(tokenizer), PeekCharStream(tokenizer));
            }
            return s;
        }
        private TokenType PeekAndGetType(JsonTokenizer tokenizer)
        {
            JsonToken s = tokenizer.Peek();
            if (s.IsInvalid) {
                throw new UnknownParserException("Failed to get the Next json token.", GetTailCharStream(tokenizer), PeekCharStream(tokenizer));
            }
            var type = s.Type;
            return type;
        }
        private JsonToken NextAndGetToken(JsonTokenizer tokenizer)
        {
            JsonToken s = tokenizer.Next();
            if (s.IsInvalid) {
                throw new UnknownParserException("Failed to get the Next json token.", GetTailCharStream(tokenizer), PeekCharStream(tokenizer));
            }
            return s;
        }
        private TokenType NextAndGetType(JsonTokenizer tokenizer)
        {
            JsonToken s = tokenizer.Next();
            if (s.IsInvalid) {
                throw new UnknownParserException("Failed to get the Next json token.", GetTailCharStream(tokenizer), PeekCharStream(tokenizer));
            }
            var type = s.Type;
            return type;
        }

        private JsonStringNode ProduceJsonString(JsonTokenizer tokenizer)
        {
            JsonStringNode jString = null;
            try {
                JsonToken t = tokenizer.Next();
                string value = (string)t.Value;
                jString = (JsonStringNode)jsonTypeFactory.CreateString(value);
            } catch (Exception e) {
                throw new UnknownParserException("Failed to create a String node.", e, GetTailCharStream(tokenizer), PeekCharStream(tokenizer));
            }
            return jString;
        }
        private JsonNumberNode ProduceJsonNumber(JsonTokenizer tokenizer)
        {
            JsonNumberNode jNumber = null;
            try {
                JsonToken t = tokenizer.Next();
                Number value = (Number)t.Value;
                jNumber = (JsonNumberNode)jsonTypeFactory.CreateNumber(value);
            } catch (Exception e) {
                throw new UnknownParserException("Failed to create a Number node.", e, GetTailCharStream(tokenizer), PeekCharStream(tokenizer));
            }
            return jNumber;
        }
        private JsonBooleanNode ProduceJsonBoolean(JsonTokenizer tokenizer)
        {
            JsonBooleanNode jBoolean = null;
            try {
                JsonToken t = tokenizer.Next();
                bool? value = (bool?)t.Value;
                // log.warning(">>>>>>>>>>>>>>>>>> Boolean value = " + value);
                jBoolean = (JsonBooleanNode)jsonTypeFactory.CreateBoolean(value);
                // log.warning(">>>>>>>>>>>>>>>>>> jBoolean = " + jBoolean);
            } catch (Exception e) {
                throw new UnknownParserException("Failed to create a Boolean node.", e, GetTailCharStream(tokenizer), PeekCharStream(tokenizer));
            }
            return jBoolean;
        }

        private JsonNullNode ProduceJsonNull(JsonTokenizer tokenizer)
        {
            JsonNullNode jNull = null;
            try {
                tokenizer.Next(); // Consume the "null" literal.
                jNull = (JsonNullNode)jsonTypeFactory.CreateNull();
            } catch (Exception e) {
                throw new UnknownParserException("Failed to create a Null node.", e, GetTailCharStream(tokenizer), PeekCharStream(tokenizer));
            }
            return jNull;
        }

    }

}