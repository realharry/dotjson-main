using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HoloJson.Builder
{
    /// <summary>
    /// Builds JSON strings from a given object.
    /// </summary>
    public interface JsonBuilder
    {
        /////////////////////////////////////////////////////////////////
        // The following corresponds to the methods in JsonSerializable

        /// <summary>
        /// Generates a JSON string from the given jsonObj. </summary>
        /// <param name="jsonObj"> </param>
        /// <returns> JSON string representation of the given jsonObj. </returns>
        /// <exception cref="JsonBuilderException"> </exception>
        Task<string> BuildAsync(object jsonObj);

        /// <summary>
        /// Generates a JSON string from the given jsonObj and writes it to the writer. </summary>
        /// <param name="writer"> </param>
        /// <param name="jsonObj"> </param>
        /// <exception cref="IOException"> </exception>
        /// <exception cref="JsonBuilderException"> </exception>
        Task BuildAsync(TextWriter writer, object jsonObj);

    }

}