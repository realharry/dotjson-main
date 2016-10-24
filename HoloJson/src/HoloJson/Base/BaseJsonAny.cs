using HoloJson.Mini;
using HoloJson.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HoloJson.Base
{
    /// <summary>
    /// Convenience class.
    /// Can be used as a base class for classes implementing JsonAny.
    /// </summary>
    public abstract class BaseJsonAny : JsonAny
    {
        // temporary
        private const int DRILL_DOWN_DEPTH = 1;

        // Lazy initialized.
        private JsonParser miniJsonParser = null;
        private MiniJsonBuilder miniJsonBuilder = null;

        // TBD:
        // ParserPolicy????
        // BuilderPolicy????
        // ...

        public BaseJsonAny()
        {
            init();
        }
        protected internal virtual void init()
        {
            // Place-holder.
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


        // Note: Conceptually, it is a method "defined" in JsonParseable.
        // Since you cannot inherit this implementation in subclasses,
        // each subclass, or any class that implements JsonParseable, should have its own implementation of FromJson().
        // Use this class as an example.... if necessary...
        // (but, this is a rather strange implementation...)
        // (normally, each specific field of a JsonParseable class should be initialized by the corresponding values in jsonStr,
        //      and it's rather hard to Create a "generic" implementation, unless you use reflection, etc.)
        //    public static JsonParseable FromJson(String jsonStr)
        //    {
        //        BaseJsonAny jsonParseable = null;
        //        try {
        //            // Object obj = MiniJsonParser.DEFAULT_INSTANCE.parse(jsonStr);
        //            Object obj = sMiniJsonParser.parse(jsonStr);
        //            jsonParseable = new BaseJsonAny() {};
        //            // Copy the field value....
        //        } catch (JsonParserException e) {
        //            throw new RuntimeException(e);
        //        }
        //        return jsonParseable;
        //    }


        public async Task<string> ToJsonStringAsync()
        {
            return await JsonBuilder.BuildAsync(this);
        }
        public async Task<string> ToJsonStringAsync(int indent)
        {
            return await JsonBuilder.BuildAsync(this, indent);
        }

        public async Task WriteJsonStringAsync(TextWriter writer)
        {
            await JsonBuilder.BuildAsync(writer, this);
        }
        public async Task WriteJsonStringAsync(TextWriter writer, int indent)
        {
            await JsonBuilder.BuildAsync(writer, this, indent);
        }

        public async Task<object> ToJsonStructureAsync()
        {
            return await ToJsonStructureAsync(DRILL_DOWN_DEPTH);
        }

        public async Task<object> ToJsonStructureAsync(int depth)
        {
            return await JsonBuilder.BuildJsonStructureAsync(this, depth);
        }


    }

}