using DotJson.Parser.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotJson.Parser.Exceptions
{
    /// <summary>
    /// Indicates error during parsing.
    /// </summary>
    public class JsonTokenizerException : JsonParserException
    {
        private const long serialVersionUID = 1L;

        public JsonTokenizerException() : base()
        {
        }
        public JsonTokenizerException(char[] tail) : base(tail)
        {
        }
        public JsonTokenizerException(char[] tail, char[] head) : base(tail, head)
        {
        }
        public JsonTokenizerException(ErrorContext context) : base(context)
        {
        }

        public JsonTokenizerException(string message) : base(message)
        {
        }
        public JsonTokenizerException(string message, string context) : base(message, context)
        {
        }
        public JsonTokenizerException(string message, char[] tail) : base(message, tail)
        {
        }
        public JsonTokenizerException(string message, char[] tail, char[] head) : base(message, tail, head)
        {
        }
        public JsonTokenizerException(string message, ErrorContext context) : base(message, context)
        {
        }

        public JsonTokenizerException(Exception cause) : base(cause)
        {
        }
        public JsonTokenizerException(Exception cause, string context) : base(cause, context)
        {
        }
        public JsonTokenizerException(Exception cause, char[] tail) : base(cause, tail)
        {
        }
        public JsonTokenizerException(Exception cause, char[] tail, char[] head) : base(cause, tail, head)
        {
        }
        public JsonTokenizerException(Exception cause, ErrorContext context) : base(cause, context)
        {
        }

        public JsonTokenizerException(string message, Exception cause) : base(message, cause)
        {
        }
        public JsonTokenizerException(string message, Exception cause, string context) : base(message, cause, context)
        {
        }
        public JsonTokenizerException(string message, Exception cause, char[] tail) : base(message, cause, tail)
        {
        }
        public JsonTokenizerException(string message, Exception cause, char[] tail, char[] head) : base(message, cause, tail, head)
        {
        }
        public JsonTokenizerException(string message, Exception cause, ErrorContext context) : base(message, cause, context)
        {
        }

    }

}