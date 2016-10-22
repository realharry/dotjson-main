using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HoloJson.Type.Base
{
    public class AbstractJsonStringNode : AbstractJsonLeafNode, JsonStringNode
    {
        public static readonly AbstractJsonStringNode NULL = new AbstractJsonStringNode();
        public static readonly AbstractJsonStringNode EMPTY = new AbstractJsonStringNode("");


        private readonly String value;

        public AbstractJsonStringNode(string value = null)
            : base()
        {
            this.value = value;
        }


        ///////////////////////////////////
        // JsonNode interface

        public override object GetValue()
        {
            return this.value;
        }
        public object Value
        {
            get
            {
                return value;
            }
            set
            {
                // ???
                //this.value = value;
            }
        }


        ///////////////////////////////////
        // JsonSerializable interface

        public override string ToJsonString(int indent)
        {
            // temporary
            if (value == null) {
                return "null";  // ???
            } else {
                return "\"" + value + "\"";
            }
        }

        // ????
        public override void WriteJsonString(TextWriter writer, int indent)
        {
            throw new NotImplementedException();
        }


        // For debugging
        public override string ToString()
        {
            return $"{nameof(AbstractJsonStringNode)} [value={Value}]";
        }

    }
}
