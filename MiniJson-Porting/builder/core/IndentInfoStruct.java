package org.minijson.builder.core;


/**
 * Note that in order to make the public API simpler,
 *   we overload the "indent:int" variable for at least two purposes.
 * If indent == -1, then compact - no line breaks
 * If indent == 0, no compact - no line break
 * If indent > 0, no compact - line breaks - indentation: indent.
 * ("compact" means no spaces between JSON tokens.)
 */
public class IndentInfoStruct
{
    private final boolean includingWhiteSpaces;
    private final boolean includingLineBreaks;
    private final boolean lineBreakingAfterComma;
    private final int indentSize;
    
    public IndentInfoStruct(int indent)
    {
        includingWhiteSpaces = includeWhiteSpaces(indent);
        includingLineBreaks = includeLineBreaks(indent);
        lineBreakingAfterComma = lineBreakAfterComma(indent);
        indentSize = getIndentSize(indent);
    }

    public boolean isIncludingWhiteSpaces()
    {
        return includingWhiteSpaces;
    }
    public boolean isIncludingLineBreaks()
    {
        return includingLineBreaks;
    }
    public boolean isLineBreakingAfterComma()
    {
        return lineBreakingAfterComma;
    }
    public int getIndentSize()
    {
        return indentSize;
    }


    // If false, no space will be added to json string.
    private static boolean includeWhiteSpaces(int indent)
    {
        if(indent < 0) {
            return false;
        } else {
            return true;
        }
    }

    // If true, line breaks will be added after opening/closing braces. 
    private static boolean includeLineBreaks(int indent)
    {
        if(indent > 0) {
            return true;
        } else {
            return false;
        }
    }

    // If true, line breaks will be added after ",". 
    private static boolean lineBreakAfterComma(int indent)
    {
        if(indent >= 3) {     // Arbitrary cutoff.
            return true;
        } else {
            return false;
        }
    }
    
    

    // Returns the indentation/tab unit.
    private static int getIndentSize(int indent)
    {
        if(indent < 0) {
            return 0;
        } else {
            return indent;
        }
    }


}
