using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HoloJson.Parser.Impl
{
    // Recursive descent parser implementation using Java types.
    public abstract class AbstractJsonParser : BareJsonParser
    {
        private const int TAIl_TRACE_LENGTH = 200; // temporary
        private const int HEAD_TRACE_LENGTH = 35; // temporary
                                                  // ...

        // If true, use "look ahead" algorithms.
        private bool lookAheadParsing;

        // Whether "tracing" is enabled or not.
        // Tracing, at this point, means that we simply keep the char tail buffer
        //    so that when an error occurs we can see the "exception context".
        private bool tracingEnabled;

        public AbstractJsonParser()
        {
            // "Look ahead" enabled by default.
            lookAheadParsing = true;
            // "Tracing" disabled by default.
            tracingEnabled = false;
        }


        protected internal virtual bool IsLookAheadParsing
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
            EnableLookAheadParsing(null);
        }
        public virtual void EnableLookAheadParsing(JsonTokenizer tokenizer)
        {
            this.lookAheadParsing = true;
            if (tokenizer != null && tokenizer is LookAheadJsonTokenizer) {
                ((LookAheadJsonTokenizer)tokenizer).EnableLookAheadParsing();
            }
        }
        public virtual void DisableLookAheadParsing()
        {
            DisableLookAheadParsing(null);
        }
        public virtual void DisableLookAheadParsing(JsonTokenizer tokenizer)
        {
            this.lookAheadParsing = false;
            if (tokenizer != null && tokenizer is LookAheadJsonTokenizer) {
                ((LookAheadJsonTokenizer)tokenizer).DisableLookAheadParsing();
            }
        }

        protected internal virtual JsonTokenizer SetLookAheadParsing(JsonTokenizer tokenizer)
        {
            if (tokenizer != null && tokenizer is LookAheadJsonTokenizer) {
                if (lookAheadParsing) {
                    ((LookAheadJsonTokenizer)tokenizer).EnableLookAheadParsing();
                } else {
                    ((LookAheadJsonTokenizer)tokenizer).DisableLookAheadParsing();
                }
            }
            return tokenizer;
        }


        public virtual bool TracingEnabled
        {
            get
            {
                return tracingEnabled;
            }
        }

        // Note that these methods (enableTracing() and disableTracing() without arguments)
        //    do not change the tokenzier.tracingEanbled
        //    if it is called after tokenizer has been created. 
        // TBD: This is a bit of a problem. maybe? maybe not?
        public virtual void EnableTracing()
        {
            EnableTracing(null);
        }
        protected internal virtual void EnableTracing(JsonTokenizer tokenizer)
        {
            tracingEnabled = true;
            if (tokenizer != null && tokenizer is TraceableJsonTokenizer) {
                ((TraceableJsonTokenizer)tokenizer).EnableTracing();
            }
        }

        public virtual void DisableTracing()
        {
            DisableTracing(null);
        }
        protected internal virtual void DisableTracing(JsonTokenizer tokenizer)
        {
            tracingEnabled = false;
            if (tokenizer != null && tokenizer is TraceableJsonTokenizer) {
                ((TraceableJsonTokenizer)tokenizer).DisableTracing();
            }
        }


        protected internal virtual JsonTokenizer SetTokenizerTracing(JsonTokenizer tokenizer)
        {
            if (tokenizer != null && tokenizer is TraceableJsonTokenizer) {
                if (tracingEnabled) {
                    ((TraceableJsonTokenizer)tokenizer).EnableTracing();
                } else {
                    ((TraceableJsonTokenizer)tokenizer).DisableTracing();
                }
            }
            return tokenizer;
        }


        // For debugging/tracing...
        protected internal virtual char[] GetTailCharStream(JsonTokenizer tokenizer)
        {
            if (tokenizer is TraceableJsonTokenizer) {
                return ((TraceableJsonTokenizer)tokenizer).GetTailCharStream(TAIl_TRACE_LENGTH);
            } else {
                return null;
            }
        }
        protected internal virtual char[] PeekCharStream(JsonTokenizer tokenizer)
        {
            if (tokenizer is TraceableJsonTokenizer) {
                return ((TraceableJsonTokenizer)tokenizer).PeekCharStream(HEAD_TRACE_LENGTH);
            } else {
                return null;
            }
        }


        public abstract Task<object> ParseAsync(string jsonStr);

        public abstract Task<object> ParseAsync(TextReader reader);

    }

}