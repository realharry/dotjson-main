using HoloJson.Builder.Policy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HoloJson.Builder
{
    /// <summary>
    /// JsonBuilder with configurable options.
    /// </summary>
    public interface FlexibleJsonBuilder : JsonBuilder
    {
        /// <summary>
        /// Get the BuilderPolicy object associated with this JsonBuilder object.
        /// </summary>
        BuilderPolicy BuilderPolicy { get; }
    }

}