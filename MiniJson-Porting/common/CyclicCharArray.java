package org.minijson.common;

import java.util.Arrays;
import java.util.logging.Logger;


/**
 * The purpose of this class is to implement "subarray" operation 
 *        that does not require the content copy (which requires new memory allocation).
 * Use CharArrayWrapper for "linear" or regular arrays.
 * This class is for wrapping "cyclic" arrays like CharQueue (ring buffer). 
 */
public final class CyclicCharArray
{
    private static final Logger log = Logger.getLogger(CyclicCharArray.class.getName());

    // backing array spec.
    private final int arrayLength;
    private final char[] backingArray;
    
    // attrs to "define a subarray"
    private int maxLength;   // virtual array length == 2 * arrayLength;
    private int offset;
    private int length;
    private int end;        // "cache" == offset + length

    // ????
    public CyclicCharArray(char[] backingArray)
    {
        // backingArray cannot be null;
        this.backingArray = backingArray;
        this.arrayLength = this.backingArray.length;
        this.maxLength = 2 * this.arrayLength;
        this.offset = 0;
        this.length = 0;
        resetEnd();
    }
    private void resetEnd()
    {
        this.end = this.offset + this.length;
    }


    // Read only.
    public final int getMaxLength()
    {
        return maxLength;
    }
    public final int getArrayLength()
    {
        return arrayLength;
    }


    // Convenience methods/builders.
    public static CyclicCharArray wrap(char[] backingArray)
    {
        CyclicCharArray charArray = new CyclicCharArray(backingArray);
        return charArray;
    }
    public static CyclicCharArray wrap(char[] backingArray, int offset, int length)
    {
        CyclicCharArray charArray = new CyclicCharArray(backingArray);
        charArray.setOffsetAndLength(offset, length);
        return charArray;
    }
    
    

    //////////////////////////////////////////////////////////
    // Usage:
    // for(int i=caw.start; i<caw.end; i++) {
    //     char ch = caw.getCharInArray(i);
    // }
    // or
    // for(int i=0; i<caw.length; i++) {
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
//        if(offset < 0) {
//            this.offset = 0;
//        } else if(offset > maxLength - 1) {
//            this.offset = maxLength - 1;
//        // } else if(offset > arrayLength - 1) {   // Why is this causing errors???
//        //     this.offset = arrayLength - 1;
//        } else {
//            this.offset = offset;
//        }
//        if(this.offset + this.length > maxLength - 1) {
//            this.length = (maxLength - 1) - this.offset;
//        }
        this.offset = offset;
        resetEnd();
    }

    public int getLength()
    {
        return length;
    }
    public void setLength(int length)
    {
//        if(length < 0) {
//            this.length = 0;
//        } else if(this.offset + length > maxLength - 1) {
//            this.length = (maxLength - 1) - this.offset;
//        } else {
//            this.length = length;
//        }
        this.length = length;
        resetEnd();
    }

    public void setOffsetAndLength(int offset, int length)
    {
//        if(offset < 0) {
//            this.offset = 0;
//        } else if(offset > maxLength - 1) {
//            this.offset = maxLength - 1;
//        // } else if(offset > arrayLength - 1) {   // Why is this causing errors???
//        //     this.offset = arrayLength - 1;
//        } else {
//            this.offset = offset;
//        }
//        if(length < 0) {
//            this.length = 0;
//        } else if(this.offset + length > maxLength - 1) {
//            this.length = (maxLength - 1) - this.offset;
//        } else {
//            this.length = length;
//        }
        this.offset = offset;
        this.length = length;
        resetEnd();
    }
    
    
    public int getStart()
    {
        return offset;
    }
    public int getEnd()
    {
        return end;
    }


    public final char[] getBackingArray()
    {
        return backingArray;
    }
   
    
    // offset <= index < offset + length
    
    public char getCharInArray()
    {
        return getCharInArray(this.offset);
    }
    public char getCharInArray(int index)
    {
        return this.backingArray[index % this.arrayLength];
    }
    public char getCharInArrayBoundsCheck(int index)
    {
        if(index < this.offset || index >= this.offset + this.length) {
            throw new IndexOutOfBoundsException("Out of bound: index = " + index + ". offset = " + offset + "; length = " + length);
        }
        return this.backingArray[index % this.arrayLength];
    }
    public void setCharInArray(char ch)
    {
        setCharInArray(this.offset, ch);
    }
    public void setCharInArray(int index, char ch)
    {
        this.backingArray[index % this.arrayLength] = ch;
    }
    public void setCharInArrayBoundsCheck(int index, char ch)
    {
        if(index < this.offset || index >= this.offset + this.length) {
            throw new IndexOutOfBoundsException("Out of bound: index = " + index + ". offset = " + offset + "; length = " + length);
        }
        this.backingArray[index % this.arrayLength] = ch;
    }
    public void setCharInArrays(char ... c)
    {
        setCharInArrays(this.offset, c);
    }
    public void setCharInArrays(int index, char ... c)
    {
        for(int i=0; i<c.length; i++) {
            this.backingArray[(index + i) % this.arrayLength] = c[i];
        }
    }
    public void setCharInArraysBoundsCheck(int index, char ... c)
    {
        if(index < this.offset || index >= this.offset + this.length - c.length) {
            throw new IndexOutOfBoundsException("Out of bound: index = " + index + ". offset = " + offset + "; length = " + length);
        }
        for(int i=0; i<c.length; i++) {
            this.backingArray[(index + i) % this.arrayLength] = c[i];
        }
    }
    
    
    public char getChar()
    {
        return getChar(0);
    }
    public char getChar(int index)
    {
        return this.backingArray[(this.offset + index) % this.arrayLength];
    }
    public char getCharBoundsCheck(int index)
    {
        if(index < 0 || index >= this.length) {
            throw new IndexOutOfBoundsException("Out of bound: index = " + index + ". offset = " + offset + "; length = " + length);
        }
        return this.backingArray[(this.offset + index) % this.arrayLength];
    }

    // Make a copy and return the slice [index, index+length).
    public char[] getChars(int index, int length)
    {
        char[] copied = new char[length];
        for(int i=0; i<length; i++) {
            copied[i] = this.backingArray[(this.offset + index + i) % this.arrayLength];
        }
        return copied;
    }

    public void setChar(char ch)
    {
        setChar(0, ch);
    }
    public void setChar(int index, char ch)
    {
        this.backingArray[(this.offset + index) % this.arrayLength] = ch;
    }
    public void setCharBoundsCheck(int index, char ch)
    {
        if(index < 0 || index >= this.length) {
            throw new IndexOutOfBoundsException("Out of bound: index = " + index + ". offset = " + offset + "; length = " + length);
        }
        this.backingArray[(this.offset + index) % this.arrayLength] = ch;
    }
    public void setChars(char ... c)
    {
        setChars(0, c);
    }
    public void setChars(int index, char ... c)
    {
        for(int i=0; i<c.length; i++) {
            this.backingArray[(this.offset + index + i) % this.arrayLength] = c[i];
        }
    }
    public void setCharsBoundsCheck(int index, char ... c)
    {
        if(index < 0 || index >= this.length - c.length) {
            throw new IndexOutOfBoundsException("Out of bound: index = " + index + ". offset = " + offset + "; length = " + length);
        }
        for(int i=0; i<c.length; i++) {
            this.backingArray[(this.offset + index + i) % this.arrayLength] = c[i];
        }
    }

    
    // Returns the copied subarray from [offset to limit)
    public char[] getArray()
    {
         // which is better???
        
        // [1] Using arraycopy.
//        char[] copied = new char[length];
//        if(offset + length < maxLength) {
//            System.arraycopy(this.backingArray, offset, copied, 0, length);
//        } else {
//            // Note the arraycopy does memcopy/memomove.
//            //   Need a more efficient way to do this? (e.g., by returning a "ref" not copy???)
//            int first = maxLength - offset;
//            int second = length - first;
//            System.arraycopy(this.backingArray, offset, copied, 0, first);
//            System.arraycopy(this.backingArray, 0, copied, first, second);
//        }
        
        // [2] Just use a loop.
        char[] copied = new char[length];
        for(int i=0; i<length; i++) {
            copied[i] = this.backingArray[(this.offset + i) % this.arrayLength];
        }

        return copied;
    }

    
    @Override
    public String toString()
    {
        return Arrays.toString(this.getArray());
    }
    
    
}
