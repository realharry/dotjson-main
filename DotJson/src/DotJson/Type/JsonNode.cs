using DotJson.Trait;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotJson.Type
{
    /// <summary>
    /// Base class for a node in a JSON parsed tree.
    /// Note that JsonNodes are used only for "Rich" parsers/builders.
    /// ("Bare" parsers/builders use only Java Maps and Lists.)
    /// </summary>
    public interface JsonNode : IndentedJsonSerializable
    {
        /// <summary>
        /// Returns the "value" of this node, that is, a Map, a List, a String, etc...
        /// </summary>
        /// <returns>The value of this node.</returns>
        // object GetValue();
        object Value { get; }

        //    boolean isObject();
        //    boolean isArray();
        //    boolean isString();
        //    boolean isNumber();
        //    boolean isBoolean();
        //    boolean isNull();

        //    boolean hasChildren();
        //    List<JsonNode> getChildren();
        //    
        //    void addChild(JsonNode child);
        //    void addChildren(List<JsonNode> children);

    }
}
