using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DotJson.Parser.Impl
{
    // TBD:
    public abstract class AbstractJsonTokenReader : TextReader, JsonTokenReader
    {
        private TextReader decoratedReader;

        public AbstractJsonTokenReader(TextReader decoratedReader)
        {
            this.decoratedReader = decoratedReader;
        }

        public override int Read(char[] buffer, int index, int count)
        {
            return decoratedReader.Read(buffer, index, count);
        }

        public override Task<int> ReadAsync(char[] buffer, int index, int count)
        {
            return decoratedReader.ReadAsync(buffer, index, count);
        }

        //public override void close()
        //{
        //    decoratedReader.close();
        //}

        // TBD:
        // ...

    }

}