using HoloJson.Parser.Policy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HoloJson.Parser
{
    /// <summary>
    /// "Flexible" (configurable) Json tokenizer.
    /// </summary>
    public interface FlexibleJsonTokenizer : JsonTokenizer
    {
        /// <summary>
        /// Returns the parserPolicy. </summary>
        /// <returns> The ParserPolciy objects associated with JsonParser. </returns>
        ParserPolicy ParserPolicy { get; }
    }

}