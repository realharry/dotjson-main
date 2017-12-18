using DotJson.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DotJson.Type.Base
{
    public class AbstractJsonNullNode : AbstractJsonLeafNode, JsonNullNode
    {
        public static readonly AbstractJsonNullNode NULL = new AbstractJsonNullNode();

        public AbstractJsonNullNode()
        {
        }


        ///////////////////////////////////
        // JsonNode interface

        //public override object GetValue()
        //{
        //    return null;
        //}

        public override object Value
        {
            get
            {
                return null;
            }
            set
            {
                // no op.
            }
        }


        ///////////////////////////////////
        // JsonSerializable interface

        public override async Task<string> ToJsonStringAsync(int indent)
        {
            return Literals.NULL;
        }

        // ???
        public override async Task WriteJsonStringAsync(TextWriter writer, int indent)
        {
            writer.Write(Literals.NULL);
        }

        // For debugging.
        public override string ToString()
        {
            return $"{nameof(AbstractJsonNullNode)}";
        }

    }
}
