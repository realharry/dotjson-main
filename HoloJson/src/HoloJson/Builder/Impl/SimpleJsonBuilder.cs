using HoloJson.Builder.Policy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HoloJson.Builder.Impl
{
    /// <summary>
    /// Simple BareJsonBuilder implementation.
    /// </summary>
    public sealed class SimpleJsonBuilder : AbstractBareJsonBuilder, BareJsonBuilder
    {
        public SimpleJsonBuilder() : base()
        {
        }
        public SimpleJsonBuilder(BuilderPolicy builderPolicy)
            : base(builderPolicy)
        {
        }
        public SimpleJsonBuilder(BuilderPolicy builderPolicy, bool threadSafe) 
            : base(builderPolicy, threadSafe)
        {
        }



    }

}