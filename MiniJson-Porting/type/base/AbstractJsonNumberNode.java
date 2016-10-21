package org.minijson.type.base;

import java.io.Serializable;
import java.util.logging.Logger;

import org.minijson.builder.JsonBuilderException;
import org.minijson.common.Literals;
import org.minijson.type.JsonNumberNode;


public class AbstractJsonNumberNode extends AbstractJsonLeafNode implements JsonNumberNode, Serializable
{
    private static final Logger log = Logger.getLogger(AbstractJsonNumberNode.class.getName());
    private static final long serialVersionUID = 1L;

    public static final AbstractJsonNumberNode NULL = new AbstractJsonNumberNode() {
        private static final long serialVersionUID = 1L;
        @Override
        public String toJsonString() throws JsonBuilderException
        {
            return Literals.NULL;
        }
    };
    public static final AbstractJsonNumberNode ZERO = new AbstractJsonNumberNode(0) {
        private static final long serialVersionUID = 1L;
    };

    private final Number value;

    private AbstractJsonNumberNode()
    {
        this(null);
    }
    public AbstractJsonNumberNode(Number value)
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
            // return "0";  // ???
            return Literals.NULL;  // ???
        } else {
            return value.toString();  // ???
        }
    }
    
    
    //////////////////////////////////////////
    // Number

    public int intValue()
    {
        return value.intValue();
    }
    public long longValue()
    {
        return value.longValue();
    }
    public float floatValue()
    {
        return value.floatValue();
    }
    public double doubleValue()
    {
        return value.doubleValue();
    }

    
    
    
    
    @Override
    public String toString()
    {
        return "AbstractJsonNumber [value=" + value + "]";
    }

    
    
}
