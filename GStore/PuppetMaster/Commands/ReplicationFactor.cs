using System;
using System.Collections.Generic;
using System.Text;

namespace PuppetMaster.Commands
{
    public class ReplicationFactor : Command
    {
        private readonly int factor;

        public ReplicationFactor(int factor)
        {
            this.factor = factor;
        }

        public void Execute()
        {
            System.Diagnostics.Debug.WriteLine(String.Format("ReplicationFactor of {0}", this.factor));
        }
    }
}
