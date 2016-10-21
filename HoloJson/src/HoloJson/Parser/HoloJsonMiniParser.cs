using HoloJson.Common;
using HoloJson.Core;
using HoloJson.Lite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;


namespace HoloJson.Parser
{
	// Recursive descent parser implementation using Java types.
	public sealed class HoloJsonMiniParser : LiteJsonParser
	{
		// temporary
		private const int HEAD_TRACE_LENGTH = 35;
		// ...


		public HoloJsonMiniParser()
		{
		}

		private char[] PeekCharStream(LiteJsonTokenizer tokenizer)
		{
			if(tokenizer is HoloJsonMiniTokenizer) {
				return ((HoloJsonMiniTokenizer) tokenizer).PeekCharStream(HEAD_TRACE_LENGTH);
			} else {
				return null;
			}
		}

        // temporary
        // Due to the limitations of WinRT API, the Parse method taking TextReader is not very useful on Windows Runtime.
        //    (e.g., StreamReader(filePath) contructor (on .Net 4.5) is not available on WinRT, etc...)
        // Instead, use this method.
        // If you have a json file, create a StorageFile first,
        //    and then call ParseAsync(jsonFile).
        //public async Task<object> ParseAsync(IStorageFile jsonFile)
        //{
        //    // Does this work???
        //    // TextReader reader = (TextReader) jsonFile.OpenSequentialReadAsync();
        //    // return await ParseAsync(reader);

        //    // For now, read the whole content into memory first....
        //    var jsonStr = await FileIO.ReadTextAsync(jsonFile);
        //    return await ParseAsync(jsonStr);
        //}

		// This requires reading the whole JSON string into the memory first.
		public async Task<object> ParseAsync(string jsonStr)
		{
			var sr = new StringReader(jsonStr);
			object jsonObj = null;
			try {
				jsonObj = await ParseAsync(sr);
			} catch (IOException e) {
				// throw new JsonException("IO error during JSON parsing. " + tokenTailBuffer.toTraceString(), e);
				throw new HoloJsonMiniException("IO error during JSON parsing.", e);
			}
			return jsonObj;
		}
		// This can (in theory) parse while reading the stream...
        public async Task<object> ParseAsync(TextReader reader)
		{
            object topNode = await _ParseJson(reader);
			// TBD:
			// Convert it to map/list... ???
			// ...
			return topNode;
		}

        // Experimenting with async/await....
        // Does this work????
        private Task<object> _ParseJson(TextReader reader)
        {
            var tcs = new TaskCompletionSource<object>();
            var jsonObj = _Parse(reader);
            tcs.SetResult(jsonObj);
            return tcs.Task;
        }

		private object _Parse(TextReader reader)
		{
			if(reader == null) {
				return null;
			}

			// TBD:
			// Does this make it thread safe???
			// ...

			LiteJsonTokenizer jsonTokenizer = null;
			jsonTokenizer = new HoloJsonMiniTokenizer(reader);

			return _Parse(jsonTokenizer);
		}
		private object _Parse(LiteJsonTokenizer tokenizer)
		{
			if(tokenizer == null) {
				return null;
			}
			
			object topNode = null;
			var type = PeekAndGetType(tokenizer);
			if(type == TokenType.EOF || type == TokenType.LCURLY || type == TokenType.LSQUARE) {
				if(type == TokenType.EOF) {
					topNode = ProduceJsonNull(tokenizer);
				} else if(type == TokenType.LCURLY) {
					topNode = ProduceJsonObject(tokenizer);
				} else if(type == TokenType.LSQUARE) {
					topNode = ProduceJsonArray(tokenizer);            
				}
			} else {
				// ???
				throw new HoloJsonMiniException("Json string should be object or Array. Input tokenType = " + TokenTypes.GetDisplayName(type));
			}

			System.Diagnostics.Debug.WriteLine("topnNode = " + topNode);
			return topNode;
		}
		
		
		private IDictionary<String,Object> ProduceJsonObject(LiteJsonTokenizer tokenizer)
		{
			var lcurl = NextAndGetType(tokenizer);   // pop the leading {.
			if(lcurl != TokenType.LCURLY) {
				// this cannot happen.
				throw new HoloJsonMiniException("JSON object should start with {. ");
			}

			IDictionary<String,Object> map = new Dictionary<String,Object>();
			var type = PeekAndGetType(tokenizer);
			if(type == TokenType.RCURLY) {
				// empty object
				JsonToken t = tokenizer.Next();   // discard the trailing }.
			} else {
				IDictionary<String,Object> members = ProduceJsonObjectMembers(tokenizer);
				TokenType rcurl;
				rcurl = NextAndGetType(tokenizer);  // discard the trailing }.
				if(rcurl == TokenType.RCURLY) {
					// Done
                    foreach (var k in members.Keys) {
                        map.Add(k, members[k]);
                    }
				} else {
					// ???
					throw new HoloJsonMiniException("JSON object should end with }. ");
				}
			}
            IDictionary<String, Object> jObject = map;

            System.Diagnostics.Debug.WriteLine("jObject = " + jObject);
			return jObject;
		}

		
	 
		private IDictionary<String,Object> ProduceJsonObjectMembers(LiteJsonTokenizer tokenizer)
		{
			IDictionary<String,Object> members = new Dictionary<String,Object>();
			
			var type = PeekAndGetType(tokenizer);
			while(type != TokenType.RCURLY) {
				KeyValuePair<String,Object> member = ProduceJsonObjectMember(tokenizer);
                members.Add(member.Key, member.Value);
				type = PeekAndGetType(tokenizer);
				
				// "consume" the comma.
				// Note: We are very lenient when it comes to extra/repeated commas...
				while(type == TokenType.COMMA) {
					JsonToken t = tokenizer.Next();
					type = PeekAndGetType(tokenizer);
				}
			}

			System.Diagnostics.Debug.WriteLine("members = " + members);
			return members;
		}
		private KeyValuePair<String,Object> ProduceJsonObjectMember(LiteJsonTokenizer tokenizer)
		{
			JsonToken keyToken = NextAndGetToken(tokenizer);
			var keyType = keyToken.Type;
			if(keyType != TokenType.STRING) {
				throw new HoloJsonMiniException("JSON object member should start with a string key. keyType = " + keyType + "; ");
			}
			string key = (String) keyToken.Value;
			
			JsonToken colonToken = NextAndGetToken(tokenizer);   // "consume" :.
			var colonType = colonToken.Type;
			if(colonType != TokenType.COLON) {
				throw new HoloJsonMiniException("JSON object member should include a colon (:). ");
			}

			object value = null;
			var type = PeekAndGetType(tokenizer);
			switch(type) {
			case TokenType.NULL:
				value = ProduceJsonNull(tokenizer);
				break;
			case TokenType.BOOLEAN:
				value = ProduceJsonBoolean(tokenizer);
				break;
			case TokenType.NUMBER:
				value = ProduceJsonNumber(tokenizer);
				break;
			case TokenType.STRING:
				value = ProduceJsonString(tokenizer);
				break;
			case TokenType.LCURLY:
				value = ProduceJsonObject(tokenizer);
				break;
			case TokenType.LSQUARE:
				value = ProduceJsonArray(tokenizer);
				break;
			default:
				// ???
				throw new HoloJsonMiniException("Json array element not recognized: token = " + tokenizer.Peek() + "; ");
			}
			
			// TBD: Use type factory ???
			KeyValuePair<String,Object> member = new KeyValuePair<String,Object>(key, value);
	 
			System.Diagnostics.Debug.WriteLine("member = " + member);
			return member;
		}


		
		private IList<Object> ProduceJsonArray(LiteJsonTokenizer tokenizer)
		{
			TokenType lsq;
			lsq = NextAndGetType(tokenizer);             
			if(lsq != TokenType.LSQUARE) {
				// this cannot happen.
				throw new HoloJsonMiniException("JSON array should start with [. ");
			}

			IList<Object> list = new List<Object>();
			var type = PeekAndGetType(tokenizer);
			if(type == TokenType.RSQUARE) {
				// empty array
				JsonToken t = tokenizer.Next();   // discard the trailing ].
			} else {
				IList<Object> elements = ProduceJsonArrayElements(tokenizer);

				var rsq = NextAndGetType(tokenizer);  // discard the trailing ].
				if(rsq == TokenType.RSQUARE) {
					// Done
                    foreach (var el in elements) {
                        list.Add(el);
                    }
				} else {
					// ???
					throw new HoloJsonMiniException("JSON array should end with ]. ");
				}
			}
			IList<Object> jArray = list;

			System.Diagnostics.Debug.WriteLine("jArray = " + jArray);
			return jArray;
		}

		private IList<Object> ProduceJsonArrayElements(LiteJsonTokenizer tokenizer)
		{
			IList<Object> elements = new List<Object>();

			var type = PeekAndGetType(tokenizer);
			while(type != TokenType.RSQUARE) {
				object element = ProduceJsonArrayElement(tokenizer);
				if(element != null) {
					elements.Add(element);
				}
				type = PeekAndGetType(tokenizer);

				// "consume" the comma.
				// Note: We are very lenient when it comes to extra/repeated commas...
				while(type == TokenType.COMMA) {
					JsonToken t = tokenizer.Next();
					type = PeekAndGetType(tokenizer);
				}
			}

			System.Diagnostics.Debug.WriteLine("elements = " + elements);
			return elements;
		}
		private object ProduceJsonArrayElement(LiteJsonTokenizer tokenizer)
		{
			object element = null;
			var type = PeekAndGetType(tokenizer);
			switch(type) {
			case TokenType.NULL:
				element = ProduceJsonNull(tokenizer);
				break;
			case TokenType.BOOLEAN:
				element = ProduceJsonBoolean(tokenizer);
				break;
			case TokenType.NUMBER:
				element = ProduceJsonNumber(tokenizer);
				break;
			case TokenType.STRING:
				element = ProduceJsonString(tokenizer);
				break;
			case TokenType.LCURLY:
				element = ProduceJsonObject(tokenizer);
				break;
			case TokenType.LSQUARE:
				element = ProduceJsonArray(tokenizer);
				break;
			default:
				// ???
				throw new HoloJsonMiniException("Json array element not recognized: token = " + tokenizer.Peek() + "; ");
			}

			System.Diagnostics.Debug.WriteLine("element = " + element);
			return element;
		}

		private JsonToken PeekAndGetToken(LiteJsonTokenizer tokenizer)
		{
			JsonToken s = tokenizer.Peek();
			if(JsonToken.IsInvalid(s)) {
				throw new HoloJsonMiniException("Failed to get the next json token. ");
			}
			return s;
		}
        //private int PeekAndGetType(LiteJsonTokenizer tokenizer)
		private TokenType PeekAndGetType(LiteJsonTokenizer tokenizer)
		{
			JsonToken s = tokenizer.Peek();
            if (JsonToken.IsInvalid(s)) {
				throw new HoloJsonMiniException("Failed to get the next json token. ");
			}
            var type = s.Type;
			return type;
		}
		private JsonToken NextAndGetToken(LiteJsonTokenizer tokenizer)
		{
			JsonToken s = tokenizer.Next();
            if (JsonToken.IsInvalid(s)) {
				throw new HoloJsonMiniException("Failed to get the next json token. ");
			}
			return s;
		}
        //private int NextAndGetType(LiteJsonTokenizer tokenizer)
        private TokenType NextAndGetType(LiteJsonTokenizer tokenizer)
        {
			JsonToken s = tokenizer.Next();
			if(JsonToken.IsInvalid(s)) {
				throw new HoloJsonMiniException("Failed to get the next json token. ");
			}
			var type = s.Type;
			return type;
		}

		private string ProduceJsonString(LiteJsonTokenizer tokenizer)
		{
			string jString = null;
			try {
				JsonToken t = tokenizer.Next();
				// System.Diagnostics.Debug.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>> t = " + t);
                jString = (String) t.Value;
                // System.Diagnostics.Debug.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>> jString = " + jString);
			} catch(Exception e) {
				throw new HoloJsonMiniException("Failed to create a string node. ");
			}
			return jString;
		}
        // ????
		private object ProduceJsonNumber(LiteJsonTokenizer tokenizer)
		{
			object jNumber = null;
			try {
				JsonToken t = tokenizer.Next();
				jNumber = t.Value;
                // ???
                if (jNumber is Number) {
                    jNumber = ((Number) jNumber).Value;
                }
                // ???
			} catch(Exception e) {
				throw new HoloJsonMiniException("Failed to create a Number node. ");
			}
			return jNumber;
		}
		private bool ProduceJsonBoolean(LiteJsonTokenizer tokenizer)
		{
			bool jBoolean = false;   // ?????
			try {
				JsonToken t = tokenizer.Next();
				jBoolean = (bool) t.Value;
			} catch(Exception e) {
				throw new HoloJsonMiniException("Failed to create a Boolean node. ");
			}
			return jBoolean;
		}

		private object ProduceJsonNull(LiteJsonTokenizer tokenizer)
		{
			object jNull = null;
			try {
				JsonToken t = tokenizer.Next();   // Consume the "null" literal.
				jNull = JsonNull.NULL;
			} catch(Exception e) {
				throw new HoloJsonMiniException("Failed to create a Null node. ");
			}
			return jNull;
		}


	}
}
