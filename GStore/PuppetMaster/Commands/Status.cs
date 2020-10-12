using System;
using System.Collections.Generic;
using System.Text;

namespace PuppetMaster.Commands
{
    public class Status : Command
    {
        public Status(PuppetMaster form) : base(form) { }

        protected override void DoWork()
        {
            // TODO: Implement
            Log("All status received");
        }
    }
}
