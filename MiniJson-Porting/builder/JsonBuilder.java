package org.minijson.builder;

import java.io.IOException;
import java.io.Writer;


/**
 * Builds JSON strings from a given object.
 */
public interface JsonBuilder
{
    /////////////////////////////////////////////////////////////////
    // The following corresponds to the methods in JsonSerializable

    /**
     * Generates a JSON string from the given jsonObj.
     * @param jsonObj
     * @return JSON string representation of the given jsonObj.
     * @throws JsonBuilderException
     */
    String build(Object jsonObj) throws JsonBuilderException;

    /**
     * Generates a JSON string from the given jsonObj and writes it to the writer.
     * @param writer
     * @param jsonObj
     * @throws IOException
     * @throws JsonBuilderException
     */
    void build(Writer writer, Object jsonObj) throws IOException, JsonBuilderException;

}
