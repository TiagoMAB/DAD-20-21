using System;
using System.Collections.Generic;
using System.Text;

namespace PuppetMaster.Commands
{
    public class Unfreeze : Command
    {
        private readonly string id;

        public Unfreeze(string id)
        {
            this.id = id;
        }

        public void Execute()
        {
            // TODO: Implement
            System.Diagnostics.Debug.WriteLine(String.Format("Unfreeze {0}", this.id));
        }
    }
}
