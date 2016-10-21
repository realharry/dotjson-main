package org.minijson.parser;

import org.minijson.parser.policy.ParserPolicy;


/**
 * JsonParser with configurable options.
 */
public interface FlexibleJsonParser extends JsonParser
{
    /**
     * Returns the parserPolicy.
     * @return The ParserPolciy objects associated with JsonParser.
     */
    ParserPolicy getParserPolicy();
}
