using HoloJson.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace HoloJson.Parser.Core
{
    public sealed class CharQueueTests
    {
        [Fact]
        public void TestAdd()
        {
            var charQueue = new CharQueue(10);

            int size1 = charQueue.Size;
            Console.WriteLine("size1 = " + size1);

            charQueue.Add('a');
            Console.WriteLine("charQueue = " + charQueue);

            int size2 = charQueue.Size;
            Console.WriteLine("size2 = " + size2);
            Assert.Equal(size1 + 1, size2);
        }

        [Fact]
        public void TestPoll()
        {
            var charQueue = new CharQueue(10);

            int size1 = charQueue.Size;
            Console.WriteLine("size1 = " + size1);

            charQueue.Add('a');
            charQueue.Add('b');
            charQueue.Add('c');
            Console.WriteLine("charQueue = " + charQueue);

            char c = charQueue.Poll();
            Console.WriteLine("charQueue = " + charQueue);
            Console.WriteLine("c = " + c);
            Assert.Equal('a', c);

            int size2 = charQueue.Size;
            Console.WriteLine("size2 = " + size2);
            Assert.Equal(size1 + 2, size2);
        }

        [Fact]
        public void TestPeek()
        {
            var charQueue = new CharQueue(10);

            int size1 = charQueue.Size;
            Console.WriteLine("size1 = " + size1);

            charQueue.Add('a');
            charQueue.Add('b');
            charQueue.Add('c');
            Console.WriteLine("charQueue = " + charQueue);

            char c = charQueue.Peek();
            Console.WriteLine("charQueue = " + charQueue);
            Console.WriteLine("c = " + c);
            Assert.Equal('a', c);

            int size2 = charQueue.Size;
            Console.WriteLine("size2 = " + size2);
            Assert.Equal(size1 + 3, size2);
        }

        [Fact]
        public void TestAddAll()
        {
            var charQueue = new CharQueue(10);

            int size1 = charQueue.Size;
            Console.WriteLine("size1 = " + size1);

            char[] buff1 = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };
            charQueue.AddAll(buff1);
            Console.WriteLine("charQueue = " + charQueue);

            int size2 = charQueue.Size;
            Console.WriteLine("size2 = " + size2);
            Assert.Equal(size1 + 8, size2);

            char c1 = charQueue.Poll();
            Console.WriteLine("charQueue = " + charQueue);
            Console.WriteLine("c1 = " + c1);
            Assert.Equal('a', c1);
            char c2 = charQueue.Poll();
            Console.WriteLine("charQueue = " + charQueue);
            Console.WriteLine("c2 = " + c2);
            Assert.Equal('b', c2);
            char c3 = charQueue.Poll();
            Console.WriteLine("charQueue = " + charQueue);
            Console.WriteLine("c3 = " + c3);
            Assert.Equal('c', c3);
            char[] cc = charQueue.Poll(3);
            Console.WriteLine("charQueue = " + charQueue);
            Console.WriteLine("cc = " + Arrays.ToString(cc));
            Assert.Equal('d', cc[0]);
            Assert.Equal('e', cc[1]);
            Assert.Equal('f', cc[2]);
            char c4 = charQueue.Peek();
            Console.WriteLine("charQueue = " + charQueue);
            Console.WriteLine("c4 = " + c4);
            Assert.Equal('g', c4);

            int size3 = charQueue.Size;
            Console.WriteLine("size3 = " + size3);
            Assert.Equal(size2 - 6, size3);

            char[] buff2 = new char[] { 'i', 'j', 'k', 'l' };
            charQueue.AddAll(buff2);
            Console.WriteLine("charQueue = " + charQueue);

            int size4 = charQueue.Size;
            Console.WriteLine("size4 = " + size4);
            Assert.Equal(size3 + 4, size4);

            charQueue.Add('m');
            charQueue.Add('n');
            charQueue.Add('o');
            Console.WriteLine("charQueue = " + charQueue);

            bool suc = charQueue.Add('p');
            Console.WriteLine("charQueue = " + charQueue);
            Assert.Equal(false, suc);

            char c7 = charQueue.Poll();
            Console.WriteLine("charQueue = " + charQueue);
            Assert.Equal('g', c7);
            char c8 = charQueue.Poll();
            Console.WriteLine("charQueue = " + charQueue);
            Assert.Equal('h', c8);
            char c9 = charQueue.Poll();
            Console.WriteLine("charQueue = " + charQueue);
            Assert.Equal('i', c9);
            char c10 = charQueue.Poll();
            Console.WriteLine("charQueue = " + charQueue);
            Assert.Equal('j', c10);
            char c11 = charQueue.Peek();
            Console.WriteLine("charQueue = " + charQueue);
            Assert.Equal('k', c11);
            char c12 = charQueue.Poll();
            Console.WriteLine("charQueue = " + charQueue);
            Assert.Equal('k', c12);

            charQueue.Add('p');
            Console.WriteLine("charQueue = " + charQueue);

            charQueue.Clear();
            Console.WriteLine("charQueue = " + charQueue);
            int size9 = charQueue.Size;
            Console.WriteLine("size9 = " + size9);
            Assert.Equal(0, size9);


        }



    }
}
