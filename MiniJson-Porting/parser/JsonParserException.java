package org.minijson.parser;

import org.minijson.JsonException;
import org.minijson.parser.core.ErrorContext;


/**
 * Base class for all parser/lexer related exceptions.
 */
public class JsonParserException extends JsonException
{
    private static final long serialVersionUID = 1L;

    // Stores the last X characters previously read (and, some following chars) when the error occurred.
    private ErrorContext context;


    public JsonParserException()
    {
        this((ErrorContext) null);
    }
    public JsonParserException(char[] tail)
    {
        this(ErrorContext.build(tail));
    }
    public JsonParserException(char[] tail, char[] head)
    {
        this(ErrorContext.build(tail, head));
    }
    public JsonParserException(ErrorContext context)
    {
        this.context = context;
    }

    public JsonParserException(String message)
    {
        this(message, (ErrorContext) null);
    }
    public JsonParserException(String message, String context)
    {
        this(message, ErrorContext.build(context));
    }
    public JsonParserException(String message, char[] tail)
    {
        this(message, ErrorContext.build(tail));
    }
    public JsonParserException(String message, char[] tail, char[] head)
    {
        this(message, ErrorContext.build(tail, head));
    }
    public JsonParserException(String message, ErrorContext context)
    {
        super(message);
        this.context = context;
    }

    public JsonParserException(Throwable cause)
    {
        this(cause, (ErrorContext) null);
    }
    public JsonParserException(Throwable cause, String context)
    {
        this(cause, ErrorContext.build(context));
    }
    public JsonParserException(Throwable cause, char[] tail)
    {
        this(cause, ErrorContext.build(tail));
    }
    public JsonParserException(Throwable cause, char[] tail, char[] head)
    {
        this(cause, ErrorContext.build(tail, head));
    }
    public JsonParserException(Throwable cause, ErrorContext context)
    {
        super(cause);
        this.context = context;
    }

    public JsonParserException(String message, Throwable cause)
    {
        this(message, cause, (ErrorContext) null);
    }
    public JsonParserException(String message, Throwable cause, String context)
    {
        this(message, cause, ErrorContext.build(context));
    }
    public JsonParserException(String message, Throwable cause, char[] tail)
    {
        this(message, cause, ErrorContext.build(tail));
    }
    public JsonParserException(String message, Throwable cause, char[] tail, char[] head)
    {
        this(message, cause, ErrorContext.build(tail, head));
    }
    public JsonParserException(String message, Throwable cause, ErrorContext context)
    {
        super(message, cause);
        this.context = context;
    }

    public JsonParserException(String message, Throwable cause,
            boolean enableSuppression, boolean writableStackTrace)
    {
        super(message, cause, enableSuppression, writableStackTrace);
    }


    public final ErrorContext getErrorContext()
    {
        return this.context;                
    }
    public final String getContext()
    {
        if(this.context != null) {
            return this.context.getContext();
        } else {
            return null;   // null or "" ??
        }
    }


}
