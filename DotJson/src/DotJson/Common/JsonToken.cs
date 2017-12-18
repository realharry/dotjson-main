using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DotJson.Common
{
	// JsonToken is immutable.
	public struct JsonToken
	{
        // Just use TokenPool.TOKEN_NULL?
        public static readonly JsonToken INVALID = new JsonToken(TokenType.INVALID, null);  // ???
        // public static readonly JsonToken NULL = new JsonToken(TokenType.NULL, null);  // Json NULL?

        // private readonly int type;
        private readonly TokenType type;
        private readonly object value;

        // public JsonToken(int type, object value)
        public JsonToken(TokenType type, object value)
        {
            // tbd: validate type ???
			this.type = type;
			this.value = value;
		}

		// public int GetTokenType()
        public TokenType Type
		{
            get
            {
                return type;
            }
		}
		public object Value
		{
            get
            {
                return value;
            }
		}

        // Note IsValid != !IsInvalid.
        public bool IsValid
        {
            get
            {
                // return TokenTypes.IsValid(type);
                // return !IsInvalid;    // What about EOF ????
                return (type != TokenType.INVALID && type != TokenType.EOF);
            }
        }
        public bool IsInvalid
        {
            get
            {
                // return ! TokenTypes.IsValid(type);
                return (type == TokenType.INVALID);  // What about EOF ????
                // return (type == TokenType.INVALID || type == TokenType.EOF);
            }
        }

        // Note: IsValid() != (! IsInvalid()).
        public static bool IsTokenValid(JsonToken token)
        {
            //if (token.Type == TokenType.INVALID) {
            //    return false;
            //} else {
            //    return Enum.IsDefined(typeof(TokenType), token.Type);
            //}
            return TokenTypes.IsValid(token.Type);
        }
        public static bool IsTokenInvalid(JsonToken token)
        {
            return (token.Type == TokenType.INVALID);
        }

        // TBD:
		// Note that hash collision can generate a parse error.
		// (Cf. TokenTool)
        private static readonly int prime = 7211;
        //public static int BuildHashCode(int type, object value)
        public static int BuildHashCode(TokenType type, object value)
        {
			int result = 1;
			result = prime * result + (int) type;
			result = prime * result + ((value == null) ? 0 : value.GetHashCode());
			return result;
		}

		public override int GetHashCode()
		{
            return BuildHashCode(type, value);
		}

		public override bool Equals(object obj)
		{
            //if (this == obj)
            //    return true;
            if (obj == null)
                return false;
			if (GetType() != obj.GetType())
				return false;
			JsonToken other = (JsonToken) obj;
			if (type != other.type)
				return false;
            if (value == null) {
                if (other.value != null)
                    return false;
            } else if (!value.Equals(other.value)) {
                return false;
            }
            return true;
		}

		public override string ToString()
		{
			return "JsonToken [type=" + Enum.GetName(typeof(TokenType), type) + ", value=" + value + "]";
		}


        public static bool operator ==(JsonToken lhs, JsonToken rhs)
        {
            // return lhs.Equals(rhs);
            if (lhs.Type != rhs.Type)
                return false;
            if (lhs.Value == null) {
                if (rhs.value != null)
                    return false;
            } else if (!lhs.Value.Equals(rhs.Value)) {
                return false;
            }
            return true;
        }
        public static bool operator !=(JsonToken lhs, JsonToken rhs)
        {
            // return !lhs.Equals(rhs);
            return ! (lhs == rhs);
        }


    }
}
