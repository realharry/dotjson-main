using DotJson.Builder.Policy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotJson.Builder.Partial.Impl
{
    /// <summary>
    /// Simple MixedJsonBuilder wrapper.
    /// </summary>
    public sealed class SimpleMixedJsonBuilder : AbstractMixedJsonBuilder, MixedJsonBuilder
    {
        public SimpleMixedJsonBuilder() : base()
        {
        }
        public SimpleMixedJsonBuilder(BuilderPolicy builderPolicy) 
            : base(builderPolicy)
        {
        }
        public SimpleMixedJsonBuilder(BuilderPolicy builderPolicy, bool threadSafe) 
            : base(builderPolicy, threadSafe)
        {
        }


    }

}