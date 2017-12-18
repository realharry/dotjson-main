using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotJson.Trait
{
    // What is this???
    // JsonSerializable makes sense.
    // JsonCompatible makes sense.
    // But, do we need this?
    // This is supposed to be in contrast to JsonBuildable.
    // I think, JsonCompatible does what this interface is intended to be doing....
    // But....
    // This can be considered a "marker" interface.
    // What we really need is a way to indicate whether a certain class is instantiable from JSON string, such as fromJson() or valueOf() or parseJson(), etc.
    //    e.g., E e = (E implements JsonPareseable).fromJson(String jsonStr).
    // Since Java does not allow a static method in interface (that is, possibly, until Java8),
    // this is the best we can do...

    /// <summary>
    /// "Marker" interface.
    /// This is sort of the opposite of "JsonBuildable".
    /// </summary>
    public interface JsonParseable // : JsonCompatible
    {
        // This method does not really make sense.
        // JsonParseable parseJson(string jsonStr);

        // JsonParseable class should implement this method (although we cannot enforce that contract).
        // static JsonParseable fromJson(string jsonStr);
    }
}
