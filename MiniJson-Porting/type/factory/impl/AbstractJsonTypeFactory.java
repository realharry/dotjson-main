package org.minijson.type.factory.impl;

import java.util.List;
import java.util.Map;
import java.util.logging.Logger;

import org.minijson.type.base.AbstractJsonArrayNode;
import org.minijson.type.base.AbstractJsonBooleanNode;
import org.minijson.type.base.AbstractJsonNullNode;
import org.minijson.type.base.AbstractJsonNumberNode;
import org.minijson.type.base.AbstractJsonObjectNode;
import org.minijson.type.base.AbstractJsonStringNode;
import org.minijson.type.factory.JsonTypeFactory;


public class AbstractJsonTypeFactory implements JsonTypeFactory
{
    private static final Logger log = Logger.getLogger(AbstractJsonTypeFactory.class.getName());
    
    private AbstractJsonTypeFactory()
    {
    }

    // Initialization-on-demand holder.
    private static final class AbstractJsonTypeFactoryHolder
    {
        private static final AbstractJsonTypeFactory INSTANCE = new AbstractJsonTypeFactory();
    }
    // Singleton method
    public static AbstractJsonTypeFactory getInstance()
    {
        return AbstractJsonTypeFactoryHolder.INSTANCE;
    }

    
    @Override
    public Object createNull()
    {
        return AbstractJsonNullNode.NULL;
    }
    @Override
    public Object createBoolean(Boolean value)
    {
        if(value == null) {
            return AbstractJsonBooleanNode.NULL;
        }
        if(Boolean.TRUE.equals(value)) {
            return AbstractJsonBooleanNode.TRUE;
        } else {
            return AbstractJsonBooleanNode.FALSE;
        }
    }
    @Override
    public Object createNumber(Number value)
    {
        if(value == null) {
            return AbstractJsonNumberNode.NULL;
        }
        return new AbstractJsonNumberNode(value);
    }
    @Override
    public Object createString(String value)
    {
        if(value == null) {
            return AbstractJsonStringNode.NULL;
        }
        return new AbstractJsonStringNode(value);
    }
    @Override
    public Map<String, Object> createObject(Map<String, Object> map)
    {
        if(map == null) {
            return AbstractJsonObjectNode.NULL;
        }
        return new AbstractJsonObjectNode(map);
    }
    @Override
    public List<Object> createArray(List<Object> list)
    {
        if(list == null) {
            return AbstractJsonArrayNode.NULL;
        }
        return new AbstractJsonArrayNode(list);
    }
    
    
}
