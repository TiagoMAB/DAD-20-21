using System;
using System.Collections.Generic;
using System.Text;

namespace PuppetMaster.Commands
{
    public class Partition : Command
    {
        private readonly string name;
        private readonly List<string> replicas;

        public Partition(int num, string name, IEnumerable<string> replicas)
        {
            this.name     = name;
            this.replicas = new List<string>(replicas);

            if(num != this.replicas.Count)
            {
                // TODO: Handle error
                throw new NotImplementedException();
            }

        }

        public void Execute()
        {
            // TODO: Implement
            string print = String.Format("Partition {0} shared by {1} replicas: ", this.name, this.replicas.Count);
            foreach(string id in this.replicas)
            {
                print = String.Concat(print, String.Format("{0} ", id));
            }
            System.Diagnostics.Debug.WriteLine(print);
        }
    }
}
