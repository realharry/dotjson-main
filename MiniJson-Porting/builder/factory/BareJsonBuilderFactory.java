package org.minijson.builder.factory;

import org.minijson.builder.BareJsonBuilder;


// TBD: This is potentially a problem....
public interface BareJsonBuilderFactory extends JsonBuilderFactory
{
    // ???
    // Vs. JsonBuilderFactory.createBuilder() ???
    BareJsonBuilder createBuilder();
}
