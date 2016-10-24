using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HoloJson.Type.Base
{
    public abstract class AbstractJsonLeafNode : AbstractJsonNode, JsonLeafNode
    {
        // Indent level is irrelevant for all AbstractJsonLeafNodes. 
        // But, just to be consistent, we are setting AbstractJsonLeafNodes's indent to 0;
        private const int DEFAULT_INDENT = 0;

        protected AbstractJsonLeafNode()
        {
        }

        public override async Task<string> ToJsonStringAsync()
        {
            return await ToJsonStringAsync(DEFAULT_INDENT);
        }

        // public abstract override string ToJsonString(int indent);

        public override async Task WriteJsonStringAsync(TextWriter writer)
        {
            await WriteJsonStringAsync(writer, DEFAULT_INDENT);
        }

        // public abstract override void WriteJsonString(TextWriter writer, int indent);

    }
}
