using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HoloJson.Common
{
    // tbd: is this enum necessary?
    // Just use char?
    public enum CharSymbol
    {
    	// Chars that have special meanings to the tokenizer...
		COMMA = ',',
		COLON = ':',
		LSQUARE = '[',
		RSQUARE = ']',
		LCURLY = '{',
		RCURLY = '}',
		// ...
		DQUOTE = '"',
		// ...
		BACKSLASH = '\\',
		// ...
		SLASH = '/',
		BACKSPACE = 'b',
		FORMFEED = 'f',
		NEWLINE = 'n',
		RETURN = 'r',
		TAB = 't',
		// ...
		PLUS = '+', 
		MINUS = '-',    // Or, dash.
		PERIOD = '.',   // Or, dot, decimal point.
		EXP_LOWER = 'e',    // for numbers.
		EXP_UPPER = 'E',    // for numbers.
		// ...
		ESCAPED_DQUOTE = '\"',
		ESCAPED_BACKSLASH = '\\',
		// ESCAPED_SLASH = '/',
		ESCAPED_BACKSPACE = '\b',
		ESCAPED_FORMFEED = '\f',
		ESCAPED_NEWLINE = '\n',
		ESCAPED_RETURN = '\r',
		ESCAPED_TAB = '\t',
		// ...
		// ...
		UNICODE_PREFIX = 'u',
        // ...

    }

    public enum MarkerChar
    {
		NULL_START = 'n',
		TRUE_START = 't',
		FALSE_START = 'f',
		// these are used only when parserPolicy.caseInsensitiveLiterals == true...
		NULL_START_UPPER = 'N',
		TRUE_START_UPPER = 'T',
		FALSE_START_UPPER = 'F',
        //// ...

    }


    // "Special" chars for tokenizer.
	public sealed class Symbols
	{
		private Symbols() {}

        //// Chars that have special meanings to the tokenizer...
        public const char COMMA = (char) CharSymbol.COMMA;
        public const char COLON = (char)CharSymbol.COLON;
        public const char LSQUARE = (char)CharSymbol.LSQUARE;
        public const char RSQUARE = (char)CharSymbol.RSQUARE;
        public const char LCURLY = (char)CharSymbol.LCURLY;
        public const char RCURLY = (char)CharSymbol.RCURLY;
        // ...
        public const char DQUOTE = (char)CharSymbol.DQUOTE;
        // ...
        public const char BACKSLASH = (char)CharSymbol.BACKSLASH;
        // ...
        public const char SLASH = (char)CharSymbol.SLASH;
        public const char BACKSPACE = (char)CharSymbol.BACKSPACE;
        public const char FORMFEED = (char)CharSymbol.FORMFEED;
        public const char NEWLINE = (char)CharSymbol.NEWLINE;
        public const char RETURN = (char)CharSymbol.RETURN;
        public const char TAB = (char)CharSymbol.TAB;
        // ...
        public const char PLUS = (char)CharSymbol.PLUS;
        public const char MINUS = (char)CharSymbol.MINUS;    // Or, dash.
        public const char PERIOD = (char)CharSymbol.PERIOD;   // Or, dot, decimal point.
        public const char EXP_LOWER = (char)CharSymbol.EXP_LOWER;    // for numbers.
        public const char EXP_UPPER = (char)CharSymbol.EXP_UPPER;    // for numbers.
        // ...
        public const char ESCAPED_DQUOTE = (char)CharSymbol.ESCAPED_DQUOTE;
        public const char ESCAPED_BACKSLASH = (char)CharSymbol.ESCAPED_BACKSLASH;
        // public const char ESCAPED_SLASH = '/';
        public const char ESCAPED_BACKSPACE = (char)CharSymbol.ESCAPED_BACKSPACE;
        public const char ESCAPED_FORMFEED = (char)CharSymbol.ESCAPED_FORMFEED;
        public const char ESCAPED_NEWLINE = (char)CharSymbol.ESCAPED_NEWLINE;
        public const char ESCAPED_RETURN = (char)CharSymbol.ESCAPED_RETURN;
        public const char ESCAPED_TAB = (char)CharSymbol.ESCAPED_TAB;
        // ...


        public const string ESCAPED_DQUOTE_STR = "\\\"";
		public const string ESCAPED_BACKSLASH_STR = "\\\\";
		// Note that forward slash need not be escaped in Java.
		// The following is used to generate "\/" in a json string.
		public const string ESCAPED_SLASH_STR = "\\/";
		public const string SLASH_STR = "/";
		public const string ESCAPED_BACKSPACE_STR = "\\b";
		public const string ESCAPED_FORMFEED_STR = "\\f";
		public const string ESCAPED_NEWLINE_STR = "\\n";
		public const string ESCAPED_RETURN_STR = "\\r";
		public const string ESCAPED_TAB_STR = "\\t";
        // ...

        // ...
        public const char NULL_START = 'n';
        public const char TRUE_START = 't';
        public const char FALSE_START = 'f';
        // these are used only when parserPolicy.caseInsensitiveLiterals == true...
        public const char NULL_START_UPPER = 'N';
        public const char TRUE_START_UPPER = 'T';
        public const char FALSE_START_UPPER = 'F';

        public const char UNICODE_PREFIX = (char)CharSymbol.UNICODE_PREFIX;
        // ...

        // number?
        // ...

        // For easy access
        private static readonly ISet<CharSymbol> symbolSet;
        private static readonly ISet<CharSymbol> escapables;
        private static readonly ISet<CharSymbol> escaped;

        static Symbols()
        {
            symbolSet = new HashSet<CharSymbol>();
			symbolSet.Add(CharSymbol.COMMA);
			symbolSet.Add(CharSymbol.COLON);
			symbolSet.Add(CharSymbol.LSQUARE);
			symbolSet.Add(CharSymbol.RSQUARE);
			symbolSet.Add(CharSymbol.LCURLY);
			symbolSet.Add(CharSymbol.RCURLY);
			symbolSet.Add(CharSymbol.DQUOTE);
			// etc...
			// ...
            escapables = new HashSet<CharSymbol>();
			escapables.Add(CharSymbol.DQUOTE);
			escapables.Add(CharSymbol.BACKSLASH);
			escapables.Add(CharSymbol.SLASH);
			escapables.Add(CharSymbol.BACKSPACE);
			escapables.Add(CharSymbol.FORMFEED);
			escapables.Add(CharSymbol.NEWLINE);
			escapables.Add(CharSymbol.RETURN);
			escapables.Add(CharSymbol.TAB);
			escapables.Add(CharSymbol.UNICODE_PREFIX);    // ???

            escaped = new HashSet<CharSymbol>();
			escaped.Add(CharSymbol.ESCAPED_DQUOTE);
			escaped.Add(CharSymbol.ESCAPED_BACKSLASH);
			// ???
			// escaped.Add(CharSymbol.ESCAPED_SLASH);
			escaped.Add(CharSymbol.SLASH);
			// ???
			escaped.Add(CharSymbol.ESCAPED_BACKSPACE);
			escaped.Add(CharSymbol.ESCAPED_FORMFEED);
			escaped.Add(CharSymbol.ESCAPED_NEWLINE);
			escaped.Add(CharSymbol.ESCAPED_RETURN);
			escaped.Add(CharSymbol.ESCAPED_TAB);

        }

		
		// ???
		// Is this being used???
        public static bool IsValidSymbol(char symbol)
        {
            return symbolSet.Contains((CharSymbol) symbol);
        }
        //public static bool IsValidSymbol(CharSymbol symbol)
        //{
        //    return symbolSet.Contains(symbol);
        //}

		public static bool IsEscapableChar(char ch) 
		{
			// return escapables.Contains(ch);
			
			switch(ch) {
			case (char) CharSymbol.DQUOTE:
            case (char) CharSymbol.BACKSLASH:
            case (char) CharSymbol.SLASH:
            case (char) CharSymbol.BACKSPACE:
            case (char) CharSymbol.FORMFEED:
            case (char) CharSymbol.NEWLINE:
            case (char) CharSymbol.RETURN:
            case (char) CharSymbol.TAB:
            case (char) CharSymbol.UNICODE_PREFIX:    // ???
			   return true;     
			}
			return false;
		}

		public static char GetEscapedChar(char ch)
		{
	//        if(escapedCharIDictionary.ContainsKey(ch)) {
	//            return escapedCharIDictionary.get(ch);
	//        } else {
	//            // ???
	//            return 0;
	//        }
			
			switch(ch) {
			case (char) CharSymbol.DQUOTE:
                return (char) CharSymbol.ESCAPED_DQUOTE;
			case (char) CharSymbol.BACKSLASH:
				// return BACKSLASH;
                return (char) CharSymbol.ESCAPED_BACKSLASH;
			// Note that during parsing, we do not escape slash.
			// That is, if string has "...\/...", then is converted to ".../...".
			case (char) CharSymbol.SLASH:
                return (char) CharSymbol.SLASH;
			case (char) CharSymbol.BACKSPACE:
                return (char) CharSymbol.ESCAPED_BACKSPACE;
			case (char) CharSymbol.FORMFEED:
                return (char) CharSymbol.ESCAPED_FORMFEED;
			case (char) CharSymbol.NEWLINE:
                return (char) CharSymbol.ESCAPED_NEWLINE;
			case (char) CharSymbol.RETURN:
                return (char) CharSymbol.ESCAPED_RETURN;
			case (char) CharSymbol.TAB:
                return (char) CharSymbol.ESCAPED_TAB;
			}
			// ???
			return (char) 0;
		}


		public static bool IsEscapedChar(char ch) 
		{
			// return escaped.Contains(ch);

			// Note that this is much more efficient than escaped.Contains(ch), which requires boxing, etc.
			switch(ch) {
			case (char) CharSymbol.ESCAPED_DQUOTE:
			case (char) CharSymbol.ESCAPED_BACKSLASH:
			// ???
			// Note we use slash withouth "escaping".
			// case (char) CharSymbol.ESCAPED_SLASH:
			case (char) CharSymbol.SLASH:
			case (char) CharSymbol.ESCAPED_BACKSPACE:
			case (char) CharSymbol.ESCAPED_FORMFEED:
			case (char) CharSymbol.ESCAPED_NEWLINE:
			case (char) CharSymbol.ESCAPED_RETURN:
			case (char) CharSymbol.ESCAPED_TAB:
				return true;
			}

			return false;    
		}
		public static string GetEscapedCharString(char ch)
		{
			return GetEscapedCharString(ch, false);
		}
		public static string GetEscapedCharString(char ch, bool escapeForwardSlash)
		{
			switch(ch) {
            case (char) CharSymbol.ESCAPED_DQUOTE:
				return ESCAPED_DQUOTE_STR;
            case (char) CharSymbol.ESCAPED_BACKSLASH:
				return ESCAPED_BACKSLASH_STR;
			// ????
			// case ESCAPED_SLASH:                          // ????
			//     return SLASH_STR;
			//     // return ESCAPED_SLASH_STR;
			// For JSON generation, slash may be converted to "\/" if escapeForwardSlash==true.
			// Note that, at a JsonBuilder level, "</" is always converted to "<\/" regardless of this flag.
            case (char) CharSymbol.SLASH:
				if(escapeForwardSlash) {
					return ESCAPED_SLASH_STR;
				} else {
					return SLASH_STR;
				}
			// ????
            case (char) CharSymbol.ESCAPED_BACKSPACE:
				return ESCAPED_BACKSPACE_STR;
            case (char) CharSymbol.ESCAPED_FORMFEED:
				return ESCAPED_FORMFEED_STR;
            case (char) CharSymbol.ESCAPED_NEWLINE:
				return ESCAPED_NEWLINE_STR;
            case (char) CharSymbol.ESCAPED_RETURN:
				return ESCAPED_RETURN_STR;
            case (char) CharSymbol.ESCAPED_TAB:
				return ESCAPED_TAB_STR;
			}
			// ???
			return null;
		}


		public static bool IsStartingNumber(char ch)
		{
			// We allow numbers like ".123" or "+3" although JSON.org grammar states they should be "0.123" or "3", respectively.
            if (ch == (char) CharSymbol.MINUS ||
                    ch == (char) CharSymbol.PLUS ||
                    ch == (char) CharSymbol.PERIOD || 
					Char.IsDigit(ch)) {
				return true;
			} else {
				return false;
			}
		}
		
		public static bool IsExponentChar(char ch)
		{
			switch(ch) {
            case (char) CharSymbol.EXP_LOWER:
            case (char) CharSymbol.EXP_UPPER:
				return true;
			default:
				return false;
			}
		}

	}
}
