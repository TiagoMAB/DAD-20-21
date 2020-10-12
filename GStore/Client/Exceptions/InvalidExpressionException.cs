using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Exceptions
{
    class InvalidExpressionException : System.Exception {
        public InvalidExpressionException() : base() { }
        public InvalidExpressionException(string message) : base(message) { }
    }
}
