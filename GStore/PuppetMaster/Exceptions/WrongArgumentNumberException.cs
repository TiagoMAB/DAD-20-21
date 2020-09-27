using System;
using System.Collections.Generic;
using System.Text;

namespace PuppetMaster.Exceptions
{
    public class WrongArgumentNumberException : Exception
    {
        private readonly string command;
        private readonly int expected;
        private readonly int given;

        public WrongArgumentNumberException(string command, int expected, int given)
        {
            this.command  = command;
            this.expected = expected;
            this.given    = given;
        }

        public string Command => this.command;

        public int Expected => this.expected;

        public int Given => this.given;
    }
}
