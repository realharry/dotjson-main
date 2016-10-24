using HoloJson.Parser.Policy;
using HoloJson.Type.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HoloJson.Parser.Impl
{
    public class CustomJsonParser : AbstractRichJsonParser, RichJsonParser, FlexibleJsonParser
    {
        public CustomJsonParser()
        {
        }
        public CustomJsonParser(JsonTypeFactory jsonTypeFactory) 
            : base(jsonTypeFactory)
        {
        }

        public CustomJsonParser(JsonTypeFactory jsonTypeFactory, ParserPolicy parserPolicy) 
            : base(jsonTypeFactory, parserPolicy)
        {
        }

        protected internal override void Init()
        {
            // Enable "tracing" by default.
            EnableTracing();
        }


    }

}