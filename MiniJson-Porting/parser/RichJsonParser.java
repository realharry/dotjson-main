package org.minijson.parser;

import java.io.IOException;
import java.io.Reader;

import org.minijson.type.JsonNode;


/**
 * JsonParser which parses the given JSON string and builds a JsonNode.
 * In contrast, BareJsonParser returns an object comprising Java Maps and Lists.
 */
public interface RichJsonParser extends JsonParser
{
    JsonNode parseJson(String jsonStr) throws JsonParserException;
    JsonNode parseJson(Reader reader) throws JsonParserException, IOException;
    // JsonObject parseObject();
    // JsonArray parseArray();
}
