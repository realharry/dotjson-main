using DotJson.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotJson.Parser
{
    /// <summary>
    /// Json Tokenizer base interface.
    /// </summary>
    public interface JsonTokenizer
    {
        /// <summary>
        /// Returns true if there is more tokens in the stream. </summary>
        /// <returns> true if there is more tokens. </returns>
        /// <exception cref="JsonTokenizerException"> </exception>
        bool HasMore();

        /// <summary>
        /// Returns the next token. </summary>
        /// <returns> the next token in the stream. </returns>
        /// <exception cref="JsonTokenizerException"> </exception>
        JsonToken Next();

        /// <summary>
        /// Peeks the next token in the stream. </summary>
        /// <returns> the next token, without removing it from the stream. </returns>
        /// <exception cref="JsonTokenizerException"> </exception>
        JsonToken Peek();

    }

}