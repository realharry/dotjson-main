using DotJson.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DotJson.Util
{
    public sealed class UnicodeUtilTests
    {
        [Fact]
        public void TestGetUnicodeCharFromHexSequence()
        {
            char[] c1 = new char[] { '0', '0', '3', '5' };
            char u1 = UnicodeUtil.GetUnicodeCharFromHexSequence(c1);
            Console.WriteLine("u1 = " + u1);
            Assert.Equal('5', u1);

            char[] c2 = new char[] { '0', '0', '5', 'd' };
            char u2 = UnicodeUtil.GetUnicodeCharFromHexSequence(c2);
            Console.WriteLine("u2 = " + u2);
            Assert.Equal(']', u2);

        }

        [Fact]
        public void TestGetUnicodeCodeFromChar()
        {
            char[] c1 = UnicodeUtil.GetUnicodeHexCodeFromChar('0');
            Console.WriteLine("c1 = " + Arrays.ToString(c1));
            // Assert.Equal("\\u0030".toCharArray(), c1);

            char[] c2 = UnicodeUtil.GetUnicodeHexCodeFromChar('a');
            Console.WriteLine("c2 = " + Arrays.ToString(c2));
            //Assert.Equal("".toCharArray(), c2);


        }


    }
}
