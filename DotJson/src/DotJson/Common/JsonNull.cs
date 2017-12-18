using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DotJson.Common
{
	// object to represent JSON "null" string.
	public sealed class JsonNull
	{
		public static readonly object NULL = new JsonNull();
		private JsonNull() {}

		public override string ToString()
		{
			return "null";
		}
		
	}
}
