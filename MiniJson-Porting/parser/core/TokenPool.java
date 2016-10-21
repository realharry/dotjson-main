package org.minijson.parser.core;

import static org.minijson.parser.core.TokenTypes.BOOLEAN;
import static org.minijson.parser.core.TokenTypes.COLON;
import static org.minijson.parser.core.TokenTypes.COMMA;
import static org.minijson.parser.core.TokenTypes.EOF;
import static org.minijson.parser.core.TokenTypes.LCURLY;
import static org.minijson.parser.core.TokenTypes.LSQUARE;
import static org.minijson.parser.core.TokenTypes.NULL;
import static org.minijson.parser.core.TokenTypes.RCURLY;
import static org.minijson.parser.core.TokenTypes.RSQUARE;

import java.util.HashMap;
import java.util.Map;
import java.util.logging.Level;
import java.util.logging.Logger;

import org.minijson.common.Literals;
import org.minijson.common.Symbols;


/**
 * Token "pool", to reduce the memory footprint while parsing a large JSON file.
 * Note that this singleton can be shared across different/multiple parsing operations....
 */
// TBD:
// Note that this can be a bit of problem when we have tokens with long string, etc.   ????
public final class TokenPool
{
    private static final Logger log = Logger.getLogger(TokenPool.class.getName());

    public static final JsonToken TOKEN_EOF = new JsonToken(EOF, null);
    public static final JsonToken TOKEN_NULL = new JsonToken(NULL, null);   // null or "null" (Literals.NULL) ???
    // public static final JsonToken TOKEN_TRUE = new JsonToken(TRUE, null);
    // public static final JsonToken TOKEN_FALSE = new JsonToken(FALSE, null);
    public static final JsonToken TOKEN_COMMA = new JsonToken(COMMA, Symbols.COMMA);
    public static final JsonToken TOKEN_COLON = new JsonToken(COLON, Symbols.COLON);
    public static final JsonToken TOKEN_LSQUARE = new JsonToken(LSQUARE, Symbols.LSQUARE);
    public static final JsonToken TOKEN_RSQUARE = new JsonToken(RSQUARE, Symbols.RSQUARE);
    public static final JsonToken TOKEN_LCURLY = new JsonToken(LCURLY, Symbols.LCURLY);
    public static final JsonToken TOKEN_RCURLY = new JsonToken(RCURLY, Symbols.RCURLY);
    public static final JsonToken TOKEN_TRUE = new JsonToken(BOOLEAN, true);     // Is it true or "true" (Literals.TRUE) ???
    public static final JsonToken TOKEN_FALSE = new JsonToken(BOOLEAN, false);   // ditto...  Note that the usage should be consistent across different classes.
    // public static final JsonToken TOKEN_NUMBER = new JsonToken(NUMBER, null);
    // public static final JsonToken TOKEN_STRING = new JsonToken(STRING, null);


//    // one-char tokens.
//    private final static Map<Character, JsonToken> symbolTokens;
//    static {
//        symbolTokens = new HashMap<>();
//        symbolTokens.put(Symbols.COMMA, TOKEN_COMMA);
//        symbolTokens.put(Symbols.COLON, TOKEN_COLON);
//        symbolTokens.put(Symbols.LSQUARE, TOKEN_LSQUARE);
//        symbolTokens.put(Symbols.RSQUARE, TOKEN_RSQUARE);
//        symbolTokens.put(Symbols.LCURLY, TOKEN_LCURLY);
//        symbolTokens.put(Symbols.RCURLY, TOKEN_RCURLY);
//        // symbolTokens.put(Symbols.DQUOTE, TOKEN_STRING);
//        // symbolTokens.put(Symbols.NULL_START, TOKEN_NULL);
//        // symbolTokens.put(Symbols.TRUE_START, TOKEN_TRUE);
//        // symbolTokens.put(Symbols.FALSE_START, TOKEN_FALSE);
//    }
    
    // Token Pool.
    private final Map<Integer, JsonToken> tokenPool; 
    private TokenPool()
    {
        tokenPool = new HashMap<Integer, JsonToken>();
        tokenPool.put(TOKEN_EOF.hashCode(), TOKEN_EOF);
        tokenPool.put(TOKEN_NULL.hashCode(), TOKEN_NULL);
        tokenPool.put(TOKEN_COMMA.hashCode(), TOKEN_COMMA);
        tokenPool.put(TOKEN_COLON.hashCode(), TOKEN_COLON);
        tokenPool.put(TOKEN_LSQUARE.hashCode(), TOKEN_LSQUARE);
        tokenPool.put(TOKEN_RSQUARE.hashCode(), TOKEN_RSQUARE);
        tokenPool.put(TOKEN_LCURLY.hashCode(), TOKEN_LCURLY);
        tokenPool.put(TOKEN_RCURLY.hashCode(), TOKEN_RCURLY);
        tokenPool.put(TOKEN_TRUE.hashCode(), TOKEN_TRUE);
        tokenPool.put(TOKEN_FALSE.hashCode(), TOKEN_FALSE);
        // etc...
    }

    // Initialization-on-demand holder.
    private static final class TokenPoolHolder
    {
        private static final TokenPool INSTANCE = new TokenPool();
    }
    // Singleton method
    public static TokenPool getInstance()
    {
        return TokenPoolHolder.INSTANCE;
    }
    

    // Returns "one-char tokens".
    // Returns null (not TOKEN_NULL) if the symbol is invalid.
    public static JsonToken getSymbolToken(char symbol)
    {
//        // return symbolTokens.get(symbol);
        switch(symbol) {
        case Symbols.COMMA:
            return TOKEN_COMMA;
        case Symbols.COLON:
            return TOKEN_COLON;
        case Symbols.LSQUARE:
            return TOKEN_LSQUARE;
        case Symbols.RSQUARE:
            return TOKEN_RSQUARE;
        case Symbols.LCURLY:
            return TOKEN_LCURLY;
        case Symbols.RCURLY:
            return TOKEN_RCURLY;
        default:
            return null;
        }
    }
    public static JsonToken getStockToken(int type, Object value)
    {
        switch(type) {
        case EOF:
            return TOKEN_EOF;
        case NULL:
            return TOKEN_NULL;
        case COMMA:
            return TOKEN_COMMA;
        case COLON:
            return TOKEN_COLON;
        case LSQUARE:
            return TOKEN_LSQUARE;
        case RSQUARE:
            return TOKEN_RSQUARE;
        case LCURLY:
            return TOKEN_LCURLY;
        case RCURLY:
            return TOKEN_RCURLY;
        // default:
        //     // return null;
        }
        if(type == BOOLEAN) {
            return getBooleanToken(value);
        }
        return null;
    }
    // The value should be String "true" or "false" or boolean true or false.
    public static JsonToken getBooleanToken(Object value)
    {
        // value == true or "true" ???
        // We use the boolean actually, but just to be safe, we check the string "true" as well...
        if(Boolean.TRUE.equals(value) || Literals.TRUE.equals(value)) {
            return TOKEN_TRUE;
        } else {
            // Validate ???
            return TOKEN_FALSE;
        }
    }


    // Returns the token from the pool, if any.
    // Otherwise create a new one and put it in the pool.
    // (Note: this is currently used for String and Number types only.
    //        Other types are processed before this method is called.
    //        Cf. AbstractJsonTokenizer.)
    public JsonToken getToken(int type, Object value)
    {
        JsonToken token = getStockToken(type, value);
        if(token != null) {
            return token;
        }
        if(! TokenTypes.isValid(type)) {
            return TOKEN_NULL;    // ???
        } else {
            int h = JsonToken.getHashCode(type, value);
            // if(log.isLoggable(Level.FINER)) log.finer(">>>>>>>>>>>>>>>>>>>>>>>>>> h = " + h);
            JsonToken tok = null;
            if(tokenPool.containsKey(h)) {
                tok = tokenPool.get(h);
                // if(log.isLoggable(Level.FINE)) log.fine(">>>>>>>>>>>>>>>>>>>>>>>>>> hash code key h found in the token pool = " + h + "; tok = " + tok);
                // TBD: What happens if the tok is an incorrect token?
                // Hash collision can return wrong tokens....
            } else {
                tok = new JsonToken(type, value);
                tokenPool.put(tok.hashCode(), tok);
                // if(log.isLoggable(Level.FINE)) log.fine(">>>>>>>>>>>>>>>>>>>>>>>>>> new token created for hash code key h = " + h + "; tok = " + tok);
            }
            return tok;
        }
    }
    
}
