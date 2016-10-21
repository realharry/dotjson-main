package org.minijson.parser;

import org.minijson.parser.core.JsonToken;
import org.minijson.parser.exception.JsonTokenizerException;


/**
 * Json Tokenizer base interface.
 */
public interface JsonTokenizer
{
    /**
     * Returns true if there is more tokens in the stream.
     * @return true if there is more tokens.
     * @throws JsonTokenizerException
     */
    boolean hasMore() throws JsonTokenizerException;
    
    /**
     * Returns the next token.
     * @return the next token in the stream.
     * @throws JsonTokenizerException
     */
    JsonToken next() throws JsonTokenizerException;
    
    /**
     * Peeks the next token in the stream.
     * @return the next token, without removing it from the stream.
     * @throws JsonTokenizerException
     */
    JsonToken peek() throws JsonTokenizerException;

}
