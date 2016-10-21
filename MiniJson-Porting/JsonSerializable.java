package org.minijson;

import java.io.IOException;
import java.io.Writer;

import org.minijson.builder.JsonBuilderException;


/**
 * JsonSerializable represents the "opposite" of JsonParseable.
 */
public interface JsonSerializable
{
    // Note that if an object implements both JsonSerializable and JsonCompatible.
    // They should be "consistent".
    // The structure generated using the default depth of JsonCompatible.toJsonStructure(),
    //    should be compatible with the json string returned by toJsonString().
    /**
     * Creates a JSON string from this object.
     * @return the JSON string.
     * @throws JsonBuilderException
     */
    String toJsonString() throws JsonBuilderException;

    /**
     * Writes a JSON string representation of this object to the writer. 
     * @param writer
     * @param indent Indent level for "pretty printing".
     * @throws IOException
     * @throws JsonBuilderException
     */
    void writeJsonString(Writer writer) throws IOException, JsonBuilderException;
}
