using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HoloJson.Builder
{
    /// <summary>
    /// Builds "JSON compatible" object structure comprising Map and List.
    /// </summary>
    public interface JsonStructureBuilder
    {
        /////////////////////////////////////////////////////////////////
        // The following corresponds to the methods in JsonCompatible

        /// <summary>
        /// Converts the object to a nested structure of Map/List, object/JsonCompatible, and JsonSerializable + primitive types.
        /// Uses the default depth of the object (not necessarily 0).
        /// Note that the return value (structure) is either Map<String,Object> or List<Object>. </summary>
        /// <param name="jsonObj"> </param>
        /// <returns> A nested structure of Map/List. </returns>
        /// <exception cref="JsonBuilderException"> </exception>
        Task<object> BuildJsonStructureAsync(object jsonObj);

        /// <summary>
        /// Traverses down to the depth level (in terms of Object, Map, List).
        /// 1 means this object only. No map, list, other object traversal (even it it's its own fields).
        /// 0 means no introspection.
        /// depth is always additive during traversal.
        /// that is, if depth is 3, then object -> object -> object -> primitive only. </summary>
        /// <param name="jsonObj"> </param>
        /// <param name="depth"> Traversal depth. </param>
        /// <returns> A nested structure of Map/List. </returns>
        /// <exception cref="JsonBuilderException"> </exception>
        Task<object> BuildJsonStructureAsync(object jsonObj, int depth);
        // Map<String,Object> ToJsonObject();   // Reserved for later. (e.g., JsonObject ToJsonObject())

    }

}