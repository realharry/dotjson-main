package org.minijson.type.base;

import java.io.IOException;
import java.io.Serializable;
import java.io.Writer;
import java.util.Collection;
import java.util.HashMap;
import java.util.HashSet;
import java.util.Iterator;
import java.util.LinkedHashMap;
import java.util.Map;
import java.util.Set;
import java.util.logging.Logger;

import org.minijson.builder.JsonBuilderException;
import org.minijson.builder.core.IndentInfoStruct;
import org.minijson.type.JsonNode;
import org.minijson.type.JsonObjectMember;
import org.minijson.type.JsonObjectNode;
import org.minijson.type.JsonStringNode;


public class AbstractJsonObjectNode extends AbstractJsonStructNode implements JsonObjectNode, Serializable
{
    private static final Logger log = Logger.getLogger(AbstractJsonObjectNode.class.getName());
    private static final long serialVersionUID = 1L;

    public static final AbstractJsonObjectNode NULL = new AbstractJsonObjectNode() {
        private static final long serialVersionUID = 1L;
        @Override
        public String toJsonString() throws JsonBuilderException
        {
            return "null";
        }
    };

    // Decorated object.
    private final Map<String,Object> map;
    
    private AbstractJsonObjectNode()
    {
        this(null);
    }
    public AbstractJsonObjectNode(Map<String,Object> map)
    {
        if(map == null) {
            this.map = new HashMap<String,Object>();
        } else {
            this.map = map;
        }
    }


    ///////////////////////////////////
    // JsonNode interface
    
    @Override
    public Object getValue()
    {
        // ????
        return map;
    }


    ///////////////////////////////////
    // JsonSerializable interface
    // Note: The default depth of AbstractJsonNodes is always 1.   

//    @Override
//    public String toJsonString(int indent)
//    {
//        // temporary
//        StringBuilder sb = new StringBuilder();
//        sb.append("{");
//        for(String key : map.keySet()) {
//            JsonNode node = (JsonNode) map.get(key);
//            JsonStringNode jsonKey = new AbstractJsonStringNode(key);
//            sb.append(jsonKey.toJsonString()).append(":").append(node.toJsonString());
//            sb.append(",");
//        }
//        if(sb.charAt(sb.length() - 1) == ',') {
//            sb.deleteCharAt(sb.length() - 1);
//        }
//        sb.append("}");
//        return sb.toString();
//    }
    @Override
    public void writeJsonString(Writer writer, int indent) throws IOException, JsonBuilderException
    {
        IndentInfoStruct indentInfo = new IndentInfoStruct(indent);
        boolean includeWS = indentInfo.isIncludingWhiteSpaces();
        boolean includeLB = indentInfo.isIncludingLineBreaks();
        boolean lbAfterComma = indentInfo.isLineBreakingAfterComma();
        int indentSize = indentInfo.getIndentSize();
        
        // ???
        // We need a way to set the "global indent level" ....
        int indentLevel = 0;
        String WS = "";
        if(includeWS) {
            WS = " ";
        }
        String LB = "";
        if(includeLB) {
            LB = "\n";
        }
        String IND = "";
        String INDX = "";
        if(indentSize > 0 && indentLevel > 0) {
            IND = String.format("%1$" + (indentSize * indentLevel) + "s", "");
        }
        if(indentSize > 0 && indentLevel >= 0) {
            INDX = String.format("%1$" + (indentSize * (indentLevel+1)) + "s", "");
        }
        
        writer.append("{").append(LB).append(INDX);
        Iterator<String> it = map.keySet().iterator();
        while(it.hasNext()) {
            String key = it.next();                    
            JsonStringNode jsonKey = new AbstractJsonStringNode(key);
            JsonNode node = (JsonNode) map.get(key);
            writer.append("\"").append(jsonKey.toJsonString(indent)).append("\":").append(WS);
            writer.append(node.toJsonString());
            if(it.hasNext()) {
                writer.append(",");
                if(lbAfterComma) {
                    writer.append(LB).append(INDX);
                } else {
                    writer.append(WS);
                }
            }
        }
        writer.append(LB).append(IND).append("}");
    }


    ///////////////////////////////////////////////////////
    // TBD: JsonCompatible interface..
    // ....

    // @Override
    public boolean isJsonStructureArray()
    {
        return false;
    }

    @Override
    public Object toJsonStructure(int depth) throws JsonBuilderException
    {
        // ????
        // return map;

        Map<String,Object> struct = new LinkedHashMap<String,Object>();
        
        // TBD:
        // Traverse the map down to depth...
        struct = map;
        // ...
        
        return struct;
    }

    


    ///////////////////////////////////
    // JsonObject interface

    @Override
    public boolean hasMembers()
    {
        return ! map.isEmpty();
    }

    @Override
    public Set<JsonObjectMember> getMembers()
    {
        Set<Entry<String,Object>> entrySet = map.entrySet();
        if(entrySet != null) {
            Set<JsonObjectMember> members = new HashSet<JsonObjectMember>();
            for(Entry<String,Object> e : entrySet) {
                String key = e.getKey();
                Object value = e.getValue();
                JsonObjectMember member = new AbstractJsonObjectMember(key, (JsonNode) value);
                members.add(member);
            }
            return members;
        }
        return null;
    }

    @Override
    public JsonNode getMemberNode(String key)
    {
        JsonNode node =  (JsonNode) this.get((Object) key);
        return node;
    }

    @Override
    public void addMember(JsonObjectMember member)
    {
        if(member != null) {
            String key = member.getKey();
            JsonNode node = member.getValue();
            map.put(key, node);
        }
    }

    @Override
    public void addAllMembers(Set<JsonObjectMember> members)
    {
        if(members != null) {
            for(JsonObjectMember m : members) {
                String key = m.getKey();
                JsonNode node = m.getValue();
                map.put(key, node);
            }
        }
    }


    
    /////////////////////////////////////
    // Map interface.
    
    @Override
    public int size()
    {
        return map.size();
    }
    @Override
    public boolean isEmpty()
    {
        return map.isEmpty();
    }
    @Override
    public boolean containsKey(Object key)
    {
        return map.containsKey(key);
    }
    @Override
    public boolean containsValue(Object value)
    {
        return map.containsValue(value);
    }
    @Override
    public Object get(Object key)
    {
        return map.get(key);
    }
    @Override
    public Object put(String key, Object value)
    {
        return map.put(key, value);
    }
    @Override
    public Object remove(Object key)
    {
        return map.remove(key);
    }
    @Override
    public void putAll(Map<? extends String, ? extends Object> m)
    {
        map.putAll(m);
    }
    @Override
    public void clear()
    {
        map.clear();
    }
    @Override
    public Set<String> keySet()
    {
        return map.keySet();
    }
    @Override
    public Collection<Object> values()
    {
        return map.values();
    }
    @Override
    public Set<java.util.Map.Entry<String, Object>> entrySet()
    {
        return map.entrySet();
    }

    
    
    
    @Override
    public String toString()
    {
        return "AbstractJsonObject [map=" + map + "]";
    }
    
    
    
    
}
