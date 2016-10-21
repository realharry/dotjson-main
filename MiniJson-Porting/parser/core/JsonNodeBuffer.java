package org.minijson.parser.core;

import java.util.Collection;
import java.util.Iterator;

import org.minijson.common.TailArrayQueue;
import org.minijson.type.JsonNode;


/**
 * A "tail buffer". It keeps the finite number of objects that have been added last.
 * Implemented by a ring buffer.
 * JsonNodeBuffer can be used to keep the "last X objects that have been read" while reading an object stream. 
 * (Note: the implementation is not thread-safe.)
 */
public final class JsonNodeBuffer extends TailArrayQueue<Object>
{
    private static final long serialVersionUID = 1L;

    //    // temporary
//    private static final int MAX_BUFFER_SIZE = 4096;
    private static final int DEF_BUFFER_SIZE = 32;
//    private static final int MIN_BUFFER_SIZE = 8;
//    // ...


    public JsonNodeBuffer()
    {
        this(DEF_BUFFER_SIZE);
    }
    public JsonNodeBuffer(int capacity)
    {
        super(capacity);
    }
    public JsonNodeBuffer(int capacity, boolean fair)
    {
        super(capacity, fair);
    }
    public JsonNodeBuffer(int capacity, boolean fair, Collection<? extends JsonNode> c)
    {
        super(capacity, fair, c);
    }

    @Override
    public String toTraceString()
    {
//        JsonNode[] nodes = toArray(new JsonNode[]{});
//        return Arrays.toString(nodes);
        
        StringBuilder sb = new StringBuilder();
        sb.append("<<Processed Nodes: ...");
        Iterator<Object> it = iterator();
        while(it.hasNext()) {
            Object node = it.next();
            Object value = null;
            if(node instanceof JsonNode) {
                value = ((JsonNode) node).getValue();
            } else {
                value = node.toString();
            }
            String str = "";
            if(value != null) {
                str = value.toString();
                if(str.length() > 16) {
                    str = str.substring(0, 14) + "..";
                }
            }
            sb.append("(").append(str).append("), ");
        }
        sb.append(">>");
        
        return sb.toString();
    }


    
}
