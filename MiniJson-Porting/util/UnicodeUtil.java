package org.minijson.util;

import org.minijson.common.CyclicCharArray;


/**
 * Unicode-related utility functions.
 */
public final class UnicodeUtil
{
    private UnicodeUtil() {}

    public static boolean isUnicodeHex(char ch)
    {
        if((ch >= 'A' && ch <= 'F') || (ch >= 'a' && ch <= 'f') || (ch >= '0' && ch <= '9')) {
            return true;
        }
//        switch(ch) {
//        case '0':
//        case '1':
//        case '2':
//        case '3':
//        case '4':
//        case '5':
//        case '6':
//        case '7':
//        case '8':
//        case '9':
//        case 'a': case 'A':
//        case 'b': case 'B':
//        case 'c': case 'C':
//        case 'd': case 'D':
//        case 'e': case 'E':
//        case 'f': case 'F':
//            return true;
//        }
        return false;
    }
    public static int getIntEquivalent(char ch)
    {
        if(ch >= '0' && ch <= '9') {
            return ((int) ch) - 48;            
        } else if(ch >= 'A' && ch <= 'F') {
            return ((int) ch) - 55;    // 65 - 10
        } else if(ch >= 'a' && ch <= 'f') {
            return ((int) ch) - 87;    // 97 - 10
        }
//        switch(ch) {
//        case '0':
//        case '1':
//        case '2':
//        case '3':
//        case '4':
//        case '5':
//        case '6':
//        case '7':
//        case '8':
//        case '9':
//            return ((int) ch) - 48;
//        case 'a':
//        case 'b':
//        case 'c':
//        case 'd':
//        case 'e':
//        case 'f':
//            return ((int) ch) - 87;    // 97 - 10
//        case 'A':
//        case 'B':
//        case 'C':
//        case 'D':
//        case 'E':
//        case 'F':
//            return ((int) ch) - 55;    // 65 - 10
//        }
        return 0;
    }
    
    public static char getUnicodeChar(CyclicCharArray hex)
    {
        if(hex == null || hex.getLength() != 4) {
            // ???
            return 0;
        }
        // return getUnicodeCharNoCheck(hex.getArray());
        return getUnicodeCharNoCheck(hex);
    }

    public static char getUnicodeChar(char[] hex)
    {
        if(hex == null || hex.length != 4) {
            // ???
            return 0;
        }
        return getUnicodeCharNoCheck(hex);
    }


    public static char getUnicodeCharNoCheck(CyclicCharArray hex)
    {
        char u = getUnicodeCharFromHexSequence(hex);
        return u;
    }

//    public static char getUnicodeCharNoCheck(char[] hex)
//    {
//        // TBD:
//        // Is there a better way???
//        String str = new String(hex);
//        char u = 0;
//        // try {
//            u = (char) Integer.parseInt(str, 16);
//        // } catch(NumberFormatException e) {
//        //     // ???
//        //     if(log.isLoggable(Level.INFO)) log.log(Level.INFO, "Invalid unicode hex sequence: " + str, e);
//        // }
//        return u;
//    }
    public static char getUnicodeCharNoCheck(char[] hex)
    {
        char u = getUnicodeCharFromHexSequence(hex);
        return u;
    }

    
    
    // TBD:
    // Need to verify this really works...
    public static char getUnicodeCharFromHexSequence(char[] c)
    {
        // int x = ((getIntEquivalent(c[0]) << 12) + ((getIntEquivalent(c[1]) << 8) + ((getIntEquivalent(c[2]) << 4) + (getIntEquivalent(c[3]);

        int x = 0;
        for(int i=0; i<4; i++) {
            if(isUnicodeHex(c[i])) {
                x += (getIntEquivalent(c[i]) << (3-i)*4);
            } else {
                // ???
                return 0;
            }
        }

        return (char) x;
    }
    public static char getUnicodeCharFromHexSequence(CyclicCharArray hex)
    {
        int x = 0;
        for(int i=0; i<4; i++) {
            if(isUnicodeHex(hex.getChar(i))) {
                x += (getIntEquivalent(hex.getChar(i)) << (3-i)*4);
            } else {
                // ???
                return 0;
            }
        }
        return (char) x;
    }


    private static char[] HEXNUM = "0123456789abcdef".toCharArray();
    public static char[] getUnicodeHexCodeFromChar(char ch) 
    {
//        char[] c6 = new char[6];
//        c6[0] = '\\';
//        c6[1] = 'u';
        char[] c6 = new char[]{'\\', 'u', '0', '0', '0', '0'};

        int code = (int) ch;
        c6[2] = HEXNUM[(code & 0xf000) >> 12];
        c6[3] = HEXNUM[(code & 0x0f00) >> 8];
        c6[4] = HEXNUM[(code & 0x00f0) >> 4];
        c6[5] = HEXNUM[(code & 0x000f)];
//        for (int i = 0; i < 4; ++i) {
//            int digit = (code & 0xf000) >> 12;
//            c6[i+2] = HEXNUM[digit];
//            code <<= 4;
//        }

        return c6;
    }

}
