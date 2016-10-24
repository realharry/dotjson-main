using HoloJson.Type;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HoloJson.Parser
{
    /// <summary>
    /// JsonParser which parses the given JSON string and builds a JsonNode.
    /// In contrast, BareJsonParser returns an object comprising Java Maps and Lists.
    /// </summary>
    public interface RichJsonParser : JsonParser
    {
        Task<JsonNode> ParseJsonAsync(string jsonStr);
        Task<JsonNode> ParseJsonAsync(TextReader reader);
        // JsonObject parseObject();
        // JsonArray parseArray();
    }

}