package org.minijson.parser.core;


/**
 * JsonToken is immutable.
 */
public final class JsonToken
{
    private final int type;
    private final Object value;

    public JsonToken(int type, Object value)
    {
        this.type = type;
        this.value = value;
    }

    public int getType()
    {
        return type;
    }
    public Object getValue()
    {
        return value;
    }

    // TBD:
    // Note that hash collision can generate a parse error.
    // (Cf. TokenTool)
    public static int getHashCode(int type, Object value)
    {
        final int prime = 7211;
        int result = 1;
        result = prime * result + type;
        result = prime * result + ((value == null) ? 0 : value.hashCode());
        return result;
    }

    @Override
    public int hashCode()
    {
        return getHashCode(type, value);
    }

    @Override
    public boolean equals(Object obj)
    {
        if (this == obj)
            return true;
        if (obj == null)
            return false;
        if (getClass() != obj.getClass())
            return false;
        JsonToken other = (JsonToken) obj;
        if (type != other.type)
            return false;
        if (value == null) {
            if (other.value != null)
                return false;
        } else if (!value.equals(other.value))
            return false;
        return true;
    }

    @Override
    public String toString()
    {
        return "JsonToken [type=" + type + ", value=" + value + "]";
    }


}
