using DotJson.Parser.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotJson.Parser.Exceptions
{
    /// <summary>
    /// Indicates invalid/unexpected token during parsing.
    /// </summary>
    public class InvalidJsonTokenException : JsonParserException
    {
        private const long serialVersionUID = 1L;

        public InvalidJsonTokenException() : base()
        {
        }
        public InvalidJsonTokenException(char[] tail) : base(tail)
        {
        }
        public InvalidJsonTokenException(char[] tail, char[] head) : base(tail, head)
        {
        }
        public InvalidJsonTokenException(ErrorContext context) : base(context)
        {
        }

        public InvalidJsonTokenException(string message) : base(message)
        {
        }
        public InvalidJsonTokenException(string message, string context) : base(message, context)
        {
        }
        public InvalidJsonTokenException(string message, char[] tail) : base(message, tail)
        {
        }
        public InvalidJsonTokenException(string message, char[] tail, char[] head) : base(message, tail, head)
        {
        }
        public InvalidJsonTokenException(string message, ErrorContext context) : base(message, context)
        {
        }

        public InvalidJsonTokenException(Exception cause) : base(cause)
        {
        }
        public InvalidJsonTokenException(Exception cause, string context) : base(cause, context)
        {
        }
        public InvalidJsonTokenException(Exception cause, char[] tail) : base(cause, tail)
        {
        }
        public InvalidJsonTokenException(Exception cause, char[] tail, char[] head) : base(cause, tail, head)
        {
        }
        public InvalidJsonTokenException(Exception cause, ErrorContext context) : base(cause, context)
        {
        }

        public InvalidJsonTokenException(string message, Exception cause) : base(message, cause)
        {
        }
        public InvalidJsonTokenException(string message, Exception cause, string context) : base(message, cause, context)
        {
        }
        public InvalidJsonTokenException(string message, Exception cause, char[] tail) : base(message, cause, tail)
        {
        }
        public InvalidJsonTokenException(string message, Exception cause, char[] tail, char[] head) : base(message, cause, tail, head)
        {
        }
        public InvalidJsonTokenException(string message, Exception cause, ErrorContext context) : base(message, cause, context)
        {
        }

    }

}