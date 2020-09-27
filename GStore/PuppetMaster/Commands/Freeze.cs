using System;

namespace PuppetMaster.Commands
{
    public class Freeze : Command
    {
        private readonly string id;

        public Freeze(string id)
        {
            this.id = id;
        }

        public void Execute()
        {
            // TODO: Implement
            System.Diagnostics.Debug.WriteLine(String.Format("Freeze {0}", id));
        }
    }
}
