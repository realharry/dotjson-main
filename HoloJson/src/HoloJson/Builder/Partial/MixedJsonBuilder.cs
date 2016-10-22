using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HoloJson.Builder.Partial
{
    /// <summary>
    /// "Partial" JSON String builder.
    /// It builds JSON string down to the given depth.
    /// Any structure below the depth is evaluated as a JSON string.
    /// </summary>
    // TBD: Rename it to HybridJsonBuilder ???
    public interface MixedJsonBuilder : JsonBuilder // JsonStructureBuilder
    {
        // Any sub-tree structure below depth is ignored. ???? --> Not true..
        // Any string at the depth level ??? Or any string at or below the depth level  ????
        //      is interpreted as JSON string (representing a sub-tree)
        //      rather than a string.
        Task<string> BuildMixedAsync(object jsonObj);
        // String buildMixedAsync(Object jsonObj, int minDepth) throws JsonBuilderException;
        Task<string> BuildMixedAsync(object jsonObj, int minDepth, int maxDepth);
        Task<string> BuildMixedAsync(object jsonObj, int minDepth, int maxDepth, int indent);
        Task buildMixedAsync(TextWriter writer, object jsonObj);
        // void buildMixedAsync(Writer writer, Object jsonObj, int minDepth) throws JsonBuilderException, IOException;
        Task buildMixedAsync(TextWriter writer, object jsonObj, int minDepth, int maxDepth);
        Task buildMixedAsync(TextWriter writer, object jsonObj, int minDepth, int maxDepth, int indent);

    }

}