using DotJson.Trait;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotJson
{
    /// <summary>
    /// Top level json object representation.
    /// </summary>
    public interface JsonAny : JsonBuildable, JsonParseable
    {
    }
}
