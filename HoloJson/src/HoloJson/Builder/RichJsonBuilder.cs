using HoloJson.Type;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HoloJson.Builder
{
    /// <summary>
    /// JSON builder using internal JSON node variables.
    /// In contract, BareJsonBuilder only uses JDK Map and List.
    /// </summary>
    public interface RichJsonBuilder : IndentedJsonBuilder, JsonStructureBuilder
    {
        Task<string> BuildJsonAsync(JsonNode node);
        Task<string> BuildJsonAsync(JsonNode node, int indent);

        Task BuildJsonAsync(TextWriter writer, JsonNode node);
        Task BuildJsonAsync(TextWriter writer, JsonNode node, int indent);
    }

}