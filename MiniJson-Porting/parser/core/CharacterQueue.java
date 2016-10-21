package org.minijson.parser.core;

import java.util.ArrayList;
import java.util.Collection;
import java.util.Iterator;
import java.util.LinkedList;
import java.util.List;
import java.util.Queue;


// Not being used.
// Use CharQueue instead...

/**
 * The goal of using this class is to reduce the memory usage while reading the input stream.
 * However, due to the Java language limitation (see the comment below wrt convertToList()),
 * use of this "queue" can make parsing rather inefficient.
 * We need a "char queue" which manipulates char[] not Character collection....
 */
public class CharacterQueue implements Queue<Character>
{
    // The actual implementation is delegated to LinkedList.
    private final Queue<Character> queue;

    
    public CharacterQueue()
    {
        queue = new LinkedList<Character>();
    }

    
    @Override
    public int size()
    {
        return queue.size();
    }

    @Override
    public boolean isEmpty()
    {
        return queue.isEmpty();
    }

    @Override
    public boolean contains(Object o)
    {
        return queue.contains(o);
    }

    @Override
    public Iterator<Character> iterator()
    {
        return queue.iterator();
    }

    @Override
    public Object[] toArray()
    {
        return queue.toArray();
    }

    @Override
    public <T> T[] toArray(T[] a)
    {
        return queue.toArray(a);
    }

    @Override
    public boolean remove(Object o)
    {
        return queue.remove(o);
    }

    @Override
    public boolean containsAll(Collection<?> c)
    {
        return queue.containsAll(c);
    }

    @Override
    public boolean addAll(Collection<? extends Character> c)
    {
        return queue.addAll(c);
    }
    // See the comment below wrt convertToList().
    public boolean addAll(char[] buff) 
    {
        if(buff == null) {
            return false;
        }
        List<Character> list = convertToList(buff);
        return addAll(list);        
    }
    public boolean addAll(char[] buff, int size) 
    {
        if(buff == null || size <= 0) {   // Note: "<="
            return false;
        }
        List<Character> list = convertToList(buff, size);
        return addAll(list);        
    }

    @Override
    public boolean removeAll(Collection<?> c)
    {
        return queue.removeAll(c);
    }

    @Override
    public boolean retainAll(Collection<?> c)
    {
        return queue.retainAll(c);
    }

    @Override
    public void clear()
    {
        queue.clear();
    }

    @Override
    public boolean add(Character e)
    {
        return queue.add(e);
    }

    @Override
    public boolean offer(Character e)
    {
        return queue.offer(e);
    }

    @Override
    public Character remove()
    {
        return queue.remove();
    }

    @Override
    public Character poll()
    {
        return queue.poll();
    }

    @Override
    public Character element()
    {
        return queue.element();
    }

    @Override
    public Character peek()
    {
        return queue.peek();
    }


    // Java has this limitation in which 
    //   converting between collection types and arrays of primitive types
    //   is not generally possible without going through element by element conversion
    //   (which is O(n) operation).
    // With all this auto-boxing and generics, unfortunately, this is one of the "fatal" limitations of Java.
    // This function returns a collection, particularly list, of Characters given char[]
    public static List<Character> convertToList(char[] buff)
    {
        if(buff == null) {
            return null;
        }
        return convertToList(buff, buff.length);
    }
    public static List<Character> convertToList(char[] buff, int size)
    {
        if(buff == null || size < 0) {   // Note: "<"
            return null;
        }
        List<Character> list = new ArrayList<Character>(size);
        for(int i=0; i<size; i++) {
            list.add(buff[i]);       // auto-boxing.
        }
        return list;        
    }


    // For debugging purposes..
    @Override
    public String toString()
    {
        return "CharacterQueue [queue=" + queue + "]";
    }

    
}
