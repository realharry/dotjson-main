package org.minijson.parser;

import java.io.IOException;
import java.io.Reader;


/**
 * Json Parser: Creates an Object from the given JSON string.
 */
public interface JsonParser
{
    /**
     * Parses the given JSON string.
     * @param jsonStr
     * @return Object corresponding to the given JSON string.
     * @throws JsonParserException
     */
    Object parse(String jsonStr) throws JsonParserException;

    /**
     * Parses the JSON string from the given Reader.
     * @param reader
     * @return Object corresponding to the read JSON string.
     * @throws JsonParserException
     * @throws IOException
     */
    Object parse(Reader reader) throws JsonParserException, IOException;
}
