package org.minijson.parser.core;

import java.util.Arrays;
import java.util.logging.Logger;

import org.minijson.common.CyclicCharArray;


/**
 * "Ring buffer" implementation. It's a FIFO queue.
 * The reason for using CharQueue is to reduce the memory footprint while reading the input stream.
 * We could have used Queue<Character>, but using the primitive type, char, should be better for performance. 
 * (Note: the implementation is not thread-safe.)
 */
public final class CharQueue
{
    private static final Logger log = Logger.getLogger(CharQueue.class.getName());

    // temporary
    // private static final int MAX_BUFFER_SIZE = 4096;
    private static final int MAX_BUFFER_SIZE = 10000000;    // ????
    private static final int DEF_BUFFER_SIZE = 2048;
    // private static final int MIN_BUFFER_SIZE = 512;
    private static final int MIN_BUFFER_SIZE = 8;      // ????
    // ...

    // Internal buffer.
    private final char[] buffer;
    // Ring buffer size.
    private final int maxSize;
    
    // We need two indexes.
    // They run in the range [0, maxSize)
    //   e.g., head/tail pointers==0 could mean an empty buffer or full buffer (although we never allow full buffer).
    // For example,, if the buffer look like this: [0---h----t------]
    //   then the data is contained in the range from h (inclusive) to t (exclusive).
    private int tailPointer = 0;
    private int headPointer = 0;

    // Reusing one same object.
    // (note: the implication of using one object is that
    //     the caller cannot call more than one methods that return CyclicCharArray simultaneously
    //     (e.g., in a nested loop, etc.)
    // --> 
    // TBD: This is not really necessary.
    //   Maybe just use multiple instances of CyclicCharArray with the same backedArray, buffer.
    // private final CyclicCharArray cCharArray;

    public CharQueue()
    {
        this(DEF_BUFFER_SIZE);
    }
    public CharQueue(int maxSize)
    {
        if(maxSize < MIN_BUFFER_SIZE) {
            this.maxSize = MIN_BUFFER_SIZE;
        } else if(maxSize > MAX_BUFFER_SIZE) {
            this.maxSize = MAX_BUFFER_SIZE;
        } else {
            this.maxSize = maxSize;
        }
        buffer = new char[this.maxSize];
        // Note that the size of charArray should be at least twice that of buffer
        //    because the whole point of using charArray is to avoid the complexity of the cyclic data in buffer.
        // int arraySize = this.maxSize * 2;
        // Note the comment above.
        // cCharArray = new CyclicCharArray(buffer);
    }
    
    
    // Circular increment operator.
    private void incrementHead()
    {
        ++headPointer;
        // if(headPointer == maxSize) {
        if(headPointer >= maxSize) {
            headPointer = 0;
        }
    }
    // 0 < delta < maxSize
    private void incrementHead(int delta)
    {
        headPointer += delta;
        if(headPointer >= maxSize) {
            headPointer %= maxSize;
        }
    }
//    // Cannot decrement head pointer.
//    private void decrementHead()
//    {        
//    }
    // Note that tail pointer progresses only when new data is added. 
    private void incrementTail()
    {
        ++tailPointer;
        // if(tailPointer == maxSize) {
        if(tailPointer >= maxSize) {
            tailPointer = 0;
        }
    }
    // 0 < delta < maxSize
    private void incrementTail(int delta)
    {
        tailPointer += delta;
        if(tailPointer >= maxSize) {
            tailPointer %= maxSize;
        }
    }
//    // Cannot decrement tail pointer either.
//    private void decrementTail()
//    {
//    }
    
    
    // Returns the size of the "empty" slots.
    // (Not all empty slots are usable though...)
    // We use one slot as a collision buffering zone.
    public int margin()
    {
        // Note the -1.
        int margin;
        if(tailPointer < headPointer) {
            margin = headPointer - tailPointer - 1;
        } else {
            margin = maxSize - (tailPointer - headPointer) - 1;
        }
        return margin;
    }

    // Because of the one empty slot buffering,
    // the "usable size" is maxSize - 1.
    public int maxCapacity()
    {
        return this.maxSize - 1;
    }

    // Returns the size of the data.
    public int size()
    {
        int size;
        if(tailPointer < headPointer) {
            size = maxSize + tailPointer - headPointer;
        } else {
            size = tailPointer - headPointer;
        }
        return size;
    }

    // Returns true if there is no data in the buffer.
    public boolean isEmpty()
    {
        return (tailPointer == headPointer);
//        if(tailPointer == headPointer) {
//            return true;
//        } else {
//            return false;
//        }
    }

    // Adds the given char to the ring buffer.
    // If the buffer is full, then it returns false.
    public boolean add(char ch)
    {
        if(margin() == 0) {
            return false;
        } else {
            buffer[tailPointer] = ch;
            incrementTail();
            return true;
        }
    }
    public boolean addAll(char[] c)
    {
        if(c == null || c.length == 0) {
            return false;
        }
        int len = c.length;
        return addAll(c, len);
    }
    // Adds the char array c to the buffer, up to length, but no more than the c.size().
    public boolean addAll(char[] c, int length)
    {
        if(c == null || c.length == 0) {
            return false;
        }
        int len = c.length;
        if(len < length) {
            length = len;
        }
        if(margin() < length) {
            return false;
        } else {
            if(tailPointer + length < maxSize) {
                System.arraycopy(c, 0, buffer, tailPointer, length);
            } else {
                int first = maxSize - tailPointer;
                int second = length - first;
                System.arraycopy(c, 0, buffer, tailPointer, first);
                System.arraycopy(c, first, buffer, 0, second);
            }
            incrementTail(length);
            return true;
        }
    }
    
    
    /////////////////////////////////////////////////////////
    // Note that CharQueue is primarily "read-only"
    //      in the sense that the caller never modifies the peek'ed/next'ed char or char[].
    //      Internal buffer really only changes during add/addAll.
    // Hence we can optimize the following methods in various ways...
    // ......

    
//    public boolean offer(char ch)
//    {
//        return add(ch);
//    }

    // Poll() pops the char at the head pointer.
    public char poll()
    {
        if(isEmpty()) {
            return 0;
        }
        char ch = buffer[headPointer];
        incrementHead();
        return ch;
    }
    // Returns the char array at the head up to length.
    // If the buffer contains less than length chars, it returns all data.
    public char[] poll(int length)
    {
        if(isEmpty()) {
            // return null;
            return new char[]{};
        }
        if(length > size()) {
            length = size();
        }
        char[] polled = new char[length];
        if(headPointer + length < maxSize) {
            
            
            System.arraycopy(buffer, headPointer, polled, 0, length);
            
            // This does not work
            // log.warning(">>>>>>>>>>>>>>>>>>>>>> length = " + length);
            // java.nio.CharBuffer cb = java.nio.CharBuffer.wrap(buffer, headPointer, length);
            // polled = cb.array();
            // log.warning(">>>>>>>>>>>>>>>>>>>>>> polled.length = " + polled.length);
            // log.warning(">>>>>>>>>>>>>>>>>>>>>> polled = " + Arrays.toString(polled));
            
        } else {
            // Note the arraycopy does memcopy/memomove.
            //   Need a more efficient way to do this? (e.g., by returning a "ref" not copy???)
            int first = maxSize - headPointer;
            int second = length - first;
            System.arraycopy(buffer, headPointer, polled, 0, first);
            System.arraycopy(buffer, 0, polled, first, second);
        }
        incrementHead(length);
        return polled;
    }

    public CyclicCharArray pollBuffer(int length)
    {
        if(isEmpty()) {
            return null;
        }
        if(length > size()) {
            length = size();
        }
        // cCharArray.setOffsetAndLength(headPointer, length);
        CyclicCharArray charArray = CyclicCharArray.wrap(buffer, headPointer, length);
        incrementHead(length);
        return charArray;
    }

    
    // Pops one char from the buffer, and removes it.
    // If buffer is empty, it's ignored.
    public void skip()
    {
        if(! isEmpty()) {
            incrementHead();
        }
    }
    // "skips" up to length chars.
    public void skip(int length)
    {
        if(! isEmpty()) {
            if(length > size()) {
                length = size();
            }
            incrementHead(length);
        }
    }


    // Peek() returns the char at the head pointer, without popping the entry.
    // Note that peek() methods are idempotent.
    public char peek()
    {
        if(isEmpty()) {
            return 0;
        }
        char ch = buffer[headPointer];
        return ch;
    }
    // Peeks the chars at the head up to length.
    // If the buffer contains less than length chars, it just returns all chars,
    //     without removing those entries from the data buffer. 
    public char[] peek(int length)
    {
        if(isEmpty()) {
            // return null;
            return new char[]{};
        }
        if(length > size()) {
            length = size();
        }
        char[] peeked = new char[length];
        if(headPointer + length < maxSize) {
            System.arraycopy(buffer, headPointer, peeked, 0, length);
        } else {
            // Note the arraycopy does memcopy/memomove.
            //   Need a more efficient way to do this? (e.g., by returning a "ref" not copy???)
            int first = maxSize - headPointer;
            int second = length - first;
            System.arraycopy(buffer, headPointer, peeked, 0, first);
            System.arraycopy(buffer, 0, peeked, first, second);
        }
        return peeked;
    }
    
    public CyclicCharArray peekBuffer(int length)
    {
        if(isEmpty()) {
            return null;
        }
        if(length > size()) {
            length = size();
        }
        // cCharArray.setOffsetAndLength(headPointer, length);
        CyclicCharArray charArray = CyclicCharArray.wrap(buffer, headPointer, length);
        return charArray;
    }
    public CyclicCharArray peekBuffer(int offset, int length)
    {
        if(isEmpty()) {
            return null;
        }
        if(offset < 0 || offset >= size()) {
            return null;
        }
        if(length > size() - offset) {
            length = size() - offset;
        }
        // cCharArray.setOffsetAndLength(headPointer + offset, length);
        CyclicCharArray charArray = CyclicCharArray.wrap(buffer, headPointer + offset, length);
        return charArray;
    }


//    public boolean contains(char ch)
//    {
//        return false;
//    }

//    public Iterator<Character> iterator()
//    {
//        return null;
//    }


    // Returns the copy of the data buffer, as a regular array.
    public char[] toArray()
    {
        if(isEmpty()) {
            // return null;
            return new char[]{};
        }
        int length = size();
        char[] copied = new char[length];
        if(headPointer + length < maxSize) {
            System.arraycopy(buffer, headPointer, copied, 0, length);
        } else {
            // Note the arraycopy does memcopy/memomove.
            //   Need a more efficient way to do this? (e.g., by returning a "ref" not copy???)
            int first = maxSize - headPointer;
            int second = length - first;
            System.arraycopy(buffer, headPointer, copied, 0, first);
            System.arraycopy(buffer, 0, copied, first, second);
        }
        return copied;
    }

    // Remove the data from the buffer.
    public void clear()
    {
//        headPointer = tailPointer;
        headPointer = tailPointer = 0;
    }

    
    // For debugging...
    @Override
    public String toString()
    {
        return "CharQueue [buffer=" + Arrays.toString(peek(100)) + ", maxSize="
                + maxSize + ", tailPointer=" + tailPointer + ", headPointer="
                + headPointer + "]";
    }
    
}
