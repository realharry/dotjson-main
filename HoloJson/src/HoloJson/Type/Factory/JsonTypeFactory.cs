using HoloJson.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HoloJson.Type.Factory
{
    public interface JsonTypeFactory
    {
        object CreateNull();
        object CreateBoolean(bool? value);
        object CreateNumber(Number? value);
        object CreateString(string value);
        IDictionary<string, object> CreateObject(IDictionary<string, object> map);
        IList<object> CreateArray(IList<object> list);
    }
}
