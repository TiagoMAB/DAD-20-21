using System;
using System.Threading;

namespace PuppetMaster.Commands
{
    public class Wait : Command
    {
        private readonly int milliseconds;

        public Wait(int amount)
        {
            this.milliseconds = amount;
        }

        public void Execute()
        {
            System.Diagnostics.Debug.WriteLine(String.Format("Wait {0} milliseconds", this.milliseconds));
            Thread.Sleep(this.milliseconds);
        }
    }
}
