package org.minijson.parser.core;

import java.io.Serializable;


/**
 * Simple wrapper around a String. To be used as a member of ParserException.
 * (It's hard to overload exception constructor with a String arg.
 *    Use this class instead of String, if apropriate.)
 */
public final class ErrorContext implements Serializable
{
    private static final long serialVersionUID = 1L;
    private static final String ERROR_POINT_MARKER = " ... ";   // temporary 

    // Before and After the error during the character reading.
    private final char[] tail;   // past
    private final char[] head;   // future
    // "cache" - lazy initialized.
    // Note that we have this strange dual data representation.
    // Once context is set (e.g., through Ctor, or via toString()), then tail/head is ignored.
    private String context = null;
    
    // Use builders.
    private ErrorContext(char[] tail)
    {
        this(tail, null);
    }
    private ErrorContext(char[] tail, char[] head)
    {
        this.tail = tail;
        this.head = head;
    }
    private ErrorContext(String context)
    {
        this.tail = this.head = null;
        this.context = context;
    }
    
    public static ErrorContext build(char[] tail)
    {
        return build(tail, null);
    }
    public static ErrorContext build(char[] tail, char[] head)
    {
        return new ErrorContext(tail, head);
    }
    public static ErrorContext build(String context)
    {
        // if context == null, return null ?????
        return new ErrorContext(context);
    }
    public static ErrorContext build(String tail, String head)
    {
        String context = buildContextString(tail, head);
        return new ErrorContext(context);
    }
    
    public String getContext()
    {
        if(context == null) {
            context = buildContextString();
        }
        return context;
    }
    
    public static String buildContextString(char[] tail, char[] head)
    {
        StringBuilder sb = new StringBuilder();
        if(tail != null) {
            sb.append(tail);
        }
        sb.append(ERROR_POINT_MARKER);
        if(head != null) {
            sb.append(head);
        }
        return sb.toString();
    }
    public static String buildContextString(String tail, String head)
    {
        StringBuilder sb = new StringBuilder();
        if(tail != null) {
            sb.append(tail);
        }
        sb.append(ERROR_POINT_MARKER);
        if(head != null) {
            sb.append(head);
        }
        return sb.toString();
    }

    private String buildContextString()
    {
        return buildContextString(tail, head);
    }

    @Override
    public String toString()
    {
        if(context == null) {
            context = buildContextString();
        }
        return context;
    }

    
}
