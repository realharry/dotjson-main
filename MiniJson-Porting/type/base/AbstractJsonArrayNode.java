package org.minijson.type.base;

import java.io.IOException;
import java.io.Serializable;
import java.io.Writer;
import java.util.ArrayList;
import java.util.Collection;
import java.util.Iterator;
import java.util.List;
import java.util.ListIterator;
import java.util.logging.Logger;

import org.minijson.builder.JsonBuilderException;
import org.minijson.builder.core.IndentInfoStruct;
import org.minijson.common.Literals;
import org.minijson.type.JsonArrayNode;
import org.minijson.type.JsonNode;


public class AbstractJsonArrayNode extends AbstractJsonStructNode implements JsonArrayNode, Serializable
{
    private static final Logger log = Logger.getLogger(AbstractJsonArrayNode.class.getName());
    private static final long serialVersionUID = 1L;

    public static final AbstractJsonArrayNode NULL = new AbstractJsonArrayNode() {
        private static final long serialVersionUID = 1L;
        @Override
        public String toJsonString() throws JsonBuilderException
        {
            return Literals.NULL;
        }
    };
    
    // Decorated object.
    private final List<Object> list;

    private AbstractJsonArrayNode()
    {
        this(null);
    }
    public AbstractJsonArrayNode(List<Object> list)
    {
        super();
        if(list == null) {
            this.list = new ArrayList<Object>();
        } else {
            this.list = list;
        }
    }

    
    ///////////////////////////////////
    // JsonNode interface

    @Override
    public Object getValue()
    {
        // ?????
        return list;
    }


    ///////////////////////////////////
    // JsonSerializable interface
    // Note: The default depth of AbstractJsonNodes is always 1.   

//    @Override
//    public String toJsonString(int indent)
//    {
//        // temporary
//        StringBuilder sb = new StringBuilder();
//        sb.append("[");
//        for(Object o : list) {
//            JsonNode node = (JsonNode) o;
//            sb.append(node.toJsonString());
//            sb.append(",");
//        }
//        if(sb.charAt(sb.length() - 1) == ',') {
//            sb.deleteCharAt(sb.length() - 1);
//        }
//        sb.append("]");
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

        writer.append("[").append(LB).append(INDX);
        Iterator<Object> it = list.iterator();
        while(it.hasNext()) {
            JsonNode node = (JsonNode) it.next();
            writer.write(node.toJsonString());

            if(it.hasNext()) {
                writer.append(",");
                if(lbAfterComma) {
                    writer.append(LB).append(INDX);
                } else {
                    writer.append(WS);
                }
            }
        }
        writer.append(LB).append(IND).append("]");
    }


    ///////////////////////////////////////////////////////
    // TBD: JsonCompatible interface..
    // ....

    // @Override
    public boolean isJsonStructureArray()
    {
        return true;
    }

    @Override
    public Object toJsonStructure(int depth) throws JsonBuilderException
    {
        // ????
        // return list; 
        
        List<Object> struct = new ArrayList<Object>();
        
        // TBD:
        // Traverse the list down to depth...
        struct = list;
        // ...
        
        return struct;
    }

    


    ///////////////////////////////////
    // JsonArray interface

    @Override
    public boolean hasChildren()
    {
        return ! list.isEmpty();
    }

    @Override
    public List<JsonNode> getChildren()
    {
        // ????
        List<JsonNode> children = new ArrayList<JsonNode>();
        for(Object o : list) {
            children.add( (JsonNode) o);
        }
        return children;
        
        // ????
//        return (List<JsonNode>) ((List<?>) list);
    }

    @Override
    public JsonNode getChildNode(int index)
    {
        JsonNode node = (JsonNode) this.get(index);
        return node;
    }

    @Override
    public void addChild(JsonNode child)
    {
        this.add((Object) child);
    }

    @Override
    public void addAllChildren(List<JsonNode> children)
    {
        // ????
        this.addAll(children);
    }

    
    ///////////////////////////////////
    // List<Object> interface

    
    @Override
    public int size()
    {
        return list.size();
    }

    @Override
    public boolean isEmpty()
    {
        return list.isEmpty();
    }

    @Override
    public boolean contains(Object o)
    {
        return list.contains(o);
    }

    @Override
    public Iterator<Object> iterator()
    {
        return list.iterator();
    }

    @Override
    public Object[] toArray()
    {
        return list.toArray();
    }

    @Override
    public <T> T[] toArray(T[] a)
    {
        return list.toArray(a);
    }

    @Override
    public boolean add(Object e)
    {
        return list.add(e);
    }

    @Override
    public boolean remove(Object o)
    {
        return list.remove(o);
    }

    @Override
    public boolean containsAll(Collection<?> c)
    {
        return list.containsAll(c);
    }

    @Override
    public boolean addAll(Collection<? extends Object> c)
    {
        return list.addAll(c);
    }

    @Override
    public boolean addAll(int index, Collection<? extends Object> c)
    {
        return list.addAll(index, c);
    }

    @Override
    public boolean removeAll(Collection<?> c)
    {
        return list.removeAll(c);
    }

    @Override
    public boolean retainAll(Collection<?> c)
    {
        return list.retainAll(c);
    }

    @Override
    public void clear()
    {
        list.clear();
    }

    @Override
    public Object get(int index)
    {
        return list.get(index);
    }

    @Override
    public Object set(int index, Object element)
    {
        return list.set(index, element);
    }

    @Override
    public void add(int index, Object element)
    {
        list.add(index, element);
    }

    @Override
    public Object remove(int index)
    {
        return list.remove(index);
    }

    @Override
    public int indexOf(Object o)
    {
        return list.indexOf(o);
    }

    @Override
    public int lastIndexOf(Object o)
    {
        return list.lastIndexOf(o);
    }

    @Override
    public ListIterator<Object> listIterator()
    {
        return list.listIterator();
    }

    @Override
    public ListIterator<Object> listIterator(int index)
    {
        return list.listIterator(index);
    }

    @Override
    public List<Object> subList(int fromIndex, int toIndex)
    {
        return list.subList(fromIndex, toIndex);
    }


    // For dubugging...    
    @Override
    public String toString()
    {
        return "AbstractJsonArray [list=" + list + "]";
    }
    

}
