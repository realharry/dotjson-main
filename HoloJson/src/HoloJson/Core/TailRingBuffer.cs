using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoloJson.Core
{
    /// <summary>
    /// A "tail buffer". It keeps the finite number of objects that have been added last.
    /// Implemented by a ring buffer.
    /// TailRingBuffer can be used to keep the "last X objects that have been read" while reading an object stream. 
    /// (Note: the implementation is not thread-safe.)
    /// </summary>
    public class TailRingBuffer<T>
    {
        // Note: This is essentially a copy of (a generic version of) CharQueue.
        // For object types such as JsonToken and JsonNode, this is really not necessary.
        // We can just use ArrayBlockingQuue.

        // Note --> Now we have TailArrayBuffer...
        // ....


        //    // temporary
        //    private static final int MAX_BUFFER_SIZE = 4096;
        private const int DEF_BUFFER_SIZE = 256;
        //    private static final int MIN_BUFFER_SIZE = 8;
        //    // ...

        // Internal buffer.
        //JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        private readonly T[] buffer_Renamed;
        // Ring buffer size.
        private readonly int maxSize;

        // We need two indexes.
        // They run in the range [0, maxSize)
        // For example,, if the buffer look like this: [0---h----t------]
        //   then the data is contained in the range from h (inclusive) to t (exclusive).
        // If a new data is added at the tail, the head part can be erased.
        // A more typical example: [0---t*h-------], where * is the one slot that is not being used.
        // When t is incremented, h will be incremented by the same amount.
        private int tailPointer = 0;
        private int headPointer = 0;

        // ????
        private readonly System.Type thisClassType;

        public TailRingBuffer(System.Type thisClassType) : this(thisClassType, DEF_BUFFER_SIZE)
        {
        }
        public TailRingBuffer(System.Type thisClassType, int maxSize)
        {
            //        if(maxSize < MIN_BUFFER_SIZE) {
            //            this.maxSize = MIN_BUFFER_SIZE;
            //        } else if(maxSize > MAX_BUFFER_SIZE) {
            //            this.maxSize = MAX_BUFFER_SIZE;
            //        } else {
            //            this.maxSize = maxSize;
            //        }
            this.thisClassType = thisClassType;
            this.maxSize = maxSize;
            buffer_Renamed = createGenericArray(this.maxSize);
        }

        // temporary
        private T[] createGenericArray(int length)
        {
            T[] arr = (T[])Array.CreateInstance(thisClassType, length);
            return arr;
        }


        // Circular increment operator.
        // Note that the tail pointer increments when new data is added, but never decrements. 
        // The head pointer merely follows (is pushed by) the tail, and it cannot be moved indepedently.
        //    private void incrementTail()
        //    {
        //        incrementTail(1);
        //    }
        private void incrementTail()
        {
            bool oldIsFull = Full;
            ++tailPointer;
            // if(tailPointer == maxSize) {
            if (tailPointer >= maxSize) {
                tailPointer = 0;
            }
            if (oldIsFull) {
                ++headPointer;
                // if(headPointer == maxSize) {
                if (headPointer >= maxSize) {
                    headPointer = 0;
                }
            }
        }
        // 0 < delta < maxSize
        private void incrementTail(int delta)
        {
            int oldMargin = margin();
            tailPointer += delta;
            if (tailPointer >= maxSize) {
                tailPointer %= maxSize;
            }
            if (oldMargin < delta) {
                int push = delta - oldMargin;
                headPointer += push;
                if (headPointer >= maxSize) {
                    headPointer %= maxSize;
                }
            }
        }
        // Returns the index of the "last element" (just before the tailPointer).
        private int lastIndex()
        {
            return lastNthIndex(1);
        }
        // 0 < n < maxSize
        private int lastNthIndex(int n)
        {
            if (tailPointer >= n) {
                return tailPointer - n;
            } else {
                int x = n - tailPointer;
                return maxSize - x;
            }
        }

        // Returns the size of the "empty" slots.
        // (Not all empty slots are usable though...)
        // We use one slot as a collision buffering zone.
        public int margin()
        {
            // Note the -1.
            int margin;
            if (tailPointer < headPointer) {
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
            if (tailPointer < headPointer) {
                size = maxSize + tailPointer - headPointer;
            } else {
                size = tailPointer - headPointer;
            }
            return size;
        }

        // Returns true if there is no data in the buffer.
        public bool Empty
        {
            get
            {
                if (tailPointer == headPointer) {
                    return true;
                } else {
                    return false;
                }
            }
        }
        // Returns true if margin() == 0.
        public bool Full
        {
            get
            {
                switch (headPointer) {
                    case 0:
                        // return (tailPointer == maxSize - 2);
                        return (tailPointer >= maxSize - 2);
                    case 1:
                        // return (tailPointer == maxSize - 1);
                        return (tailPointer >= maxSize - 1);
                    default:
                        // return (headPointer - tailPointer == 2);
                        return (headPointer - tailPointer <= 2);
                }
            }
        }

        // Adds the given object to the ring buffer.
        // If the buffer is full, then it returns false.
        public bool push(T ch)
        {
            buffer_Renamed[tailPointer] = ch;
            incrementTail();
            return true;
        }
        public bool push(T[] c)
        {
            if (c == null || c.Length == 0) {
                return false;
            }
            int len = c.Length;
            return push(c, len);
        }
        // Adds the object array c to the buffer, up to length, but no more than the c.size().
        public bool push(T[] c, int length)
        {
            if (c == null || c.Length == 0) {
                return false;
            }
            int len = c.Length;
            if (len < length) {
                length = len;
            }
            if (tailPointer + length < maxSize) {
                Array.Copy(c, 0, buffer_Renamed, tailPointer, length);
            } else {
                int first = maxSize - tailPointer;
                int second = length - first;
                Array.Copy(c, 0, buffer_Renamed, tailPointer, first);
                Array.Copy(c, first, buffer_Renamed, 0, second);
            }
            incrementTail(length);
            return true;
        }

        // tail() returns the object at the end of the data buffer.
        public T tail()
        {
            if (Empty) {
                return default(T);
            }
            T ch = buffer_Renamed[lastIndex()];
            return ch;
        }
        // Peeks the objects at the tail part of the buffer.
        // If the buffer contains less than length objects, it returns all objects (same as toArray()).
        public T[] tail(int length)
        {
            if (Empty) {
                return null;
                // return createGenericArray(0);
            }
            if (length > size()) {
                length = size();
            }
            T[] tail = createGenericArray(length);
            int begin = lastNthIndex(length);
            if (begin + length < maxSize) {
                Array.Copy(buffer_Renamed, begin, tail, 0, length);
            } else {
                int first = maxSize - begin;
                int second = length - first;
                Array.Copy(buffer_Renamed, begin, tail, 0, first);
                Array.Copy(buffer_Renamed, 0, tail, first, second);
            }
            return tail;
        }
        public string tailAsString(int length)
        {
            T[] c = tail(length);
            string t = arrayAsString(c);
            return t;
        }

        // Returns a copy of the entire buffer, as a regular array. Same as toArray().
        public T[] buffer()
        {
            return toArray();
        }
        public string bufferAsString()
        {
            T[] c = buffer();
            string t = arrayAsString(c);
            return t;
        }

        // Returns the copy of the entire data buffer, as a regular array.
        public T[] toArray()
        {
            if (Empty) {
                return null;
                // return createGenericArray(0);
            }
            int length = size();
            T[] copied = createGenericArray(length);
            if (headPointer + length < maxSize) {
                Array.Copy(buffer_Renamed, headPointer, copied, 0, length);
            } else {
                int first = maxSize - headPointer;
                int second = length - first;
                Array.Copy(buffer_Renamed, headPointer, copied, 0, first);
                Array.Copy(buffer_Renamed, 0, copied, first, second);
            }
            return copied;
        }

        // Removes the data from the buffer.
        public void clear()
        {
            //        headPointer = tailPointer;
            headPointer = tailPointer = 0;
        }

        private string arrayAsString(T[] arr)
        {
            if (arr == null) {
                return null;
            }
            if (arr.Length == 0) {
                return "";
            }
            StringBuilder sb = new StringBuilder();
            string comma = "";
            foreach (T a in arr) {
                sb.Append(comma).Append(a.ToString());
                comma = ",";
            }

            return sb.ToString();
        }

        // For debugging...
        public override string ToString()
        {
            // tbd:
            // return "TailRingBuffer [buffer=" + Arrays.ToString(tail(100)) + ", maxSize=" + maxSize + ", tailPointer=" + tailPointer + ", headPointer=" + headPointer + "]";
            return "TailRingBuffer [buffer=" + "..." + ", maxSize=" + maxSize + ", tailPointer=" + tailPointer + ", headPointer=" + headPointer + "]";
        }

    }

}