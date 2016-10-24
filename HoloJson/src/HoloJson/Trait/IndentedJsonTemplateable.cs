using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HoloJson.Trait
{
    // place holder.
    public interface IndentedJsonTemplateable : JsonTemplateable
    {
        Task<string> GetJsonTemplateAsync(int indent);
    }
}
