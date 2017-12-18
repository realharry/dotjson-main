using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotJson.Parser.Core
{
    /// <summary>
    /// A "tail buffer". It keeps the finite number of elements that have been added last.
    /// Implemented by a ring buffer.
    /// CharBuffer can be used to keep the "last X characters that have been read" while reading a character stream. 
    /// (Note: the implementation is not thread-safe.)
    /// </summary>
    public sealed class CharBuffer
    {
        // temporary
        private const int MAX_BUFFER_SIZE = 4096;
        private const int DEF_BUFFER_SIZE = 256;
        private const int MIN_BUFFER_SIZE = 8;
        // ...

        // Internal buffer.
        private readonly char[] buffer;
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

        public CharBuffer() : this(DEF_BUFFER_SIZE)
        {
        }
        public CharBuffer(int maxSize)
        {
            if (maxSize < MIN_BUFFER_SIZE) {
                this.maxSize = MIN_BUFFER_SIZE;
            } else if (maxSize > MAX_BUFFER_SIZE) {
                this.maxSize = MAX_BUFFER_SIZE;
            } else {
                this.maxSize = maxSize;
            }
            buffer = new char[this.maxSize];
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
            int oldMargin = Margin;
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
        private int LastIndex()
        {
            return LastNthIndex(1);
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

        // Returns the size of the "empty" slots.
        // (Not all empty slots are usable though...)
        // We use one slot as a collision buffering zone.
        public int Margin
        {
            get
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
        }

        // Because of the one empty slot buffering,
        // the "usable size" is maxSize - 1.
        public uint Capacity
        {
            get
            {
                return (uint)(maxSize - 1);
            }
        }

        // Returns the size of the data.
        public int Size
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
        public bool IsFull
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

        // Adds the given char to the ring buffer.
        // If the buffer is full, then it returns false.
        public bool Push(char ch)
        {
            buffer[tailPointer] = ch;
            IncrementTail();
            return true;
        }
        public bool Push(char[] c)
        {
            if (c == null || c.Length == 0) {
                return false;
            }
            int len = c.Length;
            return Push(c, len);
        }
        // Adds the char array c to the buffer, up to length, but no more than the c.size().
        public bool Push(char[] c, int length)
        {
            if (c == null || c.Length == 0) {
                return false;
            }
            int len = c.Length;
            if (len < length) {
                length = len;
            }
            if (length > maxSize) {
                // c is larger than buffer!!
                //if (log.isLoggable(Level.INFO)) {
                //    log.info("Input char array is bigger than the buffer size. Input length: " + length + ". Resetting the length: " + maxSize);
                //}
                length = maxSize;
            }

            if (tailPointer + length < maxSize) {
                Array.Copy(c, 0, buffer, tailPointer, length);
            } else {
                int first = maxSize - tailPointer;
                int second = length - first;
                // log.warning(">>>>>>>>>>>>>>>>>> length = " + length + "; first = " + first + "; second = " + second);

                Array.Copy(c, 0, buffer, tailPointer, first);
                Array.Copy(c, first, buffer, 0, second);
            }
            IncrementTail(length);
            return true;
        }

        // tail() returns the char at the end of the data buffer.
        public char Tail()
        {
            if (IsEmpty) {
                return (char) 0;
            }
            char ch = buffer[LastIndex()];
            return ch;
        }
        // Peeks the chars at the tail part of the buffer.
        // If the buffer contains less than length chars, it returns all chars (same as toArray()).
        public char[] Tail(int length)
        {
            if (IsEmpty) {
                // return null;
                return new char[] { };
            }
            if (length > Size) {
                length = Size;
            }
            char[] tail = new char[length];
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
            char[] c = Tail(length);
            string t = new string(c);
            return t;
        }

        // Returns a copy of the entire buffer, as a regular array. Same as toArray().
        public char[] Buffer
        {
            get
            {
                return ToArray();
            }
        }
        public string GetBufferAsString()
        {
            char[] c = Buffer;
            string t = new string(c);
            return t;
        }

        // Returns the copy of the entire data buffer, as a regular array.
        public char[] ToArray()
        {
            if (IsEmpty) {
                // return null;
                return new char[] { };
            }
            int length = Size;
            char[] copied = new char[length];
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
            //        headPointer = tailPointer;
            headPointer = tailPointer = 0;
        }

        // For debugging...
        public override string ToString()
        {
            return "CharBuffer [buffer=" + string.Join<char>(",", Tail(100)) + ", maxSize=" + maxSize + ", tailPointer=" + tailPointer + ", headPointer=" + headPointer + "]";
        }

    }

}