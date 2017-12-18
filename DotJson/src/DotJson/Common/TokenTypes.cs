using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DotJson.Common
{
    public enum TokenType
    {
        EOF = -1,    // Not exactly a token.
        INVALID = 0,   // Not a valid token.  null.
		NULL = 1,      // JSON "null". Valid token.
		// TRUE = 2,
		// FALSE = 3,
		COMMA = 4,
		COLON = 5,
		LSQUARE = 6,
		RSQUARE = 7,
		LCURLY = 8,
		RCURLY = 9,
		BOOLEAN = 10,
		NUMBER = 11,
		STRING = 12,
		// ...

    }

    public static class TokenTypes
	{
        //public const int EOF = -1;    // Not exactly a token.
        //public const int NULL = 0;
        //// public const int TRUE = 1;
        //// public const int FALSE = 2;
        //public const int COMMA = 3;
        //public const int COLON = 4;
        //public const int LSQUARE = 5;
        //public const int RSQUARE = 6;
        //public const int LCURLY = 7;
        //public const int RCURLY = 8;
        //public const int BOOLEAN = 9;
        //public const int NUMBER = 10;
        //public const int STRING = 11;
        //// ...

		// For easy access
        private static readonly ISet<TokenType> typeSet;
		static TokenTypes() {
            typeSet = new HashSet<TokenType>();
			typeSet.Add(TokenType.EOF);
            // No INVALID!
			typeSet.Add(TokenType.NULL);
			typeSet.Add(TokenType.COMMA);
			typeSet.Add(TokenType.COLON);
			typeSet.Add(TokenType.LSQUARE);
			typeSet.Add(TokenType.RSQUARE);
			typeSet.Add(TokenType.LCURLY);
			typeSet.Add(TokenType.RCURLY);
			typeSet.Add(TokenType.BOOLEAN);
			typeSet.Add(TokenType.NUMBER);
			typeSet.Add(TokenType.STRING);
			// ...
		}

        //public static bool IsValid(int type)
        //{
        //    return typeSet.Contains((TokenType) type);
        //}
        public static bool IsValid(TokenType type)
        {
            // return Enum.IsDefined(typeof(TokenType), type);
            return typeSet.Contains(type);
        }
		
		
		// for debugging purpose
        public static string GetTokenName(TokenType type)
		{
			switch(type) {
			case TokenType.EOF:
				return "eof";
			case TokenType.NULL:
				return "null";
			case TokenType.COMMA:
				return "comma";
			case TokenType.COLON:
				return "colon";
			case TokenType.LSQUARE:
				return "l-square";
			case TokenType.RSQUARE:
				return "r-square";
			case TokenType.LCURLY:
				return "l-curly";
			case TokenType.RCURLY:
				return "r-curly";
			case TokenType.BOOLEAN:
				return "boolean";
			case TokenType.NUMBER:
				return "number";
			case TokenType.STRING:
				return "string";
			default:
				return "unknown";
			}
		}
        public static string GetDisplayName(TokenType type)
		{
			switch(type) {
			case TokenType.EOF:
				return "eof";
			case TokenType.NULL:
				return "null";
			case TokenType.COMMA:
				return "comma";
			case TokenType.COLON:
				return "colon";
			case TokenType.LSQUARE:
				return "left square bracket";
			case TokenType.RSQUARE:
				return "right square bracket";
			case TokenType.LCURLY:
				return "left curly brace";
			case TokenType.RCURLY:
				return "right curly brace";
			case TokenType.BOOLEAN:
				return "boolean";
			case TokenType.NUMBER:
				return "number";
			case TokenType.STRING:
				return "string";
			default:
				return "unknown type";
			}
		}

	}
}
