using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotJson.Builder.Factory
{
    // TBD: This is potentially a problem....
    public interface BareJsonBuilderFactory : JsonBuilderFactory
    {
        // ???
        // Vs. JsonBuilderFactory.createBuilder() ???
        new BareJsonBuilder CreateBuilder();
    }

}