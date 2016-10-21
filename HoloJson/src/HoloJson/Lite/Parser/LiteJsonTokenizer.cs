using HoloJson.Common;
using HoloJson.Core;
using HoloJson.Lite;
using HoloJson.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HoloJson.Lite.Parser
{
	// Base class for JsonTokenizer implementation.
	// TBD: 
	// Make a tokenzier reusable (without having to create a new tokenizer for every Parse()'ing.) ???
	// ...
	// Note:
	// Current implementation is not thread-safe (and, probably, never will be).
	// 
	public sealed class LiteJsonTokenizer : Lite.LiteJsonTokenizer
	{
		// MAX_STRING_LOOKAHEAD_SIZE should be greater than 6.
		private const int MAX_STRING_LOOKAHEAD_SIZE = 128;   // temporary
		// private const int MAX_STRING_LOOKAHEAD_SIZE = 512;   // temporary
		private const int MAX_SPACE_LOOKAHEAD_SIZE = 32;     // temporary
		// Note that the charqueue size should be bigger than the longest string in the file + reader_buff_size
		//       (if we use "look ahead" for string parsing)!!!!
		// The parser/tokenizer fails when it encounters a string longer than that.
		// We cannot obviously use arbitrarily long char queue size, and
		// unfortunately, there is no limit in the size of a string in JSON,
		//    which makes it possible that the uncilient parser can always fail, potentially...
		private const int CHARQUEUE_SIZE = 4096;     // temporary
		// Note that CHARQUEUE_SIZE - delta >= READER_BUFF_SIZE
		//     where delta is "false".Length,
		//          or, more precisely the max length of PeekChars(len).
		//          or, if we use string lookahead, it should be the size of the longest string rounded up to MAX_STRING_LOOKAHEAD_SIZE multiples...
		private const int READER_BUFF_SIZE = 1024;   // temporary
		private const int HEAD_TRACE_LENGTH = 25;     // temporary
		// ...

		private TextReader reader;
		private int curTextReaderIndex = 0;       // global var to keep track of the reader reading state.
		private bool readerEOF = false;     // true does not mean we are done, because we are using buffer.
		private JsonToken curToken = JsonToken.INVALID;        // ?????
        private JsonToken nextToken = JsonToken.INVALID;       // or, TokenPool.TOKEN_EOF ???
        private JsonToken nextNextToken = JsonToken.INVALID;   // ??? TBD: ...
	//    private charQueue charQueue = null;
		// Note that charQueue is class-wide global variable.
		private readonly CharQueue charQueue;
		// If true, use "look ahead" algorithms.
		// private bool lookAheadParsing;

		// Ctor's
		public LiteJsonTokenizer(string str)
            : this(new StringReader(str))
		{
		}
		public LiteJsonTokenizer(TextReader reader)
            : this(reader, CHARQUEUE_SIZE)
		{
		}
		public LiteJsonTokenizer(TextReader reader, int charQueueSize)
		{
			// TextReader cannot cannot be null.
			this.reader = reader;
			this.charQueue = new CharQueue(charQueueSize);
			// lookAheadParsing = true;
			
			// For subclasses
			Init();
		}
		
		// Having multiple ctor's is a bit inconvenient.
		// Put all init routines here.
		protected void Init()
		{
			// Override this in subclasses.
		}
		
		
		// Make tokenizer re-usable through Reset().
		// TBD: Need to verify this....

		public void Reset(string str)
		{
			Reset(new StringReader(str));
		}
		public void Reset(TextReader reader)
		{
			// This is essentially a copy of ctor.
			
			// TextReader cannot cannot be null.
			this.reader = reader;
			this.charQueue.Clear();
			
			// Reset the "current state" vars.
			readerEOF = false;
            curToken = JsonToken.INVALID;
            nextToken = JsonToken.INVALID;

			// No need to call this... ???
			// Init();
		}
		
		
	//    public bool isLookAheadParsing()
	//    {
	//        return lookAheadParsing;
	//    }

		
		// TraceableJsonTokenizer interface.
		// These are primarily for debugging purposes...


		public string PeekCharsAsString(int length)
		{
			char[] c = PeekCharStream(length);
			if(c != null) {
				return new String(c);
			} else {
				return "";   // ????
			}
		}
		public char[] PeekCharStream(int length)
		{
			char[] c = null;
			try {
				c = PeekChars(length);
			} catch (HoloJsonMiniException e) {
				System.Diagnostics.Debug.WriteLine("Failed to peek char stream: length = " + length, e);
			}
			return c;
		}
		public char[] PeekCharStream()
		{
			return PeekCharStream(HEAD_TRACE_LENGTH);
		}
	 
		
		// TBD:
		// These methods really need to be synchronized.
		// ...
		
		/// @Override
		public bool HasMore()
		{
			if(JsonToken.IsInvalid(nextToken)) {
				nextToken = PrepareNextToken();
			}

            if (JsonToken.IsInvalid(nextToken) || TokenPool.TOKEN_EOF.Equals(nextToken)) {
				return false;
			} else {
				return true;
			}
		}

		/// @Override
		public JsonToken Next()
		{
			if(JsonToken.IsInvalid(nextToken)) {
				nextToken = PrepareNextToken();
			}
			curToken = nextToken;
            nextToken = JsonToken.INVALID;
	 
			return curToken;
		}

		/// @Override
		public JsonToken Peek()
		{
			if(JsonToken.IsValid(nextToken)) {
				return nextToken;
			}
			
			nextToken = PrepareNextToken();
			return nextToken;
		}

		// temporary
		// Does this save anything compared to Next();Peek(); ????
		//    (unless we can do prepareNextTwoTokens() .... ? )
		// Remove the next token (and throw away),
		// and return the next token (without removing it).
		public JsonToken NextAndPeek()
		{
			if(JsonToken.IsInvalid(nextToken)) {
				curToken = PrepareNextToken();
			} else {
				curToken = nextToken;
			}
			nextToken = PrepareNextToken();
			return nextToken;
		}

		
		// Note that this method is somewhat unusual in that it cannot be called arbitrarily.
		// This method changes the internal state by changing the charQueue.
		// This should be called by a certain select methods only!!!
		private JsonToken PrepareNextToken()
		{
			if(JsonToken.IsValid(nextToken)) {
				// ???
				return nextToken;
			}

            JsonToken token = JsonToken.INVALID;
			char ch;
			// ch has been peeked, but not popped.
			// ...
			// "Look ahead" version should be just a bit faster....
			ch = GobbleUpSpaceLookAhead();
			// ch = GobbleUpSpace();
			// ....

			// Create a JsonToken and,
			// reset the curToken.
			switch(ch) {
            case (char) CharSymbol.COMMA:
			case (char) CharSymbol.COLON:
			case (char) CharSymbol.LSQUARE:
			case (char) CharSymbol.LCURLY:
			case (char) CharSymbol.RSQUARE:
			case (char) CharSymbol.RCURLY:
				token = TokenPool.GetSymbolToken(ch);
				// NextChar();   // Consume the current token.
				SkipCharNoCheck();   // Consume the current token.
				// nextToken = null;
				break;
			case (char) MarkerChar.NULL_START:
			case (char) MarkerChar.NULL_START_UPPER:
				token = DoNullLiteral();
				break;
			case (char) MarkerChar.TRUE_START:
			case (char) MarkerChar.TRUE_START_UPPER:
				token = DoTrueLiteral();
				break;
			case (char) MarkerChar.FALSE_START:
			case (char) MarkerChar.FALSE_START_UPPER:
				token = DoFalseLiteral();
				break;
			case (char) CharSymbol.DQUOTE:
				token = DoString();
				break;
			case (char) 0:
				// ???
				token = TokenPool.TOKEN_EOF;
				// NextChar();   // Consume the current token.
				SkipCharNoCheck();   // Consume the current token.
				break;
			default:
				if(Symbols.IsStartingNumber(ch)) {
					// System.Diagnostics.Debug.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>> ch = " + ch);
					token = DoNumber();
					// System.Diagnostics.Debug.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>> number token = " + token);
				} else {
					throw new HoloJsonMiniException("Invalid symbol encountered: ch = " + ch);
				}
				break;
			}

			return token;
		}

		
		private char GobbleUpSpace()
		{
			char c = (char) 0;
			try {
				c = PeekChar();
				//while(c != 0 && char.isSpaceChar(c)) {  // ???  -> this doesn't seem to work....
				// while(c != 0 && char.IsWhitespace(c)  ) {  // ???
				while(c != 0 && CharUtil.IsWhitespace(c)  ) {  // ???
					// NextChar();   // gobble up space.
					// c = PeekChar();
					c = SkipAndPeekChar();
				}
			} catch(HoloJsonMiniException e) {
				// ????
				System.Diagnostics.Debug.WriteLine("Failed to consume space.", e);
				c = (char) 0;
			}
			return c;
		}

		// Returns the next peeked character.
		// Return value of 0 means we have reached the end of the json string.
		// TBD: use "look ahead" implementation similar to ReadString() ????
		// Note that this is effective only for "formatted" JSON with lots of consecutive spaces...
		private char GobbleUpSpaceLookAhead()
		{
			char c = (char) 0;
			try {
				c = PeekChar();
				// if(char.IsWhitespace(c)) {
				if(CharUtil.IsWhitespace(c)) {
					// SkipCharNoCheck();
					c = SkipAndPeekChar();
					
					// Spaces tend appear together.
					// if(char.IsWhitespace(c)) {
					if(CharUtil.IsWhitespace(c)) {
						int chunkLength;
						CyclicCharArray charArray = PeekCharsInQueue(MAX_SPACE_LOOKAHEAD_SIZE);
						// if(charArray == null || (chunkLength = charArray.GetLength()) == 0) {
						//     return c;
						// }
						chunkLength = charArray.Length;
		
						int chunkCounter = 0;
						int totalLookAheadLength = 0;
						c = charArray.GetChar(0);
						// while((chunkCounter < chunkLength - 1) && char.IsWhitespace(c) ) {
						while((chunkCounter < chunkLength - 1) && CharUtil.IsWhitespace(c) ) {
							++chunkCounter;
		
							if(chunkCounter >= chunkLength - 1) {
								totalLookAheadLength += chunkCounter;
								chunkCounter = 0;   // restart a loop.
		
								charArray = PeekCharsInQueue(totalLookAheadLength, MAX_SPACE_LOOKAHEAD_SIZE);
								if(charArray == null || (chunkLength = charArray.Length) == 0) {
									break;
								}
							}
							c = charArray.GetChar(chunkCounter);
						}
						totalLookAheadLength += chunkCounter;
						SkipChars(totalLookAheadLength);
						c = PeekChar();
					}
				}
			} catch(HoloJsonMiniException e) {
				// ????
				System.Diagnostics.Debug.WriteLine("Failed to consume space.", e);
				c = (char) 0;
			}
			return c;
		}

		private JsonToken DoNullLiteral()
		{
            JsonToken token = JsonToken.INVALID;
			int length = Literals.NULL_LENGTH;
			// char[] c = nextChars(length);
			CyclicCharArray c = NextCharsInQueue(length);
			if(LiteralUtil.IsNull(c)) {
				token = TokenPool.TOKEN_NULL;
				// nextToken = null;
			} else {
				// throw new JsonException("Unexpected string: " + Arrays.ToString(c), tailCharStream());
				throw new HoloJsonMiniException("Unexpected string: ");
			}        
			return token;
		}
		private JsonToken DoTrueLiteral()
		{
            JsonToken token = JsonToken.INVALID;
			int length = Literals.TRUE_LENGTH;
			// char[] c = nextChars(length);
			CyclicCharArray c = NextCharsInQueue(length);
			if(LiteralUtil.IsTrue(c)) {
				token = TokenPool.TOKEN_TRUE;
				// nextToken = null;
			} else {
				// throw new JsonException("Unexpected string: " + Arrays.ToString(c), tailCharStream());
				throw new HoloJsonMiniException("Unexpected string: ");
			}        
			return token;
		}
		private JsonToken DoFalseLiteral()
		{
            JsonToken token = JsonToken.INVALID;
			int length = Literals.FALSE_LENGTH;
			// char[] c = nextChars(length);
			CyclicCharArray c = NextCharsInQueue(length);
			if(LiteralUtil.IsFalse(c)) {
				token = TokenPool.TOKEN_FALSE;
				// nextToken = null;
			} else {
				// throw new JsonException("Unexpected string: " + Arrays.ToString(c), tailCharStream());
				throw new HoloJsonMiniException("Unexpected string: ");
			}   
			return token;
		}
		
		// Note that there is no "character".
		// char is a single letter string.
		private JsonToken DoString()
		{
            JsonToken token = JsonToken.INVALID;
			string value;
			// ....
			// value = ReadString();
			// Note that this will fail if we encounter a looooong string.
			//     See the comment below. We try at least once with ReadString() version...
			value = ReadStringWithLookAhead();
			// ....
			token = TokenPool.INSTANCE.GetToken(TokenType.STRING, value);
			// nextToken = null;
			return token;
		}
		
		private string ReadString()
		{
			// Note that we may have already "consumed" the beginning \" if we are calling this from ReadStringWithLookAhead()...
			// So, the following does not work....

	//        // char c = NextChar();
	//        char c = NextCharNoCheck();
	//        if(c == 0 || c != Symbols.DQUOTE) {
	//            // This cannot happen.
	//            throw new HoloJsonMiniException("Expecting String. Invalid token encountered: c = " + c);
	//        }

			StringBuilder sb = new StringBuilder();

			char c = PeekChar();
			if(c == 0) {
				// This cannot happen.
				throw new HoloJsonMiniException("Expecting String. Invalid token encountered: c = " + c);
			} else if(c == (char) CharSymbol.DQUOTE) {
				// consume the leading \".
				// c = NextCharNoCheck();
				SkipCharNoCheck();
				// sb.Append(c);   // No append: Remove the leading \".
			} else {
				// We are already at the beginning of the string.
				// proceed.
			}

			bool escaped = false;
			char d = PeekChar();
			while(d != 0 && (escaped == true || d != (char) CharSymbol.DQUOTE )) {
				// d = NextChar();
				d = NextCharNoCheck();
				if(escaped == false && d == (char) CharSymbol.BACKSLASH) {
					escaped = true;
					// skip
				} else {
					if(escaped == true) {
						if(d == (char) CharSymbol.UNICODE_PREFIX) {
							// char[] hex = nextChars(4);
							CyclicCharArray hex = NextCharsInQueue(4);
							// TBD: validate ??
							
							try {
								// ????
								// sb.Append((char) CharSymbol.BACKSLASH).Append(d).Append(hex);
								char u = UnicodeUtil.GetUnicodeChar(hex);
								if(u != 0) {
									sb.Append(u);
								} else {
									// ????
								}
							} catch(Exception e) {
								throw new HoloJsonMiniException("Invalid unicode char: hex = " + hex.ToString(), e);
							}
						} else {
							if(Symbols.IsEscapableChar(d)) {
								// TBD:
								// Newline cannot be allowed within a string....
								// ....
								char e = Symbols.GetEscapedChar(d);
								if(e != 0) {
									sb.Append(e);
								} else {
									// This cannot happen.
								}
							} else {
								// error?
								throw new HoloJsonMiniException("Invalid escaped char: d = \\" + d);
							}
						}
						// toggle the flag.
						escaped = false;
					} else {
						
						// TBD:
						// Exclude control characters ???
						// ...
						
						sb.Append(d);
					}
				}
				d = PeekChar();
			}
			if(d == (char) CharSymbol.DQUOTE) {
				// d = NextChar();
				SkipCharNoCheck();
				// sb.Append(d);  // No append: Remove the trailing \".
			} else {
				// end of the json string.
				// error???
				// return null;
			}
			
			return sb.ToString();
		}

		// Note:
		// This will cause parse failing
		//     if the longest string in JSON is longer than (CHARQUEUE_SIZE - READER_BUFF_SIZE)
		//     because Forward() will fail.
		// TBD:
		// There might be bugs when dealing with short strings, or \\u escaped unicodes at the end of a json string
		// ...
		private string ReadStringWithLookAhead()
		{
			// char c = NextChar();
			char c = NextCharNoCheck();
			if(c == 0 || c != (char) CharSymbol.DQUOTE) {
				// This cannot happen.
				throw new HoloJsonMiniException("Expecting String. Invalid token encountered: c = " + c);
			}
			StringBuilder sb = new StringBuilder();
			// sb.Append(c);   // No append: Remove the leading \".
			
			bool escaped = false;
			
			
			int chunkLength;
			CyclicCharArray charArray = PeekCharsInQueue(MAX_STRING_LOOKAHEAD_SIZE);
			if(charArray == null || (chunkLength = charArray.Length) == 0) {
				// ????
				throw new HoloJsonMiniException("string token terminated unexpectedly.");
			}
			bool noMoreCharsInQueue = false;
			if(chunkLength < MAX_STRING_LOOKAHEAD_SIZE) {
				noMoreCharsInQueue = true;
			}
			bool needMore = false;
			int chunkCounter = 0;
			int totalLookAheadLength = 0;
			char d = charArray.GetChar(0);
			// System.Diagnostics.Debug.WriteLine(">>>>>>>>>>>>>>>>>> d = " + d);
			// System.Diagnostics.Debug.WriteLine(">>>>>>>>>>>>>>>>>> chunkLength = " + chunkLength);
			while((chunkCounter < chunkLength - 1) &&    // 6 for "\\uxxxx". 
					d != 0 && 
					(escaped == true || d != (char) CharSymbol.DQUOTE )) {
				// d = charArray.GetChar(++chunkCounter);
				++chunkCounter;
				
				// System.Diagnostics.Debug.WriteLine(">>>>>>>>>>>>>>>>>> d = " + d);
				
				if(escaped == false && d == (char) CharSymbol.BACKSLASH) {
					escaped = true;
					// skip
				} else {
					if(escaped == true) {
						if(d == (char) CharSymbol.UNICODE_PREFIX) {
							if(chunkCounter < chunkLength - 4) {
								char[] hex = charArray.GetChars(chunkCounter, 4);
								chunkCounter += 4;
								
								try {
									// ????
									// sb.Append((char) CharSymbol.BACKSLASH).Append(d).Append(hex);
									char u = UnicodeUtil.GetUnicodeChar(hex);
									if(u != 0) {
										sb.Append(u);
									} else {
										// ????
									}
								} catch(Exception e) {
									throw new HoloJsonMiniException("Invalid unicode char: hex = " + String.Join<char>(",", hex), e);
								}
							} else {
								if(noMoreCharsInQueue == false) {
									needMore = true;
									chunkCounter -= 2;     // Reset the counter backward for "\\u".
								} else {
									// error
									throw new HoloJsonMiniException("Invalid unicode char.");
								}
							}
						} else {
							if(Symbols.IsEscapableChar(d)) {
								// TBD:
								// Newline cannot be allowed within a string....
								// ....
								char e = Symbols.GetEscapedChar(d);
								if(e != 0) {
									sb.Append(e);
								} else {
									// This cannot happen.
								}
							} else {
								// error?
								throw new HoloJsonMiniException("Invalid escaped char: d = \\" + d);
							}
						}
						// toggle the flag.
						escaped = false;
					} else {
						
						// TBD:
						// Exclude control characters ???
						// ...
						
						sb.Append(d);
					}
				}
				
				if((noMoreCharsInQueue == false) && (needMore || chunkCounter >= chunkLength - 1)) {
					totalLookAheadLength += chunkCounter;
					chunkCounter = 0;   // restart a loop.
					needMore = false;
					// System.Diagnostics.Debug.WriteLine(">>>>>>>>>>>>>>>>>>>>>> AddAll() totalLookAheadLength = " + totalLookAheadLength);

					try {
						charArray = PeekCharsInQueue(totalLookAheadLength, MAX_STRING_LOOKAHEAD_SIZE);
					} catch(HoloJsonMiniException e) {
						// Not sure if this makes sense....
						// but since this error might have been due to the fact that we have encountered a looooong string,
						// Try again???
						// ...
						// Note that this applies one, this particular, string only.
						// Next time when we encounter a long string, 
						// this may be invoked again....
						// ....
						// We should be careful not to get into the infinite loop....
						System.Diagnostics.Debug.WriteLine("string token might have been too long. Trying again with no look-ahead ReadString().");

						// Reset the buffer (Peek() status) ????, and call the non "look ahead" version...
						return ReadString();   // Is this starting from the beginning???
						// ...
					}
					if(charArray == null || (chunkLength = charArray.Length) == 0) {
						// ????
						throw new HoloJsonMiniException("string token terminated unexpectedly.");
					}
					if(chunkLength < MAX_STRING_LOOKAHEAD_SIZE) {
						noMoreCharsInQueue = true;
					}
				}
				
				d = charArray.GetChar(chunkCounter);
			}
			totalLookAheadLength += chunkCounter;
			SkipChars(totalLookAheadLength);
			d = PeekChar();

			if(d == (char) CharSymbol.DQUOTE) {
				// d = NextChar();
				SkipCharNoCheck();
				// sb.Append(d);  // No append: Remove the trailing \".
			} else {
				// end of the json string.
				// error???
				// return null;
			}
			
			return sb.ToString();
		}
		
		
		private JsonToken DoNumber()
		{
            JsonToken token = JsonToken.INVALID;
			var value = ReadNumber();
			token = TokenPool.INSTANCE.GetToken(TokenType.NUMBER, value);
			// nextToken = null;
			return token;
		}

		// Need a better way to do this ....
		private Number ReadNumber()
		{
			// char c = NextChar();
			char c = NextCharNoCheck();
			if(! Symbols.IsStartingNumber(c)) {
				throw new HoloJsonMiniException("Expecting a number. Invalid symbol encountered: c = " + c);
			}

			if(c == (char) CharSymbol.PLUS) {
				// remove the leading +.
				c = NextChar();
			}

			var sb = new StringBuilder();

			if(c == (char) CharSymbol.MINUS) {
				sb.Append(c);
				c = NextChar();
			}

			bool periodRead = false;
			if(c == (char) CharSymbol.PERIOD) {
				periodRead = true;
				sb.Append("0.");
			} else {
				// Could be a number, nothing else.
				if(c == '0') {
					char c2 = PeekChar();
					// This does not work because the number happens to be just zero ("0").
					// if(c2 != (char) CharSymbol.PERIOD) {
					//     throw new JsonException("Invalid number: c = " + sb.ToString() + c + c2, tailCharStream());
					// }
					// This should be better.
					// zero followed by other number is not allowed.
					if(Char.IsDigit(c2)) {
						throw new HoloJsonMiniException("Invalid number: c = " + sb.ToString() + c + c2);
					}
					sb.Append(c);
                    if (c2 == (char) CharSymbol.PERIOD) {
						periodRead = true;
						// sb.Append(NextChar());
						sb.Append(NextCharNoCheck());
					}
				} else {
					sb.Append(c);
				}
			}
			
			bool exponentRead = false;
			
			char d = PeekChar();
			while(d != 0 && (Char.IsDigit(d) || 
					(periodRead == false && d == (char) CharSymbol.PERIOD) ||
					(exponentRead == false && Symbols.IsExponentChar(d))
					)) {
				// sb.Append(NextChar());
				sb.Append(NextCharNoCheck());
				if(d == (char) CharSymbol.PERIOD) {
					periodRead = true;
				}
				if(Symbols.IsExponentChar(d)) {
					char d2 = PeekChar();
					if(d2 == (char) CharSymbol.PLUS || d2 == (char) CharSymbol.MINUS || Char.IsDigit(d2)) {
						// sb.Append(NextChar());
						sb.Append(NextCharNoCheck());
					} else {
						throw new HoloJsonMiniException("Invalid number: " + sb.ToString() + d2);
					}
					exponentRead = true;
				}
				d = PeekChar();
			}
			if(d == 0) {
				// end of the json string.
				// ????
				// throw new JsonException("Invalid number: " + sb.ToString(), tailCharStream());
			} else {
				// sb.Append(NextChar());
			}
			
			string str = sb.ToString();
			
			Number number = Number.Invalid;   // ????
			try {
                //if(str.Contains(".")) {
                //    double x = Convert.ToDouble(str);
                //    number = new Number(x);
                //} else {
                //    long y = Convert.ToInt64(str);
                //    number = new Number(y);
                //}
                number = str.ToNumber();
			} catch(Exception e) {
				// ???
				throw new HoloJsonMiniException("Invalid number encountered: str = " + str, e);
			}
			return number;
		}

		
		// because we called PeekChar() already,
		//      no need for check error conditions.
		private char NextCharNoCheck()
		{
			char ch = charQueue.Poll();
			return ch;
		}
		private void SkipCharNoCheck()
		{
			charQueue.Skip();
		}

		private char NextChar()
		{
			if(charQueue.IsEmpty) {
				if(readerEOF == false) {
					try {
						Forward();
					} catch (IOException e) {
						// ???
						throw new HoloJsonMiniException("Failed to forward character stream.", e);
					}
				}
			}
			if(charQueue.IsEmpty) {
				return (char) 0;   // ???
				// throw new JsonException("There is no character in the buffer.");
			}
			char ch = charQueue.Poll();
			return ch;
		}
		private char[] NextChars(int length)
		{
			// assert length > 0
			if(charQueue.Size < length) {
				if(readerEOF == false) {
					try {
						Forward();
					} catch (IOException e) {
						// ???
						throw new HoloJsonMiniException("Failed to forward character stream.", e);
					}
				}
			}
			char[] c = null;
			if(charQueue.Size < length) {
				c = charQueue.Poll(charQueue.Size);
				// throw new JsonException("There is not enough characters in the buffer. length = " + length);
			}
			c = charQueue.Poll(length);
			return c;
		}
		private CyclicCharArray NextCharsInQueue(int length)
		{
			// assert length > 0
			if(charQueue.Size < length) {
				if(readerEOF == false) {
					try {
						Forward();
					} catch (IOException e) {
						// ???
						throw new HoloJsonMiniException("Failed to forward character stream.", e);
					}
				}
			}
			CyclicCharArray charArray = null;
			if(charQueue.Size < length) {
				charArray = charQueue.PollBuffer(charQueue.Size);
				// throw new JsonException("There is not enough characters in the buffer. length = " + length);
			}
			charArray = charQueue.PollBuffer(length);
			return charArray;
		}
		
		private void SkipChars(int length)
		{
			// assert length > 0
			if(charQueue.Size < length) {
				if(readerEOF == false) {
					try {
						Forward();
					} catch (IOException e) {
						// ???
						throw new HoloJsonMiniException("Failed to forward character stream.", e);
					}
				}
			}
			charQueue.Skip(length);
		}

		
		

		// Note that PeekChar() and PeekChars() are "idempotent". 
		private char PeekChar()
		{
			if(charQueue.IsEmpty) {
				if(readerEOF == false) {
					try {
						Forward();
					} catch (IOException e) {
						// ???
						throw new HoloJsonMiniException("Failed to forward character stream.", e);
					}
				}
			}
			if(charQueue.IsEmpty) {
				return (char) 0;
				// throw new JsonException("There is no character in the buffer.");
			}
			return charQueue.Peek();
		}
		private char[] PeekChars(int length)
		{
			// assert length > 0
			if(charQueue.Size < length) {
				if(readerEOF == false) {
					try {
						Forward();
					} catch (IOException e) {
						// ???
						throw new HoloJsonMiniException("Failed to forward character stream.", e);
					}
				}
			}
			if(charQueue.Size < length) {
				return charQueue.Peek(charQueue.Size);
				// throw new JsonException("There is not enough characters in the buffer. length = " + length);
			}
			return charQueue.Peek(length);
		}
		private CyclicCharArray PeekCharsInQueue(int length)
		{
			// assert length > 0
			if(charQueue.Size < length) {
				if(readerEOF == false) {
					try {
						Forward();
					} catch (IOException e) {
						// ???
						throw new HoloJsonMiniException("Failed to forward character stream.", e);
					}
				}
			}
			if(charQueue.Size < length) {
				return charQueue.PeekBuffer(charQueue.Size);
				// throw new JsonException("There is not enough characters in the buffer. length = " + length);
			}
			return charQueue.PeekBuffer(length);
		}
		private CyclicCharArray PeekCharsInQueue(int offset, int length)
		{
			// assert length > 0
			if(charQueue.Size < offset + length) {
				if(readerEOF == false) {
					try {
						Forward();
					} catch (IOException e) {
						// ???
						throw new HoloJsonMiniException("Failed to forward character stream.", e);
					}
				}
			}
			if(charQueue.Size < offset + length) {
				return charQueue.PeekBuffer(offset, charQueue.Size - offset);
				// throw new JsonException("There is not enough characters in the buffer. length = " + length);
			}
			return charQueue.PeekBuffer(offset, length);
		}

		
		// Poll next char (and gobble up),
		// and return the next char (without removing it)
		private char SkipAndPeekChar()
		{
			int qSize = charQueue.Size;
			if(qSize < 2) {
				if(readerEOF == false) {
					try {
						Forward();
						qSize = charQueue.Size;
					} catch (IOException e) {
						// ???
						throw new HoloJsonMiniException("Failed to forward character stream.", e);
					}
				}
			}
			if(qSize > 0) {
				charQueue.Skip();
				if(qSize > 1) {
					return charQueue.Peek();
				}
			}
			return (char) 0;
			// throw new JsonException("There is no character in the buffer.");
		}

		
		// Read some more bytes from the reader.
		private readonly char[] buff = new char[READER_BUFF_SIZE];
		private async void Forward()
		{
			if(readerEOF == false) {
				int cnt = 0;
				try {
					// This throws OOB excpetion at the end....
					// cnt = reader.read(buff, curTextReaderIndex, READER_BUFF_SIZE);
                    cnt = await reader.ReadAsync(buff, 0, READER_BUFF_SIZE);
				} catch(ArgumentOutOfRangeException e) {
					// ???
					// Why does this happen? Does it happen for StringReader only???
					//    Does read(,,) ever return -1 in the case of StringReader ???
					System.Diagnostics.Debug.WriteLine("Looks like we have reached the end of the reader.", e);
				}
				if(cnt == -1 || cnt == 0) {
					readerEOF = true;
				} else {
					bool suc = charQueue.AddAll(buff, cnt);
					if(suc) {
						curTextReaderIndex += cnt;
					} else {
						// ???
						throw new HoloJsonMiniException("Unexpected internal error occurred. chars were not added to CharQueue: cnt = " + cnt);
					}
				}
			}
		}
		
	}
}
