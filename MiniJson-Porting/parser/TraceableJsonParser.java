package org.minijson.parser;

import org.minijson.parser.core.JsonToken;


/**
 * JsonParser with tracing/debugging information.
 */
public interface TraceableJsonParser extends JsonParser
{    
    /**
     * Enable "tracing".
     * Tracing, at this point, means that we simply keep the token/nodes in a tail buffer
     *     so that when an error occurs we can see the "exception context".
     */
    void enableTracing();
    
    /**
     * Disable "tracing".
     */
    void disableTracing();
    
    /**
     * Returns true if "tracing" is enabled.
     * 
     * @return whether "tracing" is enabled or not. 
     */
    boolean isTracingEnabled();


    /**
     * @param length Max length of the tokens to be returned from the tail buffer.
     * @return the tail part of the tail token buffer as a string.
     */
    JsonToken tailTokensAsString(int length);
    JsonToken[] tailTokenStream(int length);

    /**
     * @param length Max length of the nodes to be returned from the tail buffer.
     * @return the tail part of the tail node buffer as a string.
     */
    Object tailNodesAsString(int length);
    Object[] tailNodeStream(int length);

}
