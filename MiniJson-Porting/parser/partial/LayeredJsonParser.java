package org.minijson.parser.partial;

import java.io.IOException;
import java.io.Reader;

import org.minijson.builder.JsonBuilderException;
import org.minijson.parser.JsonParser;
import org.minijson.parser.JsonParserException;


/**
 * LayeredJsonParser is a "partial" json parser.
 * While parsing a json string, if it reaches the specified depth,
 *     rather than continuing to parse the child elements/nodes,
 *     it just returns the partial json string representing the node. 
 */
public interface LayeredJsonParser extends JsonParser
{
    // TBD:
    Object parse(String jsonStr, int depth) throws JsonParserException;
    Object parse(Reader reader, int depth) throws JsonParserException, IOException;
}
