package org.minijson.parser.exception;

import org.minijson.parser.JsonParserException;
import org.minijson.parser.core.ErrorContext;


/**
 * Indicates parse failure due to an unknown reason.
 */
public class UnknownParserException extends JsonParserException
{
    private static final long serialVersionUID = 1L;

    public UnknownParserException()
    {
        super();
    }
    public UnknownParserException(char[] tail)
    {
        super(tail);
    }
    public UnknownParserException(char[] tail, char[] head)
    {
        super(tail, head);
    }
    public UnknownParserException(ErrorContext context)
    {
        super(context);
    }

    public UnknownParserException(String message)
    {
        super(message);
    }
    public UnknownParserException(String message, String context)
    {
        super(message, context);
    }
    public UnknownParserException(String message, char[] tail)
    {
        super(message, tail);
    }
    public UnknownParserException(String message, char[] tail, char[] head)
    {
        super(message, tail, head);
    }
    public UnknownParserException(String message, ErrorContext context)
    {
        super(message, context);
    }

    public UnknownParserException(Throwable cause)
    {
        super(cause);
    }
    public UnknownParserException(Throwable cause, String context)
    {
        super(cause, context);
    }
    public UnknownParserException(Throwable cause, char[] tail)
    {
        super(cause, tail);
    }
    public UnknownParserException(Throwable cause, char[] tail, char[] head)
    {
        super(cause, tail, head);
    }
    public UnknownParserException(Throwable cause, ErrorContext context)
    {
        super(cause, context);
    }

    public UnknownParserException(String message, Throwable cause)
    {
        super(message, cause);
    }
    public UnknownParserException(String message, Throwable cause, String context)
    {
        super(message, cause, context);
    }
    public UnknownParserException(String message, Throwable cause, char[] tail)
    {
        super(message, cause, tail);
    }
    public UnknownParserException(String message, Throwable cause, char[] tail, char[] head)
    {
        super(message, cause, tail, head);
    }
    public UnknownParserException(String message, Throwable cause, ErrorContext context)
    {
        super(message, cause, context);
    }

}
