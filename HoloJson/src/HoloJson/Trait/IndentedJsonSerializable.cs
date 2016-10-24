using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HoloJson.Trait
{
    /// <summary>
    /// JsonSerializable represents the "opposite" of JsonParseable.
    /// IndentedJsonSerializable defines serialize methods with indentations.
    /// </summary>
    public interface IndentedJsonSerializable : JsonSerializable
    {
        // TBD:
        // Move "indent" options to BuilderPolicy ????

        /// <summary>
        /// Creates a JSON string from this object.
        /// </summary>
        /// <param name="indent">Indent level for "pretty printing".</param>
        /// <returns>Returns the JSON string.</returns>
        Task<string> ToJsonStringAsync(int indent);


        /// <summary>
        /// Writes a JSON string representation of this object to the writer. 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="indent"></param>
        Task WriteJsonStringAsync(TextWriter writer, int indent);

        // String toJsonString(JsonCompatible jsonObj);
        // String toJsonString(Object obj);
    }
}
