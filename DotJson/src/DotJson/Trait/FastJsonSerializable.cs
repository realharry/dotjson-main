using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotJson.Trait
{
    // Placeholder.
    public interface FastJsonSerializable : JsonSerializable, JsonTemplateable
    {
        // string BuildJsonUsingTemplate(params object[] args);
        Task<string> BuildJsonUsingTemplateAsync(params string[] args);
    }
}
