using DotJson.Builder.Core;
using DotJson.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DotJson.Type.Base
{
    public class AbstractJsonObjectNode : AbstractJsonStructNode, JsonObjectNode
    {
        public static readonly AbstractJsonObjectNode NULL = new AbstractJsonObjectNode();

        // Decorated object.
        private readonly IDictionary<string, object> map;

        private AbstractJsonObjectNode()
        {
            map = null;
        }
        public AbstractJsonObjectNode(IDictionary<string, object> map)
        {
            if (map == null) {
                this.map = new Dictionary<string, object>();
            } else {
                this.map = map;
            }
        }


        ///////////////////////////////////
        // JsonNode interface

        //public override object GetValue()
        //{
        //    // ???
        //    return this.map;
        //}
        public override object Value
        {
            get
            {
                // ???
                return this.map;
            }
            set
            {
                // ???
                //this.value = value;
            }
        }


        ///////////////////////////////////
        // JsonSerializable interface
        // Note: The default depth of AbstractJsonNodes is always 1.   

        public override async Task WriteJsonStringAsync(TextWriter writer, int indent)
        {
            if(map == null) {
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

            writer.Write("{");
            writer.Write(LB);
            writer.Write(INDX);
            var it = map.Keys.GetEnumerator();
            var isFirst = true;
            while (it.MoveNext()) {
                if (! isFirst) {
                    writer.Write(",");
                    if (lbAfterComma) {
                        writer.Write(LB);
                        writer.Write(INDX);
                    } else {
                        writer.Write(WS);
                    }
                }
                string key = it.Current;
                JsonStringNode jsonKey = new AbstractJsonStringNode(key);
                var node = map[key] as JsonNode;
                writer.Write("\"");
                writer.Write(await jsonKey.ToJsonStringAsync(indent));
                writer.Write("\":");
                writer.Write(WS);
                writer.Write(await node.ToJsonStringAsync());
                isFirst = false;
            }
            writer.Write(LB);
            writer.Write(IND);
            writer.Write("}");
        }


        ///////////////////////////////////////////////////////
        // TBD: JsonCompatible interface..
        // ....

        // ???
        public /* override */ bool IsJsonStructureArray()
        {
            return false;
        }

        public override async Task<object> ToJsonStructureAsync(int depth)
        {
            // ????
            // return map;

            //IDictionary<string, object> obj = new Dictionary<string, object>();

            //// TBD:
            //// Traverse the map down to depth...
            //struct = map;
            //// ...

            //return obj;

            return null;
        }



        ///////////////////////////////////
        // JsonObject interface

        public bool HasMembers()
        {
            return map == null ? false : map.Any();
        }

        public ISet<JsonObjectMember> GetMembers()
        {
            // tbd:
            throw new NotImplementedException();

            //Set<Entry<string, object>> entrySet = map.entrySet();
            //if (entrySet != null) {
            //    Set<JsonObjectMember> members = new HashSet<JsonObjectMember>();
            //    for (Entry<string, object> e : entrySet) {
            //        string key = e.getKey();
            //        object value = e.getValue();
            //        JsonObjectMember member = new AbstractJsonObjectMember(key, (JsonNode)value);
            //        members.add(member);
            //    }
            //    return members;
            //}
            //return null;
        }

        public JsonNode GetMemberNode(string key)
        {
            var node = map?[key] as JsonNode;
            return node;
        }

        public void AddMember(JsonObjectMember member)
        {
            if (member != null) {
                string key = member.Key;
                JsonNode node = member.Value;
                map[key] = node;
            }
        }

        public void AddAllMembers(ISet<JsonObjectMember> members)
        {
            if (members != null && members.Any()) {
                foreach (JsonObjectMember m in members) {
                    string key = m.Key;
                    JsonNode node = m.Value;
                    map[key] = node;
                }
            }
        }


        /////////////////////////////////////
        // IDictionary interface.

        public object this[string key]
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

        public ICollection<string> Keys
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ICollection<object> Values
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void Add(KeyValuePair<string, object> item)
        {
            throw new NotImplementedException();
        }

        public void Add(string key, object value)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(string key)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            throw new NotImplementedException();
        }

        public bool Remove(string key)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(string key, out object value)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }


        // For debugging
        public override string ToString()
        {
            return $"{nameof(AbstractJsonObjectNode)} [map={map}]";
        }

    }
}
