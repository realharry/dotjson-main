using DotJson.Common;
using DotJson.Core;
using DotJson.Parser.Core;
using DotJson.Parser.Exceptions;
using DotJson.Parser.Policy;
using DotJson.Parser.Policy.Base;
using DotJson.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotJson.Parser.Impl
{
    // Base class for JsonTokenizer implementation.
    // TBD: // Make a tokenzier reusable (without having to create a new tokenizer for every parse()'ing.) ???
    // Note: // Current implementation is not thread-safe (and, probably, never will be).
    public abstract class AbstractJsonTokenizer : TraceableJsonTokenizer, LookAheadJsonTokenizer
    {
        // MAX_STRING_LOOKAHEAD_SIZE should be greater than 6.
        private const int MAX_STRING_LOOKAHEAD_SIZE = 256; // temporary
                                                           // private static final int MAX_STRING_LOOKAHEAD_SIZE = 512;   // temporary
        private const int MAX_SPACE_LOOKAHEAD_SIZE = 32; // temporary
                                                         // Note that the charqueue size should be bigger than the longest string in the file + reader_buff_size
                                                         //       (if we use "look ahead" for string parsing)!!!!
                                                         // The parser/tokenizer fails when it encounters a string longer than that.
                                                         // We cannot obviously use arbitrarily long char queue size, and
                                                         // unfortunately, there is no limit in the size of a string in JSON,
                                                         //    which makes it possible that the mini json parser can always fail, potentially...
        private const int CHARQUEUE_SIZE = 4096; // temporary
                                                 // Note that CHARQUEUE_SIZE - delta >= READER_BUFF_SIZE
                                                 //     where delta is "false".length,
                                                 //          or, more precisely the max length of PeekChars(len).
                                                 //          or, if we use string lookahead, it should be the size of the longest string rounded up to MAX_STRING_LOOKAHEAD_SIZE multiples...
        private const int READER_BUFF_SIZE = 1024; // temporary
        private const int TAILBUFFER_SIZE = 512; // temporary
        private const int TAIl_TRACE_LENGTH = 150; // temporary
        private const int HEAD_TRACE_LENGTH = 25; // temporary
        private TextReader reader;
        private int curReaderIndex = 0; // global var to keep track of the reader reading state.
        private bool readerEOF = false; // true does not mean we are done, because we are using buffer.
        private JsonToken curToken = JsonToken.INVALID;
        private JsonToken nextToken = JsonToken.INVALID; // or, TokenPool.TOKEN_EOF ???
        private JsonToken nextNextToken = JsonToken.INVALID; // ??? TBD: ...
                                                //    private CharacterQueue charQueue = null;
                                                // Note that charQueue is class-wide global variable.
        private readonly CharQueue charQueue;
        // tailBuffer is used for debugging/error handling purpose.
        private readonly CharBuffer tailBuffer;
        // Strict or Lenient?
        private readonly ParserPolicy parserPolicy;
        // If true, use "look ahead" algorithms.
        private bool lookAheadParsing;
        // Whether "tracing" is enabled or not.
        // Tracing, at this point, means that we simply keep the char tail buffer
        //    so that when an error occurs we can see the "exception context".
        private bool tracingEnabled;

        // Ctor's
        public AbstractJsonTokenizer(string str) : this(new StringReader(str))
        {
        }
        public AbstractJsonTokenizer(TextReader reader) : this(reader, null)
        {
        }
        public AbstractJsonTokenizer(TextReader reader, ParserPolicy parserPolicy)
            : this(reader, parserPolicy, CHARQUEUE_SIZE)
        {
        }
        public AbstractJsonTokenizer(TextReader reader, ParserPolicy parserPolicy, int charQueueSize)
        {
            // Reader cannot cannot be null.
            this.reader = reader;
            if (parserPolicy == null) {
                this.parserPolicy = DefaultParserPolicy.MINIJSON;
            } else {
                this.parserPolicy = parserPolicy;
            }
            this.charQueue = new CharQueue(charQueueSize);
            this.tailBuffer = new CharBuffer(TAILBUFFER_SIZE);
            // Note the comment above.
            // Enabling lookaheadparsing can potentially cause parse failure because it cannot handle a loooong string.
            lookAheadParsing = true;
            tracingEnabled = false;
            // This is not necessary.
            // Start by cleaning up the leading spaces...  ???
            // GobbleUpSpace();
            // For subclasses
            Init();
        }
        // Having multiple ctor's is a bit inconvenient.
        // Put all init routines here.
        protected internal virtual void Init()
        {
            // Override this in subclasses.
        }
        // Make tokenizer re-usable through Reset().
        // TBD: Need to verify this....
        public virtual void Reset(string str)
        {
            Reset(new StringReader(str));
        }
        public virtual void Reset(TextReader reader)
        {
            // This is essentially a copy of ctor.

            // Reader cannot cannot be null.
            this.reader = reader;
            this.charQueue.Clear();
            this.tailBuffer.Clear();
            // this.parserPolicy cannot be changed.
            // lookAheadParsing = true;
            // tracingEnabled = false;

            // Reset the "current state" vars.
            readerEOF = false;
            curToken = JsonToken.INVALID;
            nextToken = JsonToken.INVALID;

            // No need to call this... ???
            // Init();
        }
        // For subclasses.
        public virtual ParserPolicy ParserPolicy
        {
            get
            {
                return this.parserPolicy;
            }
        }
        public virtual bool LookAheadParsing
        {
            get
            {
                return lookAheadParsing;
            }
        }
        //    public void setLookAheadParsing(boolean lookAheadParsing)
        //    {
        //        this.lookAheadParsing = lookAheadParsing;
        //    }
        public virtual void EnableLookAheadParsing()
        {
            this.lookAheadParsing = true;
        }
        public virtual void DisableLookAheadParsing()
        {
            this.lookAheadParsing = false;
        }
        // TraceableJsonTokenizer interface.
        // These are primarily for debugging purposes...
        public virtual void EnableTracing()
        {
            tracingEnabled = true;
        }
        public virtual void DisableTracing()
        {
            tracingEnabled = false;
        }
        public virtual bool TracingEnabled
        {
            get
            {
                return tracingEnabled;
            }
        }
        public virtual string GetTailCharsAsString(int length)
        {
            // return tailBuffer.tailAsString(length);
            char[] c = GetTailCharStream(length);
            if (c != null) {
                return new string(c);
            } else {
                return ""; // ????
            }
        }
        public virtual char[] GetTailCharStream(int length)
        {
            return tailBuffer.Tail(length);
        }
        private char[] GetTailCharStream()
        {
            return GetTailCharStream(TAIl_TRACE_LENGTH);
        }
        public virtual string PeekCharsAsString(int length)
        {
            char[] c = PeekCharStream(length);
            if (c != null) {
                return new string(c);
            } else {
                return ""; // ????
            }
        }
        public virtual char[] PeekCharStream(int length)
        {
            char[] c = null;
            try {
                c = PeekChars(length);
            } catch (JsonTokenizerException e) {
                // log.log(Level.WARNING, "Failed to peek char stream: length = " + length, e);
            }
            return c;
        }
        public virtual char[] PeekCharStream()
        {
            return PeekCharStream(HEAD_TRACE_LENGTH);
        }
        // TBD:
        // These methods really need to be synchronized.
        public virtual bool HasMore()
        {
            if (nextToken.IsInvalid) {
                nextToken = PrepareNextToken();
            }
            if (nextToken.IsInvalid || TokenPool.TOKEN_EOF.Equals(nextToken)) {
                return false;
            } else {
                return true;
            }
        }
        public virtual JsonToken Next()
        {
            if (nextToken.IsInvalid) {
                nextToken = PrepareNextToken();
            }
            curToken = nextToken;
            nextToken = JsonToken.INVALID;
            return curToken;
        }
        public virtual JsonToken Peek()
        {
            if (nextToken.IsValid) {   // vs !nextToken.IsInvalid ????
                return nextToken;
            }
            nextToken = PrepareNextToken();
            return nextToken;
        }
        // temporary
        // Does this save anything compared to next();Peek(); ????
        //    (unless we can do prepareNextTwoTokens() .... ? )
        // Remove the next token (and throw away),
        // and return the next token (without removing it).
        public virtual JsonToken NextAndPeek()
        {
            if (nextToken.IsInvalid) {
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
            if (nextToken.IsValid) {   // vs !nextToken.IsInvalid ????
                // ???
                return nextToken;
            }
            JsonToken token = JsonToken.INVALID;
            char ch;
            // ch has been peeked, but not popped.
            if (lookAheadParsing) {
                ch = GobbleUpSpaceLookAhead();
            } else {
                ch = GobbleUpSpace();
            }
            // Create a JsonToken and,
            // Reset the curToken.
            switch (ch) {
                case Symbols.COMMA:
                case Symbols.COLON:
                case Symbols.LSQUARE:
                case Symbols.LCURLY:
                case Symbols.RSQUARE:
                case Symbols.RCURLY:
                    token = TokenPool.GetSymbolToken(ch);
                    // nextChar();   // Consume the current token.
                    SkipCharNoCheck(); // Consume the current token.
                                       // nextToken = null;
                    break;
                case Symbols.NULL_START:
                case Symbols.NULL_START_UPPER:
                    token = DoNullLiteral();
                    break;
                case Symbols.TRUE_START:
                case Symbols.TRUE_START_UPPER:
                    token = DoTrueLiteral();
                    break;
                case Symbols.FALSE_START:
                case Symbols.FALSE_START_UPPER:
                    token = DoFalseLiteral();
                    break;
                case Symbols.DQUOTE:
                    token = DoString();
                    break;
                case (char) 0:
                    // ???
                    token = TokenPool.TOKEN_EOF;
                    // nextChar();   // Consume the current token.
                    SkipCharNoCheck(); // Consume the current token.
                    break;
                default:
                    if (Symbols.IsStartingNumber(ch)) {
                        // // log.warning(">>>>>>>>>>>>>>>>>>>>>>>>>> ch = " + ch);
                        token = DoNumber();
                        // // log.warning(">>>>>>>>>>>>>>>>>>>>>>>>>> number token = " + token);
                    } else {
                        throw new UnexpectedSymbolException("Invalid symbol encountered: ch = " + ch, GetTailCharStream(), PeekCharStream());
                    }
                    break;
            }
            return token;
        }
        // Returns the next peeked character.
        // Return value of 0 means we have reached the end of the json string.
        // TBD: use "look ahead" implementation similar to readString() ????
        private char GobbleUpSpace()
        {
            char c = (char)0;
            try {
                c = PeekChar();
                //while(c != 0 && Character.isSpaceChar(c)) {  // ???  -> this doesn't seem to work....
                // while(c != 0 && Character.isWhitespace(c)  ) {  // ???
                while (c != 0 && CharUtil.IsWhitespace(c)) { // ???
                                                                  // nextChar();   // gobble up space.
                                                                  // c = PeekChar();
                    c = SkipAndPeekChar();
                }
            } catch (JsonTokenizerException e) {
                // ????
                //if (log.isLoggable(Level.INFO)) {
                //    // log.log(Level.INFO, "Failed to consume space: " + ErrorContext.buildContextString(GetTailCharStream(), PeekCharStream()), e);
                //}
                c = (char)0;
            }
            return c;
        }
        // Note that this is effective only for "formatted" JSON with lots of consecutive spaces...
        private char GobbleUpSpaceLookAhead()
        {
            char c = (char)0;
            try {
                c = PeekChar();
                // if(Character.isWhitespace(c)) {
                if (CharUtil.IsWhitespace(c)) {
                    // SkipCharNoCheck();
                    c = SkipAndPeekChar();
                    // Spaces tend appear together.
                    // if(Character.isWhitespace(c)) {
                    if (CharUtil.IsWhitespace(c)) {
                        int chunkLength;
                        CyclicCharArray charArray = PeekCharsInQueue(MAX_SPACE_LOOKAHEAD_SIZE);
                        // if(charArray == null || (chunkLength = charArray.getLength()) == 0) {
                        //     return c;
                        // }
                        chunkLength = charArray.Length;

                        int chunkCounter = 0;
                        int totalLookAheadLength = 0;
                        c = charArray.GetChar(0);
                        // while((chunkCounter < chunkLength - 1) && Character.isWhitespace(c) ) {
                        while ((chunkCounter < chunkLength - 1) && CharUtil.IsWhitespace(c)) {
                            ++chunkCounter;

                            if (chunkCounter >= chunkLength - 1) {
                                totalLookAheadLength += chunkCounter;
                                if (tracingEnabled) {
                                    this.tailBuffer.Push(charArray.GetArray(), chunkCounter);
                                }
                                chunkCounter = 0; // restart a loop.

                                charArray = PeekCharsInQueue(totalLookAheadLength, MAX_SPACE_LOOKAHEAD_SIZE);
                                if (charArray == null || (chunkLength = charArray.Length) == 0) {
                                    break;
                                }
                            }
                            c = charArray.GetChar(chunkCounter);
                        }
                        totalLookAheadLength += chunkCounter;
                        if (tracingEnabled) {
                            this.tailBuffer.Push(charArray.GetArray(), chunkCounter);
                        }
                        SkipChars(totalLookAheadLength);
                        c = PeekChar();
                    }
                }
            } catch (JsonTokenizerException e) {
                // ????
                //if (log.isLoggable(Level.INFO)) {
                //    // log.log(Level.INFO, "Failed to consume space: " + ErrorContext.buildContextString(GetTailCharStream(), PeekCharStream()), e);
                //}
                c = (char)0;
            }
            return c;
        }
        private JsonToken DoNullLiteral()
        {
            JsonToken token = JsonToken.INVALID;
            int length = Literals.NULL_LENGTH;
            // char[] c = nextChars(length);
            CyclicCharArray c = NextCharsInQueue(length);
            if (parserPolicy.CaseInsensitiveLiterals ? LiteralUtil.IsNullIgnoreCase(c) : LiteralUtil.IsNull(c)) {
                token = TokenPool.TOKEN_NULL;
                // nextToken = null;
            } else {
                // throw new UnexpectedSymbolException("Unexpected string: " + Arrays.toString(c), GetTailCharStream(), PeekCharStream());
                throw new UnexpectedSymbolException("Unexpected string: " + (c == null ? "" : c.ToString()), GetTailCharStream(), PeekCharStream());
            }
            return token;
        }
        private JsonToken DoTrueLiteral()
        {
            JsonToken token = JsonToken.INVALID;
            int length = Literals.TRUE_LENGTH;
            // char[] c = nextChars(length);
            CyclicCharArray c = NextCharsInQueue(length);
            if (parserPolicy.CaseInsensitiveLiterals ? LiteralUtil.IsTrueIgnoreCase(c) : LiteralUtil.IsTrue(c)) {
                token = TokenPool.TOKEN_TRUE;
                // nextToken = null;
            } else {
                // throw new UnexpectedSymbolException("Unexpected string: " + Arrays.toString(c), GetTailCharStream(), PeekCharStream());
                throw new UnexpectedSymbolException("Unexpected string: " + (c == null ? "" : c.ToString()), GetTailCharStream(), PeekCharStream());
            }
            return token;
        }
        private JsonToken DoFalseLiteral()
        {
            JsonToken token = JsonToken.INVALID;
            int length = Literals.FALSE_LENGTH;
            // char[] c = nextChars(length);
            CyclicCharArray c = NextCharsInQueue(length);
            if (parserPolicy.CaseInsensitiveLiterals ? LiteralUtil.IsFalseIgnoreCase(c) : LiteralUtil.IsFalse(c)) {
                token = TokenPool.TOKEN_FALSE;
                // nextToken = null;
            } else {
                // throw new UnexpectedSymbolException("Unexpected string: " + Arrays.toString(c), GetTailCharStream(), PeekCharStream());
                throw new UnexpectedSymbolException("Unexpected string: " + (c == null ? "" : c.ToString()), GetTailCharStream(), PeekCharStream());
            }
            return token;
        }
        // Note that there is no "character".
        // Character is a single letter string.
        private JsonToken DoString()
        {
            JsonToken token = JsonToken.INVALID;
            string value;
            if (lookAheadParsing) {
                value = ReadStringWithLookAhead();
            } else {
                value = ReadString();
            }
            token = TokenPool.Instance.GetToken(TokenType.STRING, value);
            // nextToken = null;
            return token;
        }
        // Note:
        // This will cause parse failing
        //     if the longest string in JSON is longer than (CHARQUEUE_SIZE - READER_BUFF_SIZE)
        //     because forward() will fail.
        // TBD:
        // There might be bugs when dealing with short strings, or \\u escaped unicodes at the end of a json string
        private string ReadStringWithLookAhead()
        {
            // char c = nextChar();
            char c = NextCharNoCheck();
            if (c == 0 || c != Symbols.DQUOTE) {
                // This cannot happen.
                throw new UnexpectedSymbolException("Expecting String. Invalid token encountered: c = " + c, GetTailCharStream(), PeekCharStream());
            }
            StringBuilder sb = new StringBuilder();
            // sb.append(c);   // No append: Remove the leading \".

            bool escaped = false;
            int chunkLength;
            CyclicCharArray charArray = PeekCharsInQueue(MAX_STRING_LOOKAHEAD_SIZE);
            if (charArray == null || (chunkLength = charArray.Length) == 0) {
                // ????
                throw new UnexpectedEndOfStreamException("String token terminated unexpectedly.", GetTailCharStream(), PeekCharStream());
            }
            bool noMoreCharsInQueue = false;
            if (chunkLength < MAX_STRING_LOOKAHEAD_SIZE) {
                noMoreCharsInQueue = true;
            }
            bool needMore = false;
            int chunkCounter = 0;
            int totalLookAheadLength = 0;
            char d = charArray.GetChar(0);
            while ((chunkCounter < chunkLength - 1) && d != 0 && (escaped == true || d != Symbols.DQUOTE)) { // 6 for "\\uxxxx".
                                                                                                             // d = charArray.GetChar(++chunkCounter);
                ++chunkCounter;
                if (escaped == false && d == Symbols.BACKSLASH) {
                    escaped = true;
                    // skip
                } else {
                    if (escaped == true) {
                        if (d == Symbols.UNICODE_PREFIX) {
                            if (chunkCounter < chunkLength - 4) {
                                char[] hex = charArray.GetChars(chunkCounter, 4);
                                chunkCounter += 4;
                                try {
                                    // ????
                                    // sb.append(Symbols.BACKSLASH).append(d).append(hex);
                                    char u = UnicodeUtil.GetUnicodeChar(hex);
                                    if (u != 0) {
                                        sb.Append(u);
                                    } else { // ????
                                    }
                                } catch (Exception e) {
                                    // throw new UnexpectedSymbolException("Invalid unicode char: hex = " + Arrays.ToString(hex), e, GetTailCharStream(), PeekCharStream());
                                    throw new UnexpectedSymbolException("Invalid unicode char: hex = " + string.Join<char>(",", hex), e, GetTailCharStream(), PeekCharStream());
                                }
                            } else {
                                if (noMoreCharsInQueue == false) {
                                    needMore = true;
                                    chunkCounter -= 2; // Reset the counter backward for "\\u".
                                } else { // error
                                    throw new UnexpectedSymbolException("Invalid unicode char.", GetTailCharStream(), PeekCharStream());
                                }
                            }
                        } else {
                            if (Symbols.IsEscapableChar(d)) {
                                // TBD:
                                // Newline cannot be allowed within a string....
                                char e = Symbols.GetEscapedChar(d);
                                if (e != 0) {
                                    sb.Append(e);
                                } else { // This cannot happen.
                                }
                            } else {
                                // error?
                                throw new UnexpectedSymbolException("Invalid escaped char: d = \\" + d, GetTailCharStream(), PeekCharStream());
                            }
                        }
                        // toggle the flag.
                        escaped = false;
                    } else {
                        // TBD:
                        // Exclude control characters ???
                        sb.Append(d);
                    }
                }
                if ((noMoreCharsInQueue == false) && (needMore || chunkCounter >= chunkLength - 1)) {
                    totalLookAheadLength += chunkCounter;
                    if (tracingEnabled) {
                        this.tailBuffer.Push(charArray.GetArray(), chunkCounter);
                    }
                    chunkCounter = 0; // restart a loop.
                    needMore = false;
                    // // log.warning(">>>>>>>>>>>>>>>>>>>>>> addAll() totalLookAheadLength = " + totalLookAheadLength);
                    try {
                        charArray = PeekCharsInQueue(totalLookAheadLength, MAX_STRING_LOOKAHEAD_SIZE);
                    } catch (UnexpectedEndOfStreamException e) {
                        // Not sure if this makes sense....
                        // but since this error might have been due to the fact that we have encountered a looooong string,
                        // Try again???
                        // Note that this makes it hard to reuse the parser instance....
                        // (in some way, it's a good thing, because the json files tend to be similar in the given context,
                        //     and if one file has a loooong string, then it's likely that others have long strings as well....)
                        // We should be careful not to get into the infinite loop....
                        if (LookAheadParsing) { // This if() is always true at this point...
                            DisableLookAheadParsing();
                            // log.warning("String token might have been too long.  Trying again after calling DisableLookAheadParsing().");
                            // Reset the buffer (Peek() status) ????, and call the non "look ahead" version...
                            return ReadString(); // Is this starting from the beginning???
                        } else { // This cannot happen..
                            throw e;
                        }
                    }
                    if (charArray == null || (chunkLength = charArray.Length) == 0) {
                        // ????
                        throw new UnexpectedEndOfStreamException("String token terminated unexpectedly.", GetTailCharStream(), PeekCharStream());
                    }
                    if (chunkLength < MAX_STRING_LOOKAHEAD_SIZE) {
                        noMoreCharsInQueue = true;
                    }
                }
                d = charArray.GetChar(chunkCounter);
            }
            totalLookAheadLength += chunkCounter;
            if (tracingEnabled) {
                this.tailBuffer.Push(charArray.GetArray(), chunkCounter);
            }
            SkipChars(totalLookAheadLength);
            d = PeekChar();

            if (d == Symbols.DQUOTE) {
                // d = nextChar();
                SkipCharNoCheck();
                // sb.append(d);  // No append: Remove the trailing \".
            } else {
                // end of the json string.            // error???
                // return null;
            }
            return sb.ToString();
        }
        private string ReadString()
        {
            // Note that we may have already "consumed" the beginning \" if we are calling this from readStringWithLookAhead()...
            // So, the following does not work....
            //        // char c = nextChar();
            //        char c = nextCharNoCheck();
            //        if(c == 0 || c != Symbols.DQUOTE) {
            //            // This cannot happen.
            //            throw new UnexpectedSymbolException("Expecting String. Invalid token encountered: c = " + c, GetTailCharStream(), PeekCharStream());
            //        }
            StringBuilder sb = new StringBuilder();
            char c = PeekChar();
            if (c == 0) {
                // This cannot happen.
                throw new UnexpectedSymbolException("Expecting String. Invalid token encountered: c = " + c, GetTailCharStream(), PeekCharStream());
            } else if (c == Symbols.DQUOTE) {
                // consume the leading \".
                // c = nextCharNoCheck();
                SkipCharNoCheck();
                // sb.append(c);   // No append: Remove the leading \".
            } else {
                // We are already at the beginning of the string.
                // proceed.
            }
            bool escaped = false;
            char d = PeekChar();
            while (d != 0 && (escaped == true || d != Symbols.DQUOTE)) {
                // d = nextChar();
                d = NextCharNoCheck();
                if (escaped == false && d == Symbols.BACKSLASH) {
                    escaped = true;
                    // skip
                } else {
                    if (escaped == true) {
                        if (d == Symbols.UNICODE_PREFIX) {
                            // char[] hex = nextChars(4);
                            CyclicCharArray hex = NextCharsInQueue(4);
                            // TBD: validate ??
                            try {
                                // ????
                                // sb.append(Symbols.BACKSLASH).append(d).append(hex);
                                char u = UnicodeUtil.GetUnicodeChar(hex);
                                if (u != 0) {
                                    sb.Append(u);
                                } else {
                                    // ????
                                }
                            } catch (Exception e) {
                                throw new UnexpectedSymbolException("Invalid unicode char: hex = " + hex.ToString(), e, GetTailCharStream(), PeekCharStream());
                            }
                        } else {
                            if (Symbols.IsEscapableChar(d)) {
                                // TBD:
                                // Newline cannot be allowed within a string....
                                char e = Symbols.GetEscapedChar(d);
                                if (e != 0) {
                                    sb.Append(e);
                                } else { // This cannot happen.
                                }
                            } else { // error?
                                throw new UnexpectedSymbolException("Invalid escaped char: d = \\" + d, GetTailCharStream(), PeekCharStream());
                            }
                        }
                        // toggle the flag.
                        escaped = false;
                    } else {
                        // TBD:
                        // Exclude control characters ??? .....
                        sb.Append(d);
                    }
                }
                d = PeekChar();
            }
            if (d == Symbols.DQUOTE) {
                // d = nextChar();
                SkipCharNoCheck();
                // sb.append(d);  // No append: Remove the trailing \".
            } else {
                // end of the json string.            // error???
                // return null;
            }
            return sb.ToString();
        }
        private JsonToken DoNumber()
        {
            JsonToken token = JsonToken.INVALID;
            Number value = ReadNumber();
            token = TokenPool.Instance.GetToken(TokenType.NUMBER, value);
            // nextToken = null;
            return token;
        }
        // Need a better way to do this ....
        private Number ReadNumber()
        {
            // char c = nextChar();
            char c = NextCharNoCheck();
            if (!Symbols.IsStartingNumber(c)) {
                throw new UnexpectedSymbolException("Expecting a number. Invalid symbol encountered: c = " + c, GetTailCharStream(), PeekCharStream());
            }
            if (c == Symbols.PLUS) {
                // remove the leading +.
                c = NextChar();
            }
            StringBuilder sb = new StringBuilder();
            if (c == Symbols.MINUS) {
                sb.Append(c);
                c = NextChar();
            }
            bool periodRead = false;
            if (c == Symbols.PERIOD) {
                periodRead = true;
                sb.Append("0.");
            } else {
                // Could be a number, nothing else.
                if (c == '0') {
                    char c2 = PeekChar();
                    // This does not work because the number happens to be just zero ("0").
                    // if(c2 != Symbols.PERIOD) {
                    //     throw new UnexpectedSymbolException("Invalid number: c = " + sb.toString() + c + c2, GetTailCharStream(), PeekCharStream());
                    // }
                    // This should be better.    // zero followed by other number is not allowed.
                    if (char.IsDigit(c2)) {
                        throw new UnexpectedSymbolException("Invalid number: c = " + sb.ToString() + c + c2, GetTailCharStream(), PeekCharStream());
                    }
                    sb.Append(c);
                    if (c2 == Symbols.PERIOD) {
                        periodRead = true;
                        // sb.append(nextChar());
                        sb.Append(NextCharNoCheck());
                    }
                } else {
                    sb.Append(c);
                }
            }
            bool exponentRead = false;
            char d = PeekChar();
            while (d != 0 && (char.IsDigit(d) || (periodRead == false && d == Symbols.PERIOD) || (exponentRead == false && Symbols.IsExponentChar(d)))) {
                // sb.append(nextChar());
                sb.Append(NextCharNoCheck());
                if (d == Symbols.PERIOD) {
                    periodRead = true;
                }
                if (Symbols.IsExponentChar(d)) {
                    char d2 = PeekChar();
                    if (d2 == Symbols.PLUS || d2 == Symbols.MINUS || char.IsDigit(d2)) {
                        // sb.append(nextChar());
                        sb.Append(NextCharNoCheck());
                    } else {
                        throw new UnexpectedSymbolException("Invalid number: " + sb.ToString() + d2, GetTailCharStream(), PeekCharStream());
                    }
                    exponentRead = true;
                }
                d = PeekChar();
            }
            if (d == 0) {
                // end of the json string.            // ????
                // throw new UnexpectedEndOfStreamException("Invalid number: " + sb.toString(), GetTailCharStream(), PeekCharStream());
            } else { 
                // sb.append(nextChar());
            }
            string str = sb.ToString();
            var number = Number.Invalid;
            try {
                if (str.Contains(".")) {
                    double x = double.Parse(str);
                    // number = BigDecimal.valueOf(x);
                    number = new Number(x);
                } else {
                    long y = long.Parse(str);
                    // number = BigDecimal.valueOf(y);
                    number = new Number(y);
                }
                // } catch(NumberFormatException e) {
            } catch (Exception e) {
                throw new JsonTokenizerException("Invalid number encountered: str = " + str, e, GetTailCharStream(), PeekCharStream());
            }
            return number;
        }
        // because we called PeekChar() already,
        //      no need for check error conditions.
        private char NextCharNoCheck()
        {
            char ch = charQueue.Poll();
            if (tracingEnabled) {
                tailBuffer.Push(ch);
            }
            return ch;
        }
        private void SkipCharNoCheck()
        {
            if (tracingEnabled) {
                char ch = charQueue.Poll();
                tailBuffer.Push(ch);
            } else {
                charQueue.Skip();
            }
        }
        private char NextChar()
        {
            if (charQueue.IsEmpty) {
                if (readerEOF == false) {
                    try {
                        Forward();
                    } catch (IOException e) {
                        // ???
                        throw new JsonTokenizerException("Failed to forward character stream.", e, GetTailCharStream());
                    }
                }
            }
            if (charQueue.IsEmpty) {
                return (char) 0; 
                // ???
                // throw new UnexpectedEndOfStreamException("There is no character in the buffer.");
            }
            char ch = charQueue.Poll();
            if (tracingEnabled) {
                tailBuffer.Push(ch);
            }
            return ch;
        }
        private char[] NextChars(int length)
        {
            // assert length > 0
            if (charQueue.Size < length) {
                if (readerEOF == false) {
                    try {
                        Forward();
                    } catch (IOException e) {
                        // ???
                        throw new JsonTokenizerException("Failed to forward character stream.", e, GetTailCharStream());
                    }
                }
            }
            char[] c = null;
            if (charQueue.Size < length) {
                c = charQueue.Poll(charQueue.Size);
                // throw new UnexpectedEndOfStreamException("There is not enough characters in the buffer. length = " + length);
            }
            c = charQueue.Poll(length);
            if (tracingEnabled) {
                tailBuffer.Push(c);
            }
            return c;
        }
        private CyclicCharArray NextCharsInQueue(int length)
        {
            // assert length > 0
            if (charQueue.Size < length) {
                if (readerEOF == false) {
                    try {
                        Forward();
                    } catch (IOException e) {
                        throw new JsonTokenizerException("Failed to forward character stream.", e, GetTailCharStream());
                    }
                }
            }
            CyclicCharArray charArray = null;
            if (charQueue.Size < length) {
                charArray = charQueue.PollBuffer(charQueue.Size);
                // throw new UnexpectedEndOfStreamException("There is not enough characters in the buffer. length = " + length);
            }
            charArray = charQueue.PollBuffer(length);
            if (tracingEnabled) {
                char[] c = charArray.GetArray();
                tailBuffer.Push(c);
            }
            return charArray;
        }
        private void SkipChars(int length)
        {
            // assert length > 0
            if (charQueue.Size < length) {
                if (readerEOF == false) {
                    try {
                        Forward();
                    } catch (IOException e) {
                        throw new JsonTokenizerException("Failed to forward character stream.", e, GetTailCharStream());
                    }
                }
            }
            if (tracingEnabled) {
                char[] c = charQueue.Poll(length);
                tailBuffer.Push(c);
            } else {
                charQueue.Skip(length);
            }
        }
        // Note that PeekChar() and PeekChars() are "idempotent". 
        private char PeekChar()
        {
            if (charQueue.IsEmpty) {
                if (readerEOF == false) {
                    try {
                        Forward();
                    } catch (IOException e) {
                        throw new JsonTokenizerException("Failed to forward character stream.", e, GetTailCharStream());
                    }
                }
            }
            if (charQueue.IsEmpty) {
                return (char) 0;
                // throw new UnexpectedEndOfStreamException("There is no character in the buffer.");
            }
            return charQueue.Peek();
        }
        private char[] PeekChars(int length)
        {
            // assert length > 0
            if (charQueue.Size < length) {
                if (readerEOF == false) {
                    try {
                        Forward();
                    } catch (IOException e) {
                        throw new JsonTokenizerException("Failed to forward character stream.", e, GetTailCharStream());
                    }
                }
            }
            if (charQueue.Size < length) {
                return charQueue.Peek(charQueue.Size);
                // throw new UnexpectedEndOfStreamException("There is not enough characters in the buffer. length = " + length);
            }
            return charQueue.Peek(length);
        }
        private CyclicCharArray PeekCharsInQueue(int length)
        {
            // assert length > 0
            if (charQueue.Size < length) {
                if (readerEOF == false) {
                    try {
                        Forward();
                    } catch (IOException e) {
                        // ???
                        throw new JsonTokenizerException("Failed to forward character stream.", e, GetTailCharStream());
                    }
                }
            }
            if (charQueue.Size < length) {
                return charQueue.PeekBuffer(charQueue.Size);
                // throw new UnexpectedEndOfStreamException("There is not enough characters in the buffer. length = " + length);
            }
            return charQueue.PeekBuffer(length);
        }
        private CyclicCharArray PeekCharsInQueue(int offset, int length)
        {
            // assert length > 0
            if (charQueue.Size < offset + length) {
                if (readerEOF == false) {
                    try {
                        Forward();
                    } catch (IOException e) {
                        // ???
                        throw new JsonTokenizerException("Failed to forward character stream.", e, GetTailCharStream());
                    }
                }
            }
            if (charQueue.Size < offset + length) {
                return charQueue.PeekBuffer(offset, charQueue.Size - offset);
                // throw new UnexpectedEndOfStreamException("There is not enough characters in the buffer. length = " + length);
            }
            return charQueue.PeekBuffer(offset, length);
        }
        // Poll next char (and gobble up),
        // and return the next char (without removing it)
        private char SkipAndPeekChar()
        {
            int qSize = charQueue.Size;
            if (qSize < 2) {
                if (readerEOF == false) {
                    try {
                        Forward();
                        qSize = charQueue.Size;
                    } catch (IOException e) {
                        throw new JsonTokenizerException("Failed to forward character stream.", e, GetTailCharStream());
                    }
                }
            }
            if (qSize > 0) {
                if (tracingEnabled) {
                    char ch = charQueue.Poll();
                    tailBuffer.Push(ch);
                } else {
                    charQueue.Skip();
                }
                if (qSize > 1) {
                    return charQueue.Peek();
                }
            }
            return (char) 0;
            // throw new UnexpectedEndOfStreamException("There is no character in the buffer.");
        }
        // Read some more bytes from the reader.
        private readonly char[] buff = new char[READER_BUFF_SIZE];
        private void Forward()
        {
            if (readerEOF == false) {
                // if (reader.ready()) { // To avoid blocking
                    int cnt = 0;
                    try {
                        // Java: This throws OOB excpetion at the end....
                        // cnt = reader.read(buff, curReaderIndex, READER_BUFF_SIZE);
                        // ???
                        cnt = reader.Read(buff, 0, READER_BUFF_SIZE);
                    } catch (System.IndexOutOfRangeException e) {
                        // ???
                        // Why does this happen? Does it happen for StringReader only???
                        //    Does read(,,) ever return -1 in the case of StringReader ???
                        //if (log.isLoggable(Level.INFO)) {
                        //    // log.log(Level.INFO, "Looks like we have reached the end of the reader.", e);
                        //}
                    }
                    if (cnt == -1 || cnt == 0) {
                        readerEOF = true;
                    } else {
                        bool suc = charQueue.AddAll(buff, cnt);
                        if (suc) {
                            curReaderIndex += cnt;
                        } else {
                            // ???
                            // Is this because the json file includes a loooooong string???
                            // temporarily change the LookAheadParsing flag and try again???
                            // --->
                            // Unfortunately this does not work because we are already in the middle of parsing string..
                            //    Unless we can rewind the stack in some way, this does not really help...
                            // if(isLookAheadParsing()) {
                            //     DisableLookAheadParsing();
                            //     // log.warning("Unexpected internal error occurred. Characters were not added to CharQueue: cnt = " + cnt + ". Trying again after calling DisableLookAheadParsing().");
                            //     forward();   // We should be careful not to get into the infinite loop....
                            // } else {
                            //     throw new JsonTokenizerException("Unexpected internal error occurred. Characters were not added to CharQueue: cnt = " + cnt, GetTailCharStream());
                            // }
                            throw new UnexpectedEndOfStreamException("Unexpected internal error occurred. Characters were not added to CharQueue: cnt = " + cnt, GetTailCharStream());
                        }
                    }
                //} else {
                //    // ????
                //    readerEOF = true;
                //    // Why does this happen ????
                //    // if(log.isLoggable(Level.INFO)) // log.log(Level.INFO, "Looks like we have not read all characters because the reader is blocked. We'll likely have a parser error down the line.");
                //    // throw new UnexpectedEndOfStreamException("Read is blocked. Bailing out.");
                //}
            }
        }
    }
}