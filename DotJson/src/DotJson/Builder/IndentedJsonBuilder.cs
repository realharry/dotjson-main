using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DotJson.Builder
{
    /// <summary>
    /// JsonBuilder with "indent" options.
    /// See the comment for the class, IndentInfoStruct.
    /// </summary>
    public interface IndentedJsonBuilder : JsonBuilder
    {
        /// <summary>
        /// Creates a json string from the given jsonObj. </summary>
        /// <param name="jsonObj"> </param>
        /// <param name="indent"> Indent level for "pretty printing" </param>
        /// <returns> JSON String representation of the given object. </returns>
        /// <exception cref="JsonBuilderException"> </exception>
        Task<string> BuildAsync(object jsonObj, int indent);

        /// <summary>
        /// Writes a json string to the writer from the given jsonObj. </summary>
        /// <param name="writer"> </param>
        /// <param name="jsonObj"> </param>
        /// <param name="indent"> Indent level for "pretty printing" </param>
        /// <exception cref="IOException"> </exception>
        /// <exception cref="JsonBuilderException"> </exception>
        Task BuildAsync(TextWriter writer, object jsonObj, int indent);

    }

}