package org.minijson.base;

import java.util.List;

import org.minijson.JsonArray;


public final class MiniJsonArray extends MiniJsonWrapper implements JsonArray
{

    // JSON builder
    public MiniJsonArray(List<Object> list)
    {
        super(list);
    }

    // JSON Parser
    public MiniJsonArray(String jsonStr)
    {
        super(jsonStr);
    }


//    // @Override
//    public boolean isJsonStructureArray()
//    {
//        return true;
//    }

}
