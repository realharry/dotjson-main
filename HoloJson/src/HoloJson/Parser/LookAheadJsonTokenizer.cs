using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HoloJson.Parser
{
    /// <summary>
    /// "Look ahead" tokenizing.
    /// In the current implementation, "look ahead" has a particular meaning.
    /// </summary>
    public interface LookAheadJsonTokenizer : JsonTokenizer
    {
        // TBD: Need a better name for this interface...

        bool LookAheadParsing { get; }
        //    void setLookAheadParsing(boolean lookAheadParsing);
        void EnableLookAheadParsing();
        void DisableLookAheadParsing();

    }

}