using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DotJson.Parser
{
    // TBD:
    public interface ObjectJsonParser : JsonParser
    {
        //Task<object> ParseAsync(string jsonStr, ObjectConverter converter);
        //Task<object> ParseAsync(TextReader reader, ObjectConverter converter);
    }

}