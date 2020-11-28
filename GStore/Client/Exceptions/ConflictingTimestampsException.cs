using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Exceptions
{
    class ConflictingTimestampsException : System.Exception {
        public ConflictingTimestampsException() : base() { }
        public ConflictingTimestampsException(string message) : base(message) { }
    }
}
