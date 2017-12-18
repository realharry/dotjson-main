using DotJson.Parser.Policy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DotJson.Parser.Impl
{
    // Base class for customizable Json tokenizer.
    public class CustomJsonTokenizer : AbstractJsonTokenizer, TraceableJsonTokenizer, FlexibleJsonTokenizer
    {
        public CustomJsonTokenizer(string str) : base(str)
        {
        }
        public CustomJsonTokenizer(TextReader reader) : base(reader)
        {
        }
        public CustomJsonTokenizer(TextReader reader, ParserPolicy parserPolicy) 
            : base(reader, parserPolicy)
        {
        }

        protected internal override void Init()
        {
            // Enable "tracing" by default.
            EnableTracing();
        }

        //public override ParserPolicy ParserPolicy
        //{
        //    get
        //    {
        //        return base.ParserPolicy;
        //    }
        //}

    }

}