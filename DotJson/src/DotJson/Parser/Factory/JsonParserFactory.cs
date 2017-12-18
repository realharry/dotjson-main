using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotJson.Parser.Factory
{
    public interface JsonParserFactory
    {
        JsonParser CreateParser();
    }

}