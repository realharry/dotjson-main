using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DotJson.Parser.Partial
{
    /// <summary>
    /// LayeredJsonParser is a "partial" json parser.
    /// While parsing a json string, if it reaches the specified depth,
    ///     rather than continuing to parse the child elements/nodes,
    ///     it just returns the partial json string representing the node. 
    /// </summary>
    public interface LayeredJsonParser : JsonParser
    {
        // TBD:
        Task<object> ParseAsync(string jsonStr, int depth);
        Task<object> ParseAsync(TextReader reader, int depth);
    }

}