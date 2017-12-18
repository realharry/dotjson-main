using DotJson.Parser.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotJson.Parser.Exceptions
{
    /// <summary>
    /// Indicates an error due to ill-formed JSON string (or, read error), etc.
    /// </summary>
    public class UnexpectedEndOfStreamException : JsonTokenizerException
    {
        private const long serialVersionUID = 1L;

        public UnexpectedEndOfStreamException() : base()
        {
        }
        public UnexpectedEndOfStreamException(char[] tail) : base(tail)
        {
        }
        public UnexpectedEndOfStreamException(char[] tail, char[] head) : base(tail, head)
        {
        }
        public UnexpectedEndOfStreamException(ErrorContext context) : base(context)
        {
        }

        public UnexpectedEndOfStreamException(string message) : base(message)
        {
        }
        public UnexpectedEndOfStreamException(string message, string context) : base(message, context)
        {
        }
        public UnexpectedEndOfStreamException(string message, char[] tail) : base(message, tail)
        {
        }
        public UnexpectedEndOfStreamException(string message, char[] tail, char[] head) : base(message, tail, head)
        {
        }
        public UnexpectedEndOfStreamException(string message, ErrorContext context) : base(message, context)
        {
        }

        public UnexpectedEndOfStreamException(Exception cause) : base(cause)
        {
        }
        public UnexpectedEndOfStreamException(Exception cause, string context) : base(cause, context)
        {
        }
        public UnexpectedEndOfStreamException(Exception cause, char[] tail) : base(cause, tail)
        {
        }
        public UnexpectedEndOfStreamException(Exception cause, char[] tail, char[] head) : base(cause, tail, head)
        {
        }
        public UnexpectedEndOfStreamException(Exception cause, ErrorContext context) : base(cause, context)
        {
        }

        public UnexpectedEndOfStreamException(string message, Exception cause) : base(message, cause)
        {
        }
        public UnexpectedEndOfStreamException(string message, Exception cause, string context) : base(message, cause, context)
        {
        }
        public UnexpectedEndOfStreamException(string message, Exception cause, char[] tail) : base(message, cause, tail)
        {
        }
        public UnexpectedEndOfStreamException(string message, Exception cause, char[] tail, char[] head) : base(message, cause, tail, head)
        {
        }
        public UnexpectedEndOfStreamException(string message, Exception cause, ErrorContext context) : base(message, cause, context)
        {
        }

    }

}