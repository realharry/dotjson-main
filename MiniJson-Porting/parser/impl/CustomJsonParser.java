package org.minijson.parser.impl;

import java.util.logging.Logger;

import org.minijson.parser.FlexibleJsonParser;
import org.minijson.parser.RichJsonParser;
import org.minijson.parser.policy.ParserPolicy;
import org.minijson.type.factory.JsonTypeFactory;


public class CustomJsonParser extends AbstractRichJsonParser implements RichJsonParser, FlexibleJsonParser
{
    private static final Logger log = Logger.getLogger(CustomJsonParser.class.getName());

    public CustomJsonParser()
    {
    }
    public CustomJsonParser(JsonTypeFactory jsonTypeFactory)
    {
        super(jsonTypeFactory);
    }

    public CustomJsonParser(JsonTypeFactory jsonTypeFactory, ParserPolicy parserPolicy)
    {
        super(jsonTypeFactory, parserPolicy);
    }

    @Override
    protected void init()
    {
        // Enable "tracing" by default.
        enableTracing();
    }


}
