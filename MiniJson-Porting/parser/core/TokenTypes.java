package org.minijson.parser.core;

import java.util.HashSet;
import java.util.Set;


/**
 * Defines a token "type" 
 * To be used by the tokenizer.
 */
public final class TokenTypes
{
    private TokenTypes() {}

    public static final int EOF = -1;    // Not exactly a token.
    public static final int NULL = 0;
    // public static final int TRUE = 1;
    // public static final int FALSE = 2;
    public static final int COMMA = 3;
    public static final int COLON = 4;
    public static final int LSQUARE = 5;
    public static final int RSQUARE = 6;
    public static final int LCURLY = 7;
    public static final int RCURLY = 8;
    public static final int BOOLEAN = 9;
    public static final int NUMBER = 10;
    public static final int STRING = 11;
    // ...

    // For easy access
    private static final Set<Integer> typeSet;
    static {
        typeSet = new HashSet<Integer>();
        typeSet.add(EOF);
        typeSet.add(NULL);
        typeSet.add(COMMA);
        typeSet.add(COLON);
        typeSet.add(LSQUARE);
        typeSet.add(RSQUARE);
        typeSet.add(LCURLY);
        typeSet.add(RCURLY);
        typeSet.add(BOOLEAN);
        typeSet.add(NUMBER);
        typeSet.add(STRING);
        // ...
    }
    
    public static boolean isValid(int type) 
    {
        return typeSet.contains(type);
    }
    
    
    // for debugging purpose
    public static String getTokenName(int type)
    {
        switch(type) {
        case EOF:
            return "eof";
        case NULL:
            return "null";
        case COMMA:
            return "comma";
        case COLON:
            return "colon";
        case LSQUARE:
            return "l-square";
        case RSQUARE:
            return "r-square";
        case LCURLY:
            return "l-curly";
        case RCURLY:
            return "r-curly";
        case BOOLEAN:
            return "boolean";
        case NUMBER:
            return "number";
        case STRING:
            return "string";
        default:
            return "unknown";
        }
    }
    public static String getDisplayName(int type)
    {
        switch(type) {
        case EOF:
            return "eof";
        case NULL:
            return "null";
        case COMMA:
            return "comma";
        case COLON:
            return "colon";
        case LSQUARE:
            return "left square bracket";
        case RSQUARE:
            return "right square bracket";
        case LCURLY:
            return "left curly brace";
        case RCURLY:
            return "right curly brace";
        case BOOLEAN:
            return "boolean";
        case NUMBER:
            return "number";
        case STRING:
            return "string";
        default:
            return "unknown type";
        }
    }

}
