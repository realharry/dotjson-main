using HoloJson.Mini;
using DotJson.Parser;
using DotJson.Trait;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotJson.Base
{
    // Convenience class to be used as a base class for JsonParseable classes.
    // TBD: Just use BaseJsonCompatible ???
    // ....
    // TBD: This is a very strange implementation...
    // Do not use this class, for now, until we can come up with with a better interface/implementation.....
    public abstract class BaseJsonParseable : JsonParseable
    {
        // Is this safe to reuse this across multiple instances of this class???
        private static JsonParser sMiniJsonParser = new MiniJsonParser();

        // temporary
        // JsonParseable is a very strange interface...
        // this seems to be the only way to support JsonParseable.parseJson() ...
        // ????
        private object parsedObject = null;

        public BaseJsonParseable() : this(null)
        {
        }
        public BaseJsonParseable(object parsedObject)
        {
            this.parsedObject = parsedObject;
        }

        //    // @Override
        //    public JsonParseable parseJson(String jsonStr)
        //    {
        //        // ???
        //        try {
        //            parsedObject = MiniJsonParser.DEFAULT_INSTANCE.parse(jsonStr);
        //        } catch (JsonParserException e) {
        //            throw new RuntimeException(e);
        //        }
        //        return this;
        //    }


        // Note: Conceptually, it is a method "defined" in JsonParseable.
        // Since you cannot inherit this implementation in subclasses,
        // each subclass, or any class that implements JsonParseable, should have its own implementation of FromJson().
        // Use this class as an example.... if necessary...
        // (but, this is a rather strange implementation...)
        // (normally, each specific field of a JsonParseable class should be initialized by the corresponding values in jsonStr,
        //      and it's rather hard to Create a "generic" implementation, unless you use reflection, etc.)
        public static async Task<JsonParseable> FromJsonAsync(string jsonStr)
        {
            BaseJsonParseable jsonParseable = null;
            try {
                // Object obj = MiniJsonParser.DEFAULT_INSTANCE.parse(jsonStr);
                object obj = await sMiniJsonParser.ParseAsync(jsonStr);
                jsonParseable = new BaseJsonParseableAnonymousInnerClass(obj);
            } catch (JsonParserException e) {
                throw new Exception(e.Message, e);
            }
            return jsonParseable;
        }

        private class BaseJsonParseableAnonymousInnerClass : BaseJsonParseable
        {
            public BaseJsonParseableAnonymousInnerClass(object obj) : base(obj)
            {
            }

        }

    }

}