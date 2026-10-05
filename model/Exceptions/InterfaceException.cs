using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace model.Exceptions
{
    public class InterfaceException : Exception
    {
        public InterfaceException()
        {
        }

        public InterfaceException(string? message) : base(message)
        {
        }

        public InterfaceException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected InterfaceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}