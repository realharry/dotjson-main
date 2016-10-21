using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HoloJson.Trait
{
    // Note that if an object implements both JsonSerializable and JsonCompatible.
    // They should be "consistent".
    // The structure generated using the default depth of JsonCompatible.toJsonStructure(),
    //    should be compatible with the json string returned by toJsonString().

    public interface JsonSerializable
    {
        /// <summary>
        /// Creates a JSON string from this object.
        /// </summary>
        /// <returns>JSON string.</returns>
        string ToJsonString();

        /// <summary>
        /// Writes a JSON string representation of this object to the writer.
        /// </summary>
        /// <param name="writer">Target writer.</param>
        void WriteJsonString(TextWriter writer);
    }
}
