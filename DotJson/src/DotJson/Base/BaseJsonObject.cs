using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotJson.Base
{
    public sealed class BaseJsonObject : BaseJsonWrapper, JsonObject
    {

        // JSON builder
        public BaseJsonObject(IDictionary<string, object> map) : base(map)
        {
        }

        // JSON Parser
        public BaseJsonObject(string jsonStr) : base(jsonStr)
        {
        }


        //    // @Override
        //    public boolean isJsonStructureArray()
        //    {
        //        return false;
        //    }


    }

}