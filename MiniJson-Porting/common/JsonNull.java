package org.minijson.common;

import java.io.Serializable;


/**
 * Object to represent JSON "null" string.
 */
public final class JsonNull implements Serializable
{
    private static final long serialVersionUID = 1L;

    public static final Object NULL = new JsonNull();
    private JsonNull() {}

    @Override
    public String toString()
    {
        return "null";
    }
    
}
