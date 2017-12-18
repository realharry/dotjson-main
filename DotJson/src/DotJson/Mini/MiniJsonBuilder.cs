using DotJson.Builder;
using DotJson.Builder.Factory;
using DotJson.Builder.Impl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HoloJson.Mini
{
    /// <summary>
    /// JsonBuilder wrapper.
    /// The primary purpose of this class is to "minimize" the interface of the real implementation.
    /// 
    /// Usage:
    /// <pre>
    /// <code>
    /// String jsonStr = MiniJsonBuilder.BuildAsync(object);
    /// </code>
    /// </pre>
    /// 
    /// </summary>
    // This is kind of an "immutable" wrapper around SimpleJsonBuilder or other BareJsonBuilder.
    public sealed class MiniJsonBuilder : BareJsonBuilder
    {
        // "semi-singleton".
        // Note that MiniJsonBuilder does not have setters, at least at this point (although the Ctors take different args),
        //    and it can be made immutable, or we can just use it as a singleton (by changing Ctors).
        // But, that may change in the future. 
        //    So, just use this special instance sort of as a singleton for now, which is immutable (currently).
        // On second thought, MiniJsonBuilder may not be entirely multi-thread/concurrency safe...
        //    (Just to be symmetric with MiniJsonParser, which is not thread safe, do not use this...)
        public static readonly MiniJsonBuilder DEFAULT_INSTANCE = new MiniJsonBuilder(new SimpleJsonBuilder(null, true));

        // Delegate the implementation through decorator-like pattern.
        // private JsonBuilderFactory jsonBuilderFactory;
        private readonly BareJsonBuilder decoratedBuilder;

        // TBD:
        // Builder policy???
        // ...

        public MiniJsonBuilder() 
            : this(new SimpleJsonBuilder())
        {
        }
        public MiniJsonBuilder(BareJsonBuilderFactory jsonBuilderFactory) 
            : this(jsonBuilderFactory.CreateBuilder())
        {
        }
        public MiniJsonBuilder(BareJsonBuilder decoratedBuilder)
        {
            this.decoratedBuilder = decoratedBuilder;
        }


        /// <summary>
        /// Generates a JSON string of the given object.
        /// </summary>
        public async Task<string> BuildAsync(object jsonObj)
        {
            return await decoratedBuilder.BuildAsync(jsonObj);
        }
        public async Task<string> BuildAsync(object jsonObj, int indent)
        {
            return await decoratedBuilder.BuildAsync(jsonObj, indent);
        }
        public async Task BuildAsync(TextWriter writer, object jsonObj)
        {
            await decoratedBuilder.BuildAsync(writer, jsonObj);
        }
        public async Task BuildAsync(TextWriter writer, object jsonObj, int indent)
        {
            await decoratedBuilder.BuildAsync(writer, jsonObj, indent);
        }

        public async Task<object> BuildJsonStructureAsync(object jsonObj)
        {
            return await decoratedBuilder.BuildJsonStructureAsync(jsonObj);
        }
        public async Task<object> BuildJsonStructureAsync(object jsonObj, int depth)
        {
            return await decoratedBuilder.BuildJsonStructureAsync(jsonObj, depth);
        }

    }

}