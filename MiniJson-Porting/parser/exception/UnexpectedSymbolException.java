package org.minijson.parser.exception;

import org.minijson.parser.core.ErrorContext;


/**
 * Indicates invalid/unexpected symbol during parsing.
 */
public class UnexpectedSymbolException extends JsonTokenizerException
{
    private static final long serialVersionUID = 1L;

    public UnexpectedSymbolException()
    {
        super();
    }
    public UnexpectedSymbolException(char[] tail)
    {
        super(tail);
    }
    public UnexpectedSymbolException(char[] tail, char[] head)
    {
        super(tail, head);
    }
    public UnexpectedSymbolException(ErrorContext context)
    {
        super(context);
    }

    public UnexpectedSymbolException(String message)
    {
        super(message);
    }
    public UnexpectedSymbolException(String message, String context)
    {
        super(message, context);
    }
    public UnexpectedSymbolException(String message, char[] tail)
    {
        super(message, tail);
    }
    public UnexpectedSymbolException(String message, char[] tail, char[] head)
    {
        super(message, tail, head);
    }
    public UnexpectedSymbolException(String message, ErrorContext context)
    {
        super(message, context);
    }

    public UnexpectedSymbolException(Throwable cause)
    {
        super(cause);
    }
    public UnexpectedSymbolException(Throwable cause, String context)
    {
        super(cause, context);
    }
    public UnexpectedSymbolException(Throwable cause, char[] tail)
    {
        super(cause, tail);
    }
    public UnexpectedSymbolException(Throwable cause, char[] tail, char[] head)
    {
        super(cause, tail, head);
    }
    public UnexpectedSymbolException(Throwable cause, ErrorContext context)
    {
        super(cause, context);
    }

    public UnexpectedSymbolException(String message, Throwable cause)
    {
        super(message, cause);
    }
    public UnexpectedSymbolException(String message, Throwable cause, String context)
    {
        super(message, cause, context);
    }
    public UnexpectedSymbolException(String message, Throwable cause, char[] tail)
    {
        super(message, cause, tail);
    }
    public UnexpectedSymbolException(String message, Throwable cause, char[] tail, char[] head)
    {
        super(message, cause, tail, head);
    }
    public UnexpectedSymbolException(String message, Throwable cause, ErrorContext context)
    {
        super(message, cause, context);
    }

}
