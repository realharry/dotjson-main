package org.minijson.parser.partial.impl;

import static org.minijson.parser.core.TokenTypes.BOOLEAN;
import static org.minijson.parser.core.TokenTypes.COLON;
import static org.minijson.parser.core.TokenTypes.COMMA;
import static org.minijson.parser.core.TokenTypes.EOF;
import static org.minijson.parser.core.TokenTypes.LCURLY;
import static org.minijson.parser.core.TokenTypes.LSQUARE;
import static org.minijson.parser.core.TokenTypes.NULL;
import static org.minijson.parser.core.TokenTypes.NUMBER;
import static org.minijson.parser.core.TokenTypes.RCURLY;
import static org.minijson.parser.core.TokenTypes.RSQUARE;
import static org.minijson.parser.core.TokenTypes.STRING;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.Reader;
import java.io.StringReader;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.logging.Level;
import java.util.logging.Logger;

import org.minijson.builder.JsonBuilderException;
import org.minijson.builder.impl.AbstractBareJsonBuilder;
import org.minijson.builder.impl.SimpleJsonBuilder;
import org.minijson.common.Symbols;
import org.minijson.parser.JsonParserException;
import org.minijson.parser.JsonTokenizer;
import org.minijson.parser.core.JsonNodeBuffer;
import org.minijson.parser.core.JsonToken;
import org.minijson.parser.core.JsonTokenBuffer;
import org.minijson.parser.core.MapEntry;
import org.minijson.parser.core.TokenTypes;
import org.minijson.parser.exception.InvalidJsonTokenException;
import org.minijson.parser.exception.UnknownParserException;
import org.minijson.parser.impl.AbstractJsonParser;
import org.minijson.parser.impl.AbstractJsonTokenizer;
import org.minijson.parser.partial.LayeredJsonParser;
import org.minijson.parser.policy.ParserPolicy;
import org.minijson.parser.policy.base.DefaultParserPolicy;
import org.minijson.type.factory.JsonTypeFactory;
import org.minijson.type.factory.impl.BareJsonTypeFactory;
import org.minijson.util.CharacterUtil;
import org.minijson.util.UnicodeUtil;


/**
 * Recursive descent parser implementation.
 * Note that although it's a "partial" parser,
 *    there is no way to achieve "partial" parsing without scanning the whole json string.
 * parse() returns a nested tree of Map/List down to the maxDepth.
 * Any sub-tree below the maxDepth is just returned as "partial" json string representing that subtree.
 */
public abstract class AbstractLayeredJsonParser extends AbstractJsonParser implements LayeredJsonParser
{
    private static final Logger log = Logger.getLogger(AbstractLayeredJsonParser.class.getName());

    // temporary
    private static final int MAX_PARSE_DEPTH = (int) Byte.MAX_VALUE;  // arbitrary
    
    // temporary
    private static final int DEF_TOKEN_TAIL_BUFFER_SIZE = 24;
    private static final int DEF_NODE_TAIL_BUFFER_SIZE = 16;

    private final JsonTypeFactory jsonTypeFactory;
    private final ParserPolicy parserPolicy;

    // TBD: Not sure if threadSafe==true makes the code really thread safe.
    // It's just a flag for now (which may or may not ensure thread safety).
    private final boolean threadSafe;

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
    private JsonNodeBuffer mNodeTailBuffer = null;

    // temporary.
    // Is it safe to re-use this ????
    private final AbstractBareJsonBuilder jsonBuilder;
    // ....

    
    public AbstractLayeredJsonParser()
    {
        this(null);
    }
    public AbstractLayeredJsonParser(JsonTypeFactory jsonTypeFactory)
    {
        this(jsonTypeFactory, null);
    }
    public AbstractLayeredJsonParser(JsonTypeFactory jsonTypeFactory, ParserPolicy parserPolicy)
    {
        this(jsonTypeFactory, parserPolicy, false);   // true or false ??
    }
    public AbstractLayeredJsonParser(JsonTypeFactory jsonTypeFactory, ParserPolicy parserPolicy, boolean threadSafe)
    {
        // temporary
        if(jsonTypeFactory == null) {
            this.jsonTypeFactory = BareJsonTypeFactory.getInstance();
        } else {
            this.jsonTypeFactory = jsonTypeFactory;
        }
        if(parserPolicy == null) {
            this.parserPolicy = DefaultParserPolicy.MINIJSON;
        } else {
            this.parserPolicy = parserPolicy;
        }
        this.threadSafe = threadSafe;
        
        // ???
        // jsonBuilder = new AbstractBareJsonBuilder() {};
        jsonBuilder = new SimpleJsonBuilder();

        // For subclasses
        init();
    }

    // Having multiple ctor's is a bit inconvenient.
    // Put all init routines here.
    // To be overridden in subclasses.
    protected void init()
    {
        // Enabling look-ahead-parsing can cause parse failure because it cannot handle long strings...
        enableLookAheadParsing();
        // disableLookAheadParsing();
        disableTracing();
    }


    @Override
    public Object parse(String jsonStr) throws JsonParserException
    {
        StringReader sr = new StringReader(jsonStr);
        Object jsonObj = null;
        try {
            jsonObj = parse(sr);
        } catch (IOException e) {
            // throw new JsonParserException("IO error during JSON parsing. " + tokenTailBuffer.toTraceString(), e);
            throw new JsonParserException("IO error during JSON parsing.", e);
        }
        return jsonObj;
    }
    @Override
    public Object parse(Reader reader) throws JsonParserException, IOException
    {
        Object topNode = _parse(reader);
        // TBD:
        // Convert it to map/list... ??
        // ...
        return topNode;
    }

    @Override
    public Object parse(String jsonStr, int depth) throws JsonParserException
    {
        StringReader sr = new StringReader(jsonStr);
        Object jsonObj = null;
        try {
            jsonObj = parse(sr, depth);
        } catch (IOException e) {
            // throw new JsonParserException("IO error during JSON parsing. " + tokenTailBuffer.toTraceString(), e);
            throw new JsonParserException("IO error during JSON parsing.", e);
        }
        return jsonObj;
    }

    @Override
    public Object parse(Reader reader, int depth) throws JsonParserException, IOException
    {
        Object topNode = _parse(reader, depth);
        return topNode;
    }


    private Object _parse(Reader reader) throws JsonParserException, IOException
    {
        return _parse(reader, MAX_PARSE_DEPTH);
    }
    private Object _parse(Reader reader, int maxDepth) throws JsonParserException, IOException
    {
        if(reader == null) {
            return null;
        }
        if(maxDepth < 0) {
            maxDepth = MAX_PARSE_DEPTH;
        }
        
        // ???
        if(maxDepth == 0) {
            // Returns the original JSON string ???
            String jStr = _readFromReader(reader);
            return jStr;
        }

        // TBD:
        // Does this make it thread safe???
        // ...

        JsonTokenizer jsonTokenizer = null;
        if(threadSafe) {
            // ???
            jsonTokenizer = new AbstractJsonTokenizer(reader, parserPolicy) {};
            setLookAheadParsing(jsonTokenizer);
            setTokenizerTracing(jsonTokenizer);
        } else {
            if(mJsonTokenizer != null && mJsonTokenizer instanceof AbstractJsonTokenizer) {
                ((AbstractJsonTokenizer) mJsonTokenizer).reset(reader);
            } else {
                mJsonTokenizer = new AbstractJsonTokenizer(reader, parserPolicy) {};
                setLookAheadParsing(mJsonTokenizer);
                setTokenizerTracing(mJsonTokenizer);
            }
            jsonTokenizer = mJsonTokenizer;
        }

        JsonTokenBuffer tokenTailBuffer = null;
        if(threadSafe) {
            tokenTailBuffer = new JsonTokenBuffer(DEF_TOKEN_TAIL_BUFFER_SIZE);
        } else {
            if(mTokenTailBuffer != null) {
                mTokenTailBuffer.reset();
            } else {
                mTokenTailBuffer = new JsonTokenBuffer(DEF_TOKEN_TAIL_BUFFER_SIZE);
            }
            tokenTailBuffer = mTokenTailBuffer;
        }

        JsonNodeBuffer nodeTailBuffer = null;
        if(threadSafe) {
            nodeTailBuffer = new JsonNodeBuffer(DEF_NODE_TAIL_BUFFER_SIZE);
        } else {
            if(mNodeTailBuffer != null) {
                mNodeTailBuffer.reset();
            } else {
                mNodeTailBuffer = new JsonNodeBuffer(DEF_NODE_TAIL_BUFFER_SIZE);
            }
            nodeTailBuffer = mNodeTailBuffer;
        }

        return _parse(jsonTokenizer, maxDepth, tokenTailBuffer, nodeTailBuffer);
    }

    private Object _parse(JsonTokenizer tokenizer, int depth, JsonTokenBuffer tokenTailBuffer, JsonNodeBuffer nodeTailBuffer) throws JsonParserException, IOException
    {
        Object topNode = null;
        int type = peekAndGetType(tokenizer, tokenTailBuffer, nodeTailBuffer);
        if(type == EOF || type == LCURLY || type == LSQUARE) {
            if(type == EOF) {
                topNode = produceJsonNull(tokenizer, tokenTailBuffer, nodeTailBuffer);
            } else if(type == LCURLY) {
                topNode = produceJsonObject(tokenizer, depth, tokenTailBuffer, nodeTailBuffer);
            } else if(type == LSQUARE) {
                topNode = produceJsonArray(tokenizer, depth, tokenTailBuffer, nodeTailBuffer);            
            }
        } else {
            // TBD:
            // Process it here if parserPolicy.allowLeadingJsonMarker() == true,
            // ???
            if(parserPolicy.allowNonObjectOrNonArray()) {
                // This is actually error according to json.org JSON grammar.
                // But, we allow partial JSON string.
                switch(type) {
                case NULL:
                    topNode = produceJsonNull(tokenizer, tokenTailBuffer, nodeTailBuffer);
                    break;
                case BOOLEAN:
                    topNode = produceJsonBoolean(tokenizer, tokenTailBuffer, nodeTailBuffer);
                    break;
                case NUMBER:
                    topNode = produceJsonNumber(tokenizer, tokenTailBuffer, nodeTailBuffer);
                    break;
                case STRING:
                    // log.warning(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
                    topNode = produceJsonString(tokenizer, tokenTailBuffer, nodeTailBuffer);
                    // log.warning(">>>>>>>>>>>>>>>>>>>>>>>>>>>>> topNode = " + topNode);
                    break;
                default:
                    // ???
                    throw new InvalidJsonTokenException("JsonToken not recognized: tokenType = " + TokenTypes.getDisplayName(type) + "; " + tokenTailBuffer.toTraceString(), tailCharStream(tokenizer), peekCharStream(tokenizer));
                }
            } else {
                // TBD
                // this is a bit too lenient probably...
                // there was some special char sequence which some parsers allowed, which I cannot remember..
                // For now, if parserPolicy.allowLeadingJsonMarker() == true is interpreted as allowLeadingNonObjectNonArrayChars....
                //    --> we remove all leading chars until we reach { or [.
                if(parserPolicy.allowLeadingJsonMarker()) {
                    while(type != LCURLY && type != LSQUARE) {
                        JsonToken t = tokenizer.next();  // swallow one token.
                        if(isTracingEnabled()) {
                            tokenTailBuffer.add(t);
                        }
                        type = peekAndGetType(tokenizer, tokenTailBuffer, nodeTailBuffer);
                    }
                    if(type == LCURLY) {
                        topNode = produceJsonObject(tokenizer, depth, tokenTailBuffer, nodeTailBuffer);
                    } else if(type == LSQUARE) {
                        topNode = produceJsonArray(tokenizer, depth, tokenTailBuffer, nodeTailBuffer);            
                    } else {
                        // ???
                        throw new InvalidJsonTokenException("Invalid input Json string. " + tokenTailBuffer.toTraceString(), tailCharStream(tokenizer), peekCharStream(tokenizer));
                    }
                } else {
                    // ???
                    throw new InvalidJsonTokenException("Json string should be Object or Array. Input tokenType = " + TokenTypes.getDisplayName(type) + "; " +  tokenTailBuffer.toTraceString(), tailCharStream(tokenizer), peekCharStream(tokenizer));
                }
            }
        }

        if(log.isLoggable(Level.FINE)) log.fine("topnNode = " + topNode);
        return topNode;
    }
    
    
    private Map<String,Object> produceJsonObject(JsonTokenizer tokenizer, int depth, JsonTokenBuffer tokenTailBuffer, JsonNodeBuffer nodeTailBuffer) throws JsonParserException, IOException
    {
        int lcurl = nextAndGetType(tokenizer, tokenTailBuffer, nodeTailBuffer);   // pop the leading {.
        if(lcurl != LCURLY) {
            // this cannot happen.
            throw new InvalidJsonTokenException("JSON object should start with {. " + tokenTailBuffer.toTraceString(), tailCharStream(tokenizer), peekCharStream(tokenizer));
        }

        Map<String,Object> map = new HashMap<String,Object>();
        int type = peekAndGetType(tokenizer, tokenTailBuffer, nodeTailBuffer);
        if(type == RCURLY) {
            // empty object
            JsonToken t = tokenizer.next();   // discard the trailing }.
            if(isTracingEnabled()) {
                tokenTailBuffer.add(t);
            }
        } else {
            Map<String,Object> members = produceJsonObjectMembers(tokenizer, depth, tokenTailBuffer, nodeTailBuffer);
            int rcurl;
            if(isTracingEnabled()) {
                JsonToken t = nextAndGetToken(tokenizer, tokenTailBuffer, nodeTailBuffer);  // discard the trailing }.
                tokenTailBuffer.add(t);
                rcurl = t.getType(); 
            } else {
                rcurl = nextAndGetType(tokenizer, tokenTailBuffer, nodeTailBuffer);  // discard the trailing }.
            }
            if(rcurl == RCURLY) {
                // Done
                map.putAll(members);
            } else {
                // ???
                throw new InvalidJsonTokenException("JSON object should end with }. " + tokenTailBuffer.toTraceString(), tailCharStream(tokenizer), peekCharStream(tokenizer));
            }
        }
        Map<String,Object> jObject = jsonTypeFactory.createObject(map);
        if(isTracingEnabled()) {
            nodeTailBuffer.add(jObject);
        }

        if(log.isLoggable(Level.FINE)) log.fine("jObject = " + jObject);
        return jObject;
    }

    
 
    private Map<String,Object> produceJsonObjectMembers(JsonTokenizer tokenizer, int depth, JsonTokenBuffer tokenTailBuffer, JsonNodeBuffer nodeTailBuffer) throws JsonParserException, IOException
    {
        Map<String,Object> members = new HashMap<String,Object>();        
        int type = peekAndGetType(tokenizer, tokenTailBuffer, nodeTailBuffer);
        while(type != RCURLY) {
            Map.Entry<String,Object> member = produceJsonObjectMember(tokenizer, depth, tokenTailBuffer, nodeTailBuffer);     // No depth change...
            if(member != null) {
                // This is a bit strange implementation, but it actually works...
                // The parser traverses the object tree (depth first) down to the leaf,
                //    and on the way up, if it reaches depth==1, it converts the sub-tree below depth==1 to Json String.
                //    the json string is then used as the node value (instead of the object sub tree).
                // Going above depth > 1, this transformation does not happen.
                if(depth == 1) { 
                    Object mem = member.getValue();
                    try {
                        // String jStr = _escapeString(jsonBuilder.build(mem));
                        String jStr = jsonBuilder.build(mem);
                        // Object memObj = jsonBuilder.buildJsonStructure(mem);
                        //  String jStr = jsonBuilder.build(memObj);
                        // String jStr = jsonBuilder.build(mem, 1);
                        // log.warning(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> depth = " + depth + " }}}}}}}} jStr = " + jStr);
                        members.put(member.getKey(), jStr);
                    } catch (JsonBuilderException e) {
                        throw new JsonParserException(e);
                    }
                } else {
                    members.put(member.getKey(), member.getValue());
                }
            }
            type = peekAndGetType(tokenizer, tokenTailBuffer, nodeTailBuffer);
            
            // "consume" the comma.
            if(parserPolicy.allowExtraCommas()) {
                while(type == COMMA) {
                    JsonToken t = tokenizer.next();
                    if(isTracingEnabled()) {
                        tokenTailBuffer.add(t);
                    }
                    type = peekAndGetType(tokenizer, tokenTailBuffer, nodeTailBuffer);
                }
            } else {
                if(type == COMMA) {
                    JsonToken t = tokenizer.next();
                    if(isTracingEnabled()) {
                        tokenTailBuffer.add(t);
                    }
                    type = peekAndGetType(tokenizer, tokenTailBuffer, nodeTailBuffer);

                    if(parserPolicy.allowTrailingComma()) {
                        // Continue.
                    } else {
                        // Invalid  char sequence: ",}" 
                        if(type == RCURLY) {
                            throw new InvalidJsonTokenException("Syntax error: Object has a trailing comma. " + tokenTailBuffer.toTraceString(), tailCharStream(tokenizer), peekCharStream(tokenizer));
                        }
                    }
                }
            }
        }

        if(log.isLoggable(Level.FINER)) log.finer("members = " + members);
        return members;
    }
    private Map.Entry<String,Object> produceJsonObjectMember(JsonTokenizer tokenizer, int depth, JsonTokenBuffer tokenTailBuffer, JsonNodeBuffer nodeTailBuffer) throws JsonParserException, IOException
    {
        JsonToken keyToken = nextAndGetToken(tokenizer, tokenTailBuffer, nodeTailBuffer);
        if(isTracingEnabled()) {
            tokenTailBuffer.add(keyToken);
        }
        int keyType = keyToken.getType();
        if(keyType != STRING) {
            throw new InvalidJsonTokenException("JSON Object member should start with a string key. keyType = " + keyType + "; " + tokenTailBuffer.toTraceString(), tailCharStream(tokenizer), peekCharStream(tokenizer));
        }
        String key = (String) keyToken.getValue();
        
        JsonToken colonToken = nextAndGetToken(tokenizer, tokenTailBuffer, nodeTailBuffer);   // "consume" :.
        if(isTracingEnabled()) {
            tokenTailBuffer.add(colonToken);
        }
        int colonType = colonToken.getType();
        if(colonType != COLON) {
            throw new InvalidJsonTokenException("JSON Object member should include a colon (:). " + tokenTailBuffer.toTraceString(), tailCharStream(tokenizer), peekCharStream(tokenizer));
        }

        Object value = null;
        int type = peekAndGetType(tokenizer, tokenTailBuffer, nodeTailBuffer);
        switch(type) {
        case NULL:
            value = produceJsonNull(tokenizer, tokenTailBuffer, nodeTailBuffer);
            break;
        case BOOLEAN:
            value = produceJsonBoolean(tokenizer, tokenTailBuffer, nodeTailBuffer);
            break;
        case NUMBER:
            value = produceJsonNumber(tokenizer, tokenTailBuffer, nodeTailBuffer);
            break;
        case STRING:
            value = produceJsonString(tokenizer, tokenTailBuffer, nodeTailBuffer);
            break;
        case LCURLY:
            value = produceJsonObject(tokenizer, depth-1, tokenTailBuffer, nodeTailBuffer);
            break;
        case LSQUARE:
            value = produceJsonArray(tokenizer, depth-1, tokenTailBuffer, nodeTailBuffer);
            break;
        default:
            // ???
            throw new InvalidJsonTokenException("Json array element not recognized: token = " + tokenizer.peek() + "; " + tokenTailBuffer.toTraceString(), tailCharStream(tokenizer), peekCharStream(tokenizer));
        }
        
        // TBD: Use type factory ???
        Map.Entry<String,Object> member = new MapEntry<String,Object>(key, value);
 
        if(log.isLoggable(Level.FINER)) log.finer("member = " + member);
        return member;
    }


    
    private List<Object> produceJsonArray(JsonTokenizer tokenizer, int depth, JsonTokenBuffer tokenTailBuffer, JsonNodeBuffer nodeTailBuffer) throws JsonParserException, IOException
    {
        int lsq;
        if(isTracingEnabled()) {
            JsonToken t = nextAndGetToken(tokenizer, tokenTailBuffer, nodeTailBuffer);    // pop the leading [.
            tokenTailBuffer.add(t);
            lsq = t.getType();
        } else {
            lsq = nextAndGetType(tokenizer, tokenTailBuffer, nodeTailBuffer);             
        }
        if(lsq != LSQUARE) {
            // this cannot happen.
            throw new InvalidJsonTokenException("JSON array should start with [. " + tokenTailBuffer.toTraceString(), tailCharStream(tokenizer), peekCharStream(tokenizer));
        }

        List<Object> list = new ArrayList<Object>();
        int type = peekAndGetType(tokenizer, tokenTailBuffer, nodeTailBuffer);
        if(type == RSQUARE) {
            // empty array
            JsonToken t = tokenizer.next();   // discard the trailing ].
            if(isTracingEnabled()) {
                tokenTailBuffer.add(t);
            }
        } else {
            List<Object> elements = produceJsonArrayElements(tokenizer, depth, tokenTailBuffer, nodeTailBuffer);

            int rsq = nextAndGetType(tokenizer, tokenTailBuffer, nodeTailBuffer);  // discard the trailing ].
            if(rsq == RSQUARE) {
                // Done
                list.addAll(elements);
            } else {
                // ???
                throw new InvalidJsonTokenException("JSON array should end with ]. " + tokenTailBuffer.toTraceString(), tailCharStream(tokenizer), peekCharStream(tokenizer));
            }
        }
        List<Object> jArray = jsonTypeFactory.createArray(list);
        if(isTracingEnabled()) {
            nodeTailBuffer.add(jArray);
        }

        if(log.isLoggable(Level.FINE)) log.fine("jArray = " + jArray);
        return jArray;
    }

    private List<Object> produceJsonArrayElements(JsonTokenizer tokenizer, int depth, JsonTokenBuffer tokenTailBuffer, JsonNodeBuffer nodeTailBuffer) throws JsonParserException, IOException
    {
        List<Object> elements = new ArrayList<Object>();
        int type = peekAndGetType(tokenizer, tokenTailBuffer, nodeTailBuffer);
        while(type != RSQUARE) {
            Object element = produceJsonArrayElement(tokenizer, depth, tokenTailBuffer, nodeTailBuffer);      // No depth change...
            if(element != null) {
                // This is a bit strange implementation, but it actually works...
                // The parser traverses the object tree (depth first) down to the leaf,
                //    and on the way up, if it reaches depth==1, it converts the sub-tree below depth==1 to Json String.
                //    the json string is then used as the node value (instead of the object sub tree).
                // Going above depth > 1, this transformation does not happen.
                if(depth == 1) { 
                    try {
                        // String jStr = _escapeString(jsonBuilder.build(element));
                        String jStr = jsonBuilder.build(element);
                        // Object elObj = jsonBuilder.buildJsonStructure(element);
                        // String jStr = jsonBuilder.build(elObj);
                        // String jStr = jsonBuilder.build(element, 1);
                        // log.warning(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> depth = " + depth + " ]]]]]]] jStr = " + jStr);
                        elements.add(jStr);
                    } catch (JsonBuilderException e) {
                        throw new JsonParserException(e);
                    }
                } else {
                    elements.add(element);
                }
            }
            type = peekAndGetType(tokenizer, tokenTailBuffer, nodeTailBuffer);

            // "consume" the comma.
            if(parserPolicy.allowExtraCommas()) {
                while(type == COMMA) {
                    JsonToken t = tokenizer.next();
                    if(isTracingEnabled()) {
                        tokenTailBuffer.add(t);
                    }
                    type = peekAndGetType(tokenizer, tokenTailBuffer, nodeTailBuffer);
                }
            } else {
                if(type == COMMA) {
                    JsonToken t = tokenizer.next();
                    if(isTracingEnabled()) {
                        tokenTailBuffer.add(t);
                    }
                    type = peekAndGetType(tokenizer, tokenTailBuffer, nodeTailBuffer);

                    if(parserPolicy.allowTrailingComma()) {
                        // Continue.
                    } else {
                        // Invalid  char sequence: ",]" 
                        if(type == RSQUARE) {
                            throw new InvalidJsonTokenException("Syntax error: Array has a trailing comma. " + tokenTailBuffer.toTraceString(), tailCharStream(tokenizer), peekCharStream(tokenizer));
                        }
                    }
                }
            }
        }

        if(log.isLoggable(Level.FINER)) log.finer("elements = " + elements);
        return elements;
    }
    private Object produceJsonArrayElement(JsonTokenizer tokenizer, int depth, JsonTokenBuffer tokenTailBuffer, JsonNodeBuffer nodeTailBuffer) throws JsonParserException, IOException
    {
        Object element = null;
        int type = peekAndGetType(tokenizer, tokenTailBuffer, nodeTailBuffer);
        switch(type) {
        case NULL:
            element = produceJsonNull(tokenizer, tokenTailBuffer, nodeTailBuffer);
            break;
        case BOOLEAN:
            element = produceJsonBoolean(tokenizer, tokenTailBuffer, nodeTailBuffer);
            break;
        case NUMBER:
            element = produceJsonNumber(tokenizer, tokenTailBuffer, nodeTailBuffer);
            break;
        case STRING:
            element = produceJsonString(tokenizer, tokenTailBuffer, nodeTailBuffer);
            break;
        case LCURLY:
            element = produceJsonObject(tokenizer, depth-1, tokenTailBuffer, nodeTailBuffer);
            break;
        case LSQUARE:
            element = produceJsonArray(tokenizer, depth-1, tokenTailBuffer, nodeTailBuffer);
            break;
        default:
            // ???
            throw new InvalidJsonTokenException("Json array element not recognized: token = " + tokenizer.peek() + "; " + tokenTailBuffer.toTraceString(), tailCharStream(tokenizer), peekCharStream(tokenizer));
        }

        if(log.isLoggable(Level.FINER)) log.finer("element = " + element);
        return element;
    }

    private JsonToken peekAndGetToken(JsonTokenizer tokenizer, JsonTokenBuffer tokenTailBuffer, JsonNodeBuffer nodeTailBuffer) throws JsonParserException
    {
        JsonToken s = tokenizer.peek();
        if(s == null) {
            throw new UnknownParserException("Failed to get the next json token. " + tokenTailBuffer.toTraceString(), tailCharStream(tokenizer), peekCharStream(tokenizer));
        }
        return s;
    }
    private int peekAndGetType(JsonTokenizer tokenizer, JsonTokenBuffer tokenTailBuffer, JsonNodeBuffer nodeTailBuffer) throws JsonParserException
    {
        JsonToken s = tokenizer.peek();
        if(s == null) {
            throw new UnknownParserException("Failed to get the next json token. " + tokenTailBuffer.toTraceString(), tailCharStream(tokenizer), peekCharStream(tokenizer));
        }
        int type = s.getType();
        return type;
    }
    private JsonToken nextAndGetToken(JsonTokenizer tokenizer, JsonTokenBuffer tokenTailBuffer, JsonNodeBuffer nodeTailBuffer) throws JsonParserException
    {
        JsonToken s = tokenizer.next();
        if(s == null) {
            throw new UnknownParserException("Failed to get the next json token. " + tokenTailBuffer.toTraceString(), tailCharStream(tokenizer), peekCharStream(tokenizer));
        }
        return s;
    }
    private int nextAndGetType(JsonTokenizer tokenizer, JsonTokenBuffer tokenTailBuffer, JsonNodeBuffer nodeTailBuffer) throws JsonParserException
    {
        JsonToken s = tokenizer.next();
        if(s == null) {
            throw new UnknownParserException("Failed to get the next json token. " + tokenTailBuffer.toTraceString(), tailCharStream(tokenizer), peekCharStream(tokenizer));
        }
        int type = s.getType();
        return type;
    }

    private String produceJsonString(JsonTokenizer tokenizer, JsonTokenBuffer tokenTailBuffer, JsonNodeBuffer nodeTailBuffer) throws JsonParserException
    {
        String jString = null;
        try {
            JsonToken t = tokenizer.next();
            // log.warning(">>>>>>>>>>>>>>>>>>>>>>>>>>>>> t = " + t);
            if(isTracingEnabled()) {
                tokenTailBuffer.add(t);
            }
            String value = (String) t.getValue();
            // log.warning(">>>>>>>>>>>>>>>>>>>>>>>>>>>>> value = " + value);
            jString = (String) jsonTypeFactory.createString(value);
            // log.warning(">>>>>>>>>>>>>>>>>>>>>>>>>>>>> jString = " + jString);
        } catch(Exception e) {
            throw new UnknownParserException("Failed to create a String node. " + tokenTailBuffer.toTraceString(), e, tailCharStream(tokenizer), peekCharStream(tokenizer));
        }
        if(isTracingEnabled()) {
            nodeTailBuffer.add(jString);
        }
        return jString;
    }
    private Number produceJsonNumber(JsonTokenizer tokenizer, JsonTokenBuffer tokenTailBuffer, JsonNodeBuffer nodeTailBuffer) throws JsonParserException
    {
        Number jNumber = null;
        try {
            JsonToken t = tokenizer.next();
            if(isTracingEnabled()) {
                tokenTailBuffer.add(t);
            }
            Number value = (Number) t.getValue();
            jNumber = (Number) jsonTypeFactory.createNumber(value);
        } catch(Exception e) {
            throw new UnknownParserException("Failed to create a Number node. " + tokenTailBuffer.toTraceString(), e, tailCharStream(tokenizer), peekCharStream(tokenizer));
        }
        if(isTracingEnabled()) {
            nodeTailBuffer.add(jNumber);
        }
        return jNumber;
    }
    private Boolean produceJsonBoolean(JsonTokenizer tokenizer, JsonTokenBuffer tokenTailBuffer, JsonNodeBuffer nodeTailBuffer) throws JsonParserException
    {
        Boolean jBoolean = null;
        try {
            JsonToken t = tokenizer.next();
            if(isTracingEnabled()) {
                tokenTailBuffer.add(t);
            }
            Boolean value = (Boolean) t.getValue();
            jBoolean = (Boolean) jsonTypeFactory.createBoolean(value);
        } catch(Exception e) {
            throw new UnknownParserException("Failed to create a Boolean node. " + tokenTailBuffer.toTraceString(), e, tailCharStream(tokenizer), peekCharStream(tokenizer));
        }
        if(isTracingEnabled()) {
            nodeTailBuffer.add(jBoolean);
        }
        return jBoolean;
    }

    private Object produceJsonNull(JsonTokenizer tokenizer, JsonTokenBuffer tokenTailBuffer, JsonNodeBuffer nodeTailBuffer) throws JsonParserException
    {
        Object jNull = null;
        try {
            JsonToken t = tokenizer.next();   // Consume the "null" literal.
            if(isTracingEnabled()) {
                tokenTailBuffer.add(t);
            }
            jNull = jsonTypeFactory.createNull();
        } catch(Exception e) {
            throw new UnknownParserException("Failed to create a Null node. " + tokenTailBuffer.toTraceString(), e, tailCharStream(tokenizer), peekCharStream(tokenizer));
        }
        if(isTracingEnabled()) {
            nodeTailBuffer.add(jNull);
        }
        return jNull;
    }

    
    private String _escapeString(String primStr) throws IOException
    {
        StringBuilder sb = new StringBuilder();
        if(primStr != null && !primStr.isEmpty()) {
            char[] primChars = primStr.toCharArray();
            char prevEc = 0;
            for(char ec : primChars) {
                if(Symbols.isEscapedChar(ec)) {
                    if(prevEc == '<' && ec == '/') {
                        sb.append("\\/");
                    } else {
                        String str = Symbols.getEscapedCharString(ec, false);
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
        return sb.toString();
    }

    private String _readFromReader(Reader reader) throws IOException
    {
        if(! (reader instanceof BufferedReader)) {
            reader = new BufferedReader(reader);
        }
        String line = null;
        StringBuilder sb = new StringBuilder();
        while((line = ((BufferedReader) reader).readLine()) != null) {
            sb.append(line);
        }
        return sb.toString();
    }
    

}
