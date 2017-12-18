using DotJson.Parser.Policy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotJson.Parser
{
    /// <summary>
    /// JsonParser with configurable options.
    /// </summary>
    public interface FlexibleJsonParser : JsonParser
    {
        /// <summary>
        /// Returns the parserPolicy. </summary>
        /// <returns> The ParserPolciy objects associated with JsonParser. </returns>
        ParserPolicy ParserPolicy { get; }
    }

}