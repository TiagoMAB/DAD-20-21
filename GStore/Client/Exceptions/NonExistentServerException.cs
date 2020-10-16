using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Exceptions
{
    class NonExistentServerException : System.Exception {
        public NonExistentServerException() : base() { }
        public NonExistentServerException(string message) : base(message) { }
    }
}
