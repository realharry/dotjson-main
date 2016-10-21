package org.minijson.util;

import static org.minijson.common.Literals.FALSE_LENGTH;
import static org.minijson.common.Literals.NULL_LENGTH;
import static org.minijson.common.Literals.TRUE_LENGTH;

import java.util.logging.Logger;

import org.minijson.common.CyclicCharArray;


/**
 * Utility functions related to the "JSON literals".
 * E.g., "null", "true", and "false". 
 */
public class LiteralUtil
{
    private static final Logger log = Logger.getLogger(LiteralUtil.class.getName());

    private static final char[] NULL_CHARS = new char[]{'n','u','l','l'};
    private static final char[] TRUE_CHARS = new char[]{'t','r','u','e'};
    private static final char[] FALSE_CHARS = new char[]{'f','a','l','s','e'};
    private static final char[] NULL_CHARS_UPPER = new char[]{'N','U','L','L'};
    private static final char[] TRUE_CHARS_UPPER = new char[]{'T','R','U','E'};
    private static final char[] FALSE_CHARS_UPPER = new char[]{'F','A','L','S','E'};

    private LiteralUtil() {}

    
    // Convenience methods.
    public static boolean isNull(char[] c)
    {
        if(c == null || c.length != NULL_LENGTH) {
            return false;
        } else {
            for(int i=0; i<NULL_LENGTH; i++ ) {
                // if(c[i] != NULL.charAt(i)) {
                if(c[i] != NULL_CHARS[i]) {
                    return false;
                }
            }
        }
        return true;
    }
    public static boolean isNullIgnoreCase(char[] c)
    {
        if(c == null || c.length != NULL_LENGTH) {
            return false;
        } else {
//            String str = new String(c);
//            // Which is better?
//            // return str.toLowerCase().equals(NULL);
//            return str.equalsIgnoreCase(NULL);
            for(int i=0; i<NULL_LENGTH; i++ ) {
                // if(c[i] != NULL.charAt(i)) {
                if((c[i] != NULL_CHARS[i] || c[i] != NULL_CHARS_UPPER[i])) {
                    return false;
                }
            }
        }
        return true;
    }
    public static boolean isTrue(char[] c)
    {
        if(c == null || c.length != TRUE_LENGTH) {
            return false;
        } else {
            for(int i=0; i<TRUE_LENGTH; i++ ) {
                // if(c[i] != TRUE.charAt(i)) {
                if(c[i] != TRUE_CHARS[i]) {
                    return false;
                }
            }
        }
        return true;
    }
    public static boolean isTrueIgnoreCase(char[] c)
    {
        if(c == null || c.length != TRUE_LENGTH) {
            return false;
        } else {
//            String str = new String(c);
//            // Which is better?
//            // return str.toLowerCase().equals(TRUE);
//            return str.equalsIgnoreCase(TRUE);
            for(int i=0; i<TRUE_LENGTH; i++ ) {
                // if(c[i] != TRUE.charAt(i)) {
                if((c[i] != TRUE_CHARS[i] || c[i] != TRUE_CHARS_UPPER[i])) {
                    return false;
                }
            }
        }
        return true;
    }
    public static boolean isFalse(char[] c)
    {
        if(c == null || c.length != FALSE_LENGTH) {
            return false;
        } else {
            for(int i=0; i<FALSE_LENGTH; i++ ) {
                //if(c[i] != FALSE.charAt(i)) {
                if(c[i] != FALSE_CHARS[i]) {
                    return false;
                }
            }
        }
        return true;
    }
    public static boolean isFalseIgnoreCase(char[] c)
    {
        if(c == null || c.length != FALSE_LENGTH) {
            return false;
        } else {
//            String str = new String(c);
//            // Which is better?
//            // return str.toLowerCase().equals(FALSE);
//            return str.equalsIgnoreCase(FALSE);
            for(int i=0; i<FALSE_LENGTH; i++ ) {
                // if(c[i] != FALSE.charAt(i)) {
                if((c[i] != FALSE_CHARS[i] || c[i] != FALSE_CHARS_UPPER[i])) {
                    return false;
                }
            }
        }
        return true;
    }
    
    
    public static boolean isNull(CyclicCharArray c)
    {
        if(c == null || c.getLength() != NULL_LENGTH) {
            return false;
        } else {
            for(int i=0; i<NULL_LENGTH; i++ ) {
                // if(c.getChar(i) != NULL.charAt(i)) {
                if(c.getChar(i) != NULL_CHARS[i]) {
                    return false;
                }
            }
        }
        return true;
    }
    public static boolean isNullIgnoreCase(CyclicCharArray c)
    {
        if(c == null || c.getLength() != NULL_LENGTH) {
            return false;
        } else {
//            String str = new String(c.getArray());
//            // Which is better?
//            // return str.toLowerCase().equals(NULL);
//            return str.equalsIgnoreCase(NULL);
            for(int i=0; i<NULL_LENGTH; i++ ) {
                // if(c[i] != NULL.charAt(i)) {
                if((c.getChar(i) != NULL_CHARS[i] || c.getChar(i) != NULL_CHARS_UPPER[i])) {
                    return false;
                }
            }
        }
        return true;
    }
    public static boolean isTrue(CyclicCharArray c)
    {
        if(c == null || c.getLength() != TRUE_LENGTH) {
            return false;
        } else {
            for(int i=0; i<TRUE_LENGTH; i++ ) {
                // if(c.getChar(i) != TRUE.charAt(i)) {
                if(c.getChar(i) != TRUE_CHARS[i]) {
                    return false;
                }
            }
        }
        return true;
    }
    public static boolean isTrueIgnoreCase(CyclicCharArray c)
    {
        if(c == null || c.getLength() != TRUE_LENGTH) {
            return false;
        } else {
//            String str = new String(c.getArray());
//            // Which is better?
//            // return str.toLowerCase().equals(TRUE);
//            return str.equalsIgnoreCase(TRUE);
            for(int i=0; i<TRUE_LENGTH; i++ ) {
                // if(c[i] != TRUE.charAt(i)) {
                if((c.getChar(i) != TRUE_CHARS[i] || c.getChar(i) != TRUE_CHARS_UPPER[i])) {
                    return false;
                }
            }
        }
        return true;
    }
    public static boolean isFalse(CyclicCharArray c)
    {
        if(c == null || c.getLength() != FALSE_LENGTH) {
            return false;
        } else {
            for(int i=0; i<FALSE_LENGTH; i++ ) {
                //if(c.getChar(i) != FALSE.charAt(i)) {
                if(c.getChar(i) != FALSE_CHARS[i]) {
                    return false;
                }
            }
        }
        return true;
    }
    public static boolean isFalseIgnoreCase(CyclicCharArray c)
    {
        if(c == null || c.getLength() != FALSE_LENGTH) {
            return false;
        } else {
//            String str = new String(c.getArray());
//            // Which is better?
//            // return str.toLowerCase().equals(FALSE);
//            return str.equalsIgnoreCase(FALSE);
            for(int i=0; i<FALSE_LENGTH; i++ ) {
                // if(c[i] != FALSE.charAt(i)) {
                if((c.getChar(i) != FALSE_CHARS[i] || c.getChar(i) != FALSE_CHARS_UPPER[i])) {
                    return false;
                }
            }
        }
        return true;
    }

}
