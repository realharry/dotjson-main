package org.minijson.mini;

import java.io.IOException;
import java.io.Reader;
import java.io.StringReader;
import java.util.logging.Logger;

import org.minijson.parser.JsonParser;
import org.minijson.parser.JsonParserException;
import org.minijson.parser.JsonTokenizer;
import org.minijson.parser.LookAheadJsonTokenizer;
import org.minijson.parser.factory.JsonParserFactory;
import org.minijson.parser.impl.AbstractJsonParser;
import org.minijson.parser.impl.SimpleJsonParser;


/**
 * JsonParser wrapper.
 * The primary purpose of this class is to "minimize" the interface of the real implementation.
 * 
 * Usage:
 * <pre>
 * <code>
 * try {
 *     Object obj = MiniJsonParser.parse(jsonStr);
 * } catch (JsonParserException e) {
 * }
 * </code>
 * </pre>
 *
 */
// This is kind of an "immutable" wrapper around SimpleJsonParser or other JsonParser.
public final class MiniJsonParser implements JsonParser
{
    // private static final Logger log = Logger.getLogger(MiniJsonParser.class.getName());

    // "semi-singleton".
    // Note that MiniJsonParser does not have setters, at least at this point (although the Ctors take different args),
    //    and it can be made immutable, or we can just use it as a singleton (by changing Ctors).
    // But, that may change in the future. 
    //    So, just use this special instance sort of as a singleton for now, which is immutable (currently).
    // -->
    // On second thought, MiniJsonParser is not entirely multi-thread/concurrency safe...
    //     (some optimization code for performance makes it non-thread safe.)
    public static final MiniJsonParser DEFAULT_INSTANCE = new MiniJsonParser(new SimpleJsonParser(null, null, true));

    // Delegate the implementation through decorator-like pattern.
    // private JsonParserFactory jsonParserFactory;
    private final JsonParser decoratedParser;
    
    // TBD:
    // parser policty?
    // ....

    public MiniJsonParser()
    {
        this(new SimpleJsonParser());
    }
    public MiniJsonParser(JsonParserFactory jsonParserFactory)
    {
        this(jsonParserFactory.createParser());
    }
    public MiniJsonParser(JsonParser decoratedParser)
    {
        this.decoratedParser = decoratedParser;
    }


    public boolean isLookAheadParsing()
    {
        return  ((AbstractJsonParser) decoratedParser).isLookAheadParsing();
    }
    public void enableLookAheadParsing()
    {
        ((AbstractJsonParser) decoratedParser).enableLookAheadParsing();
    }
    public void disableLookAheadParsing()
    {
        ((AbstractJsonParser) decoratedParser).disableLookAheadParsing();
    }

    public boolean isTracingEnabled()
    {
        return  ((AbstractJsonParser) decoratedParser).isTracingEnabled();
    }
    public void enableTracing()
    {
        ((AbstractJsonParser) decoratedParser).enableTracing();
    }
    public void disableTracing()
    {
        ((AbstractJsonParser) decoratedParser).disableTracing();
    }


    /**
     * Creates a Java object based on the given jsonStr.
     */
    @Override
    public Object parse(String jsonStr) throws JsonParserException
    {
        return decoratedParser.parse(jsonStr);
    }

    @Override
    public Object parse(Reader reader) throws JsonParserException, IOException
    {
        return decoratedParser.parse(reader);
    }

}
