using System;
using System.Collections.Generic;
using System.Text;

namespace PuppetMaster.Exceptions
{
    public class PartitionParameterNumberMismatchException : Exception
    {
        private readonly int expected;
        private readonly int given;

        public PartitionParameterNumberMismatchException(int expected, int given)
        {
            this.expected = expected;
            this.given    = given;
        }

        public int Expected => this.expected;

        public int Given => this.given;
    }
}
