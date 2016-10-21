package org.minijson.parser.core;


// Not being used. (and, implementation not tested/verified.)
// This can be useful for regular/linear array,
// but not suitable for "cyclic" array as used in CharQueue.
// Use CyclicCharArray for cyclic arrays.
// ...

/**
 * The purpose of this class is to implement "subarray" operation 
 *     that does not require the content copy (which requires new memory allocation).
 */
public final class CharArrayWrapper
{
    // temporary.
    // Note that the size of CharArrayWrapper should be at least twice that of CharQueue. ???
    private static final int MAX_BACKING_ARRAY_LENGTH = 20000000; // ????
    private static final int DEF_BACKING_ARRAY_LENGTH = 4096;
    private static final int MIN_BACKING_ARRAY_LENGTH = 16;

    // backing array spec.
    private final int maxLength;
    private final char[] backingArray;
    
    // attrs to "define a subarray"
    private int offset;
    private int length;
    private int limit;  // "cache" == offset + length

    // ????
    private CharArrayWrapper()
    {
        this(DEF_BACKING_ARRAY_LENGTH);
    }
    private CharArrayWrapper(int maxLength)
    {
        if(maxLength > MAX_BACKING_ARRAY_LENGTH) {
            this.maxLength = MAX_BACKING_ARRAY_LENGTH;
        } else if(maxLength < MIN_BACKING_ARRAY_LENGTH) {
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
        if(backingArray == null) {
            this.backingArray = new char[DEF_BACKING_ARRAY_LENGTH];            
        } else {
            this.backingArray = backingArray;
        }
        this.maxLength = this.backingArray.length;
        this.offset = 0;
        this.length = 0;
        resetLimit();
    }
    private void resetLimit()
    {
        this.limit = this.offset + this.length;
    }

    // Read only.
    public final int getMaxLength()
    {
        return maxLength;
    }


    //////////////////////////////////////////////////////////
    // Usage:
    // for(int i=caw.offset; i<caw.limit; i++) {
    //     char ch = caw.getChar(i);
    // }
    
 
    // Note setOffset() should be called before setLength(), in the current implementation,
    //     if both values need to be set.
    public int getOffset()
    {
        return offset;
    }
    public void setOffset(int offset)
    {
        if(offset < 0) {
            this.offset = 0;
        } else if(offset > maxLength - 1) {
            this.offset = maxLength - 1;
        } else {
            this.offset = offset;
        }
        if(this.offset + this.length > maxLength - 1) {
            this.length = (maxLength - 1) - this.offset;
        }
        resetLimit();
    }

    public int getLength()
    {
        return length;
    }
    public void setLength(int length)
    {
        if(length < 0) {
            this.length = 0;
        } else if(this.offset + length > maxLength - 1) {
            this.length = (maxLength - 1) - this.offset;
        } else {
            this.length = length;
        }
        resetLimit();
    }

    public void setOffsetAndLength(int offset, int length)
    {
        if(offset < 0) {
            this.offset = 0;
        } else if(offset > maxLength - 1) {
            this.offset = maxLength - 1;
        } else {
            this.offset = offset;
        }
        if(length < 0) {
            this.length = 0;
        } else if(this.offset + length > maxLength - 1) {
            this.length = (maxLength - 1) - this.offset;
        } else {
            this.length = length;
        }
        resetLimit();
    }
    
    public int getLimit()
    {
        return limit;
    }


    public final char[] getBackingArray()
    {
        return backingArray;
    }
   
    
    // offset <= index < offset + length
    
    public char getChar()
    {
        return getChar(this.offset);
    }
    public char getChar(int index)
    {
        return this.backingArray[index];
    }
    public char getCharBoundsCheck(int index)
    {
        if(index < this.offset || index >= this.offset + this.length) {
            throw new IndexOutOfBoundsException("Out of bound: index = " + index + ". offset = " + offset + "; length = " + length);
        }
        return this.backingArray[index];
    }
    public void setChar(char ch)
    {
        setChar(this.offset, ch);
    }
    public void setChar(int index, char ch)
    {
        this.backingArray[index] = ch;
    }
    public void setCharBoundsCheck(int index, char ch)
    {
        if(index < this.offset || index >= this.offset + this.length) {
            throw new IndexOutOfBoundsException("Out of bound: index = " + index + ". offset = " + offset + "; length = " + length);
        }
        this.backingArray[index] = ch;
    }
    public void setChars(char ... c)
    {
        setChars(this.offset, c);
    }
    public void setChars(int index, char ... c)
    {
        for(int i=0; i<c.length; i++) {
            this.backingArray[index] = c[i];
        }
    }
    public void setCharsBoundsCheck(int index, char ... c)
    {
        if(index < this.offset || index >= this.offset + this.length - c.length) {
            throw new IndexOutOfBoundsException("Out of bound: index = " + index + ". offset = " + offset + "; length = " + length);
        }
        for(int i=0; i<c.length; i++) {
            this.backingArray[index] = c[i];
        }
    }
    
    
    
}
