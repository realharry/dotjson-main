using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotJson.Builder.Core
{
    /// <summary>
    /// Note that in order to make the public API simpler,
    ///   we overload the "indent:int" variable for at least two purposes.
    /// If indent == -1, then compact - no line breaks
    /// If indent == 0, no compact - no line break
    /// If indent > 0, no compact - line breaks - indentation: indent.
    /// ("compact" means no spaces between JSON tokens.)
    /// </summary>
    public struct IndentInfoStruct
    {
        private readonly bool includingWhiteSpaces;
        private readonly bool includingLineBreaks;
        private readonly bool lineBreakingAfterComma;
        private readonly int indentSize;

        public IndentInfoStruct(int indent)
        {
            includingWhiteSpaces = includeWhiteSpaces(indent);
            includingLineBreaks = includeLineBreaks(indent);
            lineBreakingAfterComma = lineBreakAfterComma(indent);
            indentSize = getIndentSize(indent);
        }

        public bool IsIncludingWhiteSpaces
        {
            get
            {
                return includingWhiteSpaces;
            }
        }
        public bool IsIncludingLineBreaks
        {
            get
            {
                return includingLineBreaks;
            }
        }
        public bool IsLineBreakingAfterComma
        {
            get
            {
                return lineBreakingAfterComma;
            }
        }
        public int IndentSize
        {
            get
            {
                return indentSize;
            }
        }


        // If false, no space will be added to json string.
        private static bool includeWhiteSpaces(int indent)
        {
            if (indent < 0) {
                return false;
            } else {
                return true;
            }
        }

        // If true, line breaks will be added after opening/closing braces. 
        private static bool includeLineBreaks(int indent)
        {
            if (indent > 0) {
                return true;
            } else {
                return false;
            }
        }

        // If true, line breaks will be added after ",". 
        private static bool lineBreakAfterComma(int indent)
        {
            if (indent >= 3) {     // Arbitrary cutoff.
                return true;
            } else {
                return false;
            }
        }


        // Returns the indentation/tab unit.
        private static int getIndentSize(int indent)
        {
            if (indent < 0) {
                return 0;
            } else {
                return indent;
            }
        }

    }
}
