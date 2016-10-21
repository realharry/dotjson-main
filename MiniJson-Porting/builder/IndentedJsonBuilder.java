package org.minijson.builder;

import java.io.IOException;
import java.io.Writer;


/**
 * JsonBuilder with "indent" options.
 * See the comment for the class, IndentInfoStruct.
 */
public interface IndentedJsonBuilder extends JsonBuilder
{
    /**
     * Creates a json string from the given jsonObj. 
     * @param jsonObj
     * @param indent Indent level for "pretty printing"
     * @return JSON String representation of the given object.
     * @throws JsonBuilderException
     */
    String build(Object jsonObj, int indent) throws JsonBuilderException;
    
    /**
     * Writes a json string to the writer from the given jsonObj. 
     * @param writer 
     * @param jsonObj
     * @param indent Indent level for "pretty printing"
     * @throws IOException
     * @throws JsonBuilderException
     */
    void build(Writer writer, Object jsonObj, int indent) throws IOException, JsonBuilderException;

}
