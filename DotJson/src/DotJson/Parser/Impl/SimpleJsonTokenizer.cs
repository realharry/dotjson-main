using DotJson.Parser.Policy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DotJson.Parser.Impl
{
    /// <summary>
    /// Simple JsonTokenizer implementation.
    /// It's a "default" tokenizer for MiniJson.
    /// </summary>
    public sealed class SimpleJsonTokenizer : AbstractJsonTokenizer, TraceableJsonTokenizer
    {
        public SimpleJsonTokenizer(string str) : base(str)
        {
        }
        public SimpleJsonTokenizer(TextReader reader) : base(reader)
        {
        }
        public SimpleJsonTokenizer(TextReader reader, ParserPolicy parserPolicy) 
            : base(reader, parserPolicy)
        {
        }

        protected internal override void Init()
        {
            // Disable "tracing" by default.
            DisableTracing();
        }

    }

}