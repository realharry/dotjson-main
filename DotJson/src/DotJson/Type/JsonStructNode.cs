using DotJson.Trait;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotJson.Type
{
    // Parent node for JsonObjectNode and JsonArrayNode.
    public interface JsonStructNode : JsonNode, JsonCompatible
    {
    }
}
