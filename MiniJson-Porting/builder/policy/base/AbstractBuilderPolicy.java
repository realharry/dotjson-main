package org.minijson.builder.policy.base;

import java.io.Serializable;

import org.minijson.builder.policy.BuilderPolicy;


/**
 * Base implementation for BuilderPolicy.
 */
public abstract class AbstractBuilderPolicy implements BuilderPolicy, Serializable
{
    private static final long serialVersionUID = 1L;

    // Max value: equivalent to -1.
    private static final int MAX_DRILL_DOWN_DEPTH = (int) Byte.MAX_VALUE;  // Arbitrary.

    // drillDownDepth >= 0.
    // how to validate?
    private final int drillDownDepth;
    // Whether to use bean inspection.
    private final boolean useBeanIntrospection;
    // Whether to escape "/".
    // Even if it's false "</" is always escaped.
    private final int escapeForwardSlash;

//    public AbstractBuilderPolicy()
//    {
//        this(false, 1);
//    }
    public AbstractBuilderPolicy(int drillDownDepth, boolean useBeanIntrospection, int escapeForwardSlash)
    {
        super();
        if(drillDownDepth < 0 || drillDownDepth > MAX_DRILL_DOWN_DEPTH) {
            this.drillDownDepth = MAX_DRILL_DOWN_DEPTH;
        } else {
            this.drillDownDepth = drillDownDepth;
        }
        this.useBeanIntrospection = useBeanIntrospection;
        this.escapeForwardSlash = escapeForwardSlash;
    }

    @Override
    public int drillDownDepth()
    {
        return this.drillDownDepth;
    }
//    public void setDrillDownDepth(int drillDownDepth)
//    {
//        if(drillDownDepth < 0 || drillDownDepth > MAX_DRILL_DOWN_DEPTH) {
//            this.drillDownDepth = MAX_DRILL_DOWN_DEPTH;
//        } else {
//            this.drillDownDepth = drillDownDepth;
//        }
//    }

    @Override
    public boolean useBeanIntrospection()
    {
        return this.useBeanIntrospection;
    }

    @Override
    public int escapeForwardSlash()
    {
        return this.escapeForwardSlash;
    }


    @Override
    public String toString()
    {
        return "AbstractBuilderPolicy [drillDownDepth=" + drillDownDepth
                + ", useBeanIntrospection=" + useBeanIntrospection
                + ", escapeForwardSlash=" + escapeForwardSlash + "]";
    }


}
