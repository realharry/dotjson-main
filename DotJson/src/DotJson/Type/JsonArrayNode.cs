using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotJson.Type
{
    public interface JsonArrayNode : JsonStructNode, IList<object>
    {
        bool HasChildren();
        IList<JsonNode> GetChildren();

        JsonNode GetChildNode(int index);

        void AddChild(JsonNode child);
        void AddAllChildren(IList<JsonNode> children);
    }
}
