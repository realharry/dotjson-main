package org.minijson.mini;

import java.io.IOException;
import java.io.Writer;

import org.minijson.builder.JsonBuilderException;
import org.minijson.builder.partial.MixedJsonBuilder;
import org.minijson.builder.partial.impl.SimpleMixedJsonBuilder;
import org.minijson.parser.impl.AbstractJsonParser;


/**
 * MixedJsonBuilder wrapper.
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
public final class MiniMixedBuilder implements MixedJsonBuilder
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
    public static final MiniMixedBuilder DEFAULT_INSTANCE = new MiniMixedBuilder(new SimpleMixedJsonBuilder(null, true));

    // Delegate the implementation through decorator-like pattern.
    // private JsonParserFactory jsonParserFactory;
    private final MixedJsonBuilder decoratedBuilder;
    
    // TBD:
    // parser policty?
    // ....

    public MiniMixedBuilder()
    {
        this(new SimpleMixedJsonBuilder());
    }
    public MiniMixedBuilder(MixedJsonBuilder decoratedBuilder)
    {
        this.decoratedBuilder = decoratedBuilder;
    }


    public boolean isLookAheadParsing()
    {
        return  ((AbstractJsonParser) decoratedBuilder).isLookAheadParsing();
    }
    public void enableLookAheadParsing()
    {
        ((AbstractJsonParser) decoratedBuilder).enableLookAheadParsing();
    }
    public void disableLookAheadParsing()
    {
        ((AbstractJsonParser) decoratedBuilder).disableLookAheadParsing();
    }

    public boolean isTracingEnabled()
    {
        return  ((AbstractJsonParser) decoratedBuilder).isTracingEnabled();
    }
    public void enableTracing()
    {
        ((AbstractJsonParser) decoratedBuilder).enableTracing();
    }
    public void disableTracing()
    {
        ((AbstractJsonParser) decoratedBuilder).disableTracing();
    }


    @Override
    public String build(Object jsonObj) throws JsonBuilderException
    {
        return decoratedBuilder.build(jsonObj);
    }
    @Override
    public void build(Writer writer, Object jsonObj) throws IOException, JsonBuilderException
    {
        decoratedBuilder.build(writer, jsonObj);
    }
    @Override
    public String buildMixed(Object jsonObj) throws JsonBuilderException
    {
        return decoratedBuilder.buildMixed(jsonObj);
    }
    @Override
    public String buildMixed(Object jsonObj, int minDepth, int maxDepth) throws JsonBuilderException
    {
        return decoratedBuilder.buildMixed(jsonObj, minDepth, maxDepth);
    }
    @Override
    public String buildMixed(Object jsonObj, int minDepth, int maxDepth, int indent) throws JsonBuilderException
    {
        return decoratedBuilder.buildMixed(jsonObj, minDepth, maxDepth, indent);
    }
    @Override
    public void buildMixed(Writer writer, Object jsonObj) throws JsonBuilderException, IOException
    {
        decoratedBuilder.buildMixed(writer, jsonObj);
    }
    @Override
    public void buildMixed(Writer writer, Object jsonObj, int minDepth, int maxDepth) throws JsonBuilderException, IOException
    {
        decoratedBuilder.buildMixed(writer, jsonObj, minDepth, maxDepth);
    }
    @Override
    public void buildMixed(Writer writer, Object jsonObj, int minDepth, int maxDepth, int indent) throws JsonBuilderException, IOException
    {
        decoratedBuilder.buildMixed(writer, jsonObj, minDepth, maxDepth, indent);
    }

}
