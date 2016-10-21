package org.minijson.parser.exception;

import org.minijson.parser.JsonParserException;
import org.minijson.parser.core.ErrorContext;


/**
 * Indicates invalid/unexpected token during parsing.
 */
public class InvalidJsonTokenException extends JsonParserException
{
    private static final long serialVersionUID = 1L;

    public InvalidJsonTokenException()
    {
        super();
    }
    public InvalidJsonTokenException(char[] tail)
    {
        super(tail);
    }
    public InvalidJsonTokenException(char[] tail, char[] head)
    {
        super(tail, head);
    }
    public InvalidJsonTokenException(ErrorContext context)
    {
        super(context);
    }

    public InvalidJsonTokenException(String message)
    {
        super(message);
    }
    public InvalidJsonTokenException(String message, String context)
    {
        super(message, context);
    }
    public InvalidJsonTokenException(String message, char[] tail)
    {
        super(message, tail);
    }
    public InvalidJsonTokenException(String message, char[] tail, char[] head)
    {
        super(message, tail, head);
    }
    public InvalidJsonTokenException(String message, ErrorContext context)
    {
        super(message, context);
    }

    public InvalidJsonTokenException(Throwable cause)
    {
        super(cause);
    }
    public InvalidJsonTokenException(Throwable cause, String context)
    {
        super(cause, context);
    }
    public InvalidJsonTokenException(Throwable cause, char[] tail)
    {
        super(cause, tail);
    }
    public InvalidJsonTokenException(Throwable cause, char[] tail, char[] head)
    {
        super(cause, tail, head);
    }
    public InvalidJsonTokenException(Throwable cause, ErrorContext context)
    {
        super(cause, context);
    }

    public InvalidJsonTokenException(String message, Throwable cause)
    {
        super(message, cause);
    }
    public InvalidJsonTokenException(String message, Throwable cause, String context)
    {
        super(message, cause, context);
    }
    public InvalidJsonTokenException(String message, Throwable cause, char[] tail)
    {
        super(message, cause, tail);
    }
    public InvalidJsonTokenException(String message, Throwable cause, char[] tail, char[] head)
    {
        super(message, cause, tail, head);
    }
    public InvalidJsonTokenException(String message, Throwable cause, ErrorContext context)
    {
        super(message, cause, context);
    }

}
