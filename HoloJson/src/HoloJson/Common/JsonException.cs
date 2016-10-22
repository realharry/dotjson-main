using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HoloJson.Common
{
    public class JsonException : Exception
    {
        public JsonException()
            : base()
        {
        }
        public JsonException(string message)
            : base(message)
        {
        }
        public JsonException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        public JsonException(Exception innerException)
            : base(null, innerException)
        {
        }

    }
}
