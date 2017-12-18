using DotJson.Builder.Policy;
using DotJson.Trait;
using DotJson.Type;
using DotJson.Type.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DotJson.Builder.Impl
{
    // work in progress....
    // Not implemented. 
    // Do not use this class.
    /* public */ abstract class AbstractRichJsonBuilder : RichJsonBuilder, FlexibleJsonBuilder
    {
        // TBD:
        // "print/format type" ???
        // e.g., compact, normal, indented, etc.
        // ...

        // "strategy" for building json structure.
        private BuilderPolicy builderPolicy = null;


        public AbstractRichJsonBuilder()
        {
        }


        public BuilderPolicy BuilderPolicy
        {
            get
            {
                return this.builderPolicy;
            }
            set
            {
                this.builderPolicy = value;
            }
        }


        public async Task<string> BuildAsync(object jsonObj)
        {
            if (jsonObj is JsonNode) {
                return await _buildAsync((JsonNode)jsonObj);
            } else {
                return await _buildAsync(jsonObj, 0);
            }
        }

        public async Task<string> BuildAsync(object jsonObj, int indent)
        {
            // TODO Auto-generated method stub
            return null;
        }

        public async Task<string> BuildJsonAsync(JsonNode node)
        {
            return await _buildAsync(node);
        }

        public async Task<string> BuildJsonAsync(JsonNode node, int indent)
        {
            // TODO Auto-generated method stub
            return null;
        }

        public async Task BuildJsonAsync(TextWriter writer, JsonNode node)
        {
            // TODO Auto-generated method stub

        }
        public async Task BuildJsonAsync(TextWriter writer, JsonNode node, int indent)
        {
            // TODO Auto-generated method stub

        }


        public async Task BuildAsync(TextWriter writer, object jsonObj)
        {
            // TBD:
        }
        public async Task BuildAsync(TextWriter writer, object jsonObj, int indent)
        {
            // TBD:
        }

        public async Task<object> BuildJsonStructureAsync(object jsonObj)
        {
            // TODO Auto-generated method stub
            return null;
        }


        public async Task<object> BuildJsonStructureAsync(object jsonObj, int depth)
        {
            // TODO Auto-generated method stub
            return null;
        }



        // The following does not make sense...
        private async Task<string> _buildAsync(object node, int indent)
        {
            if (node == null) {
                return null;
            }
            string jsonStr = null;
            // if(node instanceof java.util.Map<?,?>)
            if (node is IDictionary<string, object>) {
                JsonObjectNode jo = new AbstractJsonObjectNode((IDictionary<string, object>)node);
                jsonStr = await BuildJsonAsync(jo);
            }
            // else if(node instanceof java.util.List<?>)
            else if (node is IList<object>) {
                JsonArrayNode ja = new AbstractJsonArrayNode((IList<object>)node);
                jsonStr = await BuildJsonAsync(ja);
            } else if (node.GetType().IsArray) {
                // ????
                var arr = node as object[];
                if (arr != null) {
                    JsonArrayNode ja = new AbstractJsonArrayNode(arr.ToList());
                    jsonStr = await BuildJsonAsync(ja);
                }
                // ????
            } else {
                // ???
                if (node is IndentedJsonSerializable) {
                    jsonStr = await ((IndentedJsonSerializable)node).ToJsonStringAsync();
                } else {
                    // ????
                    jsonStr = node.ToString();
                }
            }
            return jsonStr;
        }

        private async Task<string> _buildAsync(JsonNode node)
        {
            string jStr = await node.ToJsonStringAsync();
            return jStr;
        }



    }

}