package org.minijson;


/**
 * Base class of all MiniJson exceptions.
 */
public class JsonException extends Exception 
{
    private static final long serialVersionUID = 1L;


    public JsonException()
    {
        super();
    }
    public JsonException(String message)
    {
        super(message);
    }
    public JsonException(Throwable cause)
    {
        super(cause);
    }
    public JsonException(String message, Throwable cause)
    {
        super(message, cause);
    }
    public JsonException(String message, Throwable cause,
            boolean enableSuppression, boolean writableStackTrace)
    {
        super(message, cause, enableSuppression, writableStackTrace);
    }


}
