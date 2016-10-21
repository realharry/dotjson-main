package org.minijson.builder;

import org.minijson.JsonException;


/**
 * Base exception for all "builder"-related exceptions.
 */
public class JsonBuilderException extends JsonException
{
    private static final long serialVersionUID = 1L;

    public JsonBuilderException()
    {
        super();
    }
    public JsonBuilderException(String message)
    {
        super(message);
    }
    public JsonBuilderException(Throwable cause)
    {
        super(cause);
    }
    public JsonBuilderException(String message, Throwable cause)
    {
        super(message, cause);
    }
    public JsonBuilderException(String message, Throwable cause,
            boolean enableSuppression, boolean writableStackTrace)
    {
        super(message, cause, enableSuppression, writableStackTrace);
    }

}
