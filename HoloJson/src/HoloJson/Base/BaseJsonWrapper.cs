using HoloJson.Mini;
using HoloJson.Parser;
using HoloJson.Trait;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HoloJson.Base
{
    /// <summary>
    /// Base class for BaseJsonObject and BaseJsonArray.
    /// </summary>
    public abstract class BaseJsonWrapper : JsonAny
    {
        // temporary
        private const int DRILL_DOWN_DEPTH = 1;

        // Is this safe to reuse these across multiple instances of this class???
        private static JsonParser sMiniJsonParser = null;
        private static MiniJsonBuilder sMiniJsonBuilder = null;
        protected internal static JsonParser StaticJsonParser
        {
            get
            {
                if (sMiniJsonParser == null) {
                    sMiniJsonParser = new MiniJsonParser();
                }
                return sMiniJsonParser;
            }
        }
        protected internal static MiniJsonBuilder StaticJsonBuilder
        {
            get
            {
                if (sMiniJsonBuilder == null) {
                    sMiniJsonBuilder = new MiniJsonBuilder();
                }
                return sMiniJsonBuilder;
            }
        }


        // Note:
        // This class has an unusual implementation.
        // We use jsonObj field instead of "this" to represent JSON equivalent.
        // That is, if we do ToJsonStringAsync(), we do not return (this object).ToJsonStringAsync().
        // Rather we return jsonObj.ToJsonStringAsync().
        // ...
        // TBD:
        // This class needs to be re-implemented...
        // ....

        // Internal "wrapped" object
        private object jsonObj = null;

        // Lazy initialized.
        private JsonParser miniJsonParser = null;
        private MiniJsonBuilder miniJsonBuilder = null;

        // TBD:
        // ParserPolicy????
        // BuilderPolicy????
        // ...

        public BaseJsonWrapper()
        {
        }

        // setters???
        protected internal virtual JsonParser JsonParser
        {
            get
            {
                if (miniJsonParser == null) {
                    miniJsonParser = new MiniJsonParser();
                }
                return miniJsonParser;
            }
        }
        protected internal virtual MiniJsonBuilder JsonBuilder
        {
            get
            {
                if (miniJsonBuilder == null) {
                    miniJsonBuilder = new MiniJsonBuilder();
                }
                return miniJsonBuilder;
            }
        }



        // JSON builder
        public BaseJsonWrapper(object jsonObj)
        {
            this.jsonObj = jsonObj;
        }

        // JSON Parser
        public BaseJsonWrapper(string jsonStr)
        {
            this.jsonObj = CreateObjectFromJsonAsync(jsonStr);
        }

        private static async Task<object> CreateObjectFromJsonAsync(string jsonStr)
        {
            object obj = null;
            try {
                // obj = MiniJsonParser.DEFAULT_INSTANCE.parse(jsonStr);
                obj = await StaticJsonParser.ParseAsync(jsonStr);
            } catch (JsonParserException e) {
                throw new Exception(e.Message, e);
            }
            return obj;
        }

        // Note: Conceptually, it is a method "defined" in JsonParseable.
        // Since you cannot inherit this implementation in subclasses,
        // each subclass, or any class that implements JsonParseable, should have its own implementation of FromJson().
        // Use this class as an example.... if necessary...
        // (but, this is a rather strange implementation...)
        // (normally, each specific field of a JsonParseable class should be initialized by the corresponding values in jsonStr,
        //      and it's rather hard to Create a "generic" implementation, unless you use reflection, etc.)
        public static async Task<JsonParseable> FromJsonAsync(string jsonStr)
        {
            BaseJsonWrapper jsonParseable = null;
            try {
                // Object obj = MiniJsonParser.DEFAULT_INSTANCE.parse(jsonStr);
                object obj = await StaticJsonParser.ParseAsync(jsonStr);
                jsonParseable = new BaseJsonWrapperAnonymousInnerClass(obj);
            } catch (JsonParserException e) {
                throw new Exception(e.Message, e);
            }
            return jsonParseable;
        }

        private class BaseJsonWrapperAnonymousInnerClass : BaseJsonWrapper
        {
            public BaseJsonWrapperAnonymousInnerClass(object obj) : base(obj)
            {
            }

        }


        public async Task<string> ToJsonStringAsync()
        {
            return await JsonBuilder.BuildAsync(jsonObj);
        }
        public async Task<string> ToJsonStringAsync(int indent)
        {
            return await JsonBuilder.BuildAsync(jsonObj, indent);
        }

        public async Task WriteJsonStringAsync(TextWriter writer)
        {
            await JsonBuilder.BuildAsync(writer, jsonObj);
        }
        public async Task WriteJsonStringAsync(TextWriter writer, int indent)
        {
            await JsonBuilder.BuildAsync(writer, jsonObj, indent);
        }

        public async Task<object> ToJsonStructureAsync()
        {
            return await ToJsonStructureAsync(DRILL_DOWN_DEPTH);
        }

        public async Task<object> ToJsonStructureAsync(int depth)
        {
            return await JsonBuilder.BuildJsonStructureAsync(jsonObj, depth);
        }


        // For debugging...
        public override string ToString()
        {
            return "BaseJsonWrapper [jsonObj=" + jsonObj + "]";
        }


    }

}