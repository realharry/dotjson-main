using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotJson.Type
{
    public interface JsonObjectNode : JsonStructNode, IDictionary<string, object>
    {
        bool HasMembers();
        ISet<JsonObjectMember> GetMembers();

        JsonNode GetMemberNode(string key);

        void AddMember(JsonObjectMember member);
        void AddAllMembers(ISet<JsonObjectMember> members);
        // JsonNode Put(string key, JsonNode value);
        // JsonNode PutAll(IDictionary<string,JsonNode> m);
    }
}
