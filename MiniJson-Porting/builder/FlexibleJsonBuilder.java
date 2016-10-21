package org.minijson.builder;

import org.minijson.builder.policy.BuilderPolicy;


/**
 * JsonBuilder with configurable options.
 */
public interface FlexibleJsonBuilder extends JsonBuilder
{
    /**
     * Get the BuilderPolicy object associated with this JsonBuilder object.
     */
    BuilderPolicy getBuilderPolicy();
}
