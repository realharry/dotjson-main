package org.minijson.base;

import org.minijson.JsonParseable;
import org.minijson.mini.MiniJsonParser;
import org.minijson.parser.JsonParser;
import org.minijson.parser.JsonParserException;


// Convenience class to be used as a base class for JsonParseable classes.
// TBD: Just use MiniJsonCompatible ???
// ....
// TBD: This is a very strange implementation...
// Do not use this class, for now, until we can come up with with a better interface/implementation.....
public abstract class MiniJsonParseable implements JsonParseable
{
    // Is this safe to reuse this across multiple instances of this class???
    private static JsonParser sMiniJsonParser = new MiniJsonParser();

    // temporary
    // JsonParseable is a very strange interface...
    // this seems to be the only way to support JsonParseable.parseJson() ...
    // ????
    private Object parsedObject = null;
    
    public MiniJsonParseable()
    {
        this(null);
    }
    public MiniJsonParseable(Object parsedObject)
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
    // each subclass, or any class that implements JsonParseable, should have its own implementation of fromJson().
    // Use this class as an example.... if necessary...
    // (but, this is a rather strange implementation...)
    // (normally, each specific field of a JsonParseable class should be initialized by the corresponding values in jsonStr,
    //      and it's rather hard to create a "generic" implementation, unless you use reflection, etc.)
    public static JsonParseable fromJson(String jsonStr)
    {
        MiniJsonParseable jsonParseable = null;
        try {
            // Object obj = MiniJsonParser.DEFAULT_INSTANCE.parse(jsonStr);
            Object obj = sMiniJsonParser.parse(jsonStr);
            jsonParseable = new MiniJsonParseable(obj) {};
        } catch (JsonParserException e) {
            throw new RuntimeException(e);
        }
        return jsonParseable;        
    }

}
