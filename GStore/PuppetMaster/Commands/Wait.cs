using System;
using System.Threading;
using System.Threading.Tasks;

namespace PuppetMaster.Commands {
    public class Wait : Command {
        private readonly int milliseconds;

        public Wait(PuppetMaster form, int amount) : base(form) {
            this.milliseconds = amount;
        }

        public override Task Execute() {
            Log(String.Format("Waiting {0} milliseconds", this.milliseconds));
            Task.Delay(this.milliseconds).Wait();

            return null;
        }

        protected override Task DoWork() {
            return null;
        }
    }
}
