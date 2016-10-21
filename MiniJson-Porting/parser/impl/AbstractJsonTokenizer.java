package org.minijson.parser.impl;

import static org.minijson.parser.core.TokenPool.TOKEN_EOF;

import java.io.IOException;
import java.io.Reader;
import java.io.StringReader;
import java.util.Arrays;
import java.util.logging.Level;
import java.util.logging.Logger;

import org.minijson.common.CyclicCharArray;
import org.minijson.common.Literals;
import org.minijson.common.Symbols;
import org.minijson.parser.LookAheadJsonTokenizer;
import org.minijson.parser.TraceableJsonTokenizer;
import org.minijson.parser.core.CharBuffer;
import org.minijson.parser.core.CharQueue;
import org.minijson.parser.core.ErrorContext;
import org.minijson.parser.core.JsonToken;
import org.minijson.parser.core.TokenPool;
import org.minijson.parser.core.TokenTypes;
import org.minijson.parser.exception.JsonTokenizerException;
import org.minijson.parser.exception.UnexpectedEndOfStreamException;
import org.minijson.parser.exception.UnexpectedSymbolException;
import org.minijson.parser.policy.ParserPolicy;
import org.minijson.parser.policy.base.DefaultParserPolicy;
import org.minijson.util.CharacterUtil;
import org.minijson.util.LiteralUtil;
import org.minijson.util.UnicodeUtil;


// Base class for JsonTokenizer implementation.
// TBD: 
// Make a tokenzier reusable (without having to create a new tokenizer for every parse()'ing.) ???
// ...
// Note:
// Current implementation is not thread-safe (and, probably, never will be).
// 
public abstract class AbstractJsonTokenizer implements TraceableJsonTokenizer, LookAheadJsonTokenizer
{
    private static final Logger log = Logger.getLogger(AbstractJsonTokenizer.class.getName());

    // MAX_STRING_LOOKAHEAD_SIZE should be greater than 6.
    private static final int MAX_STRING_LOOKAHEAD_SIZE = 256;   // temporary
    // private static final int MAX_STRING_LOOKAHEAD_SIZE = 512;   // temporary
    private static final int MAX_SPACE_LOOKAHEAD_SIZE = 32;     // temporary
    // Note that the charqueue size should be bigger than the longest string in the file + reader_buff_size
    //       (if we use "look ahead" for string parsing)!!!!
    // The parser/tokenizer fails when it encounters a string longer than that.
    // We cannot obviously use arbitrarily long char queue size, and
    // unfortunately, there is no limit in the size of a string in JSON,
    //    which makes it possible that the mini json parser can always fail, potentially...
    private static final int CHARQUEUE_SIZE = 4096;     // temporary
    // Note that CHARQUEUE_SIZE - delta >= READER_BUFF_SIZE
    //     where delta is "false".length,
    //          or, more precisely the max length of peekChars(len).
    //          or, if we use string lookahead, it should be the size of the longest string rounded up to MAX_STRING_LOOKAHEAD_SIZE multiples...
    private static final int READER_BUFF_SIZE = 1024;   // temporary
    private static final int TAILBUFFER_SIZE = 512;     // temporary
    // ...
    private static final int TAIl_TRACE_LENGTH = 150;    // temporary
    private static final int HEAD_TRACE_LENGTH = 25;     // temporary
    // ...

    private Reader reader;
    private int curReaderIndex = 0;       // global var to keep track of the reader reading state.
    private boolean readerEOF = false;     // true does not mean we are done, because we are using buffer.
    private JsonToken curToken = null;
    private JsonToken nextToken = null;   // or, TOKEN_EOF ???
    private JsonToken nextNextToken = null;   // ??? TBD: ...
//    private CharacterQueue charQueue = null;
    // Note that charQueue is class-wide global variable.
    private final CharQueue charQueue;
    // tailBuffer is used for debugging/error handling purpose.
    private final CharBuffer tailBuffer;
    // Strict or Lenient?
    private final ParserPolicy parserPolicy;
    // If true, use "look ahead" algorithms.
    private boolean lookAheadParsing;
    // Whether "tracing" is enabled or not.
    // Tracing, at this point, means that we simply keep the char tail buffer
    //    so that when an error occurs we can see the "exception context".
    private boolean tracingEnabled;

    // Ctor's
    public AbstractJsonTokenizer(String str)
    {
        this(new StringReader(str));
    }
    public AbstractJsonTokenizer(Reader reader)
    {
        this(reader, null);
    }
    public AbstractJsonTokenizer(Reader reader, ParserPolicy parserPolicy)
    {
        this(reader, parserPolicy, CHARQUEUE_SIZE);
    }
    public AbstractJsonTokenizer(Reader reader, ParserPolicy parserPolicy, int charQueueSize)
    {
        // Reader cannot cannot be null.
        this.reader = reader;
        if(parserPolicy == null) {
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
        // gobbleUpSpace();
        
        // For subclasses
        init();
    }
    
    // Having multiple ctor's is a bit inconvenient.
    // Put all init routines here.
    protected void init()
    {
        // Override this in subclasses.
    }
    
    
    // Make tokenizer re-usable through reset().
    // TBD: Need to verify this....

    public void reset(String str)
    {
        reset(new StringReader(str));
    }
    public void reset(Reader reader)
    {
        // This is essentially a copy of ctor.
        
        // Reader cannot cannot be null.
        this.reader = reader;
        this.charQueue.clear();
        this.tailBuffer.clear();
        // this.parserPolicy cannot be changed.
        // lookAheadParsing = true;
        // tracingEnabled = false;
        
        // Reset the "current state" vars.
        readerEOF = false;
        curToken = null;
        nextToken = null;

        // No need to call this... ???
        // init();
    }
    

    // For subclasses.
    protected ParserPolicy getParserPolicy()
    {
        return this.parserPolicy;
    }

    
    @Override
    public boolean isLookAheadParsing()
    {
        return lookAheadParsing;
    }
//    public void setLookAheadParsing(boolean lookAheadParsing)
//    {
//        this.lookAheadParsing = lookAheadParsing;
//    }
    @Override
    public void enableLookAheadParsing()
    {
        this.lookAheadParsing = true;
    }
    @Override
    public void disableLookAheadParsing()
    {
        this.lookAheadParsing = false;
    }

    
    // TraceableJsonTokenizer interface.
    // These are primarily for debugging purposes...

    @Override
    public void enableTracing()
    {
        tracingEnabled = true;
    }
    @Override
    public void disableTracing()
    {
        tracingEnabled = false;
    }
    @Override
    public boolean isTracingEnabled()
    {
        return tracingEnabled;
    }

    @Override
    public String tailCharsAsString(int length)
    {
        // return tailBuffer.tailAsString(length);
        char[] c = tailCharStream(length);
        if(c != null) {
            return new String(c);
        } else {
            return "";   // ????
        }
    }
    @Override
    public char[] tailCharStream(int length)
    {
        return tailBuffer.tail(length);
    }
    private char[] tailCharStream()
    {
        return tailCharStream(TAIl_TRACE_LENGTH);
    }

    @Override
    public String peekCharsAsString(int length)
    {
        char[] c = peekCharStream(length);
        if(c != null) {
            return new String(c);
        } else {
            return "";   // ????
        }
    }
    @Override
    public char[] peekCharStream(int length)
    {
        char[] c = null;
        try {
            c = peekChars(length);
        } catch (JsonTokenizerException e) {
            log.log(Level.WARNING, "Failed to peek char stream: length = " + length, e);
        }
        return c;
    }
    public char[] peekCharStream()
    {
        return peekCharStream(HEAD_TRACE_LENGTH);
    }
 
    
    // TBD:
    // These methods really need to be synchronized.
    // ...
    
    @Override
    public boolean hasMore() throws JsonTokenizerException
    {
        if(nextToken == null) {
            nextToken = prepareNextToken();
        }

        if(nextToken == null || TOKEN_EOF.equals(nextToken)) {
            return false;
        } else {
            return true;
        }
    }

    @Override
    public JsonToken next() throws JsonTokenizerException
    {
        if(nextToken == null) {
            nextToken = prepareNextToken();
        }
        curToken = nextToken;
        nextToken = null;
 
        return curToken;
    }

    @Override
    public JsonToken peek() throws JsonTokenizerException
    {
        if(nextToken != null) {
            return nextToken;
        }
        
        nextToken = prepareNextToken();
        return nextToken;
    }

    // temporary
    // Does this save anything compared to next();peek(); ????
    //    (unless we can do prepareNextTwoTokens() .... ? )
    // Remove the next token (and throw away),
    // and return the next token (without removing it).
    public JsonToken nextAndPeek() throws JsonTokenizerException
    {
        if(nextToken == null) {
            curToken = prepareNextToken();
        } else {
            curToken = nextToken;
        }
        nextToken = prepareNextToken();
        return nextToken;
    }

    
    // Note that this method is somewhat unusual in that it cannot be called arbitrarily.
    // This method changes the internal state by changing the charQueue.
    // This should be called by a certain select methods only!!!
    private JsonToken prepareNextToken() throws JsonTokenizerException
    {
        if(nextToken != null) {
            // ???
            return nextToken;
        }

        JsonToken token = null;
        char ch;
        // ch has been peeked, but not popped.
        if(lookAheadParsing) {
            ch = gobbleUpSpaceLookAhead();
        } else {
            ch = gobbleUpSpace();   
        }

        // Create a JsonToken and,
        // reset the curToken.
        switch(ch) {
        case Symbols.COMMA:
        case Symbols.COLON:
        case Symbols.LSQUARE:
        case Symbols.LCURLY:
        case Symbols.RSQUARE:
        case Symbols.RCURLY:
            token = TokenPool.getSymbolToken(ch);
            // nextChar();   // Consume the current token.
            skipCharNoCheck();   // Consume the current token.
            // nextToken = null;
            break;
        case Symbols.NULL_START:
        case Symbols.NULL_START_UPPER:
            token = doNullLiteral();
            break;
        case Symbols.TRUE_START:
        case Symbols.TRUE_START_UPPER:
            token = doTrueLiteral();
            break;
        case Symbols.FALSE_START:
        case Symbols.FALSE_START_UPPER:
            token = doFalseLiteral();
            break;
        case Symbols.DQUOTE:
            token = doString();
            break;
        case 0:
            // ???
            token = TokenPool.TOKEN_EOF;
            // nextChar();   // Consume the current token.
            skipCharNoCheck();   // Consume the current token.
            break;
        default:
            if(Symbols.isStartingNumber(ch)) {
                // log.warning(">>>>>>>>>>>>>>>>>>>>>>>>>> ch = " + ch);
                token = doNumber();
                // log.warning(">>>>>>>>>>>>>>>>>>>>>>>>>> number token = " + token);
            } else {
                throw new UnexpectedSymbolException("Invalid symbol encountered: ch = " + ch, tailCharStream(), peekCharStream());
            }
            break;
        }

        return token;
    }

    
    
    // Returns the next peeked character.
    // Return value of 0 means we have reached the end of the json string.
    // TBD: use "look ahead" implementation similar to readString() ????
    private char gobbleUpSpace()
    {
        char c = 0;
        try {
            c = peekChar();
            //while(c != 0 && Character.isSpaceChar(c)) {  // ???  -> this doesn't seem to work....
            // while(c != 0 && Character.isWhitespace(c)  ) {  // ???
            while(c != 0 && CharacterUtil.isWhitespace(c)  ) {  // ???
                // nextChar();   // gobble up space.
                // c = peekChar();
                c = skipAndPeekChar();
            }
        } catch(JsonTokenizerException e) {
            // ????
            if(log.isLoggable(Level.INFO)) log.log(Level.INFO, "Failed to consume space: " + ErrorContext.buildContextString(tailCharStream(), peekCharStream()), e);
            c = 0;
        }
        return c;
    }
    // Note that this is effective only for "formatted" JSON with lots of consecutive spaces...
    private char gobbleUpSpaceLookAhead()
    {
        char c = 0;
        try {
            c = peekChar();
            // if(Character.isWhitespace(c)) {
            if(CharacterUtil.isWhitespace(c)) {
                // skipCharNoCheck();
                c = skipAndPeekChar();
                
                // Spaces tend appear together.
                // if(Character.isWhitespace(c)) {
                if(CharacterUtil.isWhitespace(c)) {
                    int chunkLength;
                    CyclicCharArray charArray = peekCharsInQueue(MAX_SPACE_LOOKAHEAD_SIZE);
                    // if(charArray == null || (chunkLength = charArray.getLength()) == 0) {
                    //     return c;
                    // }
                    chunkLength = charArray.getLength();
    
                    int chunkCounter = 0;
                    int totalLookAheadLength = 0;
                    c = charArray.getChar(0);
                    // while((chunkCounter < chunkLength - 1) && Character.isWhitespace(c) ) {
                    while((chunkCounter < chunkLength - 1) && CharacterUtil.isWhitespace(c) ) {
                        ++chunkCounter;
    
                        if(chunkCounter >= chunkLength - 1) {
                            totalLookAheadLength += chunkCounter;
                            if(tracingEnabled) {
                                this.tailBuffer.push(charArray.getArray(), chunkCounter);
                            }
                            chunkCounter = 0;   // restart a loop.
    
                            charArray = peekCharsInQueue(totalLookAheadLength, MAX_SPACE_LOOKAHEAD_SIZE);
                            if(charArray == null || (chunkLength = charArray.getLength()) == 0) {
                                break;
                            }
                        }
                        c = charArray.getChar(chunkCounter);
                    }
                    totalLookAheadLength += chunkCounter;
                    if(tracingEnabled) {
                        this.tailBuffer.push(charArray.getArray(), chunkCounter);
                    }
                    skipChars(totalLookAheadLength);
                    c = peekChar();
                }
            }
        } catch(JsonTokenizerException e) {
            // ????
            if(log.isLoggable(Level.INFO)) log.log(Level.INFO, "Failed to consume space: " + ErrorContext.buildContextString(tailCharStream(), peekCharStream()), e);
            c = 0;
        }
        return c;
    }

    private JsonToken doNullLiteral() throws JsonTokenizerException
    {
        JsonToken token = null;
        int length = Literals.NULL_LENGTH;
        // char[] c = nextChars(length);
        CyclicCharArray c = nextCharsInQueue(length);
        if(parserPolicy.caseInsensitiveLiterals() ? 
                LiteralUtil.isNullIgnoreCase(c) : 
                LiteralUtil.isNull(c)
            ) {
            token = TokenPool.TOKEN_NULL;
            // nextToken = null;
        } else {
            // throw new UnexpectedSymbolException("Unexpected string: " + Arrays.toString(c), tailCharStream(), peekCharStream());
            throw new UnexpectedSymbolException("Unexpected string: " + (c==null ? "" : c.toString()), tailCharStream(), peekCharStream());
        }        
        return token;
    }
    private JsonToken doTrueLiteral() throws JsonTokenizerException
    {
        JsonToken token = null;
        int length = Literals.TRUE_LENGTH;
        // char[] c = nextChars(length);
        CyclicCharArray c = nextCharsInQueue(length);
        if(parserPolicy.caseInsensitiveLiterals() ? 
                LiteralUtil.isTrueIgnoreCase(c) : 
                LiteralUtil.isTrue(c)
            ) {
            token = TokenPool.TOKEN_TRUE;
            // nextToken = null;
        } else {
            // throw new UnexpectedSymbolException("Unexpected string: " + Arrays.toString(c), tailCharStream(), peekCharStream());
            throw new UnexpectedSymbolException("Unexpected string: " + (c==null ? "" : c.toString()), tailCharStream(), peekCharStream());
        }        
        return token;
    }
    private JsonToken doFalseLiteral() throws JsonTokenizerException
    {
        JsonToken token = null;
        int length = Literals.FALSE_LENGTH;
        // char[] c = nextChars(length);
        CyclicCharArray c = nextCharsInQueue(length);
        if(parserPolicy.caseInsensitiveLiterals() ? 
                LiteralUtil.isFalseIgnoreCase(c) : 
                LiteralUtil.isFalse(c)
            ) {
            token = TokenPool.TOKEN_FALSE;
            // nextToken = null;
        } else {
            // throw new UnexpectedSymbolException("Unexpected string: " + Arrays.toString(c), tailCharStream(), peekCharStream());
            throw new UnexpectedSymbolException("Unexpected string: " + (c==null ? "" : c.toString()), tailCharStream(), peekCharStream());
        }   
        return token;
    }
    
    // Note that there is no "character".
    // Character is a single letter string.
    private JsonToken doString() throws JsonTokenizerException
    {
        JsonToken token = null;
        String value;
        if(lookAheadParsing) {
            value = readStringWithLookAhead();
        } else {
            value = readString();
        }
        token = TokenPool.getInstance().getToken(TokenTypes.STRING, value);
        // nextToken = null;
        return token;
    }
    

    // Note:
    // This will cause parse failing
    //     if the longest string in JSON is longer than (CHARQUEUE_SIZE - READER_BUFF_SIZE)
    //     because forward() will fail.
    // TBD:
    // There might be bugs when dealing with short strings, or \\u escaped unicodes at the end of a json string
    // ...
    private String readStringWithLookAhead() throws JsonTokenizerException
    {
        // char c = nextChar();
        char c = nextCharNoCheck();
        if(c == 0 || c != Symbols.DQUOTE) {
            // This cannot happen.
            throw new UnexpectedSymbolException("Expecting String. Invalid token encountered: c = " + c, tailCharStream(), peekCharStream());
        }
        StringBuilder sb = new StringBuilder();
        // sb.append(c);   // No append: Remove the leading \".
        
        boolean escaped = false;
        
        
        int chunkLength;
        CyclicCharArray charArray = peekCharsInQueue(MAX_STRING_LOOKAHEAD_SIZE);
        if(charArray == null || (chunkLength = charArray.getLength()) == 0) {
            // ????
            throw new UnexpectedEndOfStreamException("String token terminated unexpectedly.", tailCharStream(), peekCharStream());
        }
        boolean noMoreCharsInQueue = false;
        if(chunkLength < MAX_STRING_LOOKAHEAD_SIZE) {
            noMoreCharsInQueue = true;
        }
        boolean needMore = false;
        int chunkCounter = 0;
        int totalLookAheadLength = 0;
        char d = charArray.getChar(0);
        // log.warning(">>>>>>>>>>>>>>>>>> d = " + d);
        // log.warning(">>>>>>>>>>>>>>>>>> chunkLength = " + chunkLength);
        while((chunkCounter < chunkLength - 1) &&    // 6 for "\\uxxxx". 
                d != 0 && 
                (escaped == true || d != Symbols.DQUOTE )) {
            // d = charArray.getChar(++chunkCounter);
            ++chunkCounter;
            
            // log.warning(">>>>>>>>>>>>>>>>>> d = " + d);
            
            if(escaped == false && d == Symbols.BACKSLASH) {
                escaped = true;
                // skip
            } else {
                if(escaped == true) {
                    if(d == Symbols.UNICODE_PREFIX) {
                        if(chunkCounter < chunkLength - 4) {
                            char[] hex = charArray.getChars(chunkCounter, 4);
                            chunkCounter += 4;
                            
                            try {
                                // ????
                                // sb.append(Symbols.BACKSLASH).append(d).append(hex);
                                char u = UnicodeUtil.getUnicodeChar(hex);
                                if(u != 0) {
                                    sb.append(u);
                                } else {
                                    // ????
                                }
                            } catch(Exception e) {
                                throw new UnexpectedSymbolException("Invalid unicode char: hex = " + Arrays.toString(hex), e, tailCharStream(), peekCharStream());
                            }
                        } else {
                            if(noMoreCharsInQueue == false) {
                                needMore = true;
                                chunkCounter -= 2;     // Reset the counter backward for "\\u".
                            } else {
                                // error
                                throw new UnexpectedSymbolException("Invalid unicode char.", tailCharStream(), peekCharStream());
                            }
                        }
                    } else {
                        if(Symbols.isEscapableChar(d)) {
                            // TBD:
                            // Newline cannot be allowed within a string....
                            // ....
                            char e = Symbols.getEscapedChar(d);
                            if(e != 0) {
                                sb.append(e);
                            } else {
                                // This cannot happen.
                            }
                        } else {
                            // error?
                            throw new UnexpectedSymbolException("Invalid escaped char: d = \\" + d, tailCharStream(), peekCharStream());
                        }
                    }
                    // toggle the flag.
                    escaped = false;
                } else {
                    
                    // TBD:
                    // Exclude control characters ???
                    // ...
                    
                    sb.append(d);
                }
            }
            
            if((noMoreCharsInQueue == false) && (needMore || chunkCounter >= chunkLength - 1)) {
                totalLookAheadLength += chunkCounter;
                if(tracingEnabled) {
                    this.tailBuffer.push(charArray.getArray(), chunkCounter);
                }
                chunkCounter = 0;   // restart a loop.
                needMore = false;
                // log.warning(">>>>>>>>>>>>>>>>>>>>>> addAll() totalLookAheadLength = " + totalLookAheadLength);

                try {
                    charArray = peekCharsInQueue(totalLookAheadLength, MAX_STRING_LOOKAHEAD_SIZE);
                } catch(UnexpectedEndOfStreamException e) {
                    // Not sure if this makes sense....
                    // but since this error might have been due to the fact that we have encountered a looooong string,
                    // Try again???
                    // ...
                    // Note that this makes it hard to reuse the parser instance....
                    // (in some way, it's a good thing, because the json files tend to be similar in the given context,
                    //     and if one file has a loooong string, then it's likely that others have long strings as well....)
                    // ....
                    // We should be careful not to get into the infinite loop....
                    if(isLookAheadParsing()) {  // This if() is always true at this point...
                        disableLookAheadParsing();
                        log.warning("String token might have been too long.  Trying again after calling disableLookAheadParsing().");

                        // Reset the buffer (peek() status) ????, and call the non "look ahead" version...
                        return readString();   // Is this starting from the beginning???
                        // ...
                    } else {
                        // This cannot happen..
                        throw e;
                    }
                }
                if(charArray == null || (chunkLength = charArray.getLength()) == 0) {
                    // ????
                    throw new UnexpectedEndOfStreamException("String token terminated unexpectedly.", tailCharStream(), peekCharStream());
                }
                if(chunkLength < MAX_STRING_LOOKAHEAD_SIZE) {
                    noMoreCharsInQueue = true;
                }
            }
            
            d = charArray.getChar(chunkCounter);
        }
        totalLookAheadLength += chunkCounter;
        if(tracingEnabled) {
            this.tailBuffer.push(charArray.getArray(), chunkCounter);
        }
        skipChars(totalLookAheadLength);
        d = peekChar();

        if(d == Symbols.DQUOTE) {
            // d = nextChar();
            skipCharNoCheck();
            // sb.append(d);  // No append: Remove the trailing \".
        } else {
            // end of the json string.
            // error???
            // return null;
        }
        
        return sb.toString();
    }


    private String readString() throws JsonTokenizerException
    {
        // Note that we may have already "consumed" the beginning \" if we are calling this from readStringWithLookAhead()...
        // So, the following does not work....

//        // char c = nextChar();
//        char c = nextCharNoCheck();
//        if(c == 0 || c != Symbols.DQUOTE) {
//            // This cannot happen.
//            throw new UnexpectedSymbolException("Expecting String. Invalid token encountered: c = " + c, tailCharStream(), peekCharStream());
//        }

        StringBuilder sb = new StringBuilder();

        char c = peekChar();
        if(c == 0) {
            // This cannot happen.
            throw new UnexpectedSymbolException("Expecting String. Invalid token encountered: c = " + c, tailCharStream(), peekCharStream());
        } else if(c == Symbols.DQUOTE) {
            // consume the leading \".
            // c = nextCharNoCheck();
            skipCharNoCheck();
            // sb.append(c);   // No append: Remove the leading \".
        } else {
            // We are already at the beginning of the string.
            // proceed.
        }

        boolean escaped = false;
        char d = peekChar();
        while(d != 0 && (escaped == true || d != Symbols.DQUOTE )) {
            // d = nextChar();
            d = nextCharNoCheck();
            if(escaped == false && d == Symbols.BACKSLASH) {
                escaped = true;
                // skip
            } else {
                if(escaped == true) {
                    if(d == Symbols.UNICODE_PREFIX) {
                        // char[] hex = nextChars(4);
                        CyclicCharArray hex = nextCharsInQueue(4);
                        // TBD: validate ??
                        
                        try {
                            // ????
                            // sb.append(Symbols.BACKSLASH).append(d).append(hex);
                            char u = UnicodeUtil.getUnicodeChar(hex);
                            if(u != 0) {
                                sb.append(u);
                            } else {
                                // ????
                            }
                        } catch(Exception e) {
                            throw new UnexpectedSymbolException("Invalid unicode char: hex = " + hex.toString(), e, tailCharStream(), peekCharStream());
                        }
                    } else {
                        if(Symbols.isEscapableChar(d)) {
                            // TBD:
                            // Newline cannot be allowed within a string....
                            // ....
                            char e = Symbols.getEscapedChar(d);
                            if(e != 0) {
                                sb.append(e);
                            } else {
                                // This cannot happen.
                            }
                        } else {
                            // error?
                            throw new UnexpectedSymbolException("Invalid escaped char: d = \\" + d, tailCharStream(), peekCharStream());
                        }
                    }
                    // toggle the flag.
                    escaped = false;
                } else {
                    
                    // TBD:
                    // Exclude control characters ???
                    // ...
                    
                    sb.append(d);
                }
            }
            d = peekChar();
        }
        if(d == Symbols.DQUOTE) {
            // d = nextChar();
            skipCharNoCheck();
            // sb.append(d);  // No append: Remove the trailing \".
        } else {
            // end of the json string.
            // error???
            // return null;
        }
        
        return sb.toString();
    }
    
    
    private JsonToken doNumber() throws JsonTokenizerException
    {
        JsonToken token = null;
        Number value = readNumber();
        token = TokenPool.getInstance().getToken(TokenTypes.NUMBER, value);
        // nextToken = null;
        return token;
    }

    // Need a better way to do this ....
    private Number readNumber() throws JsonTokenizerException
    {
        // char c = nextChar();
        char c = nextCharNoCheck();
        if(! Symbols.isStartingNumber(c)) {
            throw new UnexpectedSymbolException("Expecting a number. Invalid symbol encountered: c = " + c, tailCharStream(), peekCharStream());
        }

        if(c == Symbols.PLUS) {
            // remove the leading +.
            c = nextChar();
        }

        StringBuilder sb = new StringBuilder();

        if(c == Symbols.MINUS) {
            sb.append(c);
            c = nextChar();
        }

        boolean periodRead = false;
        if(c == Symbols.PERIOD) {
            periodRead = true;
            sb.append("0.");
        } else {
            // Could be a number, nothing else.
            if(c == '0') {
                char c2 = peekChar();
                // This does not work because the number happens to be just zero ("0").
                // if(c2 != Symbols.PERIOD) {
                //     throw new UnexpectedSymbolException("Invalid number: c = " + sb.toString() + c + c2, tailCharStream(), peekCharStream());
                // }
                // This should be better.
                // zero followed by other number is not allowed.
                if(Character.isDigit(c2)) {
                    throw new UnexpectedSymbolException("Invalid number: c = " + sb.toString() + c + c2, tailCharStream(), peekCharStream());
                }
                sb.append(c);
                if(c2 == Symbols.PERIOD) {
                    periodRead = true;
                    // sb.append(nextChar());
                    sb.append(nextCharNoCheck());
                }
            } else {
                sb.append(c);
            }
        }
        
        boolean exponentRead = false;
        
        char d = peekChar();
        while(d != 0 && (Character.isDigit(d) || 
                (periodRead == false && d == Symbols.PERIOD) ||
                (exponentRead == false && Symbols.isExponentChar(d))
                )) {
            // sb.append(nextChar());
            sb.append(nextCharNoCheck());
            if(d == Symbols.PERIOD) {
                periodRead = true;
            }
            if(Symbols.isExponentChar(d)) {
                char d2 = peekChar();
                if(d2 == Symbols.PLUS || d2 == Symbols.MINUS || Character.isDigit(d2)) {
                    // sb.append(nextChar());
                    sb.append(nextCharNoCheck());
                } else {
                    throw new UnexpectedSymbolException("Invalid number: " + sb.toString() + d2, tailCharStream(), peekCharStream());
                }
                exponentRead = true;
            }
            d = peekChar();
        }
        if(d == 0) {
            // end of the json string.
            // ????
            // throw new UnexpectedEndOfStreamException("Invalid number: " + sb.toString(), tailCharStream(), peekCharStream());
        } else {
            // sb.append(nextChar());
        }
        
        String str = sb.toString();
        
        Number number = null;
        try {
            if(str.contains(".")) {
                double x = Double.parseDouble(str);
                // number = BigDecimal.valueOf(x);
                number = x;
            } else {
                long y = Long.parseLong(str);
                // number = BigDecimal.valueOf(y);
                number = y;
            }
        // } catch(NumberFormatException e) {
        } catch(Exception e) {
            // ???
            throw new JsonTokenizerException("Invalid number encountered: str = " + str, e, tailCharStream(), peekCharStream());
        }
        return number;
    }

    
    // because we called peekChar() already,
    //      no need for check error conditions.
    private char nextCharNoCheck() throws JsonTokenizerException
    {
        char ch = charQueue.poll();
        if(tracingEnabled) tailBuffer.push(ch);
        return ch;
    }
    private void skipCharNoCheck() throws JsonTokenizerException
    {
        if(tracingEnabled) {
            char ch = charQueue.poll();
            tailBuffer.push(ch);
        } else {
            charQueue.skip();
        }
    }

    private char nextChar() throws JsonTokenizerException
    {
        if(charQueue.isEmpty()) {
            if(readerEOF == false) {
                try {
                    forward();
                } catch (IOException e) {
                    // ???
                    throw new JsonTokenizerException("Failed to forward character stream.", e, tailCharStream());
                }
            }
        }
        if(charQueue.isEmpty()) {
            return 0;   // ???
            // throw new UnexpectedEndOfStreamException("There is no character in the buffer.");
        }
        char ch = charQueue.poll();
        if(tracingEnabled) tailBuffer.push(ch);
        return ch;
    }
    private char[] nextChars(int length) throws JsonTokenizerException
    {
        // assert length > 0
        if(charQueue.size() < length) {
            if(readerEOF == false) {
                try {
                    forward();
                } catch (IOException e) {
                    // ???
                    throw new JsonTokenizerException("Failed to forward character stream.", e, tailCharStream());
                }
            }
        }
        char[] c = null;
        if(charQueue.size() < length) {
            c = charQueue.poll(charQueue.size());
            // throw new UnexpectedEndOfStreamException("There is not enough characters in the buffer. length = " + length);
        }
        c = charQueue.poll(length);
        if(tracingEnabled) tailBuffer.push(c);
        return c;
    }
    private CyclicCharArray nextCharsInQueue(int length) throws JsonTokenizerException
    {
        // assert length > 0
        if(charQueue.size() < length) {
            if(readerEOF == false) {
                try {
                    forward();
                } catch (IOException e) {
                    // ???
                    throw new JsonTokenizerException("Failed to forward character stream.", e, tailCharStream());
                }
            }
        }
        CyclicCharArray charArray = null;
        if(charQueue.size() < length) {
            charArray = charQueue.pollBuffer(charQueue.size());
            // throw new UnexpectedEndOfStreamException("There is not enough characters in the buffer. length = " + length);
        }
        charArray = charQueue.pollBuffer(length);
        if(tracingEnabled) {
            char[] c = charArray.getArray();
            tailBuffer.push(c);
        }
        return charArray;
    }
    
    private void skipChars(int length) throws JsonTokenizerException
    {
        // assert length > 0
        if(charQueue.size() < length) {
            if(readerEOF == false) {
                try {
                    forward();
                } catch (IOException e) {
                    // ???
                    throw new JsonTokenizerException("Failed to forward character stream.", e, tailCharStream());
                }
            }
        }
        if(tracingEnabled) {
            char[] c = charQueue.poll(length);
            tailBuffer.push(c);
        } else {
            charQueue.skip(length);
        }
    }

    
    

    // Note that peekChar() and peekChars() are "idempotent". 
    private char peekChar() throws JsonTokenizerException
    {
        if(charQueue.isEmpty()) {
            if(readerEOF == false) {
                try {
                    forward();
                } catch (IOException e) {
                    // ???
                    throw new JsonTokenizerException("Failed to forward character stream.", e, tailCharStream());
                }
            }
        }
        if(charQueue.isEmpty()) {
            return 0;
            // throw new UnexpectedEndOfStreamException("There is no character in the buffer.");
        }
        return charQueue.peek();
    }
    private char[] peekChars(int length) throws JsonTokenizerException
    {
        // assert length > 0
        if(charQueue.size() < length) {
            if(readerEOF == false) {
                try {
                    forward();
                } catch (IOException e) {
                    // ???
                    throw new JsonTokenizerException("Failed to forward character stream.", e, tailCharStream());
                }
            }
        }
        if(charQueue.size() < length) {
            return charQueue.peek(charQueue.size());
            // throw new UnexpectedEndOfStreamException("There is not enough characters in the buffer. length = " + length);
        }
        return charQueue.peek(length);
    }
    private CyclicCharArray peekCharsInQueue(int length) throws JsonTokenizerException
    {
        // assert length > 0
        if(charQueue.size() < length) {
            if(readerEOF == false) {
                try {
                    forward();
                } catch (IOException e) {
                    // ???
                    throw new JsonTokenizerException("Failed to forward character stream.", e, tailCharStream());
                }
            }
        }
        if(charQueue.size() < length) {
            return charQueue.peekBuffer(charQueue.size());
            // throw new UnexpectedEndOfStreamException("There is not enough characters in the buffer. length = " + length);
        }
        return charQueue.peekBuffer(length);
    }
    private CyclicCharArray peekCharsInQueue(int offset, int length) throws JsonTokenizerException
    {
        // assert length > 0
        if(charQueue.size() < offset + length) {
            if(readerEOF == false) {
                try {
                    forward();
                } catch (IOException e) {
                    // ???
                    throw new JsonTokenizerException("Failed to forward character stream.", e, tailCharStream());
                }
            }
        }
        if(charQueue.size() < offset + length) {
            return charQueue.peekBuffer(offset, charQueue.size() - offset);
            // throw new UnexpectedEndOfStreamException("There is not enough characters in the buffer. length = " + length);
        }
        return charQueue.peekBuffer(offset, length);
    }

    
    // Poll next char (and gobble up),
    // and return the next char (without removing it)
    private char skipAndPeekChar() throws JsonTokenizerException
    {
        int qSize = charQueue.size();
        if(qSize < 2) {
            if(readerEOF == false) {
                try {
                    forward();
                    qSize = charQueue.size();
                } catch (IOException e) {
                    // ???
                    throw new JsonTokenizerException("Failed to forward character stream.", e, tailCharStream());
                }
            }
        }
        if(qSize > 0) {
            if(tracingEnabled) {
                char ch = charQueue.poll();
                tailBuffer.push(ch);
            } else {
                charQueue.skip();
            }
            if(qSize > 1) {
                return charQueue.peek();
            }
        }
        return 0;
        // throw new UnexpectedEndOfStreamException("There is no character in the buffer.");
    }

    
    // Read some more bytes from the reader.
    private final char[] buff = new char[READER_BUFF_SIZE];
    private void forward() throws IOException, JsonTokenizerException
    {
        if(readerEOF == false) {
            if(reader.ready()) {  // To avoid blocking
                int cnt = 0;
                try {
                    // This throws OOB excpetion at the end....
                    // cnt = reader.read(buff, curReaderIndex, READER_BUFF_SIZE);
                    cnt = reader.read(buff);
                } catch(IndexOutOfBoundsException e) {
                    // ???
                    // Why does this happen? Does it happen for StringReader only???
                    //    Does read(,,) ever return -1 in the case of StringReader ???
                    if(log.isLoggable(Level.INFO)) log.log(Level.INFO, "Looks like we have reached the end of the reader.", e);
                }
                if(cnt == -1 || cnt == 0) {
                    readerEOF = true;
                } else {
                    boolean suc = charQueue.addAll(buff, cnt);
                    if(suc) {
                        curReaderIndex += cnt;
                    } else {
                        // ???
                        // Is this because the json file includes a loooooong string???
                        // temporarily change the LookAheadParsing flag and try again???
                        // --->
                        // Unfortunately this does not work because we are already in the middle of parsing string..
                        //    Unless we can rewind the stack in some way, this does not really help...
                        // if(isLookAheadParsing()) {
                        //     disableLookAheadParsing();
                        //     log.warning("Unexpected internal error occurred. Characters were not added to CharQueue: cnt = " + cnt + ". Trying again after calling disableLookAheadParsing().");
                        //     forward();   // We should be careful not to get into the infinite loop....
                        // } else {
                        //     throw new JsonTokenizerException("Unexpected internal error occurred. Characters were not added to CharQueue: cnt = " + cnt, tailCharStream());
                        // }
                        throw new UnexpectedEndOfStreamException("Unexpected internal error occurred. Characters were not added to CharQueue: cnt = " + cnt, tailCharStream());
                    }
                }
            } else {
                // ????
                readerEOF = true;
                // Why does this happen ????
                // if(log.isLoggable(Level.INFO)) log.log(Level.INFO, "Looks like we have not read all characters because the reader is blocked. We'll likely have a parser error down the line.");
                // throw new UnexpectedEndOfStreamException("Read is blocked. Bailing out.");
            }
        }
    }
    
}
