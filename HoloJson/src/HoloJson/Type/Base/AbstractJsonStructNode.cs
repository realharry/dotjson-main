using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HoloJson.Type.Base
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


        public override string ToJsonString()
        {
            return ToJsonString(DEFAULT_INDENT);
        }

        public override string ToJsonString(int indent)
        {
            StringWriter writer = new StringWriter();
            try {
                WriteJsonString(writer, indent);
            } catch (IOException e) {
                // What to do???
                // log.log(Level.WARNING, "Failed to write to StringWriter.", e);
                return null;
            }
            String str = writer.ToString();
            return str;
        }

        public override void WriteJsonString(TextWriter writer)
        {
            WriteJsonString(writer, DEFAULT_INDENT);
        }

        public abstract override void WriteJsonString(TextWriter writer, int indent);


        public object ToJsonStructure()
        {
            return ToJsonStructure(DRILL_DOWN_DEPTH);
        }

        public abstract object ToJsonStructure(int depth);

    }
}
