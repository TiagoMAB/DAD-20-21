using System;
using System.Collections.Generic;
using System.Text;

namespace PuppetMaster.Commands
{
    public class Crash : Command
    {
        private readonly string id;

        public Crash(string id)
        {
            this.id = id;
        }

        public void Execute()
        {
            // TODO: implement
            System.Diagnostics.Debug.WriteLine(String.Format("Crash {0}", this.id));
        }
    }
}
