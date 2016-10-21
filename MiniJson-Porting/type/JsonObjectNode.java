package org.minijson.type;

import java.util.Map;
import java.util.Set;

import org.minijson.JsonCompatible;


public interface JsonObjectNode extends JsonStructNode, Map<String,Object>
{
    boolean hasMembers();
    Set<JsonObjectMember> getMembers();

    JsonNode getMemberNode(String key);

    void addMember(JsonObjectMember member);
    void addAllMembers(Set<JsonObjectMember> members);
    // JsonNode put(String key, JsonNode value);
    // JsonNode putAll(Map<String,JsonNode> m);
}
