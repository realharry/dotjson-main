using HoloJson.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HoloJson.Builder
{
    /// <summary>
    /// Base exception for all "builder"-related exceptions.
    /// </summary>
    public class JsonBuilderException : JsonException
    {
        private const long serialVersionUID = 1L;

        public JsonBuilderException() : base()
        {
        }
        public JsonBuilderException(string message) : base(message)
        {
        }
        public JsonBuilderException(Exception cause) : base(cause)
        {
        }
        public JsonBuilderException(string message, Exception cause) : base(message, cause)
        {
        }

    }

}