using HoloJson.Mini;
using HoloJson.Trait;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HoloJson.Base
{
    // Convenience class to be used as a base class for JsonSerializable classes.
    public abstract class BaseJsonBuildable : JsonBuildable
    {
        // Lazy initialized.
        private MiniJsonBuilder miniJsonBuilder = null;

        public BaseJsonBuildable()
        {
        }

        protected internal virtual MiniJsonBuilder JsonBuilder
        {
            get
            {
                if (miniJsonBuilder == null) {
                    miniJsonBuilder = new MiniJsonBuilder();
                }
                return miniJsonBuilder;
            }
        }

        public async Task<string> ToJsonStringAsync()
        {
            // return MiniJsonBuilder.DEFAULT_INSTANCE.build(this);
            return await JsonBuilder.BuildAsync(this);
        }

        public async Task<string> ToJsonStringAsync(int indent)
        {
            // return MiniJsonBuilder.DEFAULT_INSTANCE.build(this, indent);
            return await JsonBuilder.BuildAsync(this, indent);
        }

        public async Task WriteJsonStringAsync(TextWriter writer)
        {
            // MiniJsonBuilder.DEFAULT_INSTANCE.build(writer, this);
            await JsonBuilder.BuildAsync(writer, this);
        }

        public async Task WriteJsonStringAsync(TextWriter writer, int indent)
        {
            // MiniJsonBuilder.DEFAULT_INSTANCE.build(writer, this, indent);
            await JsonBuilder.BuildAsync(writer, this, indent);
        }

        public async Task<object> ToJsonStructureAsync()
        {
            // return MiniJsonBuilder.DEFAULT_INSTANCE.buildJsonStructure(this);
            return await JsonBuilder.BuildJsonStructureAsync(this);
        }

        public async Task<object> ToJsonStructureAsync(int depth)
        {
            // return MiniJsonBuilder.DEFAULT_INSTANCE.buildJsonStructure(this, depth);
            return await JsonBuilder.BuildJsonStructureAsync(this, depth);
        }

    }

}