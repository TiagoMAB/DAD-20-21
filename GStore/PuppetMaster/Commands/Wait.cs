using System;
using System.Threading;
using System.Threading.Tasks;

namespace PuppetMaster.Commands
{
    public class Wait : Command
    {
        private readonly int milliseconds;

        public Wait(int amount)
        {
            this.milliseconds = amount;
        }

        public override Task Execute()
        {
            System.Diagnostics.Debug.WriteLine(String.Format("Wait {0} milliseconds", this.milliseconds));
            Thread.Sleep(this.milliseconds);

            return null;
        }

        protected override void DoWork() {
        }
    }
}
