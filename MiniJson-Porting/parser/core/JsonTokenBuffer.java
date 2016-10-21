package org.minijson.parser.core;

import java.util.Collection;
import java.util.Iterator;

import org.minijson.common.TailArrayQueue;
import static org.minijson.parser.core.TokenTypes.NUMBER;
import static org.minijson.parser.core.TokenTypes.STRING;


/**
 * A "tail buffer". It keeps the finite number of objects that have been added last.
 * Implemented by a ring buffer.
 * JsonTokenBuffer can be used to keep the "last X objects that have been read" while reading an object stream. 
 * (Note: the implementation is not thread-safe.)
 */
public final class JsonTokenBuffer extends TailArrayQueue<JsonToken>
{
    private static final long serialVersionUID = 1L;

    //    // temporary
//    private static final int MAX_BUFFER_SIZE = 4096;
    private static final int DEF_BUFFER_SIZE = 64;
//    private static final int MIN_BUFFER_SIZE = 8;
//    // ...


    public JsonTokenBuffer()
    {
        this(DEF_BUFFER_SIZE);
    }
    public JsonTokenBuffer(int capacity)
    {
        super(capacity);
    }
    public JsonTokenBuffer(int capacity, boolean fair)
    {
        super(capacity, fair);
    }
    public JsonTokenBuffer(int capacity, boolean fair, Collection<? extends JsonToken> c)
    {
        super(capacity, fair, c);
    }

    
    // temporary
    @Override
    public String toTraceString()
    {
//        JsonToken[] tokens = toArray(new JsonToken[]{});
//        return Arrays.toString(tokens);
        
        StringBuilder sb = new StringBuilder();
        sb.append("((Processed Tokens: ...");
        Iterator<JsonToken> it = iterator();
        while(it.hasNext()) {
            JsonToken token = it.next();
            int type = token.getType();
            
            
//            switch(type) {
//            case EOF:
//            case NULL:
//            case COMMA:
//            case COLON:
//            case LSQUARE:
//            case RSQUARE:
//            case LCURLY:
//            case RCURLY:
//                sb.append(TokenTypes.getDisplayName(type)).append(" ");
//                break;
//            case BOOLEAN:
//            case NUMBER:
//            case STRING:
//                // ...
//                break;
//            default:
//                // ...
//            }
            
            sb.append("<").append(TokenTypes.getTokenName(type));
            if(type == NUMBER) {
                Object val = token.getValue();
                sb.append(":").append(val);
            } else if(type == STRING) {
                Object val = token.getValue();
                String str = (String) val;
                if(str.length() > 16) {
                    str = str.substring(0, 14) + "..";
                }
                sb.append(":").append(str);
            }
            sb.append(">, ");
        }
        sb.append("))");
        
        return sb.toString();
    }


    
}
