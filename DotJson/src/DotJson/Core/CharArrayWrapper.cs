using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotJson.Core
{
    // Not being used. (and, implementation not tested/verified.)
    // This can be useful for regular/linear array,
    // but not suitable for "cyclic" array as used in CharQueue.
    // Use CyclicCharArray for cyclic arrays.
    // ...

    /// <summary>
    /// The purpose of this class is to implement "subarray" operation 
    ///     that does not require the content copy (which requires new memory allocation).
    /// </summary>
    public sealed class CharArrayWrapper
    {
        // temporary.
        // Note that the size of CharArrayWrapper should be at least twice that of CharQueue. ???
        private const int MAX_BACKING_ARRAY_LENGTH = 20000000; // ????
        private const int DEF_BACKING_ARRAY_LENGTH = 4096;
        private const int MIN_BACKING_ARRAY_LENGTH = 16;

        // backing array spec.
        private readonly int maxLength;
        private readonly char[] backingArray;

        // attrs to "define a subarray"
        private int offset;
        private int length;
        private int limit; // "cache" == offset + length

        // ????
        private CharArrayWrapper() : this(DEF_BACKING_ARRAY_LENGTH)
        {
        }
        private CharArrayWrapper(int maxLength)
        {
            if (maxLength > MAX_BACKING_ARRAY_LENGTH) {
                this.maxLength = MAX_BACKING_ARRAY_LENGTH;
            } else if (maxLength < MIN_BACKING_ARRAY_LENGTH) {
                this.maxLength = MIN_BACKING_ARRAY_LENGTH;
            } else {
                this.maxLength = maxLength;
            }
            this.backingArray = new char[this.maxLength];
            this.offset = 0;
            this.length = 0;
            resetLimit();
        }
        public CharArrayWrapper(char[] backingArray)
        {
            if (backingArray == null) {
                this.backingArray = new char[DEF_BACKING_ARRAY_LENGTH];
            } else {
                this.backingArray = backingArray;
            }
            this.maxLength = this.backingArray.Length;
            this.offset = 0;
            this.length = 0;
            resetLimit();
        }
        private void resetLimit()
        {
            this.limit = this.offset + this.length;
        }

        // Read only.
        public int MaxLength
        {
            get
            {
                return maxLength;
            }
        }


        //////////////////////////////////////////////////////////
        // Usage:
        // for(int i=caw.offset; i<caw.limit; i++) {
        //     char ch = caw.getChar(i);
        // }


        // Note setOffset() should be called before setLength(), in the current implementation,
        //     if both values need to be set.
        public int Offset
        {
            get
            {
                return offset;
            }
            set
            {
                if (value < 0) {
                    this.offset = 0;
                } else if (value > maxLength - 1) {
                    this.offset = maxLength - 1;
                } else {
                    this.offset = value;
                }
                if (this.offset + this.length > maxLength - 1) {
                    this.length = (maxLength - 1) - this.offset;
                }
                resetLimit();
            }
        }

        public int Length
        {
            get
            {
                return length;
            }
            set
            {
                if (value < 0) {
                    this.length = 0;
                } else if (this.offset + value > maxLength - 1) {
                    this.length = (maxLength - 1) - this.offset;
                } else {
                    this.length = value;
                }
                resetLimit();
            }
        }

        public void setOffsetAndLength(int offset, int length)
        {
            if (offset < 0) {
                this.offset = 0;
            } else if (offset > maxLength - 1) {
                this.offset = maxLength - 1;
            } else {
                this.offset = offset;
            }
            if (length < 0) {
                this.length = 0;
            } else if (this.offset + length > maxLength - 1) {
                this.length = (maxLength - 1) - this.offset;
            } else {
                this.length = length;
            }
            resetLimit();
        }

        public int Limit
        {
            get
            {
                return limit;
            }
        }


        public char[] BackingArray
        {
            get
            {
                return backingArray;
            }
        }


        // offset <= index < offset + length

        public char Char
        {
            get
            {
                return getChar(this.offset);
            }
            set
            {
                setChar(this.offset, value);
            }
        }
        public char getChar(int index)
        {
            return this.backingArray[index];
        }
        public char getCharBoundsCheck(int index)
        {
            if (index < this.offset || index >= this.offset + this.length) {
                throw new System.IndexOutOfRangeException("Out of bound: index = " + index + ". offset = " + offset + "; length = " + length);
            }
            return this.backingArray[index];
        }
        public void setChar(int index, char ch)
        {
            this.backingArray[index] = ch;
        }
        public void setCharBoundsCheck(int index, char ch)
        {
            if (index < this.offset || index >= this.offset + this.length) {
                throw new System.IndexOutOfRangeException("Out of bound: index = " + index + ". offset = " + offset + "; length = " + length);
            }
            this.backingArray[index] = ch;
        }
        public char[] Chars
        {
            set
            {
                setChars(this.offset, value);
            }
        }
        public void setChars(int index, params char[] c)
        {
            for (int i = 0; i < c.Length; i++) {
                this.backingArray[index] = c[i];
            }
        }
        public void setCharsBoundsCheck(int index, params char[] c)
        {
            if (index < this.offset || index >= this.offset + this.length - c.Length) {
                throw new System.IndexOutOfRangeException("Out of bound: index = " + index + ". offset = " + offset + "; length = " + length);
            }
            for (int i = 0; i < c.Length; i++) {
                this.backingArray[index] = c[i];
            }
        }



    }

}
