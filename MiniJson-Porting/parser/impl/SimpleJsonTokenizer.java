package org.minijson.parser.impl;

import java.io.Reader;
import java.util.logging.Logger;

import org.minijson.parser.TraceableJsonTokenizer;
import org.minijson.parser.policy.ParserPolicy;


/**
 * Simple JsonTokenizer implementation.
 * It's a "default" tokenizer for MiniJson.
 */
public final class SimpleJsonTokenizer extends AbstractJsonTokenizer implements TraceableJsonTokenizer
{
    private static final Logger log = Logger.getLogger(SimpleJsonTokenizer.class.getName());

    public SimpleJsonTokenizer(String str)
    {
        super(str);
    }
    public SimpleJsonTokenizer(Reader reader)
    {
        super(reader);
    }
    public SimpleJsonTokenizer(Reader reader, ParserPolicy parserPolicy)
    {
        super(reader, parserPolicy);
    }

    @Override
    protected void init()
    {
        // Disable "tracing" by default.
        disableTracing();
    }

}
