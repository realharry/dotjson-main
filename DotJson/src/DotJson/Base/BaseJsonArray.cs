using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotJson.Base
{
 	public sealed class BaseJsonArray : BaseJsonWrapper, JsonArray
	{

		// JSON builder
		public BaseJsonArray(IList<object> list) : base(list)
		{
		}

		// JSON Parser
		public BaseJsonArray(string jsonStr) : base(jsonStr)
		{
		}


	//    // @Override
	//    public boolean isJsonStructureArray()
	//    {
	//        return true;
	//    }

	}

}