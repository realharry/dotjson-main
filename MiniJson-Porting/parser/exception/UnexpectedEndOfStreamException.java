package org.minijson.parser.exception;

import org.minijson.parser.core.ErrorContext;


/**
 * Indicates an error due to ill-formed JSON string (or, read error), etc.
 */
public class UnexpectedEndOfStreamException extends JsonTokenizerException
{
    private static final long serialVersionUID = 1L;

    public UnexpectedEndOfStreamException()
    {
        super();
    }
    public UnexpectedEndOfStreamException(char[] tail)
    {
        super(tail);
    }
    public UnexpectedEndOfStreamException(char[] tail, char[] head)
    {
        super(tail, head);
    }
    public UnexpectedEndOfStreamException(ErrorContext context)
    {
        super(context);
    }

    public UnexpectedEndOfStreamException(String message)
    {
        super(message);
    }
    public UnexpectedEndOfStreamException(String message, String context)
    {
        super(message, context);
    }
    public UnexpectedEndOfStreamException(String message, char[] tail)
    {
        super(message, tail);
    }
    public UnexpectedEndOfStreamException(String message, char[] tail, char[] head)
    {
        super(message, tail, head);
    }
    public UnexpectedEndOfStreamException(String message, ErrorContext context)
    {
        super(message, context);
    }

    public UnexpectedEndOfStreamException(Throwable cause)
    {
        super(cause);
    }
    public UnexpectedEndOfStreamException(Throwable cause, String context)
    {
        super(cause, context);
    }
    public UnexpectedEndOfStreamException(Throwable cause, char[] tail)
    {
        super(cause, tail);
    }
    public UnexpectedEndOfStreamException(Throwable cause, char[] tail, char[] head)
    {
        super(cause, tail, head);
    }
    public UnexpectedEndOfStreamException(Throwable cause, ErrorContext context)
    {
        super(cause, context);
    }

    public UnexpectedEndOfStreamException(String message, Throwable cause)
    {
        super(message, cause);
    }
    public UnexpectedEndOfStreamException(String message, Throwable cause, String context)
    {
        super(message, cause, context);
    }
    public UnexpectedEndOfStreamException(String message, Throwable cause, char[] tail)
    {
        super(message, cause, tail);
    }
    public UnexpectedEndOfStreamException(String message, Throwable cause, char[] tail, char[] head)
    {
        super(message, cause, tail, head);
    }
    public UnexpectedEndOfStreamException(String message, Throwable cause, ErrorContext context)
    {
        super(message, cause, context);
    }

}
