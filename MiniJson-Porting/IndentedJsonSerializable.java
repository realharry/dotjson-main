package org.minijson;

import java.io.IOException;
import java.io.Writer;

import org.minijson.builder.JsonBuilderException;


/**
 * JsonSerializable represents the "opposite" of JsonParseable.
 * IndentedJsonSerializable defines serialize methods with indentations.
 */
public interface IndentedJsonSerializable extends JsonSerializable
{
    // TBD:
    // Move "indent" options to BuilderPolicy ????
    /**
     * Creates a JSON string from this object.
     * @param indent 
     * @return Returns the JSON string.
     * @throws JsonBuilderException
     */
    String toJsonString(int indent) throws JsonBuilderException;

    /**
     * Writes a JSON string representation of this object to the writer. 
     * @param writer
     * @param indent Indent level for "pretty printing".
     * @throws IOException
     * @throws JsonBuilderException
     */
    void writeJsonString(Writer writer, int indent) throws IOException, JsonBuilderException;

    // String toJsonString(JsonCompatible jsonObj);
    // String toJsonString(Object obj);
}
