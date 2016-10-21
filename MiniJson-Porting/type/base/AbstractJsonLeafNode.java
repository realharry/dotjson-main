package org.minijson.type.base;

import java.io.IOException;
import java.io.Serializable;
import java.io.Writer;
import java.util.logging.Logger;

import org.minijson.builder.JsonBuilderException;
import org.minijson.type.JsonLeafNode;


public abstract class AbstractJsonLeafNode extends AbstractJsonNode implements JsonLeafNode, Serializable
{
    private static final Logger log = Logger.getLogger(AbstractJsonLeafNode.class.getName());
    private static final long serialVersionUID = 1L;
    
    // 
    // Indent level is irrelevant for all AbstractJsonLeafNodes. 
    // But, just to be consistent, we are setting AbstractJsonLeafNodes's indent to 0;
    private static final int DEFAULT_INDENT = 0;

    public AbstractJsonLeafNode()
    {
    }


    @Override
    public String toJsonString() throws JsonBuilderException
    {
        return toJsonString(DEFAULT_INDENT);
    }

    @Override
    public abstract String toJsonString(int indent) throws JsonBuilderException;

    @Override
    public void writeJsonString(Writer writer) throws IOException, JsonBuilderException
    {
        writeJsonString(writer, DEFAULT_INDENT);
    }

    @Override
    public void writeJsonString(Writer writer, int indent) throws IOException, JsonBuilderException
    {
        String str = toJsonString(indent);
        writer.write(str);
    }


}
