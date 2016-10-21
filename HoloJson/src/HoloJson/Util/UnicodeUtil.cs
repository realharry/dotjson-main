using HoloJson.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HoloJson.Util
{
	public static class UnicodeUtil
	{
		public static bool IsUnicodeHex(char ch)
		{
			if((ch >= 'A' && ch <= 'F') || (ch >= 'a' && ch <= 'f') || (ch >= '0' && ch <= '9')) {
				return true;
			}
	//        switch(ch) {
	//        case '0':
	//        case '1':
	//        case '2':
	//        case '3':
	//        case '4':
	//        case '5':
	//        case '6':
	//        case '7':
	//        case '8':
	//        case '9':
	//        case 'a': case 'A':
	//        case 'b': case 'B':
	//        case 'c': case 'C':
	//        case 'd': case 'D':
	//        case 'e': case 'E':
	//        case 'f': case 'F':
	//            return true;
	//        }
			return false;
		}
		public static int GetIntEquivalent(char ch)
		{
			if(ch >= '0' && ch <= '9') {
				return ((int) ch) - 48;            
			} else if(ch >= 'A' && ch <= 'F') {
				return ((int) ch) - 55;    // 65 - 10
			} else if(ch >= 'a' && ch <= 'f') {
				return ((int) ch) - 87;    // 97 - 10
			}
	//        switch(ch) {
	//        case '0':
	//        case '1':
	//        case '2':
	//        case '3':
	//        case '4':
	//        case '5':
	//        case '6':
	//        case '7':
	//        case '8':
	//        case '9':
	//            return ((int) ch) - 48;
	//        case 'a':
	//        case 'b':
	//        case 'c':
	//        case 'd':
	//        case 'e':
	//        case 'f':
	//            return ((int) ch) - 87;    // 97 - 10
	//        case 'A':
	//        case 'B':
	//        case 'C':
	//        case 'D':
	//        case 'E':
	//        case 'F':
	//            return ((int) ch) - 55;    // 65 - 10
	//        }
			return 0;
		}
		
		public static char GetUnicodeChar(CyclicCharArray hex)
		{
			if(hex == null || hex.Length != 4) {
				// ???
				return (char) 0;
			}
			// return GetUnicodeCharNoCheck(hex.GetArray());
			// return GetUnicodeCharNoCheck(hex);
			return GetUnicodeCharFromHexSequence(hex);
		}

		public static char GetUnicodeChar(char[] hex)
		{
			if(hex == null || hex.Length != 4) {
				// ???
				return (char) 0;
			}
			// return GetUnicodeCharNoCheck(hex);
			return GetUnicodeCharFromHexSequence(hex);
		}


		public static char GetUnicodeCharNoCheck(CyclicCharArray hex)
		{
			char u = GetUnicodeCharFromHexSequence(hex);
			return u;
		}
		public static char GetUnicodeCharNoCheck(char[] hex)
		{
			char u = GetUnicodeCharFromHexSequence(hex);
			return u;
		}

		
		
		// TBD:
		// Need to verify this really works...
		public static char GetUnicodeCharFromHexSequence(char[] c)
		{
			// int x = ((GetIntEquivalent(c[0]) << 12) + ((GetIntEquivalent(c[1]) << 8) + ((GetIntEquivalent(c[2]) << 4) + (GetIntEquivalent(c[3]);

			int x = 0;
			for(int i=0; i<4; i++) {
				if(IsUnicodeHex(c[i])) {
					x += (GetIntEquivalent(c[i]) << (3-i)*4);
				} else {
					// ???
					return (char) 0;
				}
			}

			return (char) x;
		}
		public static char GetUnicodeCharFromHexSequence(CyclicCharArray hex)
		{
			int x = 0;
			for(int i=0; i<4; i++) {
				if(IsUnicodeHex(hex.GetChar(i))) {
					x += (GetIntEquivalent(hex.GetChar(i)) << (3-i)*4);
				} else {
					// ???
					return (char) 0;
				}
			}
			return (char) x;
		}


		private static char[] HEXNUM = "0123456789abcdef".ToArray<char>();
		public static char[] GetUnicodeHexCodeFromChar(char ch) 
		{
	//        char[] c6 = new char[6];
	//        c6[0] = '\\';
	//        c6[1] = 'u';
			char[] c6 = new char[]{'\\', 'u', '0', '0', '0', '0'};

			int code = (int) ch;
			c6[2] = HEXNUM[(code & 0xf000) >> 12];
			c6[3] = HEXNUM[(code & 0x0f00) >> 8];
			c6[4] = HEXNUM[(code & 0x00f0) >> 4];
			c6[5] = HEXNUM[(code & 0x000f)];
	//        for (int i = 0; i < 4; ++i) {
	//            int digit = (code & 0xf000) >> 12;
	//            c6[i+2] = HEXNUM[digit];
	//            code <<= 4;
	//        }

			return c6;
		}

	}
}
