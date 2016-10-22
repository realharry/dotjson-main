using HoloJson.Common;
using HoloJson.Parser.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HoloJson.Parser
{
    /// <summary>
    /// Base class for all parser/lexer related exceptions.
    /// </summary>
    public class JsonParserException : JsonException
    {
        // Stores the last X characters previously read (and, some following chars) when the error occurred.
        private ErrorContext context;


        public JsonParserException() : this((ErrorContext)null)
        {
        }
        public JsonParserException(char[] tail) : this(ErrorContext.build(tail))
        {
        }
        public JsonParserException(char[] tail, char[] head) : this(ErrorContext.build(tail, head))
        {
        }
        public JsonParserException(ErrorContext context)
        {
            this.context = context;
        }

        public JsonParserException(string message) : this(message, (ErrorContext)null)
        {
        }
        public JsonParserException(string message, string context) : this(message, ErrorContext.build(context))
        {
        }
        public JsonParserException(string message, char[] tail) : this(message, ErrorContext.build(tail))
        {
        }
        public JsonParserException(string message, char[] tail, char[] head) : this(message, ErrorContext.build(tail, head))
        {
        }
        public JsonParserException(string message, ErrorContext context) : base(message)
        {
            this.context = context;
        }

        public JsonParserException(Exception cause) : this(cause, (ErrorContext)null)
        {
        }
        public JsonParserException(Exception cause, string context) : this(cause, ErrorContext.build(context))
        {
        }
        public JsonParserException(Exception cause, char[] tail) : this(cause, ErrorContext.build(tail))
        {
        }
        public JsonParserException(Exception cause, char[] tail, char[] head) : this(cause, ErrorContext.build(tail, head))
        {
        }
        public JsonParserException(Exception cause, ErrorContext context) : base(cause)
        {
            this.context = context;
        }

        public JsonParserException(string message, Exception cause) : this(message, cause, (ErrorContext)null)
        {
        }
        public JsonParserException(string message, Exception cause, string context) : this(message, cause, ErrorContext.build(context))
        {
        }
        public JsonParserException(string message, Exception cause, char[] tail) : this(message, cause, ErrorContext.build(tail))
        {
        }
        public JsonParserException(string message, Exception cause, char[] tail, char[] head) : this(message, cause, ErrorContext.build(tail, head))
        {
        }
        public JsonParserException(string message, Exception cause, ErrorContext context) : base(message, cause)
        {
            this.context = context;
        }


        public ErrorContext ErrorContext
        {
            get
            {
                return this.context;
            }
        }
        public string Context
        {
            get
            {
                if (this.context != null) {
                    return this.context.Context;
                } else {
                    return null; // null or "" ??
                }
            }
        }


    }

}