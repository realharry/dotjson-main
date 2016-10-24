using HoloJson.Mini;
using HoloJson.Trait;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HoloJson.Base
{
    // Convenience class to be used as a base class for JsonCompatible classes.
    public abstract class BaseJsonCompatible : JsonCompatible
    {
        // Lazy initialized.
        private MiniJsonBuilder miniJsonBuilder = null;

        public BaseJsonCompatible()
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