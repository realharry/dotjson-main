package org.minijson.util;


/**
 * Character-related utility functions.
 */
public final class CharacterUtil
{
    private CharacterUtil() {}


    /**
     * Returns true if the char is a "white space" char.
     * (Note: http://en.wikipedia.org/wiki/Whitespace_character)
     * @param ch
     * @return whether it is a white space char 
     */
    public static boolean isWhitespace(char ch)
    {
        // return Character.isWhitespace(ch);
        
        switch(ch) {
        case ' ':
        case '\t':
        case '\n':
        case '\r':
        // what else?
            return true;
        }
        return false;        
    }

    /**
     * Returns true if the char is a "control character".
     * @param ch
     * @return whether it is a control character.
     */
    public static boolean isISOControl(char ch)
    {
        // return Character.isISOControl(ch);
        
        int code = (int) ch;
        if((code >= 0x0 && code <= 0x1f) || (code >= 0x7f && code <= 0x9f)) {
            return true;
        } else {
            return false;
        }
    }

}
