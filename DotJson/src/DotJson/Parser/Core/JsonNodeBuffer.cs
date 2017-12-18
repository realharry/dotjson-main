using DotJson.Core;
using DotJson.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotJson.Parser.Core
{
    /// <summary>
    /// A "tail buffer". It keeps the finite number of objects that have been added last.
    /// Implemented by a ring buffer.
    /// JsonNodeBuffer can be used to keep the "last X objects that have been read" while reading an object stream. 
    /// (Note: the implementation is not thread-safe.)
    /// </summary>
    public sealed class JsonNodeBuffer : TailRingBuffer<JsonNode>
    {
        //    // temporary
        //    private static final int MAX_BUFFER_SIZE = 4096;
        private const int DEF_BUFFER_SIZE = 32;
        //    private static final int MIN_BUFFER_SIZE = 8;
        //    // ...


        // tbd:
        public JsonNodeBuffer()
            : this(DEF_BUFFER_SIZE)
        {
        }
        // tbd:
        public JsonNodeBuffer(uint capacity)
            : base(capacity)
        {
        }
        public JsonNodeBuffer(uint capacity, IList<JsonNode> c)
            : base(capacity, c)
        {
        }


        public override string ToTraceString()
        {
            //        JsonNode[] nodes = toArray(new JsonNode[]{});
            //        return Arrays.toString(nodes);

            StringBuilder sb = new StringBuilder();
            sb.Append("<<Processed Nodes: ...");
            // var it = base.Buffer.GetEnumerator();
            var it = GetEnumerator();
            while (it.MoveNext()) {
                object node = it.Current;
                object value = null;
                if (node is JsonNode) {
                    value = ((JsonNode)node).Value;
                } else {
                    value = node.ToString();
                }
                string str = "";
                if (value != null) {
                    str = value.ToString();
                    if (str.Length > 16) {
                        str = str.Substring(0, 14) + "..";
                    }
                }
                sb.Append("(").Append(str).Append("), ");
            }
            sb.Append(">>");

            return sb.ToString();
        }



    }

}