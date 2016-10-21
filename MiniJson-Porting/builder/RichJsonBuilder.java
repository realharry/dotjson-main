package org.minijson.builder;

import java.io.IOException;
import java.io.Writer;

import org.minijson.type.JsonNode;


/**
 * JSON builder using internal JSON node variables.
 * In contract, BareJsonBuilder only uses JDK Map and List.
 */
public interface RichJsonBuilder extends IndentedJsonBuilder, JsonStructureBuilder
{
    String buildJson(JsonNode node) throws JsonBuilderException;
    String buildJson(JsonNode node, int indent) throws JsonBuilderException;

    void buildJson(Writer writer, JsonNode node) throws IOException, JsonBuilderException;
    void buildJson(Writer writer, JsonNode node, int indent) throws IOException, JsonBuilderException;
}
