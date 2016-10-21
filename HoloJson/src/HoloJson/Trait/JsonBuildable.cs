using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HoloJson.Trait
{
    /// <summary>
    /// "Json Buildable" (as opposed to JsonSerializable).
    /// </summary>
    public interface JsonBuildable : JsonCompatible, IndentedJsonSerializable
    {
    }
}
