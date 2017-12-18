using DotJson.Common;
using DotJson.Core;
using DotJson.Type.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotJson.Type.Factory.Impl
{
    public class AbstractJsonTypeFactory : JsonTypeFactory
    {
        public static AbstractJsonTypeFactory Instance { get; } = new AbstractJsonTypeFactory();
        private AbstractJsonTypeFactory() { }


        public IList<object> CreateArray(IList<object> list)
        {
            if (list == null) {
                return AbstractJsonArrayNode.NULL;
            }
            return new AbstractJsonArrayNode(list);
        }

        public object CreateBoolean(bool? value)
        {
            if(value == null) {
                return AbstractJsonNullNode.NULL;
            }
            if(value.Value == true) {
                return AbstractJsonBooleanNode.TRUE;
            } else {
                return AbstractJsonBooleanNode.FALSE;
            }
        }

        public object CreateNull()
        {
            return AbstractJsonNullNode.NULL;
        }

        public object CreateNumber(Number? value)
        {
            if(value == null) {
                return AbstractJsonNumberNode.NULL;
            }
            return new AbstractJsonNumberNode(value.Value);
        }

        public IDictionary<string, object> CreateObject(IDictionary<string, object> map)
        {
            if (map == null) {
                return AbstractJsonObjectNode.NULL;
            }
            return new AbstractJsonObjectNode(map);
        }

        public object CreateString(string value)
        {
            if (value == null) {
                return AbstractJsonStringNode.NULL;
            }
            return new AbstractJsonStringNode(value);
        }
    }
}
