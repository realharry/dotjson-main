using HoloJson.Parser.Policy;
using HoloJson.Type.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HoloJson.Parser.Impl
{
    /// <summary>
    /// Simple BareJsonParser implementation.
    /// </summary>
    public sealed class SimpleJsonParser : AbstractBareJsonParser, BareJsonParser
    {
        public SimpleJsonParser()
        {
        }
        public SimpleJsonParser(JsonTypeFactory jsonTypeFactory) : base(jsonTypeFactory)
        {
        }
        public SimpleJsonParser(JsonTypeFactory jsonTypeFactory, ParserPolicy parserPolicy) 
            : base(jsonTypeFactory, parserPolicy)
        {
        }
        public SimpleJsonParser(JsonTypeFactory jsonTypeFactory, ParserPolicy parserPolicy, bool threadSafe) 
            : base(jsonTypeFactory, parserPolicy, threadSafe)
        {
        }

        protected internal override void Init()
        {
            // Disable "tracing" by default.
            DisableTracing();
        }

    }

}