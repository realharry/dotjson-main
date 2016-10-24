using HoloJson.Builder.Partial;
using HoloJson.Builder.Partial.Impl;
using HoloJson.Parser.Impl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HoloJson.Mini
{
    /// <summary>
    /// MixedJsonBuilder wrapper.
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
    public sealed class MiniMixedBuilder : MixedJsonBuilder
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
        public static readonly MiniMixedBuilder DEFAULT_INSTANCE = new MiniMixedBuilder(new SimpleMixedJsonBuilder(null, true));

        // Delegate the implementation through decorator-like pattern.
        // private JsonParserFactory jsonParserFactory;
        private readonly MixedJsonBuilder decoratedBuilder;

        // TBD:
        // parser policty?
        // ....

        public MiniMixedBuilder() : this(new SimpleMixedJsonBuilder())
        {
        }
        public MiniMixedBuilder(MixedJsonBuilder decoratedBuilder)
        {
            this.decoratedBuilder = decoratedBuilder;
        }


        public bool IsLookAheadParsing
        {
            get
            {
                return ((AbstractJsonParser)decoratedBuilder).IsLookAheadParsing;
            }
        }
        public void EnableLookAheadParsing()
        {
            ((AbstractJsonParser)decoratedBuilder).EnableLookAheadParsing();
        }
        public void DisableLookAheadParsing()
        {
            ((AbstractJsonParser)decoratedBuilder).DisableLookAheadParsing();
        }

        public bool TracingEnabled
        {
            get
            {
                return ((AbstractJsonParser)decoratedBuilder).TracingEnabled;
            }
        }
        public void EnableTracing()
        {
            ((AbstractJsonParser)decoratedBuilder).EnableTracing();
        }
        public void DisableTracing()
        {
            ((AbstractJsonParser)decoratedBuilder).DisableTracing();
        }

        public async Task<string> BuildAsync(object jsonObj)
        {
            return await decoratedBuilder.BuildAsync(jsonObj);
        }
        public async Task BuildAsync(TextWriter writer, object jsonObj)
        {
            await decoratedBuilder.BuildAsync(writer, jsonObj);
        }
        public async Task<string> BuildMixedAsync(object jsonObj)
        {
            return await decoratedBuilder.BuildMixedAsync(jsonObj);
        }
        public async Task<string> BuildMixedAsync(object jsonObj, int minDepth, int maxDepth)
        {
            return await decoratedBuilder.BuildMixedAsync(jsonObj, minDepth, maxDepth);
        }
        public async Task<string> BuildMixedAsync(object jsonObj, int minDepth, int maxDepth, int indent)
        {
            return await decoratedBuilder.BuildMixedAsync(jsonObj, minDepth, maxDepth, indent);
        }
        public async Task BuildMixedAsync(TextWriter writer, object jsonObj)
        {
            await decoratedBuilder.BuildMixedAsync(writer, jsonObj);
        }
        public async Task BuildMixedAsync(TextWriter writer, object jsonObj, int minDepth, int maxDepth)
        {
            await decoratedBuilder.BuildMixedAsync(writer, jsonObj, minDepth, maxDepth);
        }
        public async Task BuildMixedAsync(TextWriter writer, object jsonObj, int minDepth, int maxDepth, int indent)
        {
            await decoratedBuilder.BuildMixedAsync(writer, jsonObj, minDepth, maxDepth, indent);
        }

    }

}