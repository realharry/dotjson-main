using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DotJson.Core
{
    public class DotJsonMiniException : Exception
    {
        public DotJsonMiniException()
            : base()
        {
        }
        public DotJsonMiniException(string message)
            : base(message)
        {
        }
        public DotJsonMiniException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

    }
}
