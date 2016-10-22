using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HoloJson.Builder
{
    /// <summary>
    /// Base JSON Task<string> Builder interface
    /// (which uses only Map and List).
    /// This is the primary base interface for MiniJson JSON builder implementations.
    /// </summary>
    public interface BareJsonBuilder : IndentedJsonBuilder, JsonStructureBuilder
    {
    }

}