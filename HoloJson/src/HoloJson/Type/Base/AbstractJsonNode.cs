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

        public abstract string ToJsonString();

        public abstract string ToJsonString(int indent);

        public abstract void WriteJsonString(TextWriter writer);

        public abstract void WriteJsonString(TextWriter writer, int indent);

    }
}
