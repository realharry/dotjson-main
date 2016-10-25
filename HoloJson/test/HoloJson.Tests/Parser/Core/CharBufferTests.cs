using HoloJson.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace HoloJson.Parser.Core
{
    public class CharBufferTests
    {
        [Fact]
        public void TestPush()
        {
            var charBuffer = new CharBuffer(10);

            int size1 = charBuffer.Size;
            Console.WriteLine("size1 = " + size1);

            charBuffer.Push('a');
            Console.WriteLine("charBuffer = " + charBuffer);

            int size2 = charBuffer.Size;
            Console.WriteLine("size2 = " + size2);
            Assert.Equal(size1 + 1, size2);
        }

        [Fact]
        public void TestTail()
        {
            var charBuffer = new CharBuffer(10);

            int size1 = charBuffer.Size;
            Console.WriteLine("size1 = " + size1);

            charBuffer.Push('a');
            charBuffer.Push('b');
            charBuffer.Push('c');
            Console.WriteLine("charBuffer = " + charBuffer);

            char c = charBuffer.Tail();
            Console.WriteLine("charBuffer = " + charBuffer);
            Console.WriteLine("c = " + c);
            Assert.Equal('c', c);

            int size2 = charBuffer.Size;
            Console.WriteLine("size2 = " + size2);
            Assert.Equal(size1 + 3, size2);
        }

        [Fact]
        public void TestTail2()
        {
            var charBuffer = new CharBuffer(10);

            int size1 = charBuffer.Size;
            Console.WriteLine("size1 = " + size1);

            char[] buff1 = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };
            charBuffer.Push(buff1);
            Console.WriteLine("charBuffer = " + charBuffer);

            int size2 = charBuffer.Size;
            Console.WriteLine("size2 = " + size2);
            Assert.Equal(size1 + 8, size2);

            char c1 = charBuffer.Tail();
            Console.WriteLine("charBuffer = " + charBuffer);
            Console.WriteLine("c1 = " + c1);
            Assert.Equal('h', c1);
            char[] cc = charBuffer.Tail(3);
            Console.WriteLine("charBuffer = " + charBuffer);
            Console.WriteLine("cc = " + Arrays.ToString(cc));
            Assert.Equal('f', cc[0]);
            Assert.Equal('g', cc[1]);
            Assert.Equal('h', cc[2]);
            char c4 = charBuffer.Tail();
            Console.WriteLine("charBuffer = " + charBuffer);
            Console.WriteLine("c4 = " + c4);
            Assert.Equal('h', c4);

            int size3 = charBuffer.Size;
            Console.WriteLine("size3 = " + size3);
            Assert.Equal(size2, size3);

            char[] buff2 = new char[] { 'i', 'j', 'k', 'l' };
            charBuffer.Push(buff2);
            Console.WriteLine("charBuffer = " + charBuffer);

            int size4 = charBuffer.Size;
            Console.WriteLine("size4 = " + size4);
            Assert.Equal(9, size4);

            charBuffer.Push('m');
            charBuffer.Push('n');
            charBuffer.Push('o');
            Console.WriteLine("charBuffer = " + charBuffer);

            bool suc = charBuffer.Push('p');
            Console.WriteLine("charBuffer = " + charBuffer);
            Assert.Equal(true, suc);

            char c7 = charBuffer.Tail();
            Console.WriteLine("charBuffer = " + charBuffer);
            Assert.Equal('p', c7);

            charBuffer.Push('q');
            Console.WriteLine("charBuffer = " + charBuffer);

            char[] dd = charBuffer.Tail(20);
            Console.WriteLine("charBuffer = " + charBuffer);
            Console.WriteLine("dd = " + Arrays.ToString(dd));
            Assert.Equal('i', dd[0]);
            Assert.Equal('j', dd[1]);
            Assert.Equal('k', dd[2]);
            // etc.

            charBuffer.Clear();
            Console.WriteLine("charBuffer = " + charBuffer);
            int size9 = charBuffer.Size;
            Console.WriteLine("size9 = " + size9);
            Assert.Equal(0, size9);


        }

        [Fact]
        public void TestTailAsString()
        {
            var charBuffer = new CharBuffer(10);

            char[] c = new char[] { 'w', 'x', 'y', 'z', 'I', ' ', 'a', 'm', ' ', 'm', 'e', '.' };
            charBuffer.Push(c);
            Console.WriteLine("charBuffer = " + charBuffer);
            string str = charBuffer.GetTailAsString(8);
            Console.WriteLine("str = " + str);
            Assert.Equal("yzI am m", str);

        }

        [Fact]
        public void TestBufferAsString()
        {
            var charBuffer = new CharBuffer(10);

            char[] c = new char[] { 'w', 'x', 'y', 'z', 'I', ' ', 'a', 'm', ' ', 'm', 'e', '.' };
            charBuffer.Push(c);
            Console.WriteLine("charBuffer = " + charBuffer);

            var cap = charBuffer.Capacity;
            Console.WriteLine("cap = " + cap);
            Assert.Equal(9U, cap);

            string str = charBuffer.GetBufferAsString();
            Console.WriteLine("str = " + str);
            Assert.Equal("xyzI am m", str);

        }



    }
}
