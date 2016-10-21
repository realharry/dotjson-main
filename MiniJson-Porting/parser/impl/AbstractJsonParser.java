package org.minijson.parser.impl;

import java.util.logging.Logger;

import org.minijson.common.TailRingBuffer;
import org.minijson.parser.BareJsonParser;
import org.minijson.parser.JsonTokenizer;
import org.minijson.parser.LookAheadJsonTokenizer;
import org.minijson.parser.TraceableJsonTokenizer;
import org.minijson.parser.core.JsonToken;



// Recursive descent parser implementation using Java types.
public abstract class AbstractJsonParser implements BareJsonParser
{
    private static final Logger log = Logger.getLogger(AbstractJsonParser.class.getName());
    // ...
    private static final int TAIl_TRACE_LENGTH = 200;     // temporary
    private static final int HEAD_TRACE_LENGTH = 35;     // temporary
    // ...

    // If true, use "look ahead" algorithms.
    private boolean lookAheadParsing;

    // Whether "tracing" is enabled or not.
    // Tracing, at this point, means that we simply keep the char tail buffer
    //    so that when an error occurs we can see the "exception context".
    private boolean tracingEnabled;

    public AbstractJsonParser()
    {
        // "Look ahead" enabled by default.
        lookAheadParsing = true;
        // "Tracing" disabled by default.
        tracingEnabled = false;
    }

    
    public boolean isLookAheadParsing()
    {
        return lookAheadParsing;
    }
//    public void setLookAheadParsing(boolean lookAheadParsing)
//    {
//        this.lookAheadParsing = lookAheadParsing;
//    }
    public void enableLookAheadParsing()
    {
        enableLookAheadParsing(null);
    }
    public void enableLookAheadParsing(JsonTokenizer tokenizer)
    {
        this.lookAheadParsing = true;
        if(tokenizer != null && tokenizer instanceof LookAheadJsonTokenizer) {
            ((LookAheadJsonTokenizer) tokenizer).enableLookAheadParsing();
        }
    }
    public void disableLookAheadParsing()
    {
        disableLookAheadParsing(null);
    }
    public void disableLookAheadParsing(JsonTokenizer tokenizer)
    {
        this.lookAheadParsing = false;
        if(tokenizer != null && tokenizer instanceof LookAheadJsonTokenizer) {
            ((LookAheadJsonTokenizer) tokenizer).disableLookAheadParsing();
        }
    }

    protected void setLookAheadParsing(JsonTokenizer tokenizer)
    {
        if(tokenizer != null && tokenizer instanceof LookAheadJsonTokenizer) {
            if(lookAheadParsing) {
                ((LookAheadJsonTokenizer) tokenizer).enableLookAheadParsing();
            } else {
                ((LookAheadJsonTokenizer) tokenizer).disableLookAheadParsing();
            }
        }
    }


    public boolean isTracingEnabled()
    {
        return tracingEnabled;
    }

    // Note that these methods (enableTracing() and disableTracing() without arguments)
    //    do not change the tokenzier.tracingEanbled
    //    if it is called after tokenizer has been created. 
    // TBD: This is a bit of a problem. maybe? maybe not?
    public void enableTracing()
    {
        enableTracing(null);
    }
    protected void enableTracing(JsonTokenizer tokenizer)
    {
        tracingEnabled = true;
        if(tokenizer != null && tokenizer instanceof TraceableJsonTokenizer) {
            ((TraceableJsonTokenizer) tokenizer).enableTracing();
        }
    }

    public void disableTracing()
    {
        disableTracing(null);
    }
    protected void disableTracing(JsonTokenizer tokenizer)
    {
        tracingEnabled = false;
        if(tokenizer != null && tokenizer instanceof TraceableJsonTokenizer) {
            ((TraceableJsonTokenizer) tokenizer).disableTracing();
        }
    }


    protected void setTokenizerTracing(JsonTokenizer tokenizer)
    {
        if(tokenizer != null && tokenizer instanceof TraceableJsonTokenizer) {
            if(tracingEnabled) {
                ((TraceableJsonTokenizer) tokenizer).enableTracing();
            } else {
                ((TraceableJsonTokenizer) tokenizer).disableTracing();
            }
        }
    }

    
    // For debugging/tracing...
    protected char[] tailCharStream(JsonTokenizer tokenizer)
    {
        if(tokenizer instanceof TraceableJsonTokenizer) {
            return ((TraceableJsonTokenizer) tokenizer).tailCharStream(TAIl_TRACE_LENGTH);
        } else {
            return null;
        }
    }
    protected char[] peekCharStream(JsonTokenizer tokenizer)
    {
        if(tokenizer instanceof TraceableJsonTokenizer) {
            return ((TraceableJsonTokenizer) tokenizer).peekCharStream(HEAD_TRACE_LENGTH);
        } else {
            return null;
        }
    }


}
