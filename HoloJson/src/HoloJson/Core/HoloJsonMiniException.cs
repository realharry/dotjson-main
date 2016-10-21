using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HoloJson.Core
{
    public class HoloJsonMiniException : Exception
    {
        public HoloJsonMiniException()
            : base()
        {
        }
        public HoloJsonMiniException(string message)
            : base(message)
        {
        }
        public HoloJsonMiniException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

    }
}
