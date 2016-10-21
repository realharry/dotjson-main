package org.minijson.parser;


/**
 * "Look ahead" tokenizing.
 * In the current implementation, "look ahead" has a particular meaning.
 */
public interface LookAheadJsonTokenizer extends JsonTokenizer
{    
   // TBD: Need a better name for this interface...

    boolean isLookAheadParsing();
//    void setLookAheadParsing(boolean lookAheadParsing);
    void enableLookAheadParsing();
    void disableLookAheadParsing();

}
