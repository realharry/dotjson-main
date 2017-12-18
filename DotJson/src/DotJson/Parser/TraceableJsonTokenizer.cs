using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotJson.Parser
{
    /// <summary>
    /// JsonTokenizer with tracing/debugging information.
    /// </summary>
    public interface TraceableJsonTokenizer : JsonTokenizer
    {
        /// <summary>
        /// Enable "tracing".
        /// Tracing, at this point, means that we simply keep the char tail buffer
        ///     so that when an error occurs we can see the "exception context".
        /// </summary>
        void EnableTracing();

        /// <summary>
        /// Disable "tracing".
        /// </summary>
        void DisableTracing();

        /// <summary>
        /// Returns true if "tracing" is enabled.
        /// </summary>
        /// <returns> whether "tracing" is enabled or not.  </returns>
        bool TracingEnabled { get; }


        // ???
        // CharBuffer getTailBuffer();

        /// <param name="length"> Max length of the String/char[] to be returned from the tail buffer. </param>
        /// <returns> the tail part of the tail character buffer as a string. </returns>
        string GetTailCharsAsString(int length);
        char[] GetTailCharStream(int length);

        /// <param name="length"> Max length of the String/char[] to be peeked. </param>
        /// <returns> the character array in the stream as a string. </returns>
        string PeekCharsAsString(int length);
        char[] PeekCharStream(int length);

    }

}