using System;
using System.Threading.Tasks;

namespace PuppetMaster.Commands {
    public class ReplicationFactor : Command {
        private readonly int factor;

        public ReplicationFactor(int factor) {
            this.factor = factor;
        }

        protected override void DoWork() {
            System.Diagnostics.Debug.WriteLine(String.Format("ReplicationFactor of {0}", this.factor));
        }
    }
}
