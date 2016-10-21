package org.minijson.mini;

import java.io.IOException;
import java.io.Writer;
import java.util.logging.Logger;

import org.minijson.builder.BareJsonBuilder;
import org.minijson.builder.JsonBuilderException;
import org.minijson.builder.factory.BareJsonBuilderFactory;
import org.minijson.builder.impl.SimpleJsonBuilder;


/**
 * JsonBuilder wrapper.
 * The primary purpose of this class is to "minimize" the interface of the real implementation.
 * 
 * Usage:
 * <pre>
 * <code>
 * String jsonStr = MiniJsonBuilder.build(object);
 * </code>
 * </pre>
 *
 */
// This is kind of an "immutable" wrapper around SimpleJsonBuilder or other BareJsonBuilder.
public final class MiniJsonBuilder implements BareJsonBuilder
{
    // private static final Logger log = Logger.getLogger(MiniJsonBuilder.class.getName());

    // "semi-singleton".
    // Note that MiniJsonBuilder does not have setters, at least at this point (although the Ctors take different args),
    //    and it can be made immutable, or we can just use it as a singleton (by changing Ctors).
    // But, that may change in the future. 
    //    So, just use this special instance sort of as a singleton for now, which is immutable (currently).
    // On second thought, MiniJsonBuilder may not be entirely multi-thread/concurrency safe...
    //    (Just to be symmetric with MiniJsonParser, which is not thread safe, do not use this...)
    public static final MiniJsonBuilder DEFAULT_INSTANCE = new MiniJsonBuilder(new SimpleJsonBuilder(null, true));
    
    // Delegate the implementation through decorator-like pattern.
    // private JsonBuilderFactory jsonBuilderFactory;
    private final BareJsonBuilder decoratedBuilder;

    // TBD:
    // Builder policy???
    // ...

    public MiniJsonBuilder()
    {
        this(new SimpleJsonBuilder());
    }
    public MiniJsonBuilder(BareJsonBuilderFactory jsonBuilderFactory)
    {
        this(jsonBuilderFactory.createBuilder());
    }
    public MiniJsonBuilder(BareJsonBuilder decoratedBuilder)
    {
        this.decoratedBuilder = decoratedBuilder;
    }


    /**
     * Generates a JSON string of the given object.
     */
    @Override
    public String build(Object jsonObj) throws JsonBuilderException
    {
        return decoratedBuilder.build(jsonObj);
    }
    @Override
    public String build(Object jsonObj, int indent) throws JsonBuilderException
    {
        return decoratedBuilder.build(jsonObj, indent);
    }
    @Override
    public void build(Writer writer, Object jsonObj) throws IOException, JsonBuilderException
    {
        decoratedBuilder.build(writer, jsonObj);
    }
    @Override
    public void build(Writer writer, Object jsonObj, int indent) throws IOException, JsonBuilderException
    {
        decoratedBuilder.build(writer, jsonObj, indent);
    }


    @Override
    public Object buildJsonStructure(Object jsonObj) throws JsonBuilderException
    {
        return decoratedBuilder.buildJsonStructure(jsonObj);
    }
    @Override
    public Object buildJsonStructure(Object jsonObj, int depth) throws JsonBuilderException
    {
        return decoratedBuilder.buildJsonStructure(jsonObj, depth);
    }

}
