using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotJson.Core
{
    /// <summary>
    /// A "tail buffer". It keeps the finite number of objects that have been added last.
    /// Implemented by a ring buffer.
    /// TailRingBuffer can be used to keep the "last X objects that have been read" while reading an object stream. 
    /// (Note: the implementation is not thread-safe.)
    /// </summary>
    public class TailRingBuffer<T> : IEnumerable<T>
    {
        // Note: This is essentially a copy of (a generic version of) CharQueue.
        // For object types such as JsonToken and JsonNode, this is really not necessary.
        // We can just use Java ArrayBlockingQuue.
        // What's the C# equivalent of ArrayBlockingQuue ????

        // Note: 10/22/2016
        // Added "queue" operations. This is no longer just a "tail buffer".
        // ...

        //    // temporary
        //    private static final int MAX_BUFFER_SIZE = 4096;
        private const int DEF_BUFFER_SIZE = 256;
        //    private static final int MIN_BUFFER_SIZE = 8;
        //    // ...

        // Internal buffer.
        private readonly T[] buffer;
        // Ring buffer size.
        private readonly int maxSize;

        // We need two indexes.
        // They run in the range [0, maxSize)
        // For example,, if the buffer look like this: [0---h----t------]
        //   then the data is contained in the range from h (inclusive) to t (exclusive).
        // If a new data is added at the tail, the head part can be erased.
        // A more typical example: [0---t*h-------], where * is the one slot that is not being used.
        // When t is incremented, h will be incremented by the same amount. (the client should check this situation before inserting any more elements.)
        // Note that we cannot represent the "full full" state (there are maxSize elements in the queue) using these two pointers because of the ciruclar nature.
        // The maximum number of elements we can put into this queue is maxSize - 1 == capacity.
        private int tailPointer = 0;
        private int headPointer = 0;


        //public TailRingBuffer() : this(DEF_BUFFER_SIZE)
        //{
        //}
        //public TailRingBuffer(int maxSize)
        //{
        //    //        if(maxSize < MIN_BUFFER_SIZE) {
        //    //            this.maxSize = MIN_BUFFER_SIZE;
        //    //        } else if(maxSize > MAX_BUFFER_SIZE) {
        //    //            this.maxSize = MAX_BUFFER_SIZE;
        //    //        } else {
        //    //            this.maxSize = maxSize;
        //    //        }
        //    this.maxSize = maxSize;
        //    buffer = CreateGenericArray(this.maxSize);
        //}
        public TailRingBuffer() : this((uint) (DEF_BUFFER_SIZE - 1))
        {
        }
        public TailRingBuffer(uint capacity, IList<T> c = null)
        {
            maxSize = (int)(capacity + 1U);
            buffer = CreateGenericArray(maxSize);
            if(c != null) {
                buffer = CopyCollectionToArray(maxSize, c);
            }
        }

        // temporary
        private T[] CreateGenericArray(int length)
        {
            T[] arr = (T[])Array.CreateInstance(typeof(T), length);
            return arr;
        }
        private T[] CopyCollectionToArray(int maxSize, IList<T> c)
        {
            var len = Math.Min(c.Count, maxSize - 1);
            T[] arr = (T[])Array.CreateInstance(typeof(T), maxSize);
            for (var i=0;i<len;i++) {
                arr[i] = c[i];
            }
            return arr;
        }


        // Circular increment operator.
        // Note that the tail pointer increments when new data is added, but never decrements. 
        // The head pointer merely follows (is pushed by) the tail, and it cannot be moved indepedently.
        //    private void incrementTail()
        //    {
        //        incrementTail(1);
        //    }
        private void IncrementTail()
        {
            bool oldIsFull = IsFull;
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
        private void IncrementTail(int delta)
        {
            var oldMargin = Margin;
            tailPointer += delta;
            if (tailPointer >= maxSize) {
                tailPointer %= maxSize;
            }
            if (oldMargin < delta) {
                var push = delta - (int) oldMargin;
                headPointer += push;
                if (headPointer >= maxSize) {
                    headPointer %= maxSize;
                }
            }
        }

        // Returns the index of the "last element" (just before the tailPointer).
        private int LastIndex
        {
            get
            {
                return LastNthIndex(1);
            }
        }
        // 0 < n < maxSize
        private int LastNthIndex(int n)
        {
            if (tailPointer >= n) {
                return tailPointer - n;
            } else {
                int x = n - tailPointer;
                return maxSize - x;
            }
        }

        private void IncrementHead()
        {
            if (headPointer == tailPointer) {
                // do nothing
            } else {
                ++headPointer;
                if (headPointer >= maxSize) {
                    headPointer = 0;
                }
            }
        }

        // Returns the index of the "head".
        private int FirstIndex
        {
            get
            {
                return headPointer;
            }
        }


        public IEnumerator<T> GetEnumerator()
        {
            var unfoldedTail = tailPointer;
            if(tailPointer < headPointer) {
                unfoldedTail = tailPointer + maxSize;
            }
            for(var i=headPointer;i<unfoldedTail;i++) {
                var foldedIdx = i % maxSize;
                yield return buffer[foldedIdx];
            }
        }
        // ???
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }


        // Returns the size of the "empty" slots.
        // (Not all empty slots are usable though...)
        // We use one slot as a collision buffering zone.
        public uint Margin
        {
            get
            {
                // Note the -1.
                uint margin;
                if (tailPointer < headPointer) {
                    margin = (uint) (headPointer - tailPointer - 1);
                } else {
                    margin = (uint) (maxSize - (tailPointer - headPointer) - 1);
                }
                return margin;
            }
        }

        // Because of the one empty slot buffering,
        // the "usable size" is maxSize - 1.
        public uint Capacity
        {
            get
            {
                return (uint) (maxSize - 1);
            }
        }

        // Returns the size of the data.
        protected int Size
        {
            get
            {
                int size;
                if (tailPointer < headPointer) {
                    size = maxSize + tailPointer - headPointer;
                } else {
                    size = tailPointer - headPointer;
                }
                return size;
            }
        }

        // Returns true if there is no data in the buffer.
        public bool IsEmpty
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
        // that is, if there are (maxSize - 1) elements in the buffer.
        public bool IsFull
        {
            get
            {
                if(headPointer <= tailPointer) {
                    return (tailPointer - headPointer >= maxSize - 1);
                } else {
                    return (headPointer - tailPointer <= 1);
                }
            }
        }


        /// <summary>
        /// Adds the element at the end/tail of the buffer.
        /// </summary>
        /// <param name="ch">Element to be added</param>
        /// <returns>Returns true if successful.</returns>
        public bool Offer(T ch)
        {
            return Push(ch);
        }
        /// <summary>
        /// Retrieves and removes the head of this queue, or returns null if this queue is empty.
        /// </summary>
        /// <returns>Returns the head.</returns>
        public T Poll()
        {
            var ch = Head();
            IncrementHead();
            return ch;
        }
        /// <summary>
        /// Retrieves, but does not remove, the head of this queue, or returns null if this queue is empty.
        /// </summary>
        /// <returns></returns>
        public T Peek()
        {
            return Head();
        }


        // Adds the given object to the end of the ring buffer.
        // If the buffer is full, then it returns false.
        public bool Push(T ch)
        {
            buffer[tailPointer] = ch;
            IncrementTail();
            return true;
        }
        public bool Push(T[] c)
        {
            if (c == null || c.Length == 0) {
                return false;
            }
            var len = c.Length;
            return Push(c, len);
        }
        // Adds the object array c to the buffer, up to length, but no more than the c.size().
        public bool Push(T[] c, int length)
        {
            if (c == null || c.Length == 0) {
                return false;
            }
            int len = c.Length;
            if (len < length) {
                length = len;
            }
            if (tailPointer + length < maxSize) {
                Array.Copy(c, 0, buffer, tailPointer, length);
            } else {
                int first = maxSize - tailPointer;
                int second = length - first;
                Array.Copy(c, 0, buffer, tailPointer, first);
                Array.Copy(c, first, buffer, 0, second);
            }
            IncrementTail(length);
            return true;
        }

        // Head() returns the object at the start of the data buffer.
        public T Head()
        {
            if (IsEmpty) {
                return default(T);
            }
            T ch = buffer[FirstIndex];
            return ch;
        }

        // Tail() returns the object at the end of the data buffer.
        public T Tail()
        {
            if (IsEmpty) {
                return default(T);
            }
            T ch = buffer[LastIndex];
            return ch;
        }
        // Peeks the objects at the tail part of the buffer.
        // If the buffer contains less than length objects, it returns all objects (same as toArray()).
        public T[] Tail(int length)
        {
            if (IsEmpty) {
                return null;
                // return createGenericArray(0);
            }
            if (length > Size) {
                length = Size;
            }
            T[] tail = CreateGenericArray(length);
            int begin = LastNthIndex(length);
            if (begin + length < maxSize) {
                Array.Copy(buffer, begin, tail, 0, length);
            } else {
                int first = maxSize - begin;
                int second = length - first;
                Array.Copy(buffer, begin, tail, 0, first);
                Array.Copy(buffer, 0, tail, first, second);
            }
            return tail;
        }
        public string GetTailAsString(int length)
        {
            T[] c = Tail(length);
            string t = Arrays.ToString(c);
            return t;
        }

        // Returns a copy of the entire buffer, as a regular array. Same as toArray().
        public T[] Buffer
        {
            get
            {
                return ToArray();
            }
        }
        public string GetBufferAsString()
        {
            T[] c = Buffer;
            string t = Arrays.ToString(c);
            return t;
        }

        // Returns the copy of the entire data buffer, as a regular array.
        public T[] ToArray()
        {
            if (IsEmpty) {
                return null;
                // return createGenericArray(0);
            }
            int length = Size;
            T[] copied = CreateGenericArray(length);
            if (headPointer + length < maxSize) {
                Array.Copy(buffer, headPointer, copied, 0, length);
            } else {
                int first = maxSize - headPointer;
                int second = length - first;
                Array.Copy(buffer, headPointer, copied, 0, first);
                Array.Copy(buffer, 0, copied, first, second);
            }
            return copied;
        }

        // Removes the data from the buffer.
        public void Clear()
        {
            // headPointer = tailPointer;
            headPointer = tailPointer = 0;
        }
        // ???
        public virtual void Reset()
        {
            Clear();
        }


        // For debugging/tracing
        public virtual string ToTraceString()
        {
            var sb = new StringBuilder();
            sb.Append("[");
            var isFirst = true;
            var it = GetEnumerator();
            while (it.MoveNext()) {
                if (isFirst) {
                    isFirst = false;
                } else {
                    sb.Append(",");
                }
                var element = it.Current;
                sb.Append(element.ToString());
            }
            sb.Append("]");
            return sb.ToString();
        }
        public virtual string ToTraceString(int length)
        {
            var sb = new StringBuilder();
            sb.Append("[");
            var isFirst = true;
            var count = 0;
            var it = GetEnumerator();
            while (count < length && it.MoveNext()) {
                if(isFirst) {
                    isFirst = false;
                } else {
                    sb.Append(",");
                }
                var element = it.Current;
                sb.Append(element.ToString());
                ++count;
            }
            sb.Append("]");
            return sb.ToString();
        }

        //private static string ToString(T[] arr)
        //{
        //    if (arr == null) {
        //        return null;
        //    }
        //    if (arr.Length == 0) {
        //        return "";
        //    }
        //    StringBuilder sb = new StringBuilder();
        //    string comma = "";
        //    foreach (T a in arr) {
        //        sb.Append(comma).Append(a.ToString());
        //        comma = ",";
        //    }

        //    return sb.ToString();
        //}

        // For debugging...
        public override string ToString()
        {
            // return $"TailRingBuffer [maxSize={maxSize}, headPointer={headPointer}, tailPointer={tailPointer}, buffer={ToTraceString(100)}]";
            return $"TailRingBuffer [MaxCapacity={Capacity}, headPointer={headPointer}, tailPointer={tailPointer}, buffer={ToTraceString(100)}]";
        }

    }

}