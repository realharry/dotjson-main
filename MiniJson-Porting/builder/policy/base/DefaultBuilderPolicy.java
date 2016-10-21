package org.minijson.builder.policy.base;

import org.minijson.builder.policy.BuilderPolicy;


/**
 * Default implementation of BuilderPolicy.
 */
public class DefaultBuilderPolicy extends AbstractBuilderPolicy implements BuilderPolicy
{
    private static final long serialVersionUID = 1L;

    // Predefined policies.
    // These are just examples. No need to use "predefined" policies.
    public static final BuilderPolicy NODRILLDOWN = new DefaultBuilderPolicy(1, false, 0);
    public static final BuilderPolicy NOINSPECT = new DefaultBuilderPolicy(2, false, 0);
    // Default ?
    public static final BuilderPolicy SIMPLE = new DefaultBuilderPolicy(1, true, 0);
    // MiniJson default. Up to 10 levels.
    public static final BuilderPolicy MINIJSON = new DefaultBuilderPolicy(10, true, 0);
    // "deep"
    public static final BuilderPolicy BEANDRILLDOWN = new DefaultBuilderPolicy(-1, true, 0);
    // "deep"
    public static final BuilderPolicy MAPANDLISTS = new DefaultBuilderPolicy(-1, false, 0);
    // Use bean introspection + escapeForwardSlash
    public static final BuilderPolicy ESCAPESLASH = new DefaultBuilderPolicy(-1, true, 1);
    // Use bean introspection + no escapeForwardSlash (not even "</")
    public static final BuilderPolicy NOESCAPESLASH = new DefaultBuilderPolicy(-1, true, -1);
    // RPC default (e.g., REST API web service calls).
    public static final BuilderPolicy RPCOBJECT = new DefaultBuilderPolicy(3, true, -1);
    // ....

    
    // Ctor.
    public DefaultBuilderPolicy(int drillDownDepth, boolean useBeanIntrospection, int escapeForwardSlash)
    {
        super(drillDownDepth, useBeanIntrospection, escapeForwardSlash);
    }



}
