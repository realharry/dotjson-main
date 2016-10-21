package org.minijson.builder.factory;

import org.minijson.builder.IndentedJsonBuilder;


public interface JsonBuilderFactory
{
    IndentedJsonBuilder createBuilder();
}
