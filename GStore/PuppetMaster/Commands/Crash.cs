using System;
using System.Threading.Tasks;

namespace PuppetMaster.Commands {
    public class Crash : Command {
        private readonly string id;

        public Crash(PuppetMaster form, string id) : base(form) {
            this.id = id;
        }

        protected override void DoWork() {
            // TODO: implement

            Log(String.Format("Crashed server '{0}'", this.id));
        }
    }
}
