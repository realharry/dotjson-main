using HoloJson.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HoloJson.Type.Base
{
    public class AbstractJsonNullNode : AbstractJsonLeafNode, JsonNullNode
    {
        public static readonly AbstractJsonNullNode NULL = new AbstractJsonNullNode();

        public AbstractJsonNullNode()
        {
        }


        ///////////////////////////////////
        // JsonNode interface

        public override object GetValue()
        {
            return null;
        }

        ///////////////////////////////////
        // JsonSerializable interface

        public override string ToJsonString(int indent)
        {
            return Literals.NULL;
        }

        // ???
        public override void WriteJsonString(TextWriter writer, int indent)
        {
            throw new NotImplementedException();
        }

        // For debugging.
        public override string ToString()
        {
            return $"{nameof(AbstractJsonNullNode)}";
        }

    }
}
