using System;
using System.Threading.Tasks;

namespace PuppetMaster.Commands {
    public class ReplicationFactor : Command {
        private readonly int factor;

        public ReplicationFactor(PuppetMaster form, int factor) : base(form) {
            this.factor = factor;
        }

        protected override void DoWork() {
            Log(String.Format("ReplicationFactor set to {0}", this.factor));
        }
    }
}
