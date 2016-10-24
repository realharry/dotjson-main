using HoloJson.Parser.Impl;
using HoloJson.Parser.Partial;
using HoloJson.Parser.Partial.Impl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HoloJson.Mini
{
    /// <summary>
    /// LayeredJsonParser wrapper.
    /// The primary purpose of this class is to "minimize" the interface of the real implementation.
    /// 
    /// Usage:
    /// <pre>
    /// <code>
    /// try {
    ///     Object obj = MiniJsonParser.ParseAsync(jsonStr);
    /// } catch (JsonParserException e) {
    /// }
    /// </code>
    /// </pre>
    /// 
    /// </summary>
    // This is kind of an "immutable" wrapper around SimpleJsonParser or other JsonParser.
    public sealed class MiniLayeredParser : LayeredJsonParser
    {
        // private static final Logger log = Logger.getLogger(MiniJsonParser.class.getName());

        // "semi-singleton".
        // Note that MiniJsonParser does not have setters, at least at this point (although the Ctors take different args),
        //    and it can be made immutable, or we can just use it as a singleton (by changing Ctors).
        // But, that may change in the future. 
        //    So, just use this special instance sort of as a singleton for now, which is immutable (currently).
        // -->
        // On second thought, MiniJsonParser is not entirely multi-thread/concurrency safe...
        //     (some optimization code for performance makes it non-thread safe.)
        public static readonly MiniLayeredParser DEFAULT_INSTANCE = new MiniLayeredParser(new SimpleLayeredJsonParser(null, null, true));

        // Delegate the implementation through decorator-like pattern.
        // private JsonParserFactory jsonParserFactory;
        private readonly LayeredJsonParser decoratedParser;

        // TBD:
        // parser policty?
        // ....

        public MiniLayeredParser() : this(new SimpleLayeredJsonParser())
        {
        }
        public MiniLayeredParser(LayeredJsonParser decoratedParser)
        {
            this.decoratedParser = decoratedParser;
        }


        public bool IsLookAheadParsing
        {
            get
            {
                return ((AbstractJsonParser)decoratedParser).IsLookAheadParsing;
            }
        }
        public void EnableLookAheadParsing()
        {
            ((AbstractJsonParser)decoratedParser).EnableLookAheadParsing();
        }
        public void DisableLookAheadParsing()
        {
            ((AbstractJsonParser)decoratedParser).DisableLookAheadParsing();
        }

        public bool TracingEnabled
        {
            get
            {
                return ((AbstractJsonParser)decoratedParser).TracingEnabled;
            }
        }
        public void EnableTracing()
        {
            ((AbstractJsonParser)decoratedParser).EnableTracing();
        }
        public void DisableTracing()
        {
            ((AbstractJsonParser)decoratedParser).DisableTracing();
        }


        /// <summary>
        /// Creates a Java object based on the given jsonStr.
        /// </summary>
        public async Task<object> ParseAsync(string jsonStr)
        {
            return await decoratedParser.ParseAsync(jsonStr);
        }

        public async Task<object> ParseAsync(TextReader reader)
        {
            return await decoratedParser.ParseAsync(reader);
        }

        public async Task<object> ParseAsync(string jsonStr, int depth)
        {
            return await decoratedParser.ParseAsync(jsonStr, depth);
        }

        public async Task<object> ParseAsync(TextReader reader, int depth)
        {
            return await decoratedParser.ParseAsync(reader, depth);
        }

    }

}