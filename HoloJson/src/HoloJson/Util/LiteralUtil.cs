using HoloJson.Common;
using HoloJson.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HoloJson.Util
{
	public static class LiteralUtil
	{
		private static readonly char[] NULL_CHARS = new char[]{'n','u','l','l'};
		private static readonly char[] TRUE_CHARS = new char[]{'t','r','u','e'};
		private static readonly char[] FALSE_CHARS = new char[]{'f','a','l','s','e'};
		private static readonly char[] NULL_CHARS_UPPER = new char[]{'N','U','L','L'};
		private static readonly char[] TRUE_CHARS_UPPER = new char[]{'T','R','U','E'};
		private static readonly char[] FALSE_CHARS_UPPER = new char[]{'F','A','L','S','E'};

		
		// Convenience methods.
		public static bool IsNull(char[] c)
		{
			if(c == null || c.Length != Literals.NULL_LENGTH) {
				return false;
			} else {
                for (int i = 0; i < Literals.NULL_LENGTH; i++) {
					// if(c[i] != NULL.charAt(i)) {
					if(c[i] != NULL_CHARS[i]) {
						return false;
					}
				}
			}
			return true;
		}
		public static bool IsNullIgnoreCase(char[] c)
		{
            if (c == null || c.Length != Literals.NULL_LENGTH) {
				return false;
			} else {
	//            string str = new String(c);
	//            // Which is better?
	//            // return str.toLowerCase().Equals(NULL);
	//            return str.equalsIgnoreCase(NULL);
				for(int i=0; i<Literals.NULL_LENGTH; i++ ) {
					// if(c[i] != NULL.charAt(i)) {
					if((c[i] != NULL_CHARS[i] || c[i] != NULL_CHARS_UPPER[i])) {
						return false;
					}
				}
			}
			return true;
		}
		public static bool IsTrue(char[] c)
		{
			if(c == null || c.Length != Literals.TRUE_LENGTH) {
				return false;
			} else {
				for(int i=0; i<Literals.TRUE_LENGTH; i++ ) {
					// if(c[i] != TRUE.charAt(i)) {
					if(c[i] != TRUE_CHARS[i]) {
						return false;
					}
				}
			}
			return true;
		}
		public static bool IsTrueIgnoreCase(char[] c)
		{
			if(c == null || c.Length != Literals.TRUE_LENGTH) {
				return false;
			} else {
	//            string str = new String(c);
	//            // Which is better?
	//            // return str.toLowerCase().Equals(TRUE);
	//            return str.equalsIgnoreCase(TRUE);
				for(int i=0; i<Literals.TRUE_LENGTH; i++ ) {
					// if(c[i] != TRUE.charAt(i)) {
					if((c[i] != TRUE_CHARS[i] || c[i] != TRUE_CHARS_UPPER[i])) {
						return false;
					}
				}
			}
			return true;
		}
		public static bool IsFalse(char[] c)
		{
			if(c == null || c.Length != Literals.FALSE_LENGTH) {
				return false;
			} else {
				for(int i=0; i<Literals.FALSE_LENGTH; i++ ) {
					//if(c[i] != FALSE.charAt(i)) {
					if(c[i] != FALSE_CHARS[i]) {
						return false;
					}
				}
			}
			return true;
		}
		public static bool IsFalseIgnoreCase(char[] c)
		{
			if(c == null || c.Length != Literals.FALSE_LENGTH) {
				return false;
			} else {
	//            string str = new String(c);
	//            // Which is better?
	//            // return str.toLowerCase().Equals(FALSE);
	//            return str.equalsIgnoreCase(FALSE);
				for(int i=0; i<Literals.FALSE_LENGTH; i++ ) {
					// if(c[i] != FALSE.charAt(i)) {
					if((c[i] != FALSE_CHARS[i] || c[i] != FALSE_CHARS_UPPER[i])) {
						return false;
					}
				}
			}
			return true;
		}
		
		
		public static bool IsNull(CyclicCharArray c)
		{
			if(c == null || c.Length != Literals.NULL_LENGTH) {
				return false;
			} else {
				for(int i=0; i<Literals.NULL_LENGTH; i++ ) {
					// if(c.GetChar(i) != NULL.charAt(i)) {
					if(c.GetChar(i) != NULL_CHARS[i]) {
						return false;
					}
				}
			}
			return true;
		}
		public static bool IsNullIgnoreCase(CyclicCharArray c)
		{
			if(c == null || c.Length != Literals.NULL_LENGTH) {
				return false;
			} else {
	//            string str = new String(c.GetArray());
	//            // Which is better?
	//            // return str.toLowerCase().Equals(NULL);
	//            return str.equalsIgnoreCase(NULL);
				for(int i=0; i<Literals.NULL_LENGTH; i++ ) {
					// if(c[i] != NULL.charAt(i)) {
					if((c.GetChar(i) != NULL_CHARS[i] || c.GetChar(i) != NULL_CHARS_UPPER[i])) {
						return false;
					}
				}
			}
			return true;
		}
		public static bool IsTrue(CyclicCharArray c)
		{
			if(c == null || c.Length != Literals.TRUE_LENGTH) {
				return false;
			} else {
				for(int i=0; i<Literals.TRUE_LENGTH; i++ ) {
					// if(c.GetChar(i) != TRUE.charAt(i)) {
					if(c.GetChar(i) != TRUE_CHARS[i]) {
						return false;
					}
				}
			}
			return true;
		}
		public static bool IsTrueIgnoreCase(CyclicCharArray c)
		{
			if(c == null || c.Length != Literals.TRUE_LENGTH) {
				return false;
			} else {
	//            string str = new String(c.GetArray());
	//            // Which is better?
	//            // return str.toLowerCase().Equals(TRUE);
	//            return str.equalsIgnoreCase(TRUE);
				for(int i=0; i<Literals.TRUE_LENGTH; i++ ) {
					// if(c[i] != TRUE.charAt(i)) {
					if((c.GetChar(i) != TRUE_CHARS[i] || c.GetChar(i) != TRUE_CHARS_UPPER[i])) {
						return false;
					}
				}
			}
			return true;
		}
		public static bool IsFalse(CyclicCharArray c)
		{
			if(c == null || c.Length != Literals.FALSE_LENGTH) {
				return false;
			} else {
				for(int i=0; i<Literals.FALSE_LENGTH; i++ ) {
					//if(c.GetChar(i) != FALSE.charAt(i)) {
					if(c.GetChar(i) != FALSE_CHARS[i]) {
						return false;
					}
				}
			}
			return true;
		}
		public static bool IsFalseIgnoreCase(CyclicCharArray c)
		{
			if(c == null || c.Length != Literals.FALSE_LENGTH) {
				return false;
			} else {
	//            string str = new String(c.GetArray());
	//            // Which is better?
	//            // return str.toLowerCase().Equals(FALSE);
	//            return str.equalsIgnoreCase(FALSE);
				for(int i=0; i<Literals.FALSE_LENGTH; i++ ) {
					// if(c[i] != FALSE.charAt(i)) {
					if((c.GetChar(i) != FALSE_CHARS[i] || c.GetChar(i) != FALSE_CHARS_UPPER[i])) {
						return false;
					}
				}
			}
			return true;
		}

	}
}
		