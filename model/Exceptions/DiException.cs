using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace model.Exceptions
{
    public class DiException : Exception
    {
        public DiException()
        {
        }

        public DiException(string? message) : base(message)
        {
        }

        public DiException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected DiException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}