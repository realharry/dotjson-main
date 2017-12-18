using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotJson.Core;
using DotJson.Common;

namespace DotJson.Type.Factory.Impl
{
    public class BareJsonTypeFactory : JsonTypeFactory
    {
        public static BareJsonTypeFactory Instance { get; } = new BareJsonTypeFactory();
        private BareJsonTypeFactory() { }


        public IList<object> CreateArray(IList<object> list)
        {
            return list;
        }

        public object CreateBoolean(bool? value)
        {
            // ???
            // return value == null ? JsonNull.NULL : value.Value;
            return value == null ? null : (bool?) value.Value;
        }

        public object CreateNull()
        {
            return JsonNull.NULL;
        }

        public object CreateNumber(Number? value)
        {
            // ???
            // return value == null ? JsonNull.NULL : value.Value;
            return value == null ? null : (Number?) value.Value;
        }

        public IDictionary<string, object> CreateObject(IDictionary<string, object> map)
        {
            return map;
        }

        public object CreateString(string value)
        {
            return value;
        }
    }
}
