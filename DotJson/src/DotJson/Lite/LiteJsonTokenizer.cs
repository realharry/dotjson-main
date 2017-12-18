using DotJson.Common;


namespace DotJson.Lite
{
	public interface LiteJsonTokenizer
	{
		/*
		 * Returns true if there is more tokens in the stream.
		 * Throws DotJsonMiniException
		 */
		bool HasMore();
		
		/*
		 * Return the next token.
		 * Throws DotJsonMiniException
		 */
		JsonToken Next();
		
		/*
		 * Return the next token, without removing it from the stream.
		 * Throws DotJsonMiniException
		 */
		JsonToken Peek();

	}
}
