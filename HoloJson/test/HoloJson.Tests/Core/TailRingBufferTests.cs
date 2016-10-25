using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace HoloJson.Core
{
    public sealed class TailRingBufferTests
    {
        [Fact]
        public void TestOfferPollPeek()
        {
            TailRingBuffer<string> queue = new TailRingBuffer<string>(4);
            Console.WriteLine("queue = " + queue);

            queue.Offer("a");
            Console.WriteLine("queue = " + queue);
            string e1 = queue.Poll();
            Console.WriteLine("e1 = " + e1);
            Console.WriteLine("queue = " + queue);
            Assert.Equal("a", e1);

            queue.Offer("b");
            Console.WriteLine("queue = " + queue);
            string e2 = queue.Peek();
            Console.WriteLine("e2 = " + e2);
            Console.WriteLine("queue = " + queue);
            Assert.Equal("b", e2);

            queue.Offer("c");
            Console.WriteLine("queue = " + queue);
            string e3 = queue.Poll();
            Console.WriteLine("e3 = " + e3);
            Console.WriteLine("queue = " + queue);
            Assert.Equal("b", e3);

            queue.Offer("d");
            Console.WriteLine("queue = " + queue);

            queue.Offer("e");
            Console.WriteLine("queue = " + queue);

            queue.Offer("f");
            Console.WriteLine("queue = " + queue);
            string e4 = queue.Poll();
            Console.WriteLine("e4 = " + e4);
            Console.WriteLine("queue = " + queue);
            Assert.Equal("c", e4);

            queue.Offer("g");
            Console.WriteLine("queue = " + queue);

            queue.Offer("h");
            Console.WriteLine("queue = " + queue);
            string e5 = queue.Poll();
            Console.WriteLine("e5 = " + e5);
            Console.WriteLine("queue = " + queue);
            Assert.Equal("e", e5);

            queue.Offer("i");
            Console.WriteLine("queue = " + queue);

            queue.Offer("j");
            Console.WriteLine("queue = " + queue);


            Assert.True(true);
        }
    }
}
