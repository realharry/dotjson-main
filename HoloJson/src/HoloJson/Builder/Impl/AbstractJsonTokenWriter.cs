using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HoloJson.Builder.Impl
{
    // TBD:
    public abstract class AbstractJsonTokenWriter : TextWriter, JsonTokenWriter
    {
        private TextWriter decoratedWriter;

        public AbstractJsonTokenWriter(TextWriter decoratedWriter)
        {
            this.decoratedWriter = decoratedWriter;
        }

        public override void Write(char[] buffer, int index, int count)
        {
            decoratedWriter.Write(buffer, index, count);
        }

        public override void Flush()
        {
            decoratedWriter.Flush();
        }

        //public override void close()
        //{
        //    decoratedWriter.close();
        //}

        // TBD:
        // ...

    }

}