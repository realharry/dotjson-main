package org.minijson.mapper;


// TBD:
public interface ObjectConverter
{
    // ???
    // What we need is a static method that converts a "json structure" (nested map/list)
    //    into a given object type.
    Object convert(Object jsonObj);
}
