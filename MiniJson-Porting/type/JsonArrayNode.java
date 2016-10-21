package org.minijson.type;

import java.util.List;

import org.minijson.JsonCompatible;


public interface JsonArrayNode extends JsonStructNode, List<Object>
{
    boolean hasChildren();
    List<JsonNode> getChildren();

    JsonNode getChildNode(int index);

    void addChild(JsonNode child);
    void addAllChildren(List<JsonNode> children);

}
