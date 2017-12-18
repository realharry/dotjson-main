using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotJson.Parser.Core
{
    /// <summary>
    /// Simple wrapper around a String. To be used as a member of ParserException.
    /// (It's hard to overload exception constructor with a String arg.
    ///    Use this class instead of String, if apropriate.)
    /// </summary>
    // [Serializable]
    public sealed class ErrorContext
    {
        private const string ERROR_POINT_MARKER = " ... "; // temporary

        // Before and After the error during the character reading.
        private readonly char[] tail; // past
        private readonly char[] head; // future
                                      // "cache" - lazy initialized.
                                      // Note that we have this strange dual data representation.
                                      // Once context is set (e.g., through Ctor, or via toString()), then tail/head is ignored.
        private string context = null;

        // Use builders.
        private ErrorContext(char[] tail) : this(tail, null)
        {
        }
        private ErrorContext(char[] tail, char[] head)
        {
            this.tail = tail;
            this.head = head;
        }
        private ErrorContext(string context)
        {
            this.tail = this.head = null;
            this.context = context;
        }

        public static ErrorContext build(char[] tail)
        {
            return build(tail, null);
        }
        public static ErrorContext build(char[] tail, char[] head)
        {
            return new ErrorContext(tail, head);
        }
        public static ErrorContext build(string context)
        {
            // if context == null, return null ?????
            return new ErrorContext(context);
        }
        public static ErrorContext build(string tail, string head)
        {
            string context = buildContextString(tail, head);
            return new ErrorContext(context);
        }

        public string Context
        {
            get
            {
                if (string.ReferenceEquals(context, null)) {
                    context = buildContextString();
                }
                return context;
            }
        }

        public static string buildContextString(char[] tail, char[] head)
        {
            StringBuilder sb = new StringBuilder();
            if (tail != null) {
                sb.Append(tail);
            }
            sb.Append(ERROR_POINT_MARKER);
            if (head != null) {
                sb.Append(head);
            }
            return sb.ToString();
        }
        public static string buildContextString(string tail, string head)
        {
            StringBuilder sb = new StringBuilder();
            if (!string.ReferenceEquals(tail, null)) {
                sb.Append(tail);
            }
            sb.Append(ERROR_POINT_MARKER);
            if (!string.ReferenceEquals(head, null)) {
                sb.Append(head);
            }
            return sb.ToString();
        }

        private string buildContextString()
        {
            return buildContextString(tail, head);
        }

        public override string ToString()
        {
            if (string.ReferenceEquals(context, null)) {
                context = buildContextString();
            }
            return context;
        }


    }

}
