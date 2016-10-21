package org.minijson.parser;


/**
 * JsonTokenizer with tracing/debugging information.
 */
public interface TraceableJsonTokenizer extends JsonTokenizer
{    
    /**
     * Enable "tracing".
     * Tracing, at this point, means that we simply keep the char tail buffer
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
    
    
    // ???
    // CharBuffer getTailBuffer();

    /**
     * @param length Max length of the String/char[] to be returned from the tail buffer.
     * @return the tail part of the tail character buffer as a string.
     */
    String tailCharsAsString(int length);
    char[] tailCharStream(int length);
    
    /**
     * @param length Max length of the String/char[] to be peeked.
     * @return the character array in the stream as a string.
     */
    String peekCharsAsString(int length);
    char[] peekCharStream(int length);

}
