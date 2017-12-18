using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotJson.Type
{
    // Represents "key:value" member of a JsonObject.
    // Not a JsonNode ????
    public interface JsonObjectMember // : JsonNode
    {
        // string GetKey();
        // JsonNode GetValue();
        string Key { get; }
        JsonNode Value { get; }
    }
}
