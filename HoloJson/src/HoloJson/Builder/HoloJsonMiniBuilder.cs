using HoloJson.Common;
using HoloJson.Core;
using HoloJson.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;


namespace HoloJson.Builder
{
	public sealed class HoloJsonMiniBuilder : LiteJsonBuilder
	{
		// Default value.
		// private const int DEF_DRILL_DOWN_DEPTH = 2;
		// Max value: equivalent to -1.
		private const int MAX_DRILL_DOWN_DEPTH = (int) byte.MaxValue;  // Arbitrary.

		// temporary
		// private const int maxDepth = MAX_DRILL_DOWN_DEPTH;
		private const string WS = " ";  // either to use space or not after "," and ":".
		// temporary

		
		// Note:
		// It's important not to keep any "state" as class variables for this class
		//   so that a single instance can be used for multiple/concurrent build operations.
		// (Often, the recursive implementation may involve multiple objects (each as a node in an object tree),
		//   which may use the same/single instance of this builder class.)
		// ...


		public HoloJsonMiniBuilder()
		{
		}

        //public async Task BuildAsync(IStorageFile jsonFile, object obj)
        //{
        //    var jsonStr = await BuildAsync(obj);
        //    if (jsonStr != null) {   // What about empty string?
        //        await FileIO.WriteTextAsync(jsonFile, jsonStr);
        //    } else {
        //        // What to do???
        //    }
        //}

		public async Task<string> BuildAsync(object jsonObj)
		{
			// Which is better?
			
			// [1] Using StringBuilder.
			// StringBuilder sb = new StringBuilder();
			// return _Build(sb, jsonObj);
		
			// Or, [2] Using StringWriter.
			string jsonStr = null;
			var writer = new StringWriter();
			try {
				// _Build(writer, jsonObj, depth, useBeanIntrospection);
				// writer.flush();   // ???
				await BuildAsync(writer, jsonObj);
				jsonStr = writer.ToString();
				// System.Diagnostics.Debug.WriteLine(">>>>>>>>>>>>>>>>>>>> jsonStr = " + jsonStr);
				// string jsonStr2 = writer.getBuffer().ToString();
				// System.Diagnostics.Debug.WriteLine(">>>>>>>>>>>>>>>>>>>> jsonStr2 = " + jsonStr2);

				System.Diagnostics.Debug.WriteLine("jsonStr = " + jsonStr);
			} catch (IOException ex) {
				System.Diagnostics.Debug.WriteLine("Failed to write jsonObj as JSON.", ex);
			}
			return jsonStr;
		}

		public async Task BuildAsync(TextWriter writer, object obj)
		{
			if( (obj == null) || (obj is bool) || (obj is char) || (Number.IsNumber(obj)) || (obj is String)) {
				throw new HoloJsonMiniException("Top level element should be an object or an array/list.");
			}
			object jsonObj = null;

            // if (obj is IDictionary<String, Object> || obj is IList<Object> || obj.GetType().IsArray || obj is Collection<Object>) {
            if (GenericUtil.IsDictionary(obj) || GenericUtil.IsList(obj) || obj.GetType().IsArray || obj is Collection<Object>) {
                jsonObj = obj;
			} else {
				// ???
				// throw new HoloJsonMiniException("Top level element should be an object or an array/list.");
				// ...
				// We support a generic "object" by converting it to a nested map/list structure (most likely, a single level map). 
				// Note that this automatic conversion is for convenience only, and
				//     we only support an object at the top-level object (which recurses down its structure, however), not [object] or {k:object}.
				// If the user wants such a support for more complex collections,
				//    then he/she needs to call HoloJsonMiniStructureBuilder.BuildJsonStructure(obj/[]/{}, depth) explicitly
				//    before calling HoloJsonMiniBuilder.buildJson().
				LiteJsonStructureBuilder structureBuilder = new HoloJsonMiniStructureBuilder();
				jsonObj = await structureBuilder.BuildJsonStructureAsync(obj);
			}

			await _BuildAsync(writer, jsonObj, MAX_DRILL_DOWN_DEPTH);   // ????
			await writer.FlushAsync();   // ???

	//        string jsonStr = writer.ToString();
	//        System.Diagnostics.Debug.WriteLine("jsonStr = " + jsonStr);

		}


		private async Task _BuildAsync(TextWriter writer, object obj, int depth)
		{
			if(depth < 0) {
				return;
			}

			if(obj == null || obj is JsonNull) {
				await writer.WriteAsync(Literals.NULL);
			} else {
				// if(depth == 0) {
				if(depth <= 0) {
					// TBD:
					// This section of code is repeated when depth==0 and and when depth>0,
					//     almost identically... (but not quite exactly the same)
					// Need to be refactored.
					// ....
					string primStr = null;
					if(obj is bool) {
						if(((bool) obj) == true) {
							primStr = Literals.TRUE;
						} else {
							primStr = Literals.FALSE;
						}                        
						await writer.WriteAsync(primStr);
					} else if (obj is char) {
						// ???
						char strChar = (char) obj;
						await writer.WriteAsync("\"");
                        await writer.WriteAsync(strChar);
                        await writer.WriteAsync("\"");
					} else if (Number.IsNumber(obj)) {
						primStr = obj.ToString();
						await writer.WriteAsync(primStr);
					} else if(obj is String) {
						// ????
						// Is there a better/faster way to do this?

						// ???
						primStr = (String) obj;
						// await writer.WriteAsync("\"").Append(primStr).Append("\"");

						// ???
						await writer.WriteAsync("\"");
						_AppendEscapedString(writer, primStr);
						await writer.WriteAsync("\"");
						// ???
						
					} else {
						
						// TBD:
						// java.util.Date ???
						// and other JDK built-in class support???
						// ..

						if(obj is DateTime || obj is DateTimeOffset) {
							// TBD:
							// Create a struct ???
							primStr = obj.ToString();  // ???
							// ...
						} else if(obj is String) {
							primStr = (String) obj;
						} else {
							
							// TBD:
							// POJO/Bean support???
							// ...
							
							// ????
							primStr = obj.ToString();
						}
						// await writer.WriteAsync("\"").Append(primStr).Append("\"");
						await writer.WriteAsync("\"");
						_AppendEscapedString(writer, primStr);
						await writer.WriteAsync("\"");
					}
				} else {
					// if(obj is IDictionary<String,Object>) {
                    if (GenericUtil.IsDictionary(obj)) {
                        // ????
                        // IDictionary<String, Object> map = null;
                        dynamic map = null;
                        try {
							// map = (IDictionary<String,Object>) obj;
                            map = obj;
						} catch(Exception ex) {
							System.Diagnostics.Debug.WriteLine("Invalid map type.", ex);
						}
						await writer.WriteAsync("{");
						if(map != null && map.Count > 0) {
							IEnumerator<String> it = map.Keys.GetEnumerator();
                            var next = it.MoveNext();
							while(next) {
								string key = it.Current;
								object val = map[key];
								await writer.WriteAsync("\"");
                                await writer.WriteAsync(key);
                                await writer.WriteAsync("\":");
                                await writer.WriteAsync(WS);
								await _BuildAsync(writer, val, depth - 1);
                                //// TBD: how to do this????
                                //// ???
                                next = it.MoveNext();
                                if (next) {
                                    await writer.WriteAsync(",");
                                    await writer.WriteAsync(WS);
                                }
                                //// ....
							}
						}
						await writer.WriteAsync("}");
                    // } else if (obj is IList<Object>) {
                    } else if (GenericUtil.IsList(obj)) {
                        // ???
                        // IList<Object> list = null;
                        dynamic list = null;
						try {
							// list = (IList<Object>) obj;
                            list = obj;
						} catch(Exception e) {
							System.Diagnostics.Debug.WriteLine("Invalid list type.", e);
						}
						await writer.WriteAsync("[");
						if(list != null && list.Count > 0) {
							IEnumerator<Object> it = list.GetEnumerator();
                            var next = it.MoveNext();
                            while (next) {
								object o = it.Current;
								await _BuildAsync(writer, o, depth - 1);
                                //// ????
                                //// How to do this???
                                next = it.MoveNext();
                                if (next) {
                                    await writer.WriteAsync(",");
                                    await writer.WriteAsync(WS);
                                }
                                //// ????
							}
						}
						await writer.WriteAsync("]");
					} else if(obj.GetType().IsArray) {           // ???

                        // type safe casting to array????
                        // if obj.GetType().GetElementType() ...
                        // object[] array = (object[]) obj;   // ????

                        // temporary
                        dynamic array = obj;
                        // ????
                        
						await writer.WriteAsync("[");
						if(array!= null && array.Length > 0) {
							int arrLen = array.Length;
							// System.Diagnostics.Debug.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>> arrLen = " + arrLen);
							for(int i=0; i<arrLen; i++) {
								object o = array[i];
								// System.Diagnostics.Debug.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>> o = " + o + "; " + o.Type);
								await _BuildAsync(writer, o, depth - 1);
								// System.Diagnostics.Debug.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>> jsonVal = " + jsonVal + "; " + o.Type);
								if(i < arrLen - 1) {
									await writer.WriteAsync(",");
                                    await writer.WriteAsync(WS);
								}
							}
						}
						await writer.WriteAsync("]");

	//                    // ???
	//                    // This adds weird repeated brackets ([[[[[[[ ...]]]]]]]]).
	//                    _Build(writer, Arrays.asList(array), depth);
					} else if(obj is Collection<Object>) {       // ???????   Not implemented yet.
						Collection<Object> coll = null;
						try {
                            coll = (Collection<Object>) obj;
						} catch(Exception ex) {
							System.Diagnostics.Debug.WriteLine("Invalid collection type.", ex);
						}
						await writer.WriteAsync("[");
						if(coll != null && coll.Count > 0) {
							IEnumerator<Object> it = coll.GetEnumerator();
                            var next = it.MoveNext();
                            while (next) {
								object o = it.Current;
								await _BuildAsync(writer, o, depth - 1);
                                //// TBD:
                                //// ????
                                next = it.MoveNext();
                                if (next) {
                                    await writer.WriteAsync(",");
                                    await writer.WriteAsync(WS);
                                }
                                //// ????
							}
						}
						await writer.WriteAsync("]");
					} else {
						// primitive types... ???
						// ...
						string primStr = null;
						if(obj is bool) {
							if(((bool) obj) == true) {
								primStr = Literals.TRUE;
							} else {
								primStr = Literals.FALSE;
							}                        
							await writer.WriteAsync(primStr);
						} else if (obj is char) {
							// ???
							char strChar = (char) obj;
							await writer.WriteAsync("\"");
                            await writer.WriteAsync(strChar);
                            await writer.WriteAsync("\"");
						} else if (Number.IsNumber(obj)) {
							primStr = obj.ToString();   // ????
							await writer.WriteAsync(primStr);
						} else if (obj is String) {
							// ????
							// Is there a better/faster way to do this?

							// ???
							primStr = (String) obj;
							// await writer.WriteAsync("\"").Append(primStr).Append("\"");

							// ???
							await writer.WriteAsync("\"");
							_AppendEscapedString(writer, primStr);
							await writer.WriteAsync("\"");
							// ???
							
						} else {
							
							// TBD:
							// java.util.Date ???
							// and other JDK built-in class support???
							// ..

							if(obj is DateTime || obj is DateTimeOffset) {
								// TBD:
								// Create a struct ???
								primStr = obj.ToString();
								// ...
							} else if(obj is String) {
								primStr = (String) obj;
							} else {
									
								// ????
								primStr = obj.ToString();

							}
							await writer.WriteAsync("\"");
							_AppendEscapedString(writer, primStr);
							await writer.WriteAsync("\"");
						}
					}
				}
			}
		}

		
		// private static readonly int escapeForwardSlash = -1; 
		private async void _AppendEscapedString(TextWriter writer, string primStr)
		{
			if(primStr != null && primStr.Length > 0) {
                char[] primChars = primStr.ToCharArray();
				// char prevEc = 0;
				foreach(char ec in primChars) {
					if(Symbols.IsEscapedChar(ec)) {
						// if(prevEc == '<' && ec == '/') {
						//     // if(escapeForwardSlash >= 0) {
						//     //     await writer.WriteAsync("\\/");
						//     // } else {
						//         await writer.WriteAsync("/");
						//     // }
						// } else {
							// string str = Symbols.GetEscapedCharString(ec, escapeForwardSlash > 0 ? true : false);
							string str = Symbols.GetEscapedCharString(ec, false);
							if(str != null) {
								await writer.WriteAsync(str);
							} else {
								// ???
								await writer.WriteAsync(ec);
							}
						// }
					} else if(CharUtil.IsISOControl(ec)) {
						char[] uc = UnicodeUtil.GetUnicodeHexCodeFromChar(ec);
                        await writer.WriteAsync(uc);
					} else { 
						await writer.WriteAsync(ec);
					}
					// prevEc = ec;
				}
			}

		}
		
		
	}
}
