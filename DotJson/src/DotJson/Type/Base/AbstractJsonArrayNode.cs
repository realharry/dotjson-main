using DotJson.Builder.Core;
using DotJson.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;

namespace DotJson.Type.Base
{
    public class AbstractJsonArrayNode : AbstractJsonStructNode, JsonArrayNode
    {
        public static readonly AbstractJsonArrayNode NULL = new AbstractJsonArrayNode();

        // Decorated object.
        private readonly IList<object> list;

        private AbstractJsonArrayNode()
        {
            list = null;
        }
        public AbstractJsonArrayNode(IList<object> list)
            : base()
        {
            if (list == null) {
                this.list = new List<object>();
            } else {
                this.list = list;
            }
        }


        ///////////////////////////////////
        // JsonNode interface

        //public override object GetValue()
        //{
        //    // ???
        //    return this.list;
        //}
        public override object Value
        {
            get
            {
                // ???
                return this.list;
            }
            set
            {
                // ???
                //this.value = value;
            }
        }

        public int Count
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsReadOnly
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public object this[int index]
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }


        ///////////////////////////////////
        // JsonSerializable interface
        // Note: The default depth of AbstractJsonNodes is always 1.   

        public override async Task WriteJsonStringAsync(TextWriter writer, int indent)
        {
            if (list == null) {
                writer.Write(Literals.NULL);
                return;
            }

            IndentInfoStruct indentInfo = new IndentInfoStruct(indent);
            bool includeWS = indentInfo.IsIncludingWhiteSpaces;
            bool includeLB = indentInfo.IsIncludingLineBreaks;
            bool lbAfterComma = indentInfo.IsLineBreakingAfterComma;
            int indentSize = indentInfo.IndentSize;

            // ???
            // We need a way to set the "global indent level" ....
            int indentLevel = 0;
            string WS = "";
            if (includeWS) {
                WS = " ";
            }
            string LB = "";
            if (includeLB) {
                LB = "\n";
            }
            string IND = "";
            string INDX = "";
            if (indentSize > 0 && indentLevel > 0) {
                // IND = String.format("%1$" + (indentSize * indentLevel) + "s", "");
                IND = string.Format("{0," + (indentSize * indentLevel) + "}", "");
            }
            if (indentSize > 0 && indentLevel >= 0) {
                // INDX = String.format("%1$" + (indentSize * (indentLevel + 1)) + "s", "");
                INDX = string.Format("{0," + (indentSize * (indentLevel + 1)) + "}", "");
            }

            writer.Write("[");
            writer.Write(LB);
            writer.Write(INDX);
            var it = list.GetEnumerator();
            var isFirst = true;

            while (it.MoveNext()) {
                if (!isFirst) {
                    writer.Write(",");
                    if (lbAfterComma) {
                        writer.Write(LB);
                        writer.Write(INDX);
                    } else {
                        writer.Write(WS);
                    }
                }
                JsonNode node = it.Current as JsonNode;
                writer.Write(await node.ToJsonStringAsync());
            }
            writer.Write(LB);
            writer.Write(IND);
            writer.Write("]");
        }


        ///////////////////////////////////////////////////////
        // TBD: JsonCompatible interface..
        // ....

        public /* override */ bool isJsonStructureArray()
        {
            return true;
        }

        public override async Task<object> ToJsonStructureAsync(int depth)
        {
            // ????
            // return list; 

            IList<object> arr = new List<object>();

            // TBD:
            // Traverse the list down to depth...
            arr = list;
            // ...

            return arr;
        }


        ///////////////////////////////////
        // JsonArray interface

        public bool HasChildren()
        {
            return list == null ? false : list.Any();
        }

        public IList<JsonNode> GetChildren()
        {
            // ????
            var children = new List<JsonNode>();
            if(list != null && list.Any()) {
                foreach(var o in list) {
                    var n = o as JsonNode;
                    if (n != null) {
                        children.Add(n);
                    }
                }
            }
            return children;
        }

        public JsonNode GetChildNode(int index)
        {
            JsonNode node = null;
            if(index >= 0 && index < list.Count) {
                node = list[index] as JsonNode;
            }
            return node;
        }

        public void AddChild(JsonNode child)
        {
            list.Add(child);
        }

        public void AddAllChildren(IList<JsonNode> children)
        {
            // ????
            (list as List<object>)?.AddRange(children);
        }



        ///////////////////////////////////
        // IList<object> interface

        public int IndexOf(object item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, object item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public void Add(object item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(object item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(object[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(object item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<object> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }


    }
}
