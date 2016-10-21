package org.minijson.type.factory.impl;

import java.util.List;
import java.util.Map;
import java.util.logging.Logger;

import org.minijson.common.JsonNull;
import org.minijson.type.factory.JsonTypeFactory;


public class BareJsonTypeFactory implements JsonTypeFactory
{
    private static final Logger log = Logger.getLogger(BareJsonTypeFactory.class.getName());
    
    private BareJsonTypeFactory()
    {
    }

    // Initialization-on-demand holder.
    private static final class BareJsonTypeFactoryHolder
    {
        private static final BareJsonTypeFactory INSTANCE = new BareJsonTypeFactory();
    }
    // Singleton method
    public static BareJsonTypeFactory getInstance()
    {
        return BareJsonTypeFactoryHolder.INSTANCE;
    }

    
    @Override
    public Object createNull()
    {
        return JsonNull.NULL;
    }
    @Override
    public Object createBoolean(Boolean value)
    {
        return value;
    }
    @Override
    public Object createNumber(Number value)
    {
        return value;
    }
    @Override
    public Object createString(String value)
    {
        return value;
    }
    @Override
    public Map<String, Object> createObject(Map<String, Object> map)
    {
        return map;
    }
    @Override
    public List<Object> createArray(List<Object> list)
    {
        return list;
    }
    
    
}
