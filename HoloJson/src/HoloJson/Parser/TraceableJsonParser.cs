using HoloJson.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HoloJson.Parser
{
    /// <summary>
    /// JsonParser with tracing/debugging information.
    /// </summary>
    public interface TraceableJsonParser : JsonParser
    {
        /// <summary>
        /// Enable "tracing".
        /// Tracing, at this point, means that we simply keep the token/nodes in a tail buffer
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


        /// <param name="length"> Max length of the tokens to be returned from the tail buffer. </param>
        /// <returns> the tail part of the tail token buffer as a string. </returns>
        JsonToken GetTailTokensAsString(int length);
        JsonToken[] GetTailTokenStream(int length);

        /// <param name="length"> Max length of the nodes to be returned from the tail buffer. </param>
        /// <returns> the tail part of the tail node buffer as a string. </returns>
        object GetTailNodesAsString(int length);
        object[] GetTailNodeStream(int length);

    }

}