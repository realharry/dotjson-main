using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HoloJson.Core
{
	// The purpose of this class is to implement "subarray" operation 
	//        that does not require the content copy (which requires new memory allocation).
	// This class is for wrapping "cyclic" arrays like CharQueue (ring buffer). 
	public sealed class CyclicCharArray
	{
		// backing array spec.
		private readonly int arrayLength;
		private readonly char[] backingArray;
		
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
			this.arrayLength = this.backingArray.Length;
			this.maxLength = 2 * this.arrayLength;
			this.offset = 0;
			this.length = 0;
			ResetEnd();
		}
		private void ResetEnd()
		{
			this.end = this.offset + this.length;
		}


		// Read only.
		public int MaxLength
		{
            get
            {
                return maxLength;
            }
		}
		public int ArrayLength
		{
            get
            {
                return arrayLength;
            }
		}


		// Convenience methods/builders.
		public static CyclicCharArray Wrap(char[] backingArray)
		{
			CyclicCharArray charArray = new CyclicCharArray(backingArray);
			return charArray;
		}
		public static CyclicCharArray Wrap(char[] backingArray, int offset, int length)
		{
			CyclicCharArray charArray = new CyclicCharArray(backingArray);
			charArray.SetOffsetAndLength(offset, length);
			return charArray;
		}
		
		

		//////////////////////////////////////////////////////////
		// Usage:
		// for(int i=caw.start; i<caw.end; i++) {
		//     char ch = caw.GetCharInArray(i);
		// }
		// or
		// for(int i=0; i<caw.Length; i++) {
		//     char ch = caw.GetChar(i);
		// }
		
	 
		// Note SetOffset() should be called before SetLength(), in the current implementation,
		//     if both values need to be set.
		public int Offset
		{
            get
            {
                return offset;
            }
            set
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
                this.offset = value;
                ResetEnd();
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
                //        if(length < 0) {
                //            this.length = 0;
                //        } else if(this.offset + length > maxLength - 1) {
                //            this.length = (maxLength - 1) - this.offset;
                //        } else {
                //            this.length = length;
                //        }
                this.length = value;
                ResetEnd();
            }
		}

		public void SetOffsetAndLength(int offset, int length)
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
			ResetEnd();
		}
		
		
        // vs. Offset???
		public int Start
		{
            get
            {
                return offset;
            }
		}
		public int End
		{
            get
            {
                return end;
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
		
		public char GetCharInArray()
		{
			return GetCharInArray(this.offset);
		}
		public char GetCharInArray(int index)
		{
			return this.backingArray[index % this.arrayLength];
		}
		public char GetCharInArrayBoundsCheck(int index)
		{
			if(index < this.offset || index >= this.offset + this.length) {
                throw new ArgumentOutOfRangeException("Out of bound: index = " + index + ". offset = " + offset + "; length = " + length);
			}
			return this.backingArray[index % this.arrayLength];
		}
		public void SetCharInArray(char ch)
		{
			SetCharInArray(this.offset, ch);
		}
		public void SetCharInArray(int index, char ch)
		{
			this.backingArray[index % this.arrayLength] = ch;
		}
		public void SetCharInArrayBoundsCheck(int index, char ch)
		{
			if(index < this.offset || index >= this.offset + this.length) {
				throw new ArgumentOutOfRangeException("Out of bound: index = " + index + ". offset = " + offset + "; length = " + length);
			}
			this.backingArray[index % this.arrayLength] = ch;
		}
		public void SetCharInArrays(params char[] c)
		{
			SetCharInArrays(this.offset, c);
		}
		public void SetCharInArrays(int index, params char[] c)
		{
			for(int i=0; i<c.Length; i++) {
				this.backingArray[(index + i) % this.arrayLength] = c[i];
			}
		}
		public void SetCharInArraysBoundsCheck(int index, params char[] c)
		{
			if(index < this.offset || index >= this.offset + this.length - c.Length) {
				throw new ArgumentOutOfRangeException("Out of bound: index = " + index + ". offset = " + offset + "; length = " + length);
			}
			for(int i=0; i<c.Length; i++) {
				this.backingArray[(index + i) % this.arrayLength] = c[i];
			}
		}
		
		
		public char GetChar()
		{
			return GetChar(0);
		}
		public char GetChar(int index)
		{
			return this.backingArray[(this.offset + index) % this.arrayLength];
		}
		public char GetCharBoundsCheck(int index)
		{
			if(index < 0 || index >= this.length) {
				throw new ArgumentOutOfRangeException("Out of bound: index = " + index + ". offset = " + offset + "; length = " + length);
			}
			return this.backingArray[(this.offset + index) % this.arrayLength];
		}

		// Make a copy and return the slice [index, index+length).
		public char[] GetChars(int index, int length)
		{
			char[] copied = new char[length];
			for(int i=0; i<length; i++) {
				copied[i] = this.backingArray[(this.offset + index + i) % this.arrayLength];
			}
			return copied;
		}

		public void SetChar(char ch)
		{
			SetChar(0, ch);
		}
		public void SetChar(int index, char ch)
		{
			this.backingArray[(this.offset + index) % this.arrayLength] = ch;
		}
		public void SetCharBoundsCheck(int index, char ch)
		{
			if(index < 0 || index >= this.length) {
				throw new ArgumentOutOfRangeException("Out of bound: index = " + index + ". offset = " + offset + "; length = " + length);
			}
			this.backingArray[(this.offset + index) % this.arrayLength] = ch;
		}
		public void SetChars(params char[] c)
		{
			SetChars(0, c);
		}
		public void SetChars(int index, params char[] c)
		{
			for(int i=0; i<c.Length; i++) {
				this.backingArray[(this.offset + index + i) % this.arrayLength] = c[i];
			}
		}
        public void SetCharsBoundsCheck(int index, params char[] c)
		{
			if(index < 0 || index >= this.length - c.Length) {
				throw new ArgumentOutOfRangeException("Out of bound: index = " + index + ". offset = " + offset + "; length = " + length);
			}
			for(int i=0; i<c.Length; i++) {
				this.backingArray[(this.offset + index + i) % this.arrayLength] = c[i];
			}
		}

		
		// Returns the copied subarray from [offset to limit)
		public char[] GetArray()
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

		public override string ToString()
		{
			// return Arrays.ToString(this.GetArray());
            return String.Join<char>(",", this.GetArray());
		}
		
		
	}
}
