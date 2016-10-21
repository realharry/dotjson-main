package org.minijson.type.base;

import java.io.IOException;
import java.io.Serializable;
import java.io.Writer;
import java.util.Map;
import java.util.logging.Logger;

import org.minijson.builder.JsonBuilderException;
import org.minijson.common.Literals;
import org.minijson.type.JsonBooleanNode;


public class AbstractJsonBooleanNode extends AbstractJsonLeafNode implements JsonBooleanNode, Serializable
{
    private static final Logger log = Logger.getLogger(AbstractJsonBooleanNode.class.getName());
    private static final long serialVersionUID = 1L;
    

    public static final AbstractJsonBooleanNode NULL = new AbstractJsonBooleanNode() {
        private static final long serialVersionUID = 1L;
        @Override
        public String toJsonString() throws JsonBuilderException
        {
            return Literals.NULL;
        }
    };
    public static final AbstractJsonBooleanNode TRUE = new AbstractJsonBooleanNode(true) {
        private static final long serialVersionUID = 1L;
        @Override
        public String toJsonString() throws JsonBuilderException
        {
            return Literals.TRUE;
        }
    };
    public static final AbstractJsonBooleanNode FALSE = new AbstractJsonBooleanNode(false) {
        private static final long serialVersionUID = 1L;
        @Override
        public String toJsonString() throws JsonBuilderException
        {
            return Literals.FALSE;
        }
    };
    
    
    private final Boolean value;

    private AbstractJsonBooleanNode()
    {
        this(null);
    }
    public AbstractJsonBooleanNode(Boolean value)
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
            return Literals.NULL;
        } else {
            if(value.equals(Boolean.TRUE)) {
                return Literals.TRUE;
            } else {
                return Literals.FALSE;
            }
        }
    }

    
    // For debugging...
    @Override
    public String toString()
    {
        return "AbstractJsonBoolean [value=" + value + "]";
    }
   

}
