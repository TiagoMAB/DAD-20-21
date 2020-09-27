using System;
using System.Collections.Generic;
using System.Text;

namespace PuppetMaster.Exceptions
{
    public class UnknownCommandException : Exception
    {
        private readonly string command;
        public UnknownCommandException(string command)
        {
            this.command = command;
        }

        public string Command => this.command;
    }
}
