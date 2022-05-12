using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeAnalyzer.Exceptions
{
    public class GeoException : Exception
    {
        public GeoException()
        {
        }

        public GeoException(string message)
            : base(message)
        {
        }

        public GeoException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
