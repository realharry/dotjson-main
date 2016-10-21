using HoloJson.Common;


namespace HoloJson.Lite
{
	public interface LiteJsonTokenizer
	{
		/*
		 * Returns true if there is more tokens in the stream.
		 * Throws HoloJsonMiniException
		 */
		bool HasMore();
		
		/*
		 * Return the next token.
		 * Throws HoloJsonMiniException
		 */
		JsonToken Next();
		
		/*
		 * Return the next token, without removing it from the stream.
		 * Throws HoloJsonMiniException
		 */
		JsonToken Peek();

	}
}
