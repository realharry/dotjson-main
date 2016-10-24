using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HoloJson.Type.Base
{
    public abstract class AbstractJsonNode : JsonNode
    {
        protected AbstractJsonNode()
        {
        }


        // public abstract object GetValue();
        public abstract object Value { get; set; }

        public abstract Task<string> ToJsonStringAsync();

        public abstract Task<string> ToJsonStringAsync(int indent);

        public abstract Task WriteJsonStringAsync(TextWriter writer);

        public abstract Task WriteJsonStringAsync(TextWriter writer, int indent);

    }
}
