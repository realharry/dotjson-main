using System.Threading.Tasks;

namespace DotJson.Lite
{
	public interface LiteJsonStructureBuilder
	{
		// Converts the object to a nested structure of IDictionary/List, object/JsonCompatible, and JsonSerializable + primitive types.
		// Uses the default depth of the object (not necessarily 0).
		// Note that the return value (structure) is either IDictionary<String,Object> or List<Object>.
		Task<object> BuildJsonStructureAsync(object jsonObj);           

        // Note:
        // Currently, not implmented.

		// Traverses down to the depth level (in terms of Object, IDictionary, List).
		// 1 means this object only. No map, list, other object traversal (even it it's its own fields).
		// 0 means no Introspection.
		// depth is always additive during traversal.
		// that is, if depth is 3, then object -> object -> object -> primitive only. 
		Task<object> BuildJsonStructureAsync(object jsonObj, int depth);

        // IDictionary<String,Object> ToJsonObject();   // Reserved for later. (e.g., Jsonobject toJsonObject())

	}
}
