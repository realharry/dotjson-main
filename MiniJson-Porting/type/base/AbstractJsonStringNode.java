package org.minijson.type.base;

import java.io.Serializable;
import java.util.logging.Logger;

import org.minijson.builder.JsonBuilderException;
import org.minijson.type.JsonStringNode;


public class AbstractJsonStringNode extends AbstractJsonLeafNode implements JsonStringNode, Serializable
{
    private static final Logger log = Logger.getLogger(AbstractJsonStringNode.class.getName());
    private static final long serialVersionUID = 1L;

    public static final AbstractJsonStringNode NULL = new AbstractJsonStringNode() {
        private static final long serialVersionUID = 1L;
        @Override
        public String toJsonString() throws JsonBuilderException
        {
            return "null";
        }
    };
    public static final AbstractJsonStringNode EMPTY = new AbstractJsonStringNode("") {
        private static final long serialVersionUID = 1L;
    };

    private final String value;

    private AbstractJsonStringNode()
    {
        this(null);
    }
    public AbstractJsonStringNode(String value)
    {
        super();
        this.value = value;
    }


    ///////////////////////////////////
    // JsonNode interface

    @Override
    public Object getValue()
    {
        return this.value;
    }


    ///////////////////////////////////
    // JsonSerializable interface
    
    @Override
    public String toJsonString(int indent) throws JsonBuilderException
    {
        // temporary
        if(value == null) {
            return "null";  // ???
        } else {
            return "\"" + value + "\"";
        }
    }

    
    
    @Override
    public String toString()
    {
        return "AbstractJsonString [value=" + value + "]";
    }
    
    
}
