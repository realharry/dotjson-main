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
 * Convenience class.
 * Can be used as a base class for classes implementing JsonAny.
 */
public abstract class MiniJsonBase implements JsonAny
{
    private static final Logger log = Logger.getLogger(MiniJsonBase.class.getName());

    // temporary
    private static final int DRILL_DOWN_DEPTH = 1;
    
    // Lazy initialized.
    private JsonParser miniJsonParser = null;
    private BareJsonBuilder miniJsonBuilder = null;
    
    // TBD:
    // ParserPolicy????
    // BuilderPolicy????
    // ...
    
    public MiniJsonBase()
    {
        init();
    }
    protected void init()
    {
        // Place-holder.
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


    // Note: Conceptually, it is a method "defined" in JsonParseable.
    // Since you cannot inherit this implementation in subclasses,
    // each subclass, or any class that implements JsonParseable, should have its own implementation of fromJson().
    // Use this class as an example.... if necessary...
    // (but, this is a rather strange implementation...)
    // (normally, each specific field of a JsonParseable class should be initialized by the corresponding values in jsonStr,
    //      and it's rather hard to create a "generic" implementation, unless you use reflection, etc.)
//    public static JsonParseable fromJson(String jsonStr)
//    {
//        MiniJsonBase jsonParseable = null;
//        try {
//            // Object obj = MiniJsonParser.DEFAULT_INSTANCE.parse(jsonStr);
//            Object obj = sMiniJsonParser.parse(jsonStr);
//            jsonParseable = new MiniJsonBase() {};
//            // Copy the field value....
//        } catch (JsonParserException e) {
//            throw new RuntimeException(e);
//        }
//        return jsonParseable;
//    }


    @Override
    public String toJsonString() throws JsonBuilderException
    {
        return getJsonBuilder().build(this);
    }
    @Override
    public String toJsonString(int indent) throws JsonBuilderException
    {
        return getJsonBuilder().build(this, indent);
    }

    @Override
    public void writeJsonString(Writer writer) throws IOException, JsonBuilderException
    {
        getJsonBuilder().build(writer, this);
    }
    @Override
    public void writeJsonString(Writer writer, int indent) throws IOException, JsonBuilderException
    {
        getJsonBuilder().build(writer, this, indent);
    }


    @Override
    public Object toJsonStructure() throws JsonBuilderException
    {
        return toJsonStructure(DRILL_DOWN_DEPTH);
    }

    @Override
    public Object toJsonStructure(int depth) throws JsonBuilderException
    {
        return getJsonBuilder().buildJsonStructure(this, depth);
    }

    
}
