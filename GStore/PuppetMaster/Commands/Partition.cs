using PuppetMaster.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuppetMaster.Commands {
    public class Partition : Command {
        private readonly string name;
        private readonly List<string> replicas;

        public Partition(PuppetMaster form, int num, string name, IEnumerable<string> replicas) : base(form) {
            this.name = name;
            this.replicas = new List<string>(replicas);

            if (num != this.replicas.Count) {
                throw new PartitionParameterNumberMismatchException(num, this.replicas.Count);
            }
        }

        protected override void DoWork() {
            // TODO: Implement
            string print = String.Format("Partition {0} created\nShared by {1} replicas: ", this.name, this.replicas.Count);
            foreach (string id in this.replicas) {
                print = String.Concat(print, String.Format("{0} ", id));
            }

            Log(print);
        }
    }
}
