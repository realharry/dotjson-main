﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HoloJson.Type
{
    // Represents "key:value" member of a JsonObject.
    // Not a JsonNode ????
    public interface JsonObjectMember // : JsonNode
    {
        string GetKey();
        JsonNode GetValue();
    }
}
