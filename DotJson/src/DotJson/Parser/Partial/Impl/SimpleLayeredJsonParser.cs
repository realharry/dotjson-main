using DotJson.Parser.Policy;
using DotJson.Type.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotJson.Parser.Partial.Impl
{
    /// <summary>
    /// Simple LayeredJsonParser wrapper.
    /// </summary>
    public sealed class SimpleLayeredJsonParser : AbstractLayeredJsonParser, LayeredJsonParser
    {
        public SimpleLayeredJsonParser()
        {
        }
        public SimpleLayeredJsonParser(JsonTypeFactory jsonTypeFactory) 
            : base(jsonTypeFactory)
        {
        }
        public SimpleLayeredJsonParser(JsonTypeFactory jsonTypeFactory, ParserPolicy parserPolicy)
            : base(jsonTypeFactory, parserPolicy)
        {
        }
        public SimpleLayeredJsonParser(JsonTypeFactory jsonTypeFactory, ParserPolicy parserPolicy, bool threadSafe)
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