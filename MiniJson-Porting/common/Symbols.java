package org.minijson.common;

import java.util.HashSet;
import java.util.Set;
import java.util.logging.Logger;


/**
 * "Special" chars for tokenizer.
 */
public final class Symbols
{
    private static final Logger log = Logger.getLogger(Symbols.class.getName());

    private Symbols() {}

    // Chars that have special meanings to the tokenizer...
    public static final char COMMA = ',';
    public static final char COLON = ':';
    public static final char LSQUARE = '[';
    public static final char RSQUARE = ']';
    public static final char LCURLY = '{';
    public static final char RCURLY = '}';
    // ...
    public static final char DQUOTE = '"';
    // ...
    public static final String LSQUARE_STR = "[";
    public static final String LCURLY_STR = "{";
    // ...
    public static final char BACKSLASH = '\\';
    // ...
    public static final char SLASH = '/';
    public static final char BACKSPACE = 'b';
    public static final char FORMFEED = 'f';
    public static final char NEWLINE = 'n';
    public static final char RETURN = 'r';
    public static final char TAB = 't';
    // ...
    public static final char PLUS = '+';    // Or, dash.
    public static final char MINUS = '-';    // Or, dash.
    public static final char PERIOD = '.';   // Or, dot, decimal point.
    public static final char EXP_LOWER = 'e';    // for numbers.
    public static final char EXP_UPPER = 'E';    // for numbers.
    // ...
    public static final char ESCAPED_DQUOTE = '\"';
    public static final char ESCAPED_BACKSLASH = '\\';
    // public static final char ESCAPED_SLASH = '/';
    public static final char ESCAPED_BACKSPACE = '\b';
    public static final char ESCAPED_FORMFEED = '\f';
    public static final char ESCAPED_NEWLINE = '\n';
    public static final char ESCAPED_RETURN = '\r';
    public static final char ESCAPED_TAB = '\t';
    // ...
    public static final String ESCAPED_DQUOTE_STR = "\\\"";
    public static final String BACKSLASH_STR = "\\";
    public static final String ESCAPED_BACKSLASH_STR = "\\\\";
    // Note that forward slash need not be escaped in Java.
    // The following is used to generate "\/" in a json string.
    public static final String ESCAPED_SLASH_STR = "\\/";
    public static final String SLASH_STR = "/";
    public static final String ESCAPED_BACKSPACE_STR = "\\b";
    public static final String ESCAPED_FORMFEED_STR = "\\f";
    public static final String ESCAPED_NEWLINE_STR = "\\n";
    public static final String ESCAPED_RETURN_STR = "\\r";
    public static final String ESCAPED_TAB_STR = "\\t";
    // ...

    // ...
    public static final char NULL_START = 'n';
    public static final char TRUE_START = 't';
    public static final char FALSE_START = 'f';
    // these are used only when parserPolicy.caseInsensitiveLiterals == true...
    public static final char NULL_START_UPPER = 'N';
    public static final char TRUE_START_UPPER = 'T';
    public static final char FALSE_START_UPPER = 'F';
    // ...
    public static final char UNICODE_PREFIX = 'u';
    // ...
    // ...
    // number?
    // ...

    // For easy access
    private static final Set<Character> symbolSet;
    static {
        symbolSet = new HashSet<Character>();
        symbolSet.add(COMMA);
        symbolSet.add(COLON);
        symbolSet.add(LSQUARE);
        symbolSet.add(RSQUARE);
        symbolSet.add(LCURLY);
        symbolSet.add(RCURLY);
        symbolSet.add(DQUOTE);
        // etc...
        // ...
    }

    private static final Set<Character> escapables;
    static {
        escapables = new HashSet<Character>();
        escapables.add(DQUOTE);
        escapables.add(BACKSLASH);
        escapables.add(SLASH);
        escapables.add(BACKSPACE);
        escapables.add(FORMFEED);
        escapables.add(NEWLINE);
        escapables.add(RETURN);
        escapables.add(TAB);
        escapables.add(UNICODE_PREFIX);    // ???
    }
    
//    private static final Map<Character, Character> escapedCharMap;
//    static {
//        escapedCharMap = new HashMap<>();
//        escapedCharMap.put(DQUOTE, ESCAPED_DQUOTE);
//        escapedCharMap.put(BACKSLASH, BACKSLASH);
//        escapedCharMap.put(SLASH, SLASH);
//        escapedCharMap.put(BACKSPACE, ESCAPED_BACKSPACE);
//        escapedCharMap.put(FORMFEED, ESCAPED_FORMFEED);
//        escapedCharMap.put(NEWLINE, ESCAPED_NEWLINE);
//        escapedCharMap.put(RETURN, ESCAPED_RETURN);
//        escapedCharMap.put(TAB, ESCAPED_TAB);
//    }

    private static final Set<Character> escaped;
    static {
        escaped = new HashSet<Character>();
        escaped.add(ESCAPED_DQUOTE);
        escaped.add(ESCAPED_BACKSLASH);
        // ???
        // escaped.add(ESCAPED_SLASH);
        escaped.add(SLASH);
        // ???
        escaped.add(ESCAPED_BACKSPACE);
        escaped.add(ESCAPED_FORMFEED);
        escaped.add(ESCAPED_NEWLINE);
        escaped.add(ESCAPED_RETURN);
        escaped.add(ESCAPED_TAB);
    }
    
    // ???
    // Is this being used???
    public static boolean isValidSymbol(char symbol) 
    {
        return symbolSet.contains(symbol);
    }

    public static boolean isEscapableChar(char ch) 
    {
        // return escapables.contains(ch);
        
        switch(ch) {
        case DQUOTE:
        case BACKSLASH:
        case SLASH:
        case BACKSPACE:
        case FORMFEED:
        case NEWLINE:
        case RETURN:
        case TAB:
        case UNICODE_PREFIX:    // ???
           return true;     
        }
        return false;
    }

    public static char getEscapedChar(char ch)
    {
//        if(escapedCharMap.containsKey(ch)) {
//            return escapedCharMap.get(ch);
//        } else {
//            // ???
//            return 0;
//        }
        
        switch(ch) {
        case DQUOTE:
            return ESCAPED_DQUOTE;
        case BACKSLASH:
            // return BACKSLASH;
            return ESCAPED_BACKSLASH;
        // Note that during parsing, we do not escape slash.
        // That is, if string has "...\/...", then is converted to ".../...".
        case SLASH:
            return SLASH;
        case BACKSPACE:
            return ESCAPED_BACKSPACE;
        case FORMFEED:
            return ESCAPED_FORMFEED;
        case NEWLINE:
            return ESCAPED_NEWLINE;
        case RETURN:
            return ESCAPED_RETURN;
        case TAB:
            return ESCAPED_TAB;
        }
        // ???
        return 0;
    }


    public static boolean isEscapedChar(char ch) 
    {
        // return escaped.contains(ch);

        // Note that this is much more efficient than escaped.contains(ch), which requires boxing, etc.
        switch(ch) {
        case ESCAPED_DQUOTE:
        case ESCAPED_BACKSLASH:
        // ???
        // Note we use slash without "escaping".
        // case ESCAPED_SLASH:
        case SLASH:
        case ESCAPED_BACKSPACE:
        case ESCAPED_FORMFEED:
        case ESCAPED_NEWLINE:
        case ESCAPED_RETURN:
        case ESCAPED_TAB:
            return true;
        }

        return false;    
    }
    public static String getEscapedCharString(char ch)
    {
        return getEscapedCharString(ch, false);
    }
    public static String getEscapedCharString(char ch, boolean escapeForwardSlash)
    {
        switch(ch) {
        case ESCAPED_DQUOTE:
            return ESCAPED_DQUOTE_STR;
        case ESCAPED_BACKSLASH:
            // ????
            return ESCAPED_BACKSLASH_STR;
            // return BACKSLASH_STR;
        // ????
        // case ESCAPED_SLASH:                          // ????
        //     return SLASH_STR;
        //     // return ESCAPED_SLASH_STR;
        // For JSON generation, slash may be converted to "\/" if escapeForwardSlash==true.
        // Note that, at a JsonBuilder level, "</" is always converted to "<\/" regardless of this flag.
        case SLASH:
            if(escapeForwardSlash) {
                return ESCAPED_SLASH_STR;
            } else {
                return SLASH_STR;
            }
        // ????
        case ESCAPED_BACKSPACE:
            return ESCAPED_BACKSPACE_STR;
        case ESCAPED_FORMFEED:
            return ESCAPED_FORMFEED_STR;
        case ESCAPED_NEWLINE:
            return ESCAPED_NEWLINE_STR;
        case ESCAPED_RETURN:
            return ESCAPED_RETURN_STR;
        case ESCAPED_TAB:
            return ESCAPED_TAB_STR;
        }
        // ???
        return null;
    }


    public static boolean isStartingNumber(char ch)
    {
        // We allow numbers like ".123" or "+3" although JSON.org grammar states they should be "0.123" or "3", respectively.
        if(ch == MINUS || 
                ch == PLUS || 
                ch == PERIOD || 
                Character.isDigit(ch)) {   
            return true;
        } else {
            return false;
        }
    }
    
    public static boolean isExponentChar(char ch)
    {
        switch(ch) {
        case EXP_LOWER:
        case EXP_UPPER:
            return true;
        default:
            return false;
        }
    }

}
