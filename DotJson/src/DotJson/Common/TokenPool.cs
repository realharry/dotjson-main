using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DotJson.Common
{
	// Note that this singleton can be shared across different/multiple parsing operations....
	// TBD:
	// Note that this can be a bit of problem when we have tokens with long string, etc.   ????
	// ...
	public sealed class TokenPool
	{
		public static readonly JsonToken TOKEN_EOF = new JsonToken(TokenType.EOF, null);
		public static readonly JsonToken TOKEN_NULL = new JsonToken(TokenType.NULL, null);   // null or "null" (Literals.NULL) ???
		// public static readonly JsonToken TOKEN_TRUE = new JsonToken(TokenType.TRUE, null);
		// public static readonly JsonToken TOKEN_FALSE = new JsonToken(TokenType.FALSE, null);
		public static readonly JsonToken TOKEN_COMMA = new JsonToken(TokenType.COMMA, CharSymbol.COMMA);
		public static readonly JsonToken TOKEN_COLON = new JsonToken(TokenType.COLON, CharSymbol.COLON);
		public static readonly JsonToken TOKEN_LSQUARE = new JsonToken(TokenType.LSQUARE, CharSymbol.LSQUARE);
		public static readonly JsonToken TOKEN_RSQUARE = new JsonToken(TokenType.RSQUARE, CharSymbol.RSQUARE);
		public static readonly JsonToken TOKEN_LCURLY = new JsonToken(TokenType.LCURLY, CharSymbol.LCURLY);
		public static readonly JsonToken TOKEN_RCURLY = new JsonToken(TokenType.RCURLY, CharSymbol.RCURLY);
		public static readonly JsonToken TOKEN_TRUE = new JsonToken(TokenType.BOOLEAN, true);     // Is it true or "true" (Literals.TRUE) ???
		public static readonly JsonToken TOKEN_FALSE = new JsonToken(TokenType.BOOLEAN, false);   // ditto...  Note that the usage should be consistent across different classes.
		// public static readonly JsonToken TOKEN_NUMBER = new JsonToken(TokenType.NUMBER, null);
		// public static readonly JsonToken TOKEN_STRING = new JsonToken(TokenType.STRING, null);


	//    // one-char tokens.
	//    private readonly static IDictionary<char, JsonToken> symbolTokens;
	//    static {
	//        symbolTokens = new Dictionary<>();
	//        symbolTokens.Add(CharSymbol.COMMA, TOKEN_COMMA);
	//        symbolTokens.Add(CharSymbol.COLON, TOKEN_COLON);
	//        symbolTokens.Add(CharSymbol.LSQUARE, TOKEN_LSQUARE);
	//        symbolTokens.Add(CharSymbol.RSQUARE, TOKEN_RSQUARE);
	//        symbolTokens.Add(CharSymbol.LCURLY, TOKEN_LCURLY);
	//        symbolTokens.Add(CharSymbol.RCURLY, TOKEN_RCURLY);
	//        // symbolTokens.Add(CharSymbol.DQUOTE, TOKEN_STRING);
	//        // symbolTokens.Add(CharSymbol.NULL_START, TOKEN_NULL);
	//        // symbolTokens.Add(CharSymbol.TRUE_START, TOKEN_TRUE);
	//        // symbolTokens.Add(CharSymbol.FALSE_START, TOKEN_FALSE);
	//    }
		
		// Token Pool.
		private readonly IDictionary<int, JsonToken> tokenPool; 
		private TokenPool()
		{
			tokenPool = new Dictionary<int, JsonToken>();
			tokenPool.Add(TOKEN_EOF.GetHashCode(), TOKEN_EOF);
			tokenPool.Add(TOKEN_NULL.GetHashCode(), TOKEN_NULL);
			tokenPool.Add(TOKEN_COMMA.GetHashCode(), TOKEN_COMMA);
			tokenPool.Add(TOKEN_COLON.GetHashCode(), TOKEN_COLON);
			tokenPool.Add(TOKEN_LSQUARE.GetHashCode(), TOKEN_LSQUARE);
			tokenPool.Add(TOKEN_RSQUARE.GetHashCode(), TOKEN_RSQUARE);
			tokenPool.Add(TOKEN_LCURLY.GetHashCode(), TOKEN_LCURLY);
			tokenPool.Add(TOKEN_RCURLY.GetHashCode(), TOKEN_RCURLY);
			tokenPool.Add(TOKEN_TRUE.GetHashCode(), TOKEN_TRUE);
			tokenPool.Add(TOKEN_FALSE.GetHashCode(), TOKEN_FALSE);
			// etc...
		}

        // Singleton.
        public static readonly TokenPool Instance = new TokenPool();
        //public static TokenPool getInstance()
        //{
        //    return INSTANCE;
        //}
		

		// Returns "one-char tokens".
		// Returns null (not TOKEN_NULL) if the symbol is invalid.
		public static JsonToken GetSymbolToken(char symbol)
		{
	//        // return symbolTokens.get(symbol);
			switch(symbol) {
			case (char) CharSymbol.COMMA:
				return TOKEN_COMMA;
			case (char) CharSymbol.COLON:
				return TOKEN_COLON;
			case (char) CharSymbol.LSQUARE:
				return TOKEN_LSQUARE;
			case (char) CharSymbol.RSQUARE:
				return TOKEN_RSQUARE;
			case (char) CharSymbol.LCURLY:
				return TOKEN_LCURLY;
			case (char) CharSymbol.RCURLY:
				return TOKEN_RCURLY;
			default:
                // ????
                return JsonToken.INVALID;
			}
		}
        //public static JsonToken GetStockToken(int type, object value)
        public static JsonToken GetStockToken(TokenType type, object value)
        {
			switch(type) {
            case TokenType.EOF:
				return TOKEN_EOF;
            case TokenType.NULL:
				return TOKEN_NULL;
            case TokenType.COMMA:
				return TOKEN_COMMA;
            case TokenType.COLON:
				return TOKEN_COLON;
            case TokenType.LSQUARE:
				return TOKEN_LSQUARE;
            case TokenType.RSQUARE:
				return TOKEN_RSQUARE;
            case TokenType.LCURLY:
				return TOKEN_LCURLY;
            case TokenType.RCURLY:
				return TOKEN_RCURLY;
			// default:
			//     // return null;
			}
            if (type == TokenType.BOOLEAN) {
				return GetBooleanToken(value);
			}
            return JsonToken.INVALID;
		}
		// The value should be string "true" or "false" or bool true or false.
		public static JsonToken GetBooleanToken(object value)
		{
			// value == true or "true" ???
			// We use the bool actually, but just to be safe, we check the string "true" as well...
			if((value is bool && ((bool) value) == true) || Literals.TRUE.Equals(value)) {
				return TOKEN_TRUE;
			} else {
				// Validate ???
				return TOKEN_FALSE;
			}
		}


		// Returns the token from the pool, if any.
		// Otherwise create a new one and put it in the pool.
		// (Note: this is currently used for string and Number types only.
		//        Other types are processed before this method is called.
		//        Cf. AbstractJsonTokenizer.)
		// public JsonToken GetToken(int type, object value)
		public JsonToken GetToken(TokenType type, object value)
		{
			JsonToken token = GetStockToken(type, value);
            if (JsonToken.IsTokenValid(token)) {
				return token;
			}
			if(! TokenTypes.IsValid(type)) {
				return TOKEN_NULL;    // ???
			} else {
				int h = JsonToken.BuildHashCode(type, value);
				// if(log.isLoggable(Level.FINER)) log.finer(">>>>>>>>>>>>>>>>>>>>>>>>>> h = " + h);
                JsonToken tok = JsonToken.INVALID;
				if(tokenPool.ContainsKey(h)) {
					tok = tokenPool[h];
					// if(log.isLoggable(Level.FINE)) log.fine(">>>>>>>>>>>>>>>>>>>>>>>>>> hash code key h found in the token pool = " + h + "; tok = " + tok);
					// TBD: What happens if the tok is an incorrect token?
					// Hash collision can return wrong tokens....
				} else {
					tok = new JsonToken(type, value);
					tokenPool.Add(tok.GetHashCode(), tok);
					// if(log.isLoggable(Level.FINE)) log.fine(">>>>>>>>>>>>>>>>>>>>>>>>>> new token created for hash code key h = " + h + "; tok = " + tok);
				}
				return tok;
			}
		}
		
	}
}
