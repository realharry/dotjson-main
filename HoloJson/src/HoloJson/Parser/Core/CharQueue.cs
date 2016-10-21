using HoloJson.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HoloJson.Parser.Core
{
    /*
     * "Ring buffer" implementation. It's a FIFO queue.
     * The reason for using CharQueue is to reduce the memory footprint while reading the input stream.
     * We could have used Queue<char>, but using the array of primitive type, char, should be better for performance. 
     * (Note: the implementation is not thread-safe.)
     * <-- Note: Ported from Java version.
     * This class may or many not be needed in C#.
     */
    public sealed class CharQueue
	{
		// temporary
		// private const int MAX_BUFFER_SIZE = 4096;
		private const int MAX_BUFFER_SIZE = 10000000;    // ????
		private const int DEF_BUFFER_SIZE = 2048;
		// private const int MIN_BUFFER_SIZE = 512;
		private const int MIN_BUFFER_SIZE = 4;      // ????
		// ...

		// Internal buffer.
		private readonly char[] buffer;
		// Ring buffer size.
		private readonly int maxSize;
		
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
		// private readonly CyclicCharArray cCharArray;

		public CharQueue()
            : this(DEF_BUFFER_SIZE)
		{
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
		private void IncrementHead()
		{
			++headPointer;
			// if(headPointer == maxSize) {
			if(headPointer >= maxSize) {
				headPointer = 0;
			}
		}
		// 0 < delta < maxSize
		private void IncrementHead(int delta)
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
		private void IncrementTail()
		{
			++tailPointer;
			// if(tailPointer == maxSize) {
			if(tailPointer >= maxSize) {
				tailPointer = 0;
			}
		}
		// 0 < delta < maxSize
		private void IncrementTail(int delta)
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
		public int MaxCapacity
		{
            get
            {
                return this.maxSize - 1;
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
                return (tailPointer == headPointer);
                //        if(tailPointer == headPointer) {
                //            return true;
                //        } else {
                //            return false;
                //        }
            }
		}

		// Adds the given char to the ring buffer.
		// If the buffer is full, then it returns false.
		public bool Add(char ch)
		{
			if(Margin == 0) {
				return false;
			} else {
				buffer[tailPointer] = ch;
				IncrementTail();
				return true;
			}
		}
		public bool AddAll(char[] c)
		{
			if(c == null || c.Length == 0) {
				return false;
			}
			int len = c.Length;
			return AddAll(c, len);
		}
		// Adds the char array c to the buffer, up to length, but no more than the c.Size.
		public bool AddAll(char[] c, int length)
		{
			if(c == null || c.Length == 0) {
				return false;
			}
			int len = c.Length;
			if(len < length) {
				length = len;
			}
			if(Margin < length) {
				return false;
			} else {
				if(tailPointer + length < maxSize) {
					// Array.Copy(c, 0, buffer, tailPointer, length);
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
		}
		
		
		/////////////////////////////////////////////////////////
		// Note that CharQueue is primarily "read-only"
		//      in the sense that the caller never modifies the peek'ed/next'ed char or char[].
		//      Internal buffer really only changes during add/AddAll.
		// Hence we can optimize the following methods in various ways...
		// ......

		
	//    public bool offer(char ch)
	//    {
	//        return Add(ch);
	//    }

		// Poll() pops the char at the head pointer.
		public char Poll()
		{
			if(IsEmpty) {
				return (char) 0;
			}
			char ch = buffer[headPointer];
			IncrementHead();
			return ch;
		}
		// Returns the char array at the head up to length.
		// If the buffer contains less than length chars, it returns all data.
		public char[] Poll(int length)
		{
			if(IsEmpty) {
				// return null;
				return new char[]{};
			}
			if(length > Size) {
				length = Size;
			}
			char[] polled = new char[length];
			if(headPointer + length < maxSize) {
				
				
				Array.Copy(buffer, headPointer, polled, 0, length);
				
				// This does not work
				// System.Diagnostics.Debug.WriteLine(">>>>>>>>>>>>>>>>>>>>>> length = " + length);
				// java.nio.CharBuffer cb = java.nio.CharBuffer.Wrap(buffer, headPointer, length);
				// polled = cb.array();
				// System.Diagnostics.Debug.WriteLine(">>>>>>>>>>>>>>>>>>>>>> polled.Length = " + polled.Length);
				// System.Diagnostics.Debug.WriteLine(">>>>>>>>>>>>>>>>>>>>>> polled = " + Arrays.ToString(polled));
				
			} else {
				// Note the arraycopy does memcopy/memomove.
				//   Need a more efficient way to do this? (e.g., by returning a "ref" not copy???)
				int first = maxSize - headPointer;
				int second = length - first;
				Array.Copy(buffer, headPointer, polled, 0, first);
				Array.Copy(buffer, 0, polled, first, second);
			}
			IncrementHead(length);
			return polled;
		}

		public CyclicCharArray PollBuffer(int length)
		{
			if(IsEmpty) {
				return null;
			}
			if(length > Size) {
				length = Size;
			}
			// cCharArray.SetOffsetAndLength(headPointer, length);
			CyclicCharArray charArray = CyclicCharArray.Wrap(buffer, headPointer, length);
			IncrementHead(length);
			return charArray;
		}

		
		// Pops one char from the buffer, and removes it.
		// If buffer is empty, it's ignored.
		public void Skip()
		{
			if(! IsEmpty) {
				IncrementHead();
			}
		}
		// "skips" up to length chars.
		public void Skip(int length)
		{
			if(! IsEmpty) {
				if(length > Size) {
					length = Size;
				}
				IncrementHead(length);
			}
		}


		// Peek() returns the char at the head pointer, without popping the entry.
		// Note that Peek() methods are idempotent.
		public char Peek()
		{
			if(IsEmpty) {
				return (char) 0;
			}
			char ch = buffer[headPointer];
			return ch;
		}
		// Peeks the chars at the head up to length.
		// If the buffer contains less than length chars, it just returns all chars,
		//     without removing those entries from the data buffer. 
		public char[] Peek(int length)
		{
			if(IsEmpty) {
				// return null;
				return new char[]{};
			}
			if(length > Size) {
				length = Size;
			}
			char[] peeked = new char[length];
			if(headPointer + length < maxSize) {
				Array.Copy(buffer, headPointer, peeked, 0, length);
			} else {
				// Note the arraycopy does memcopy/memomove.
				//   Need a more efficient way to do this? (e.g., by returning a "ref" not copy???)
				int first = maxSize - headPointer;
				int second = length - first;
				Array.Copy(buffer, headPointer, peeked, 0, first);
				Array.Copy(buffer, 0, peeked, first, second);
			}
			return peeked;
		}
		
		public CyclicCharArray PeekBuffer(int length)
		{
			if(IsEmpty) {
				return null;
			}
			if(length > Size) {
				length = Size;
			}
			// cCharArray.SetOffsetAndLength(headPointer, length);
			CyclicCharArray charArray = CyclicCharArray.Wrap(buffer, headPointer, length);
			return charArray;
		}
		public CyclicCharArray PeekBuffer(int offset, int length)
		{
			if(IsEmpty) {
				return null;
			}
			if(offset < 0 || offset >= Size) {
				return null;
			}
			if(length > Size - offset) {
				length = Size - offset;
			}
			// cCharArray.SetOffsetAndLength(headPointer + offset, length);
			CyclicCharArray charArray = CyclicCharArray.Wrap(buffer, headPointer + offset, length);
			return charArray;
		}


	//    public bool contains(char ch)
	//    {
	//        return false;
	//    }

	//    public Iterator<char> iterator()
	//    {
	//        return null;
	//    }


		// Returns the copy of the data buffer, as a regular array.
		public char[] ToArray()
		{
			if(IsEmpty) {
				// return null;
				return new char[]{};
			}
			int length = Size;
			char[] copied = new char[length];
			if(headPointer + length < maxSize) {
				Array.Copy(buffer, headPointer, copied, 0, length);
			} else {
				// Note the arraycopy does memcopy/memomove.
				//   Need a more efficient way to do this? (e.g., by returning a "ref" not copy???)
				int first = maxSize - headPointer;
				int second = length - first;
				Array.Copy(buffer, headPointer, copied, 0, first);
				Array.Copy(buffer, 0, copied, first, second);
			}
			return copied;
		}

		// Remove the data from the buffer.
		public void Clear()
		{
	//        headPointer = tailPointer;
			headPointer = tailPointer = 0;
		}

		
		// For debugging...
		public override string ToString()
		{
			return "CharQueue [buffer=" + String.Join<char>(",", Peek(100)) + ", maxSize="
					+ maxSize + ", tailPointer=" + tailPointer + ", headPointer="
					+ headPointer + "]";
		}
		
	}
}
		