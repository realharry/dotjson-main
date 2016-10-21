package org.minijson.type.base;

import org.minijson.type.JsonNode;
import org.minijson.type.JsonObjectMember;


public class AbstractJsonObjectMember implements JsonObjectMember
{
    private String key;
    private JsonNode value;

    public AbstractJsonObjectMember()
    {
        this(null, null);
    }
    public AbstractJsonObjectMember(String key, JsonNode value)
    {
        super();
        this.key = key;
        this.value = value;
    }


    @Override
    public String getKey()
    {
        return this.key;
    }
    public void setKey(String key)
    {
        this.key = key;
    }

    @Override
    public JsonNode getValue()
    {
        return this.value;
    }
    public void setValue(JsonNode value)
    {
        this.value = value;
    }

    
    @Override
    public String toString()
    {
        return "AbstractJsonObjectMember [key=" + key + ", value=" + value
                + "]";
    }


}
