﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DotJson.Type.Base
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

        //public override object GetValue()
        //{
        //    return this.value;
        //}
        public override object Value
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

        public override async Task<string> ToJsonStringAsync(int indent)
        {
            // temporary
            if (value == null) {
                return "null";  // ???
            } else {
                return "\"" + value + "\"";
            }
        }

        // ????
        public override async Task WriteJsonStringAsync(TextWriter writer, int indent)
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
