package org.minijson.common;

import java.util.Arrays;
import java.util.Collection;
import java.util.concurrent.ArrayBlockingQueue;


/**
 * A "tail buffer". It keeps the finite number of objects that have been added last.
 * Implemented by a ring buffer.
 * TailArrayQueue can be used to keep the "last X objects that have been read" while reading an object stream. 
 * (Note: the implementation is not thread-safe.)
 */
public class TailArrayQueue<E> extends ArrayBlockingQueue<E>
{
    private static final long serialVersionUID = 1L;

    //    // temporary
//    private static final int MAX_BUFFER_SIZE = 4096;
    private static final int DEF_BUFFER_SIZE = 256;
//    private static final int MIN_BUFFER_SIZE = 8;
//    // ...

    // If offer fails (because the queue if full)
    //  then remove padding elements (rather than just one) from the queue to make room.
    private int padding = 0;

    public TailArrayQueue()
    {
        this(DEF_BUFFER_SIZE);
    }
    public TailArrayQueue(int capacity)
    {
        super(capacity);
        initPadding();
    }
    public TailArrayQueue(int capacity, boolean fair)
    {
        super(capacity, fair);
        initPadding();
    }
    public TailArrayQueue(int capacity, boolean fair, Collection<? extends E> c)
    {
        super(capacity, fair, c);
        initPadding();
    }
    
    private void initPadding()
    {
        // For now, we use about 20% of the capacity as padding.
        int cap = (int) ( remainingCapacity() * 0.8d );
        if(cap >= remainingCapacity()) {
            padding = 1;
        } else if(cap <= 1) {
            // padding = 0;    // padding cannot be less than 1. See the loop in offer().
            padding = 1;
        } else {
            padding = remainingCapacity() - cap; 
        }
        
    }

    
    // Note that our use cases are more amenable to using
    //     offer(), poll(), and peek().
    // Note: BlockQueue interface -
    //  add(), remove(), element() throws IllegalStatException.
    //  offer(), poll(), peek() returns false/null if the operation fails (because queue is empty/full). 

    @Override
    public boolean offer(E e)
    {
        boolean suc = super.offer(e);
        if(suc == false) {
            // Is there a better way?
            for(int i=0; i<padding; i++) {
                E h = super.poll();     // Remove #padding elements.
                if(h == null) {
                    break;
                }
            }
            // Retry
            suc = super.offer(e);
        }
        return suc;
    }

    @Override
    public E poll()
    {
        return super.poll();
    }

    @Override
    public E peek()
    {
        return super.peek();
    }

    @Override
    public void clear()
    {
        super.clear();
    }

    // ???
    public void reset()
    {
        clear();
    }

    
    // For debugging/tracing
    public String toTraceString()
    {
        // temporary
        return "[" + Arrays.toString(toArray()) + "]";
    }
    
    @Override
    public String toString()
    {
        return "TailArrayQueue [padding=" + padding + ", size()=" + size()
                + ", remainingCapacity()=" + remainingCapacity()
                + ", toArray()=" + Arrays.toString(toArray()) + "]";
    }


    
}
