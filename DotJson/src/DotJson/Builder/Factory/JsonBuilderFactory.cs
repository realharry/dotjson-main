using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotJson.Builder.Factory
{
    public interface JsonBuilderFactory
    {
        IndentedJsonBuilder CreateBuilder();
    }

}