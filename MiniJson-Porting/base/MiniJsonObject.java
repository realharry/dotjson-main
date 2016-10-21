package org.minijson.base;

import java.util.Map;

import org.minijson.JsonObject;


public final class MiniJsonObject extends MiniJsonWrapper implements JsonObject
{

    // JSON builder
    public MiniJsonObject(Map<String,Object> map)
    {
        super(map);
    }

    // JSON Parser
    public MiniJsonObject(String jsonStr)
    {
        super(jsonStr);
    }


//    // @Override
//    public boolean isJsonStructureArray()
//    {
//        return false;
//    }


}
