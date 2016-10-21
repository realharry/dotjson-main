using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HoloJson.Trait
{
    // Note that if an object implements both JsonSerializable and JsonCompatbile,
    // JsonSerializable is used during JSON generation.
    // Internally the object may reply on JsonCompatbile.toJsonStructure()
    // to implement JsonSerializable.toJsonString().

    public interface JsonCompatible
    {
        // We allow an object to be represented as a Json array ([]) rather than an object ({}).
        // E.g., when an object's primary purpose is to wrap a list.
        // This also applies to Java collection List<>.
        // toJsonStructure() returns Object,
        // depending on isJsonStructureArray(), the returned object can be cast to either List<Object> or Map<String,Object>.
        // boolean isJsonStructureArray();

        /// <summary>
        /// Converts this object to a nested structure of Map/List, object/JsonCompatible, and JsonSerializable + primitive types.
        /// Uses the default depth of the object (not necessarily 0).
        /// Note that the default depth is an inherent property(a fixed number) for a given type, class.
        /// It is not something that can vary per object or, that can be dynamically changed.
        /// This is important because we want to be "consistent"
        ///    especially when an object implements both JsonSerializable and JsonCompatible.
        /// </summary>
        /// <returns>A "JSON structure" of this object. </returns>
        object ToJsonStructure();

        /// <summary>
        /// Traverses down to the depth level (in terms of Object, Map, List).
        /// 1 means this object only. No map, list, other object traversal (even it it's its own fields).
        /// 0 means no introspection.
        /// depth is always additive during traversal.
        /// that is, if depth is 3, then object -> object -> object -> primitive only. 
        /// Since we want to be generally consistent when generating a json string,
        ///    this method should not be normally used for objects that implements JsonSerializable.
        /// Even when the object does not implement JsonSerialiable,
        ///    this object's sub-json representation (e.g., as a child/grandchild of another object/map/list)
        ///    may change from objects (parents) to objects (parents).
        ///    This may or may not be a desirable behavior. 
        ///    (If a consistent representation is required the object should implement JsonSerializable.)
        /// But, without this, we may run into the infinite depth problem.
        ///    Even if each object has a finite depth, due to dependencies (e.g., a circular dependency),
        ///    we may end up traversing object tree indefinitely.
        /// </summary>
        /// <param name="depth"Traversal depth></param>
        /// <returns>A "JSON structure" of this object. </returns>
        object ToJsonStructure(int depth);

    }
}
