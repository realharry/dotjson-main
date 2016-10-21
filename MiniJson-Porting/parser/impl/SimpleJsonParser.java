package org.minijson.parser.impl;

import java.util.logging.Logger;

import org.minijson.parser.BareJsonParser;
import org.minijson.parser.policy.ParserPolicy;
import org.minijson.type.factory.JsonTypeFactory;


/** 
 * Simple BareJsonParser implementation.
 */
public final class SimpleJsonParser extends AbstractBareJsonParser implements BareJsonParser
{
    private static final Logger log = Logger.getLogger(SimpleJsonParser.class.getName());

    public SimpleJsonParser()
    {
    }
    public SimpleJsonParser(JsonTypeFactory jsonTypeFactory)
    {
        super(jsonTypeFactory);
    }
    public SimpleJsonParser(JsonTypeFactory jsonTypeFactory, ParserPolicy parserPolicy)
    {
        super(jsonTypeFactory, parserPolicy);
    }
    public SimpleJsonParser(JsonTypeFactory jsonTypeFactory, ParserPolicy parserPolicy, boolean threadSafe)
    {
        super(jsonTypeFactory, parserPolicy, threadSafe);
    }

    @Override
    protected void init()
    {
        // Disable "tracing" by default.
        disableTracing();
    }

}
