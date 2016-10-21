package org.minijson.type;


// Represents "key:value" member of a JsonObject.
// Not a JsonNode ????
public interface JsonObjectMember // extends JsonNode
{
    String getKey();
    JsonNode getValue();
}
