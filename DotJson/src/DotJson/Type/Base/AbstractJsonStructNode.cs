using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DotJson.Type.Base
{
    public abstract class AbstractJsonStructNode : AbstractJsonNode, JsonStructNode
    {
        // Note: The default depth of AbstractJsonStructNodes is always 1.   
        private const int DRILL_DOWN_DEPTH = 1;

        // For all AbstractJsonStructNodes, indent is always 1 by default;
        private const int DEFAULT_INDENT = 0;


        protected AbstractJsonStructNode()
        {
        }


        public override async Task<string> ToJsonStringAsync()
        {
            return await ToJsonStringAsync(DEFAULT_INDENT);
        }

        public override async Task<string> ToJsonStringAsync(int indent)
        {
            StringWriter writer = new StringWriter();
            try {
                await WriteJsonStringAsync(writer, indent);
            } catch (IOException e) {
                // What to do???
                // log.log(Level.WARNING, "Failed to write to StringWriter.", e);
                return null;
            }
            String str = writer.ToString();
            return str;
        }

        public override async Task WriteJsonStringAsync(TextWriter writer)
        {
            await WriteJsonStringAsync(writer, DEFAULT_INDENT);
        }

        public abstract override Task WriteJsonStringAsync(TextWriter writer, int indent);


        public async Task<object> ToJsonStructureAsync()
        {
            return await ToJsonStructureAsync(DRILL_DOWN_DEPTH);
        }

        public abstract Task<object> ToJsonStructureAsync(int depth);

    }
}
