using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HoloJson.Builder.Policy.Base
{
    /// <summary>
    /// Base implementation for BuilderPolicy.
    /// </summary>
    // [Serializable]
    public abstract class AbstractBuilderPolicy : BuilderPolicy
    {
        // Max value: equivalent to -1.
        private static readonly int MAX_DRILL_DOWN_DEPTH = (int)sbyte.MaxValue; // Arbitrary.

        // drillDownDepth >= 0.
        // how to validate?
        private readonly int drillDownDepth;
        // Whether to use bean inspection.
        private readonly bool useBeanIntrospection;
        // Whether to escape "/".
        // Even if it's false "</" is always escaped.
        private readonly int escapeForwardSlash;

        //    public AbstractBuilderPolicy()
        //    {
        //        this(false, 1);
        //    }
        public AbstractBuilderPolicy(int drillDownDepth, bool useBeanIntrospection, int escapeForwardSlash) : base()
        {
            if (drillDownDepth < 0 || drillDownDepth > MAX_DRILL_DOWN_DEPTH) {
                this.drillDownDepth = MAX_DRILL_DOWN_DEPTH;
            } else {
                this.drillDownDepth = drillDownDepth;
            }
            this.useBeanIntrospection = useBeanIntrospection;
            this.escapeForwardSlash = escapeForwardSlash;
        }

        public virtual int DrillDownDepth
        {
            get
            {
                return this.drillDownDepth;
            }
        }
        //    public void setDrillDownDepth(int drillDownDepth)
        //    {
        //        if(drillDownDepth < 0 || drillDownDepth > MAX_DRILL_DOWN_DEPTH) {
        //            this.drillDownDepth = MAX_DRILL_DOWN_DEPTH;
        //        } else {
        //            this.drillDownDepth = drillDownDepth;
        //        }
        //    }

        public virtual bool UseBeanIntrospection
        {
            get
            {
                return this.useBeanIntrospection;
            }
        }

        public virtual int EscapeForwardSlash
        {
            get
            {
                return this.escapeForwardSlash;
            }
        }


        public override string ToString()
        {
            return "AbstractBuilderPolicy [drillDownDepth=" + drillDownDepth + ", useBeanIntrospection=" + useBeanIntrospection + ", escapeForwardSlash=" + escapeForwardSlash + "]";
        }


    }

}