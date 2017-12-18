using DotJson.Parser.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotJson.Parser.Exceptions
{
    /// <summary>
    /// Indicates invalid/unexpected symbol during parsing.
    /// </summary>
    public class UnexpectedSymbolException : JsonTokenizerException
    {
        private const long serialVersionUID = 1L;

        public UnexpectedSymbolException() : base()
        {
        }
        public UnexpectedSymbolException(char[] tail) : base(tail)
        {
        }
        public UnexpectedSymbolException(char[] tail, char[] head) : base(tail, head)
        {
        }
        public UnexpectedSymbolException(ErrorContext context) : base(context)
        {
        }

        public UnexpectedSymbolException(string message) : base(message)
        {
        }
        public UnexpectedSymbolException(string message, string context) : base(message, context)
        {
        }
        public UnexpectedSymbolException(string message, char[] tail) : base(message, tail)
        {
        }
        public UnexpectedSymbolException(string message, char[] tail, char[] head) : base(message, tail, head)
        {
        }
        public UnexpectedSymbolException(string message, ErrorContext context) : base(message, context)
        {
        }

        public UnexpectedSymbolException(Exception cause) : base(cause)
        {
        }
        public UnexpectedSymbolException(Exception cause, string context) : base(cause, context)
        {
        }
        public UnexpectedSymbolException(Exception cause, char[] tail) : base(cause, tail)
        {
        }
        public UnexpectedSymbolException(Exception cause, char[] tail, char[] head) : base(cause, tail, head)
        {
        }
        public UnexpectedSymbolException(Exception cause, ErrorContext context) : base(cause, context)
        {
        }

        public UnexpectedSymbolException(string message, Exception cause) : base(message, cause)
        {
        }
        public UnexpectedSymbolException(string message, Exception cause, string context) : base(message, cause, context)
        {
        }
        public UnexpectedSymbolException(string message, Exception cause, char[] tail) : base(message, cause, tail)
        {
        }
        public UnexpectedSymbolException(string message, Exception cause, char[] tail, char[] head) : base(message, cause, tail, head)
        {
        }
        public UnexpectedSymbolException(string message, Exception cause, ErrorContext context) : base(message, cause, context)
        {
        }

    }

}