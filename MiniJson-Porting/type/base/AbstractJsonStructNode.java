package org.minijson.type.base;

import java.io.IOException;
import java.io.Serializable;
import java.io.StringWriter;
import java.io.Writer;
import java.util.logging.Level;
import java.util.logging.Logger;

import org.minijson.builder.JsonBuilderException;
import org.minijson.type.JsonStructNode;


public abstract class AbstractJsonStructNode extends AbstractJsonNode implements JsonStructNode, Serializable
{
    private static final Logger log = Logger.getLogger(AbstractJsonStructNode.class.getName());
    private static final long serialVersionUID = 1L;

    // Note: The default depth of AbstractJsonStructNodes is always 1.   
    private static final int DRILL_DOWN_DEPTH = 1;

    // For all AbstractJsonStructNodes, indent is always 1 by default;
    private static final int DEFAULT_INDENT = 0;

    
    public AbstractJsonStructNode()
    {
    }


    @Override
    public String toJsonString() throws JsonBuilderException
    {
        return toJsonString(DEFAULT_INDENT);
    }

    @Override
    public String toJsonString(int indent) throws JsonBuilderException
    {
        StringWriter writer = new StringWriter();
        try {
            writeJsonString(writer, indent);
        } catch (IOException e) {
            // What to do???
            log.log(Level.WARNING, "Failed to write to StringWriter.", e);
            return null;
        }
        String str = writer.toString();
        return str;
    }

    @Override
    public void writeJsonString(Writer writer) throws IOException, JsonBuilderException
    {
        writeJsonString(writer, DEFAULT_INDENT);
    }

    @Override
    public abstract void writeJsonString(Writer writer, int indent) throws IOException, JsonBuilderException;


//    @Override
//    public abstract boolean isJsonStructureArray();

    @Override
    public Object toJsonStructure() throws JsonBuilderException
    {
        return toJsonStructure(DRILL_DOWN_DEPTH);
    }

    @Override
    public abstract Object toJsonStructure(int depth) throws JsonBuilderException;



}
