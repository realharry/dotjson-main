using HoloJson.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HoloJson.Type.Base
{
    public class AbstractJsonBooleanNode : AbstractJsonLeafNode, JsonBooleanNode
    {
        public static readonly AbstractJsonBooleanNode NULL = new AbstractJsonBooleanNode();
        public static readonly AbstractJsonBooleanNode TRUE = new AbstractJsonBooleanNode(true);
        public static readonly AbstractJsonBooleanNode FALSE = new AbstractJsonBooleanNode(false);

        private readonly bool? value = null;

        private AbstractJsonBooleanNode()
        {
        }
        public AbstractJsonBooleanNode(bool value)
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
                return value.Value;
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
                return Literals.NULL;
            } else {
                if (value.Value == true) {
                    return Literals.TRUE;
                } else {
                    return Literals.FALSE;
                }
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
            return $"{nameof(AbstractJsonBooleanNode)} [value={Value}]";
        }

    }
}
