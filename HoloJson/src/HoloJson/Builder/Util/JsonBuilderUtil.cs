using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HoloJson.Builder.Util
{
    // Place holder, for now.
    public sealed class JsonBuilderUtil
    {
        // Static methods only.
        private JsonBuilderUtil()
        {
        }

        // This has been moved to IndentInfoStruct.
        // ...

        //    // Note that in order to make the public API simpler,
        //    //   we overload the "indent:int" variable for at least two purposes.
        //    // If indent == -1, then compact - no line breaks
        //    // If indent == 0, no compact - no line break
        //    // If indent > 0, no compact - line breaks -indentation: indent.
        //
        //
        //    // If false, no space will be added to json string.
        //    public static boolean includeWhiteSpaces(int indent)
        //    {
        //        if(indent < 0) {
        //            return false;
        //        } else {
        //            return true;
        //        }
        //    }
        //
        //    // If true, line breaks will be added to json string. 
        //    public static boolean includeLineBreaks(int indent)
        //    {
        //        if(indent <= 0) {
        //            return true;
        //        } else {
        //            return false;
        //        }
        //    }
        //
        //    // Returns the indentation/tab unit.
        //    public static int getIndentSize(int indent)
        //    {
        //        if(indent <= 0) {
        //            return 0;
        //        } else {
        //            return indent;
        //        }
        //    }

    }

}