package org.minijson;

import org.minijson.builder.JsonBuilderException;


/**
 * Place holder.
 */
public interface FastJsonSerializable extends JsonSerializable, JsonTemplateable
{
    // String buildJsonUsingTemplate(Object ... args);
    String buildJsonUsingTemplate(String ... args) throws JsonBuilderException;
}
