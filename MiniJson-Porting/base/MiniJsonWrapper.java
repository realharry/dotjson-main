package org.minijson.base;

import java.io.IOException;
import java.io.Writer;
import java.util.logging.Level;
import java.util.logging.Logger;

import org.minijson.JsonAny;
import org.minijson.JsonParseable;
import org.minijson.builder.BareJsonBuilder;
import org.minijson.builder.JsonBuilderException;
import org.minijson.mini.MiniJsonBuilder;
import org.minijson.mini.MiniJsonParser;
import org.minijson.parser.JsonParser;
import org.minijson.parser.JsonParserException;


/**
 * Base class for MiniJsonObject and MiniJsonArray.
 */
public abstract class MiniJsonWrapper implements JsonAny
{
    private static final Logger log = Logger.getLogger(MiniJsonWrapper.class.getName());

    // temporary
    private static final int DRILL_DOWN_DEPTH = 1;

    // Is this safe to reuse these across multiple instances of this class???
    private static JsonParser sMiniJsonParser = null;
    private static BareJsonBuilder sMiniJsonBuilder = null;
    protected static JsonParser getStaticJsonParser()
    {
        if(sMiniJsonParser == null) {
            sMiniJsonParser = new MiniJsonParser();
        }
        return sMiniJsonParser;
    }
    protected static BareJsonBuilder getStaticJsonBuilder()
    {
        if(sMiniJsonBuilder == null) {
            sMiniJsonBuilder = new MiniJsonBuilder();
        }
        return sMiniJsonBuilder;
    }

    
    // Note:
    // This class has an unusual implementation.
    // We use jsonObj field instead of "this" to represent JSON equivalent.
    // That is, if we do toJsonString(), we do not return (this object).toJsonString().
    // Rather we return jsonObj.toJsonString().
    // ...
    // TBD:
    // This class needs to be re-implemented...
    // ....
    
    // Internal "wrapped" object
    private Object jsonObj = null;
    
    // Lazy initialized.
    private JsonParser miniJsonParser = null;
    private BareJsonBuilder miniJsonBuilder = null;
    
    // TBD:
    // ParserPolicy????
    // BuilderPolicy????
    // ...
    
    public MiniJsonWrapper()
    {
    }
    
    // setters???
    protected JsonParser getJsonParser()
    {
        if(miniJsonParser == null) {
            miniJsonParser = new MiniJsonParser();
        }
        return miniJsonParser;
    }
    protected BareJsonBuilder getJsonBuilder()
    {
        if(miniJsonBuilder == null) {
            miniJsonBuilder = new MiniJsonBuilder();
        }
        return miniJsonBuilder;
    }



    // JSON builder
    public MiniJsonWrapper(Object jsonObj)
    {
        this.jsonObj = jsonObj;
    }

    // JSON Parser
    public MiniJsonWrapper(String jsonStr)
    {
        this.jsonObj = createObjectFromJson(jsonStr);
    }

    private static Object createObjectFromJson(String jsonStr)
    {
        Object obj = null;
        try {
            // obj = MiniJsonParser.DEFAULT_INSTANCE.parse(jsonStr);
            obj = getStaticJsonParser().parse(jsonStr);
        } catch (JsonParserException e) {
            throw new RuntimeException(e);
        }
        return obj;
    }

    // Note: Conceptually, it is a method "defined" in JsonParseable.
    // Since you cannot inherit this implementation in subclasses,
    // each subclass, or any class that implements JsonParseable, should have its own implementation of fromJson().
    // Use this class as an example.... if necessary...
    // (but, this is a rather strange implementation...)
    // (normally, each specific field of a JsonParseable class should be initialized by the corresponding values in jsonStr,
    //      and it's rather hard to create a "generic" implementation, unless you use reflection, etc.)
    public static JsonParseable fromJson(String jsonStr)
    {
        MiniJsonWrapper jsonParseable = null;
        try {
            // Object obj = MiniJsonParser.DEFAULT_INSTANCE.parse(jsonStr);
            Object obj = getStaticJsonParser().parse(jsonStr);
            jsonParseable = new MiniJsonWrapper(obj) {};
        } catch (JsonParserException e) {
            throw new RuntimeException(e);
        }
        return jsonParseable;
    }


    @Override
    public String toJsonString() throws JsonBuilderException
    {
        return getJsonBuilder().build(jsonObj);
    }
    @Override
    public String toJsonString(int indent) throws JsonBuilderException
    {
        return getJsonBuilder().build(jsonObj, indent);
    }

    @Override
    public void writeJsonString(Writer writer) throws IOException, JsonBuilderException
    {
        getJsonBuilder().build(writer, jsonObj);
    }
    @Override
    public void writeJsonString(Writer writer, int indent) throws IOException, JsonBuilderException
    {
        getJsonBuilder().build(writer, jsonObj, indent);
    }


    @Override
    public Object toJsonStructure() throws JsonBuilderException
    {
        return toJsonStructure(DRILL_DOWN_DEPTH);
    }

    @Override
    public Object toJsonStructure(int depth) throws JsonBuilderException
    {
        return getJsonBuilder().buildJsonStructure(jsonObj, depth);
    }


    // For debugging...
    @Override
    public String toString()
    {
        return "MiniJsonWrapper [jsonObj=" + jsonObj + "]";
    }

    
}
