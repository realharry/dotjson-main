package org.minijson.parser.exception;

import org.minijson.parser.JsonParserException;
import org.minijson.parser.core.ErrorContext;


/**
 * Indicates error during parsing.
 */
public class JsonTokenizerException extends JsonParserException
{
    private static final long serialVersionUID = 1L;

    public JsonTokenizerException()
    {
        super();
    }
    public JsonTokenizerException(char[] tail)
    {
        super(tail);
    }
    public JsonTokenizerException(char[] tail, char[] head)
    {
        super(tail, head);
    }
    public JsonTokenizerException(ErrorContext context)
    {
        super(context);
    }

    public JsonTokenizerException(String message)
    {
        super(message);
    }
    public JsonTokenizerException(String message, String context)
    {
        super(message, context);
    }
    public JsonTokenizerException(String message, char[] tail)
    {
        super(message, tail);
    }
    public JsonTokenizerException(String message, char[] tail, char[] head)
    {
        super(message, tail, head);
    }
    public JsonTokenizerException(String message, ErrorContext context)
    {
        super(message, context);
    }

    public JsonTokenizerException(Throwable cause)
    {
        super(cause);
    }
    public JsonTokenizerException(Throwable cause, String context)
    {
        super(cause, context);
    }
    public JsonTokenizerException(Throwable cause, char[] tail)
    {
        super(cause, tail);
    }
    public JsonTokenizerException(Throwable cause, char[] tail, char[] head)
    {
        super(cause, tail, head);
    }
    public JsonTokenizerException(Throwable cause, ErrorContext context)
    {
        super(cause, context);
    }

    public JsonTokenizerException(String message, Throwable cause)
    {
        super(message, cause);
    }
    public JsonTokenizerException(String message, Throwable cause, String context)
    {
        super(message, cause, context);
    }
    public JsonTokenizerException(String message, Throwable cause, char[] tail)
    {
        super(message, cause, tail);
    }
    public JsonTokenizerException(String message, Throwable cause, char[] tail, char[] head)
    {
        super(message, cause, tail, head);
    }
    public JsonTokenizerException(String message, Throwable cause, ErrorContext context)
    {
        super(message, cause, context);
    }

}
