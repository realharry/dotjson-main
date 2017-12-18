using DotJson.Common;
using DotJson.Core;
using DotJson.Lite;
using DotJson.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DotJson.Lite.Builder
{
	// Note that this class is primarily used to convert an object to the map/list tree structure.
	//    (The default depth is DEF_DRILL_DOWN_DEPTH.)
	// Then the tree structure is fed into DotJsonMiniBuilder (which has a default depth of MAX_DRILL_DOWN_DEPTH)
	//    to get the JSON string.
	public sealed class LiteJsonStructureBuilder : Lite.LiteJsonStructureBuilder
	{
		// Default value.
		// private const int DEF_DRILL_DOWN_DEPTH = 2;
		// Max value: equivalent to -1.
		private const int MAX_DRILL_DOWN_DEPTH = (int) Byte.MaxValue;  // Arbitrary.
		private const int DEF_DRILL_DOWN_DEPTH = 3;  // For objects. Arbitrary. It should be normally >= 1.
		
		// Note:
		// It's important not to keep any "state" as class variables for this class
		//   so that a single instance can be used for multiple/concurrent build operations.
		// (Often, the recursive implementation may involve multiple objects (each as a node in an object tree),
		//   which may use the same/single instance of this builder class.)
		// ...


		public LiteJsonStructureBuilder()
		{
		}


		public async Task<object> BuildJsonStructureAsync(object obj)
		{
			return await BuildJsonStructureAsync(obj, DEF_DRILL_DOWN_DEPTH);
		}

        // Not fully implemented yet.
        // TBD: Need to include reflection....
		public async Task<object> BuildJsonStructureAsync(object jsonObj, int depth)
		{
            if (depth < 0 || depth > MAX_DRILL_DOWN_DEPTH) {
                depth = MAX_DRILL_DOWN_DEPTH;
            }
            return await _BuildJsonStructure(jsonObj, depth);
		}

        // Experimenting with async/await....
        // Does this work????
        private Task<object> _BuildJsonStructure(object jsonObj, int depth)
        {
            var tcs = new TaskCompletionSource<object>();
            var jsonStruct = _BuildJsonStruct(jsonObj, depth);
            tcs.SetResult(jsonStruct);
            return tcs.Task;
        }

		// TBD:
		// The problem with this algo is we have no way to consistently represent null node.
		// For map value and list element, we can just use Java null.
		// But, in general, it may not be possible.....  ????    Is this true????
		// Seems to be working so far (based on the limited unit test cases...)
		private object _BuildJsonStruct(object obj, int depth)
		{
	//        if(depth < 0) {
	//            return null;
	//        }
			object jsonStruct = null;
			if(obj == null || obj is JsonNull) {    // ????
				// return null;
			} else {
				// System.Diagnostics.Debug.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>> depth = " + depth);
				// if(depth == 0) {
				if(depth <= 0) {
					if(obj is bool
                        || obj is char
                        || Number.IsNumber(obj)
                        ) {
						jsonStruct = obj;
					} else if(obj is String) {
						// Note that this string is not a "json string" (e.g., forward slash escaped, etc.)
						// e.g., if the string is "...\\/...", we will read it as "...\\/..." not as ".../...".
						jsonStruct = (String) obj;
					} else {
						// ????
						jsonStruct = obj.ToString();
					}
				} else {
                    // if (obj is IDictionary<String, Object>) {
					if(GenericUtil.IsDictionary(obj)) {
                        //IDictionary<String, Object> jsonIDictionary = new OrderedMap<String, Object>();
                        IDictionary<String, Object> jsonIDictionary = new Dictionary<String, Object>();
                        // ...
		
                        // ????
						// IDictionary<String,Object> map = null;
                        dynamic map = null;
						try {
							// map = (IDictionary<String,Object>) obj;
                            map = obj;
						} catch(Exception e) {
							System.Diagnostics.Debug.WriteLine("Invalid map type.", e);
							// What to do???
							// Use map.ToString???
						}
						if(map != null && map.Count > 0) {
							foreach(string f in map.Keys) {
								object val = map[f];
								object jsonVal = _BuildJsonStruct(val, depth - 1);
								if(jsonVal != null) {
									jsonIDictionary.Add(f, jsonVal);
								} else {
									// ???
									jsonIDictionary.Add(f, null);
								}
							}
						}
						
						jsonStruct = jsonIDictionary;
                    // } else if (obj is IList<Object>) {
                    } else if (GenericUtil.IsList(obj)) {
                        IList<Object> jsonList = new List<Object>();
		
                        // ????
						// IList<Object> list = null;
                        dynamic list = null;
						try {
							// list = (IList<Object>) obj;
                            list = obj;
						} catch(Exception e) {
							System.Diagnostics.Debug.WriteLine("Invalid list type.", e);
							// What to do???
							// Use list.ToString???
						}
						if(list != null && list.Count > 0) {
							foreach(object v in list) {
								object jsonVal = _BuildJsonStruct(v, depth - 1);
								if(jsonVal != null) {
									jsonList.Add(jsonVal);
								} else {
									// ???
									jsonList.Add(null);
								}
							}
						}
						
						jsonStruct = jsonList;
                    } else if (obj.GetType().IsArray) {          // ???
						List<Object> jsonList = new List<Object>();

                        // ????
						// object array = obj;
                        dynamic array = obj;

						if(array!= null && array.Length > 0) {
							int arrLen = array.Length;
							// System.Diagnostics.Debug.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>> arrLen = " + arrLen);
							for(int i=0; i<arrLen; i++) {
                                object o = array[i];
								// System.Diagnostics.Debug.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>> o = " + o + "; " + o.Type);
								object jsonVal = _BuildJsonStruct(o, depth - 1);
								// System.Diagnostics.Debug.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>> jsonVal = " + jsonVal + "; " + o.Type);
								if(jsonVal != null) {
									jsonList.Add(jsonVal);
								} else {
									// ???
									jsonList.Add(null);
								}
							}
						}
		
						jsonStruct = jsonList;
                    } else if (obj is Collection<Object>) {     // ??????    Not implemented yet.
						List<Object> jsonList = new List<Object>();
						// jsonList.AddAll((Collection<Object>) ((Collection<?>) obj));

                        IEnumerator<Object> it = ((Collection<Object>) obj).GetEnumerator();
						while(it.MoveNext()) {
                            object o = it.Current;
							object jsonVal = _BuildJsonStruct(o, depth - 1);
							if(jsonVal != null) {
								jsonList.Add(jsonVal);
							} else {
								// ???
								jsonList.Add(null);
							}
						}
						
						jsonStruct = jsonList;
					} else {
						// ???
						// This can potentially cause infinite recursion.
						// because maybe JsonCompatible object : toJsonStructure() using JsonBuilder.buidJsonStructure()
						// which calls the object's toJsonStructure(), which calls JsonBuilder.buidJsonStructure(), etc.
						// ....
						// if(obj is JsonCompatible) {
						//     jsonStruct = ((JsonCompatible) obj).toJsonStructure(depth);
						// } else {
							// primitive types... ???
					        if(obj is bool
                                || obj is char
                                || Number.IsNumber(obj)
                                || obj is string
                                ) {
						        jsonStruct = obj;
							} else {

								// Use inspection....
								// TBD:
								// BuilderPolicy ???
								// ...
								
								IDictionary<String, Object> mapEquivalent = null;
                                try {
                                    // mapEquivalent = IntrospectionUtil.Introspect(obj, depth);   // depth? or depth - 1 ?
                                    // Because we are just converting a bean to a map,
                                    // the depth param is not used. (or, depth == 1).
                                    mapEquivalent = IntrospectionUtil.Introspect(obj);
                                } catch (Exception ex) {
                                    // Ignore.
                                    System.Diagnostics.Debug.WriteLine("Faild to Introspect a bean.", ex);
                                }
								if(mapEquivalent != null) {
									jsonStruct = _BuildJsonStruct(mapEquivalent, depth);   // Note: We do not change the depth.
								} else {
									
									// ????
									// jsonStruct = null; ???
									jsonStruct = obj.ToString();
									// ...
								}
								// System.Diagnostics.Debug.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>> jsonStruct = " + jsonStruct);
									

							}
						// }
					}
				}
			}

			return jsonStruct;
		}
	   
		
	}
}
