using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotJson.Type.Base
{
    public class AbstractJsonObjectMember : JsonObjectMember
    {
        private string key;
        private JsonNode value;

        public AbstractJsonObjectMember()
            : this(null, null)
        {
        }
        public AbstractJsonObjectMember(string key, JsonNode value)
        {
            this.key = key;
            this.value = value;
        }

        //public string GetKey()
        //{
        //    return key;
        //}
        public string Key
        {
            get
            {
                return key;
            }
            set
            {
                this.key = value;
            }
        }

        //public JsonNode GetValue()
        //{
        //    return value;
        //}
        public JsonNode Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
            }
        }

        // For debugging
        public override string ToString()
        {
            return $"{nameof(AbstractJsonObjectMember)} [key={Key}, value={Value}]";
        }
    }
}
