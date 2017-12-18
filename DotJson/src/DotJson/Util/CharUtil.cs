using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DotJson.Util
{
	public static class CharUtil
	{
		// Note: http://en.wikipedia.org/wiki/Whitespace_character
		public static bool IsWhitespace(char ch)
		{
			// return char.IsWhitespace(ch);
			
			switch(ch) {
			case ' ':
			case '\t':
			case '\n':
			case '\r':
			// what else?
				return true;
			}
			return false;        
		}
		
		public static bool IsISOControl(char ch)
		{
			// return char.IsISOControl(ch);
			
			int code = (int) ch;
			if((code >= 0x0 && code <= 0x1f) || (code >= 0x7f && code <= 0x9f)) {
				return true;
			} else {
				return false;
			}
		}

	}
}
