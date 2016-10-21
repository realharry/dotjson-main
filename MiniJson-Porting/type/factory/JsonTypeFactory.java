package org.minijson.type.factory;

import java.util.List;
import java.util.Map;


public interface JsonTypeFactory
{
    Object createNull();
    Object createBoolean(Boolean value);
    Object createNumber(Number value);
    Object createString(String value);
    Map<String,Object> createObject(Map<String,Object> map);
    List<Object> createArray(List<Object> list);

}
