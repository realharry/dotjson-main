using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HoloJson.Parser
{
    /// <summary>
    /// Json Parser: Creates an Object from the given JSON string.
    /// </summary>
    public interface JsonParser
    {
        /// <summary>
        /// Parses the given JSON string. </summary>
        /// <param name="jsonStr"> </param>
        /// <returns> Object corresponding to the given JSON string. </returns>
        /// <exception cref="JsonParserException"> </exception>
        Task<object> ParseAsync(string jsonStr);

        /// <summary>
        /// Parses the JSON string from the given TextReader. </summary>
        /// <param name="reader"> </param>
        /// <returns> Object corresponding to the read JSON string. </returns>
        /// <exception cref="JsonParserException"> </exception>
        /// <exception cref="IOException"> </exception>
        Task<object> ParseAsync(TextReader reader);
    }

}