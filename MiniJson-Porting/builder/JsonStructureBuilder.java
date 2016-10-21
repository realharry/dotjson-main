package org.minijson.builder;


/**
 * Builds "JSON compatible" object structure comprising Map and List.
 */
public interface JsonStructureBuilder
{
    /////////////////////////////////////////////////////////////////
    // The following corresponds to the methods in JsonCompatible

    /**
     * Converts the object to a nested structure of Map/List, object/JsonCompatible, and JsonSerializable + primitive types.
     * Uses the default depth of the object (not necessarily 0).
     * Note that the return value (structure) is either Map<String,Object> or List<Object>.
     * @param jsonObj
     * @return A nested structure of Map/List.
     * @throws JsonBuilderException
     */
    Object buildJsonStructure(Object jsonObj) throws JsonBuilderException;           

    /**
     * Traverses down to the depth level (in terms of Object, Map, List).
     * 1 means this object only. No map, list, other object traversal (even it it's its own fields).
     * 0 means no introspection.
     * depth is always additive during traversal.
     * that is, if depth is 3, then object -> object -> object -> primitive only. 
     * @param jsonObj
     * @param depth Traversal depth.
     * @return A nested structure of Map/List.
     * @throws JsonBuilderException
     */
    Object buildJsonStructure(Object jsonObj, int depth) throws JsonBuilderException;
    // Map<String,Object> toJsonObject();   // Reserved for later. (e.g., JsonObject toJsonObject())

}
