package org.minijson.builder.partial;

import java.io.IOException;
import java.io.Writer;

import org.minijson.builder.JsonBuilder;
import org.minijson.builder.JsonBuilderException;


/**
 * "Partial" JSON String builder.
 * It builds JSON string down to the given depth.
 * Any structure below the depth is evaluated as a JSON string.
 */
// TBD: Rename it to HybridJsonBuilder ???
public interface MixedJsonBuilder extends JsonBuilder   // JsonStructureBuilder
{
    // Any sub-tree structure below depth is ignored. ???? --> Not true..
    // Any string at the depth level ??? Or any string at or below the depth level  ????
    //      is interpreted as JSON string (representing a sub-tree)
    //      rather than a string.
    String buildMixed(Object jsonObj) throws JsonBuilderException;
    // String buildMixed(Object jsonObj, int minDepth) throws JsonBuilderException;
    String buildMixed(Object jsonObj, int minDepth, int maxDepth) throws JsonBuilderException;
    String buildMixed(Object jsonObj, int minDepth, int maxDepth, int indent) throws JsonBuilderException;
    void buildMixed(Writer writer, Object jsonObj) throws JsonBuilderException, IOException;
    // void buildMixed(Writer writer, Object jsonObj, int minDepth) throws JsonBuilderException, IOException;
    void buildMixed(Writer writer, Object jsonObj, int minDepth, int maxDepth) throws JsonBuilderException, IOException;
    void buildMixed(Writer writer, Object jsonObj, int minDepth, int maxDepth, int indent) throws JsonBuilderException, IOException;

}
