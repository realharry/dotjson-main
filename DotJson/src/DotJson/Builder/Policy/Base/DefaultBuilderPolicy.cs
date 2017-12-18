using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotJson.Builder.Policy.Base
{
    /// <summary>
    /// Default implementation of BuilderPolicy.
    /// </summary>
    // [Serializable]
    public class DefaultBuilderPolicy : AbstractBuilderPolicy, BuilderPolicy
    {
        // Predefined policies.
        // These are just examples. No need to use "predefined" policies.
        public static readonly BuilderPolicy NODRILLDOWN = new DefaultBuilderPolicy(1, false, 0);
        public static readonly BuilderPolicy NOINSPECT = new DefaultBuilderPolicy(2, false, 0);
        // Default ?
        public static readonly BuilderPolicy SIMPLE = new DefaultBuilderPolicy(1, true, 0);
        // MiniJson default. Up to 10 levels.
        public static readonly BuilderPolicy MINIJSON = new DefaultBuilderPolicy(10, true, 0);
        // "deep"
        public static readonly BuilderPolicy BEANDRILLDOWN = new DefaultBuilderPolicy(-1, true, 0);
        // "deep"
        public static readonly BuilderPolicy MAPANDLISTS = new DefaultBuilderPolicy(-1, false, 0);
        // Use bean introspection + escapeForwardSlash
        public static readonly BuilderPolicy ESCAPESLASH = new DefaultBuilderPolicy(-1, true, 1);
        // Use bean introspection + no escapeForwardSlash (not even "</")
        public static readonly BuilderPolicy NOESCAPESLASH = new DefaultBuilderPolicy(-1, true, -1);
        // RPC default (e.g., REST API web service calls).
        public static readonly BuilderPolicy RPCOBJECT = new DefaultBuilderPolicy(3, true, -1);
        // ....


        // Ctor.
        public DefaultBuilderPolicy(int drillDownDepth, bool useBeanIntrospection, int escapeForwardSlash) : base(drillDownDepth, useBeanIntrospection, escapeForwardSlash)
        {
        }


    }

}