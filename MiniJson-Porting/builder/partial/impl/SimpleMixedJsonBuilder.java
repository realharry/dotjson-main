package org.minijson.builder.partial.impl;

import java.util.logging.Logger;

import org.minijson.builder.partial.MixedJsonBuilder;
import org.minijson.builder.policy.BuilderPolicy;


/**
 * Simple MixedJsonBuilder wrapper.
 */
public final class SimpleMixedJsonBuilder extends AbstractMixedJsonBuilder implements MixedJsonBuilder
{
    private static final Logger log = Logger.getLogger(SimpleMixedJsonBuilder.class.getName());

    public SimpleMixedJsonBuilder()
    {
        super();
    }
    public SimpleMixedJsonBuilder(BuilderPolicy builderPolicy)
    {
        super(builderPolicy);
    }
    public SimpleMixedJsonBuilder(BuilderPolicy builderPolicy,
            boolean threadSafe)
    {
        super(builderPolicy, threadSafe);
    }


}
