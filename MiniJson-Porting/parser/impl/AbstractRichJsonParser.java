package org.minijson.parser.impl;

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

import java.io.IOException;
import java.io.Reader;
import java.io.StringReader;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.logging.Level;
import java.util.logging.Logger;

import org.minijson.parser.FlexibleJsonParser;
import org.minijson.parser.JsonParserException;
import org.minijson.parser.JsonTokenizer;
import org.minijson.parser.RichJsonParser;
import org.minijson.parser.TraceableJsonTokenizer;
import org.minijson.parser.core.JsonToken;
import org.minijson.parser.core.TokenTypes;
import org.minijson.parser.exception.InvalidJsonTokenException;
import org.minijson.parser.exception.UnknownParserException;
import org.minijson.parser.policy.ParserPolicy;
import org.minijson.parser.policy.base.DefaultParserPolicy;
import org.minijson.type.JsonArrayNode;
import org.minijson.type.JsonBooleanNode;
import org.minijson.type.JsonNode;
import org.minijson.type.JsonNullNode;
import org.minijson.type.JsonNumberNode;
import org.minijson.type.JsonObjectMember;
import org.minijson.type.JsonObjectNode;
import org.minijson.type.JsonStringNode;
import org.minijson.type.base.AbstractJsonObjectMember;
import org.minijson.type.factory.JsonTypeFactory;
import org.minijson.type.factory.impl.AbstractJsonTypeFactory;



// Recursive descent parser implementation.
public abstract class AbstractRichJsonParser extends AbstractJsonParser implements RichJsonParser, FlexibleJsonParser
{
    private static final Logger log = Logger.getLogger(AbstractRichJsonParser.class.getName());

    private JsonTypeFactory jsonTypeFactory = null;
    private final ParserPolicy parserPolicy;

    public AbstractRichJsonParser()
    {
        this(null);
    }
    public AbstractRichJsonParser(JsonTypeFactory jsonTypeFactory)
    {
        this(jsonTypeFactory, null);
    }
    public AbstractRichJsonParser(JsonTypeFactory jsonTypeFactory, ParserPolicy parserPolicy)
    {
        // temporary
        if(jsonTypeFactory == null) {
            this.jsonTypeFactory = AbstractJsonTypeFactory.getInstance();
        } else {
            this.jsonTypeFactory = jsonTypeFactory;
        }
        if(parserPolicy == null) {
            this.parserPolicy = DefaultParserPolicy.MINIJSON;
        } else {
            this.parserPolicy = parserPolicy;
        }

        // For subclasses
        init();
    }

    // Having multiple ctor's is a bit inconvenient.
    // Put all init routines here.
    // To be overridden in subclasses.
    protected void init()
    {
        enableLookAheadParsing();
        enableTracing();
    }


    @Override
    public ParserPolicy getParserPolicy()
    {
        return this.parserPolicy;
    }


    @Override
    public Object parse(String jsonStr) throws JsonParserException
    {
        StringReader sr = new StringReader(jsonStr);
        Object jsonObj = null;
        try {
            jsonObj = parse(sr);
        } catch (IOException e) {
            throw new JsonParserException("IO error during JSON parsing.", e);
        }
        return jsonObj;
    }
    @Override
    public Object parse(Reader reader) throws JsonParserException, IOException
    {
        JsonNode topNode = parseJson(reader);
        // TBD:
        // Convert it to map/list...
        // ...
        return topNode;
    }

    @Override
    public JsonNode parseJson(String jsonStr) throws JsonParserException
    {
        StringReader sr = new StringReader(jsonStr);
        JsonNode jsonObj = null;
        try {
            jsonObj = parseJson(sr);
        } catch (IOException e) {
            throw new JsonParserException("IO error during JSON parsing.", e);
        }
        return jsonObj;
    }
    @Override
    public JsonNode parseJson(Reader reader) throws JsonParserException, IOException
    {
        return _parse(reader);
    }


    private JsonNode _parse(Reader reader) throws JsonParserException
    {
        if(reader == null) {
            return null;
        }
        JsonTokenizer tokenizer = new CustomJsonTokenizer(reader, parserPolicy);
        setLookAheadParsing(tokenizer);
        setTokenizerTracing(tokenizer);
        return _parse(tokenizer);
    }
    private JsonNode _parse(JsonTokenizer tokenizer) throws JsonParserException
    {
        if(tokenizer == null) {
            return null;
        }
        
        JsonNode topNode = null;
        int type = peekAndGetType(tokenizer);
        if(type == EOF) {
            topNode = produceJsonNull(tokenizer);
        } else if(type == LCURLY) {
            topNode = produceJsonObject(tokenizer);
        } else if(type == LSQUARE) {
            topNode = produceJsonArray(tokenizer);            
        } else {
            // TBD:
            // Process it here if parserPolicy.allowLeadingJsonMarker() == true,
            // ???
            if(parserPolicy.allowNonObjectOrNonArray()) {
                // This is actually error according to json.org JSON grammar.
                // But, we allow partial JSON string.
                switch(type) {
                case NULL:
                    topNode = produceJsonNull(tokenizer);
                    break;
                case BOOLEAN:
                    topNode = produceJsonBoolean(tokenizer);
                    break;
                case NUMBER:
                    topNode = produceJsonNumber(tokenizer);
                    break;
                case STRING:
                    topNode = produceJsonString(tokenizer);
                    break;
                default:
                    // ???
                    throw new InvalidJsonTokenException("JsonToken not recognized: tokenType = " + TokenTypes.getDisplayName(type), tailCharStream(tokenizer), peekCharStream(tokenizer));
                }
            } else {
                // TBD
                // this is a bit too lenient probably...
                // there was some special char sequence which some parsers allowed, which I cannot remember..
                // For now, if parserPolicy.allowLeadingJsonMarker() == true is interpreted as allowLeadingNonObjectNonArrayChars....
                //    --> we remove all leading chars until we reach { or [.
                if(parserPolicy.allowLeadingJsonMarker()) {
                    while(type != LCURLY && type != LSQUARE) {
                        tokenizer.next();  // swallow oen char.
                        type = peekAndGetType(tokenizer);
                    }
                    if(type == LCURLY) {
                        topNode = produceJsonObject(tokenizer);
                    } else if(type == LSQUARE) {
                        topNode = produceJsonArray(tokenizer);            
                    } else {
                        // ???
                        throw new InvalidJsonTokenException("Invalid input Json string.", tailCharStream(tokenizer), peekCharStream(tokenizer));
                    }
                } else {
                    // ???
                    throw new InvalidJsonTokenException("Json string should be Object or Array. Input tokenType = " + TokenTypes.getDisplayName(type), tailCharStream(tokenizer), peekCharStream(tokenizer));
                }
            }
        }

        if(log.isLoggable(Level.FINE)) log.fine("topnNode = " + topNode);
        return topNode;
    }
    
    
    private JsonObjectNode produceJsonObject(JsonTokenizer tokenizer) throws JsonParserException
    {
        int lcurl = nextAndGetType(tokenizer);   // pop the leading {.
        if(lcurl != LCURLY) {
            // this cannot happen.
            throw new InvalidJsonTokenException("JSON object should start with {.", tailCharStream(tokenizer), peekCharStream(tokenizer));
        }

        Map<String,Object> map = new HashMap<String,Object>();
        int type = peekAndGetType(tokenizer);
        if(type == RCURLY) {
            // empty object
            tokenizer.next();   // discard the trailing }.
        } else {
            Map<String,JsonNode> members = produceJsonObjectMembers(tokenizer);
            int rcurl = nextAndGetType(tokenizer);  // discard the trailing }.
            if(rcurl == RCURLY) {
                // Done
                map.putAll(members);
            } else {
                // ???
                throw new InvalidJsonTokenException("JSON object should end with }.", tailCharStream(tokenizer), peekCharStream(tokenizer));
            }
        }
        JsonObjectNode jObject = (JsonObjectNode) jsonTypeFactory.createObject(map);

        if(log.isLoggable(Level.FINE)) log.fine("jObject = " + jObject);
        return jObject;
    }

    private Map<String,JsonNode> produceJsonObjectMembers(JsonTokenizer tokenizer) throws JsonParserException
    {
        Map<String,JsonNode> members = new HashMap<String,JsonNode>();
        
        int type = peekAndGetType(tokenizer);
        while(type != RCURLY) {
            JsonObjectMember member = produceJsonObjectMember(tokenizer);
            if(member != null) {
                members.put(member.getKey(), member.getValue());
            }
            type = peekAndGetType(tokenizer);
            
            // "consume" the comma.
            if(parserPolicy.allowExtraCommas()) {
                while(type == COMMA) {
                    tokenizer.next();
                    type = peekAndGetType(tokenizer);
                }
            } else {
                if(type == COMMA) {
                    tokenizer.next();
                    type = peekAndGetType(tokenizer);

                    if(parserPolicy.allowTrailingComma()) {
                        // Continue.
                    } else {
                        // Invalid  char sequence: ",}" 
                        if(type == RCURLY) {
                            throw new InvalidJsonTokenException("Syntax error: Object has a trailing comma.", tailCharStream(tokenizer), peekCharStream(tokenizer));
                        }
                    }
                }
            }
        }

        if(log.isLoggable(Level.FINER)) log.finer("members = " + members);
        return members;
    }
    private JsonObjectMember produceJsonObjectMember(JsonTokenizer tokenizer) throws JsonParserException
    {
        JsonToken keyToken = nextAndGetToken(tokenizer);
        int keyType = keyToken.getType();
        if(keyType != STRING) {
            throw new InvalidJsonTokenException("JSON Object member should start with a string key.", tailCharStream(tokenizer), peekCharStream(tokenizer));
        }
        String key = (String) keyToken.getValue();
        
        JsonToken colonToken = nextAndGetToken(tokenizer);   // "consume" :.
        int colonType = colonToken.getType();
        if(colonType != COLON) {
            throw new InvalidJsonTokenException("JSON Object member should include a colon (:).", tailCharStream(tokenizer), peekCharStream(tokenizer));
        }

        JsonNode value = null;
        int type = peekAndGetType(tokenizer);
        switch(type) {
        case NULL:
            value = produceJsonNull(tokenizer);
            break;
        case BOOLEAN:
            value = produceJsonBoolean(tokenizer);
            break;
        case NUMBER:
            value = produceJsonNumber(tokenizer);
            break;
        case STRING:
            value = produceJsonString(tokenizer);
            break;
        case LCURLY:
            value = produceJsonObject(tokenizer);
            break;
        case LSQUARE:
            value = produceJsonArray(tokenizer);
            break;
        default:
            // ???
            throw new InvalidJsonTokenException("Json array element not recognized: token = " + tokenizer.peek(), tailCharStream(tokenizer), peekCharStream(tokenizer));
        }
        
        // TBD: Use type factory ???
        JsonObjectMember member = new AbstractJsonObjectMember(key, value);
 
        if(log.isLoggable(Level.FINER)) log.finer("member = " + member);
        return member;
    }

    
    
    
    private JsonArrayNode produceJsonArray(JsonTokenizer tokenizer) throws JsonParserException
    {
        int lsq = nextAndGetType(tokenizer);   // pop the leading [.
        if(lsq != LSQUARE) {
            // this cannot happen.
            throw new InvalidJsonTokenException("JSON array should start with [.", tailCharStream(tokenizer), peekCharStream(tokenizer));
        }

        List<Object> list = new ArrayList<Object>();
        int type = peekAndGetType(tokenizer);
        if(type == RSQUARE) {
            // empty array
            tokenizer.next();   // discard the trailing ].
        } else {
            List<JsonNode> elements = produceJsonArrayElements(tokenizer);

            int rsq = nextAndGetType(tokenizer);  // discard the trailing ].
            if(rsq == RSQUARE) {
                // Done
                list.addAll(elements);
            } else {
                // ???
                throw new InvalidJsonTokenException("JSON array should end with ].", tailCharStream(tokenizer), peekCharStream(tokenizer));
            }
        }
        JsonArrayNode jArray = (JsonArrayNode) jsonTypeFactory.createArray(list);

        if(log.isLoggable(Level.FINE)) log.fine("jArray = " + jArray);
        return jArray;
    }

    private List<JsonNode> produceJsonArrayElements(JsonTokenizer tokenizer) throws JsonParserException
    {
        List<JsonNode> elements = new ArrayList<JsonNode>();

        int type = peekAndGetType(tokenizer);
        while(type != RSQUARE) {
            JsonNode element = produceJsonArrayElement(tokenizer);
            if(element != null) {
                elements.add(element);
            }
            type = peekAndGetType(tokenizer);

            // "consume" the comma.
            if(parserPolicy.allowExtraCommas()) {
                while(type == COMMA) {
                    tokenizer.next();
                    type = peekAndGetType(tokenizer);
                }
            } else {
                if(type == COMMA) {
                    tokenizer.next();
                    type = peekAndGetType(tokenizer);

                    if(parserPolicy.allowTrailingComma()) {
                        // Continue.
                    } else {
                        // Invalid  char sequence: ",]" 
                        if(type == RSQUARE) {
                            throw new InvalidJsonTokenException("Syntax error: Array has a trailing comma.", tailCharStream(tokenizer), peekCharStream(tokenizer));
                        }
                    }
                }
            }
        }

        if(log.isLoggable(Level.FINER)) log.finer("elements = " + elements);
        return elements;
    }
    private JsonNode produceJsonArrayElement(JsonTokenizer tokenizer) throws JsonParserException
    {
        JsonNode element = null;
        int type = peekAndGetType(tokenizer);
        switch(type) {
        case NULL:
            element = produceJsonNull(tokenizer);
            break;
        case BOOLEAN:
            element = produceJsonBoolean(tokenizer);
            break;
        case NUMBER:
            element = produceJsonNumber(tokenizer);
            break;
        case STRING:
            element = produceJsonString(tokenizer);
            break;
        case LCURLY:
            element = produceJsonObject(tokenizer);
            break;
        case LSQUARE:
            element = produceJsonArray(tokenizer);
            break;
        default:
            // ???
            throw new InvalidJsonTokenException("Json array element not recognized: token = " + tokenizer.peek(), tailCharStream(tokenizer), peekCharStream(tokenizer));
        }

        if(log.isLoggable(Level.FINER)) log.finer("element = " + element);
        return element;
    }

    private JsonToken peekAndGetToken(JsonTokenizer tokenizer) throws JsonParserException
    {
        JsonToken s = tokenizer.peek();
        if(s == null) {
            throw new UnknownParserException("Failed to get the next json token.", tailCharStream(tokenizer), peekCharStream(tokenizer));
        }
        return s;
    }
    private int peekAndGetType(JsonTokenizer tokenizer) throws JsonParserException
    {
        JsonToken s = tokenizer.peek();
        if(s == null) {
            throw new UnknownParserException("Failed to get the next json token.", tailCharStream(tokenizer), peekCharStream(tokenizer));
        }
        int type = s.getType();
        return type;
    }
    private JsonToken nextAndGetToken(JsonTokenizer tokenizer) throws JsonParserException
    {
        JsonToken s = tokenizer.next();
        if(s == null) {
            throw new UnknownParserException("Failed to get the next json token.", tailCharStream(tokenizer), peekCharStream(tokenizer));
        }
        return s;
    }
    private int nextAndGetType(JsonTokenizer tokenizer) throws JsonParserException
    {
        JsonToken s = tokenizer.next();
        if(s == null) {
            throw new UnknownParserException("Failed to get the next json token.", tailCharStream(tokenizer), peekCharStream(tokenizer));
        }
        int type = s.getType();
        return type;
    }

    private JsonStringNode produceJsonString(JsonTokenizer tokenizer) throws JsonParserException
    {
        JsonStringNode jString = null;
        try {
            JsonToken t = tokenizer.next();
            String value = (String) t.getValue();
            jString = (JsonStringNode) jsonTypeFactory.createString(value);
        } catch(Exception e) {
            throw new UnknownParserException("Failed to create a String node.", e, tailCharStream(tokenizer), peekCharStream(tokenizer));
        }
        return jString;
    }
    private JsonNumberNode produceJsonNumber(JsonTokenizer tokenizer) throws JsonParserException
    {
        JsonNumberNode jNumber = null;
        try {
            JsonToken t = tokenizer.next();
            Number value = (Number) t.getValue();
            jNumber = (JsonNumberNode) jsonTypeFactory.createNumber(value);
        } catch(Exception e) {
            throw new UnknownParserException("Failed to create a Number node.", e, tailCharStream(tokenizer), peekCharStream(tokenizer));
        }
        return jNumber;
    }
    private JsonBooleanNode produceJsonBoolean(JsonTokenizer tokenizer) throws JsonParserException
    {
        JsonBooleanNode jBoolean = null;
        try {
            JsonToken t = tokenizer.next();
            Boolean value = (Boolean) t.getValue();
            // log.warning(">>>>>>>>>>>>>>>>>> Boolean value = " + value);
            jBoolean = (JsonBooleanNode) jsonTypeFactory.createBoolean(value);
            // log.warning(">>>>>>>>>>>>>>>>>> jBoolean = " + jBoolean);
        } catch(Exception e) {
            throw new UnknownParserException("Failed to create a Boolean node.", e, tailCharStream(tokenizer), peekCharStream(tokenizer));
        }
        return jBoolean;
    }

    private JsonNullNode produceJsonNull(JsonTokenizer tokenizer) throws JsonParserException
    {
        JsonNullNode jNull = null;
        try {
            tokenizer.next();   // Consume the "null" literal.
            jNull = (JsonNullNode) jsonTypeFactory.createNull();
        } catch(Exception e) {
            throw new UnknownParserException("Failed to create a Null node.", e, tailCharStream(tokenizer), peekCharStream(tokenizer));
        }
        return jNull;
    }

}
