using System;
using System.Collections.Generic;
using System.Text;

namespace PuppetMaster.Commands {
    public class Unfreeze : Command {
        private readonly string id;

        public Unfreeze(PuppetMaster form, string id) : base(form) {
            this.id = id;
        }

        protected override void DoWork() {
            // TODO: Implement
            Log(String.Format("Unfreezed server '{0}'", this.id));
        }
    }
}
