using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HoloJson.Core;
using HoloJson.Common;
using System.Text;

namespace HoloJson.Parser.Core
{
    /// <summary>
    /// A "tail buffer". It keeps the finite number of objects that have been added last.
    /// Implemented by a ring buffer.
    /// JsonTokenBuffer can be used to keep the "last X objects that have been read" while reading an object stream. 
    /// (Note: the implementation is not thread-safe.)
    /// </summary>
    public sealed class JsonTokenBuffer : TailRingBuffer<JsonToken>
    {
        //    // temporary
        //    private static final int MAX_BUFFER_SIZE = 4096;
        private const int DEF_BUFFER_SIZE = 64;
        //    private static final int MIN_BUFFER_SIZE = 8;
        //    // ...


        // tbd:
        public JsonTokenBuffer() 
            : this(DEF_BUFFER_SIZE)
        {
        }
        // tbd:
        public JsonTokenBuffer(uint capacity) 
            : base(capacity)
        {
        }
        public JsonTokenBuffer(uint capacity, IList<JsonToken> c)
            : base(capacity, c)
		{
		}


		// temporary
		public override string ToTraceString()
        {
            //        JsonToken[] tokens = toArray(new JsonToken[]{});
            //        return Arrays.toString(tokens);

            StringBuilder sb = new StringBuilder();
            sb.Append("((Processed Tokens: ...");
            // var it = base.Buffer.GetEnumerator();
            var it = GetEnumerator();
            while (it.MoveNext()) {
                var token = (JsonToken) it.Current;
                var type = token.Type;


                //            switch(type) {
                //            case EOF:
                //            case NULL:
                //            case COMMA:
                //            case COLON:
                //            case LSQUARE:
                //            case RSQUARE:
                //            case LCURLY:
                //            case RCURLY:
                //                sb.append(TokenTypes.getDisplayName(type)).append(" ");
                //                break;
                //            case BOOLEAN:
                //            case NUMBER:
                //            case STRING:
                //                // ...
                //                break;
                //            default:
                //                // ...
                //            }

                sb.Append("<").Append(TokenTypes.GetTokenName(type));
                if (type == TokenType.NUMBER) {
                    object val = token.Value;
                    sb.Append(":").Append(val);
                } else if (type == TokenType.STRING) {
                    object val = token.Value;
                    string str = (string)val;
                    if (str.Length > 16) {
                        str = str.Substring(0, 14) + "..";
                    }
                    sb.Append(":").Append(str);
                }
                sb.Append(">, ");
            }
            sb.Append("))");

            return sb.ToString();
        }



    }

}