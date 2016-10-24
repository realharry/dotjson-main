using HoloJson.Common;
using HoloJson.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HoloJson.Type.Base
{
    public class AbstractJsonNumberNode : AbstractJsonLeafNode, JsonNumberNode
    {
        public static readonly AbstractJsonNumberNode NULL = new AbstractJsonNumberNode();


        private readonly Number? value = null;

        private AbstractJsonNumberNode()
        {
        }
        public AbstractJsonNumberNode(Number value)
            : base()
        {
            this.value = value;
        }


        ///////////////////////////////////
        // JsonNode interface

        //public override object GetValue()
        //{
        //    return this.value;
        //}
        public override object Value
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

        public override async Task<string> ToJsonStringAsync(int indent)
        {
            // temporary
            if (value == null) {
                return Literals.NULL;
            } else {
                return value.Value.ToString();   //
            }
        }

        // ????
        public override async Task WriteJsonStringAsync(TextWriter writer, int indent)
        {
            throw new NotImplementedException();
        }


        ////////////////////////////////////////////
        //// Number

        //public int intValue()
        //{
        //    return value.intValue();
        //}
        //public long longValue()
        //{
        //    return value.longValue();
        //}
        //public float floatValue()
        //{
        //    return value.floatValue();
        //}
        //public double doubleValue()
        //{
        //    return value.doubleValue();
        //}



        // For debugging
        public override string ToString()
        {
            return $"{nameof(AbstractJsonBooleanNode)} [value={Value}]";
        }


    }
}
