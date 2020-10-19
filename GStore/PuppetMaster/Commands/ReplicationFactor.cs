using System;
using System.Threading.Tasks;

namespace PuppetMaster.Commands {
    public class ReplicationFactor : Command {
        private readonly int factor;

        public ReplicationFactor(PuppetMaster form, int factor) : base(form) {
            this.factor = factor;
        }

        protected override async Task DoWork() {
            // TODO: Implement????
            await Task.Run(() => Log(String.Format("ReplicationFactor set to {0}", this.factor)));
        }
    }
}
