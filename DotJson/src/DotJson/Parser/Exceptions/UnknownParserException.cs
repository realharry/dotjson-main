using DotJson.Parser.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotJson.Parser.Exceptions
{
    /// <summary>
    /// Indicates parse failure due to an unknown reason.
    /// </summary>
    public class UnknownParserException : JsonParserException
    {
        private const long serialVersionUID = 1L;

        public UnknownParserException() : base()
        {
        }
        public UnknownParserException(char[] tail) : base(tail)
        {
        }
        public UnknownParserException(char[] tail, char[] head) : base(tail, head)
        {
        }
        public UnknownParserException(ErrorContext context) : base(context)
        {
        }

        public UnknownParserException(string message) : base(message)
        {
        }
        public UnknownParserException(string message, string context) : base(message, context)
        {
        }
        public UnknownParserException(string message, char[] tail) : base(message, tail)
        {
        }
        public UnknownParserException(string message, char[] tail, char[] head) : base(message, tail, head)
        {
        }
        public UnknownParserException(string message, ErrorContext context) : base(message, context)
        {
        }

        public UnknownParserException(Exception cause) : base(cause)
        {
        }
        public UnknownParserException(Exception cause, string context) : base(cause, context)
        {
        }
        public UnknownParserException(Exception cause, char[] tail) : base(cause, tail)
        {
        }
        public UnknownParserException(Exception cause, char[] tail, char[] head) : base(cause, tail, head)
        {
        }
        public UnknownParserException(Exception cause, ErrorContext context) : base(cause, context)
        {
        }

        public UnknownParserException(string message, Exception cause) : base(message, cause)
        {
        }
        public UnknownParserException(string message, Exception cause, string context) : base(message, cause, context)
        {
        }
        public UnknownParserException(string message, Exception cause, char[] tail) : base(message, cause, tail)
        {
        }
        public UnknownParserException(string message, Exception cause, char[] tail, char[] head) : base(message, cause, tail, head)
        {
        }
        public UnknownParserException(string message, Exception cause, ErrorContext context) : base(message, cause, context)
        {
        }

    }

}